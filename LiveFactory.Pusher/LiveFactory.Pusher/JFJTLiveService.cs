using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using LiveCommand.Common;
using LiveCommand.Common.NetMQ.Models;
using NetMQ;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace LiveFactory.Pusher
{
    public partial class JFJTLiveService : ServiceBase
    {
        private LocalSettings _localSettings;
        private static NetMQService _ReceiveMQService;
        private static NetMQService _SubmitMQService;
        private CancellationTokenSource _SubmitCTS = new CancellationTokenSource();
        private string receiveAddress = "tcp://";
        private string requestAddress = "tcp://";
        private static ConcurrentQueue<LogDto> LogDtos = new ConcurrentQueue<LogDto>();

        public JFJTLiveService()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            Task.Run(() =>
            {
                InitAll();
                _ReceiveMQService.OnReceiveSubscriber += IterativeReceiveCommandsFromServer;
                _ReceiveMQService.StartSubscriber<CommandDto>(_localSettings.FactoryCode);
                Thread.Sleep(1000);
                RerunCommands();
            });

            Task.Factory.StartNew((objState) =>
            {
                SubmitLogs();
            }, _SubmitCTS.Token, TaskCreationOptions.LongRunning);
        }

        protected override void OnStop()
        {
            LogHelper.SetLogError($"服务于{DateTime.Now.ToLocalTime()}停止.");
            var cps = ChannelPusherService.ChannelPushers;
            for (int i = cps.Count - 1; i >= 0; i--)
            {
                cps[i].CTS.Cancel();
            }
            _SubmitCTS.Cancel();
            Thread.Sleep(1500);
            LogDtos = new ConcurrentQueue<LogDto>();
        }

        #region 初始化
        public void InitAll()
        {
            try
            {
                //Init local settings
                _localSettings = LocalSettings.GetLocalSettings();
                receiveAddress = receiveAddress + _localSettings.ServerIP + ":" + _localSettings.ReceivePort;
                requestAddress = requestAddress + _localSettings.ServerIP + ":" + _localSettings.RequestPort;
                _ReceiveMQService = new NetMQService(receiveAddress, requestAddress);
                _SubmitMQService = new NetMQService(receiveAddress, requestAddress);
            }
            catch (Exception ex)
            {
                LogHelper.SetLogError($"本地资源初始化:{ex.ToString()}");
            }

            try
            {
                FFmpegBinariesHelper.RegisterFFmpegBinaries();
                PushAMsgIn(Guid.Empty, Guid.Empty, "服务日志: FFMPeg加载成功.", LiveStateType.Started);
            }
            catch (Exception ex)
            {
                PushAMsgIn(Guid.Empty, Guid.Empty, $"服务日志: FFMPeg加载失败.{ex.ToString()}", LiveStateType.Error);
                return;
            }
        }
        #endregion
        #region NetMQ相关方法
        /// <summary>
        /// Receive commands
        /// </summary>
        /// <param name="obj"></param>
        private void IterativeReceiveCommandsFromServer(ISocketData obj)
        {
            var command = obj as CommandDto;
            ExecuteCommand(command);
        }

        public void PushAMsgIn(Guid channeId, Guid Id, string content, LiveStateType liveStateType)
        {
            LogDtos.Enqueue(
            new LogDto()
            {
                ChanneId = channeId,
                FactoryCode = _localSettings.FactoryCode,
                Id = Id,
                LogContent = content,
                LogType = liveStateType
            }
            );
        }

        public void SubmitLogs()
        {
            try
            {
                var timer = new NetMQTimer(TimeSpan.FromSeconds(1));
                timer.Elapsed += (s, e1) =>
                {
                    if (_SubmitCTS.IsCancellationRequested)
                    {
                        e1.Timer.Enable = false;
                        return;
                    }
                    if (LogDtos.Count > 0)
                    {
                        LogDto logDto = new LogDto();

                        for (int i = 0; i < LogDtos.Count; i++)
                        {
                            var isGet = LogDtos.TryDequeue(out logDto);
                            if (isGet)
                            {
                                if (logDto.LogType == LiveStateType.Pushing)
                                {
                                    LogHelper.SetLogInfo($"{logDto.LogContent}=={logDto.Id} =={logDto.LogType}");
                                }
                                else
                                {
                                    LogHelper.SetLogError($"{logDto.LogContent}=={logDto.Id} =={logDto.LogType}");
                                }
                                _SubmitMQService.RequestSendCommandLog(logDto);
                            }
                        }
                    }
                };
                using (var poller = new NetMQPoller { timer })
                {
                    poller.Run();
                }
            }
            catch (Exception ex)
            {
                LogHelper.SetLogError($"服务日志:{ex.ToString()}");
            }
        }
        #endregion
        #region 推流相关方法
        public void ExecuteCommand(CommandDto command)
        {
            switch (command.CommandType)
            {
                case CommandTypeEnum.Start:
                    StartPush(command);
                    break;

                case CommandTypeEnum.Stop:
                    StopPush(command);
                    break;
                case CommandTypeEnum.Switch:
                case CommandTypeEnum.ReStart:
                    RestartPush(command);
                    break;
                default:
                    PushAMsgIn(Guid.Empty, Guid.Empty, $"服务日志: 错误的命令类型,请检查!{command.ToString()}", LiveStateType.Error);
                    break;
            }
        }
        public void StartPush(CommandDto commandDto)
        {
            //压入推送对象进入推送列表
            ChannelPusher channelPusher = ChannelPusherService.PushChannelIntoWorkingList(commandDto);
            if (channelPusher != null)
            {
                Task.Factory.StartNew((objStat) =>
                {
                    try
                    {
                        PushAMsgIn(channelPusher.CommandDto.ChanneId, channelPusher.CommandDto.Id, $"推流日志: 开始推送成功.", LiveStateType.Started);
                        using (var ffPublisher = new FFmpegPublisher(channelPusher.CommandDto))
                        {
                            int countTimes = 0;
                            ffPublisher.OnFinishedWritePktEvent += (retId, command) =>
                            {
                                //推流速度太快, 采取30条提取一条.
                                ++countTimes;
                                if (countTimes >= 30)
                                {
                                    if (retId == 0)
                                    {
                                        PushAMsgIn(command.ChanneId, command.Id, $"推流日志: 推送Pack成功.", LiveStateType.Pushing);
                                    }
                                    else
                                    {
                                        PushAMsgIn(command.ChanneId, command.Id, $"推流日志: 推送Pack失败, 返回值:{FFmpegHelper.GetStrError(retId)}.", LiveStateType.Error);
                                    }
                                    countTimes = 0;
                                }
                            };
                            ffPublisher.OnErrorEvent += (errorId, command, errorMsg) =>
                              {
                                  PushAMsgIn(command.ChanneId, command.Id, $"推流日志:推流异常, 异常ID:{errorId} 异常信息: {errorMsg }", LiveStateType.Error);
                              };
                            ffPublisher.OnPushStopedEvent += command =>
                            {
                                PushAMsgIn(command.ChanneId, command.Id, $"推流日志:停止推流.", LiveStateType.Stoped);
                            };
                            ffPublisher.OnStartCompletedEvent += command =>
                            {
                                PushAMsgIn(command.ChanneId, command.Id, $"推流日志:拉流/推流初始化成功,即将开始发送数据包.", LiveStateType.Started);
                            };
                            //启动推送
                            ffPublisher.Start(channelPusher.CTS.Token);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.SetLogError($"推流日志:{ex.ToString()}");
                    }
                }, channelPusher.CTS.Token, TaskCreationOptions.LongRunning);

                if (channelPusher.CommandDto.CommandType == CommandTypeEnum.Start)
                {
                    //Write commands to local file.
                    SerializationCommangs();
                }
            }
            else
            {
                PushAMsgIn(commandDto.ChanneId, commandDto.Id, $"推流日志: 频道正在推送中,未执行此命令.", LiveStateType.Error);
            }
        }
        public void StopPush(CommandDto commandDto)
        {
            ChannelPusher channelPusher = ChannelPusherService.GetChannelPusherById(commandDto);
            if (channelPusher != null)
            {
                channelPusher.CTS.Cancel();
                PushAMsgIn(commandDto.ChanneId, commandDto.Id, $"推流日志:发送停止命令完毕.", LiveStateType.Stoped);

                ChannelPusherService.RemoveChannelPusher(channelPusher);
                //重新序列化命令组
                SerializationCommangs();
            }
        }

        public void RestartPush(CommandDto commandDto)
        {
            StopPush(commandDto);
            Thread.Sleep(commandDto.CommandType == CommandTypeEnum.Switch ? 500 : 2000);
            StartPush(commandDto);
        }
        public void RerunCommands()
        {
            try
            {
                List<CommandDto> Commands = new List<CommandDto>();
                Commands = JsonConvert.DeserializeObject<List<CommandDto>>(LocalFileHelper
                .GetLocalJson(LocalFileHelper._commandsFile));

                if (Commands?.Count > 0)
                {
                    PushAMsgIn(Guid.Empty, Guid.Empty, $"服务日志: Rerun {Commands.Count} commands.", LiveStateType.Started);
                    foreach (var command in Commands)
                    {
                        ExecuteCommand(command);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.SetLogError($"推流日志: {ex.ToString()}");
            }
        }

        public void SerializationCommangs()
        {
            List<CommandDto> commandDtos = ChannelPusherService.ChannelPushers.Select(p => p.CommandDto).ToList();
            LocalFileHelper.SerializationCommands(JsonHelper.ToJSON(commandDtos));
        }
        #endregion  
    }
}
