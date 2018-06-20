using LiveCommand.Common.NetMQ;
using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using NetMQ;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using LiveCommand.Common.NetMQ.Models;
using System.Collections.Concurrent;
using System.Linq;

namespace LiveCommand.Common
{
    public class NetMQService
    {

        private readonly IContainer _container;
        //ILiveChannelManager _liveChannelManager;
        public NetMQService(string publisherUrl, string responseUrl)
        {
            //_liveChannelManager = JFJT.Framework.Utils.ServiceProviderHelper.GetServiceAutofac<ILiveChannelManager>();
            var builder = new ContainerBuilder();
            builder.Register(c => new NetMQManage(publisherUrl, responseUrl)).As<INetMQManage>();
            //builder.RegisterType<LiveChannelManager>().As<ILiveChannelManager>().SingleInstance();
            _container = builder.Build();
            _netMQManage = _container.Resolve<INetMQManage>();

            StratListen();
        }
        INetMQManage _netMQManage;

        public static List<PublisherListen> _publisherListens = new List<PublisherListen>();
        #region 发布 订阅消息
        //public ConcurrentQueue<>
        public bool Publish(string topic, ISocketData sendDto)
        {
            var result = _netMQManage.Publish(topic, sendDto);
            _publisherListens.AddListens(topic, sendDto);
            return result;
        }

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public void StartSubscriber<T>(string topic, int Milliseconds = NetmqOptions.SubscriberMillisecond) where T : ISocketData
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var isCancel = cancellationTokenSource.Token.IsCancellationRequested;
                    var mqTime = new NetMQTimer(TimeSpan.FromMilliseconds(Milliseconds));
                    mqTime.Elapsed += (sender, e) =>
                    {
                        if (isCancel)
                        {
                            e.Timer.Enable = false;
                            return;
                        }
                        var result = _netMQManage.ReceiveSubscriber<T>(topic);

                        OnReceiveSubscriberEvent(result);

                    };
                    using (var poller = new NetMQPoller { mqTime })
                    {
                        poller.Run();
                    }
                }
                catch (Exception ex)
                {
                    LogWrite.Error(ex);
                }
            }, cancellationTokenSource.Token);
        }
        public void StopSubscriber()
        {
            cancellationTokenSource.Cancel();
        }
        #endregion

        #region 请求发送消息
        /// <summary>
        /// 发送普通日志
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IResponseData RequestSendLog(LogDto dto)
        {
            var request = new RequestData()
            {
                Id = dto.Id,
                RequestType = RequestType.ServiceLog,
                JsonData = JsonConvert.SerializeObject(dto)
            };
            var result = RequestSend(request);
            return result;
        }
        /// <summary>
        /// 发送视频流日志
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IResponseData RequestSendCommandLog(LogDto dto)
        {
            var request = new RequestData()
            {
                Id = dto.Id,
                RequestType = RequestType.PushLog,
                JsonData = JsonConvert.SerializeObject(dto)
            };
            var result = RequestSend(request);
            return result;

        }
        public IResponseData RequestSend(IRequestData request)
        {
            var result = _netMQManage.RequestReceive<ResponseData>(request);
            return result;
        }
        #endregion

        #region 收到消息
        public void ResponseReceive(int Milliseconds = NetmqOptions.ResopneMillisecond)
        {
            Task.Factory.StartNew((par) =>
            {
                try
                {
                    var listen = par as List<PublisherListen>;
                    var isCancel = cancellationTokenSource.Token.IsCancellationRequested;
                    var mqTime = new NetMQTimer(TimeSpan.FromMilliseconds(Milliseconds));
                    mqTime.Elapsed += (sender, e) =>
                    {
                        if (isCancel)
                        {
                            e.Timer.Enable = false;
                            return;
                        }

                        _netMQManage.ResponseReceive((a) =>
                        {
                            //收到消息后进行回调处理业务,处理完后在 反馈消息出去
                            var request = JsonConvert.DeserializeObject<RequestData>(a);
                            listen.ResultListens(request.Id);
                            return ResultResponse(request);
                        });

                    };
                    using (var poller = new NetMQPoller { mqTime })
                    {
                        poller.Run();
                    }
                }
                catch (Exception ex)
                {
                    LogWrite.Error(ex);
                }
            }, _publisherListens, cancellationTokenSource.Token);
        }
        private ResponseData ResultResponse(RequestData request)
        {
            switch (request.RequestType)
            {
                case RequestType.ServiceLog:
                    var log = JsonConvert.DeserializeObject<LogDto>(request.JsonData);
                    OnResponseReceiveLogEvent(log);
                    return new ResponseData()
                    {
                        Id = request.Id,
                        Success = true,
                        Message = "收到普通日志消息"
                    };
                case RequestType.PushLog:
                    var data = JsonConvert.DeserializeObject<LogDto>(request.JsonData);
                    OnResponseReceivePushEvent(data);
                    return new ResponseData()
                    {
                        Id = request.Id,
                        Success = true,
                        Message = "收到推流信息",
                        JsonData = request.JsonData,
                    };
                default:
                    //暂时未实现其他类型的操作通知
                    break;
            }
            return new ResponseData()
            {
                Success = false,
                Message = "未知消息"
            };
        }
        #endregion

        #region 监听发送命令的事件
        private void StratListen(int milliseconds = NetmqOptions.PolicyMillisecond)
        {
            Task.Factory.StartNew((par) =>
            {
                var listen = par as List<PublisherListen>;
                NetMQTimer netMQTimer = new NetMQTimer(milliseconds);
                netMQTimer.Elapsed += (a, b) =>
                {
                    foreach (var item in listen)
                    {
                        //检测发送的命令是否收到客户端回复 没有则持续发送出去
                        if (item.IsSendListens())
                        {
                            Publish(item.Topic, item.SendData);
                        }
                        else
                        {
                            //检测客户端是否断网 down 机了
                            if (item.IsListensServiceError())
                            {
                                var objdata = item.SendData as CommandDto;
                                OnResponseReceivePushEvent(new LogDto() { LogType = LiveStateType.ServiceError, ChanneId = objdata.ChanneId });
                                item.SendWebClientDate = DateTime.Now;
                            }

                        }


                    }
                    ChannelOperation();
                };
                using (var poller = new NetMQPoller() { netMQTimer })
                {
                    poller.Run();
                }
            }, _publisherListens);
        }
        #endregion



        public void ChannelOperation()
        {
            foreach (var channel in LiveChannelManager.Data)
            {
                var pullList = LiveChannelManager.PullData.Where(m => m.ChannelId == channel.Id).ToList();
                //切换拉流操作
                if (channel.PullInterval.HasValue)
                {
                    int hours = -1;
                    int min = -1;
                    if (channel.PullInterval.Value.Hours > 0)
                    {
                        //开始执行的时间-当前时间=间隔的小时  等于设置间隔小时的话那么久发送切换命令
                        hours = (DateTime.Now - channel.UpdatePullInfoTime).Hours;
                    }
                    if (channel.PullInterval.Value.Minutes > 0)
                    {
                        min = (DateTime.Now - channel.UpdatePullInfoTime).Minutes;
                    }
                    if (hours >= channel.PullInterval.Value.Hours || min >= channel.PullInterval.Value.Minutes)
                    {
                       
                        //有匹配的话那么开始时间重置为当前时间
                        channel.UpdatePullInfoTime = DateTime.Now;
                        if (channel.PullIndex >= (pullList.Count-1))
                        {
                            channel.PullIndex = 0;
                        }
                        else
                        {
                            //拉流下标记录+1
                            channel.PullIndex++;
                        }
                        CommandDto commandDto = new CommandDto()
                        {
                            FactoryCode = channel.FactoryCode,
                            ChanneId = channel.Id,
                            CommandType = CommandTypeEnum.Switch,
                            PullAddress = pullList[channel.PullIndex].Url,
                            PushAddress = channel.PushUrl
                        };
                        //发送切换拉流命令
                        _netMQManage.Publish(commandDto.FactoryCode, commandDto);
                    }
                }
                //切换频道操作
                if (channel.PlayInterval.HasValue)
                {

                    int hours = -1;
                    int min = -1;
                    if (channel.PlayInterval.Value.Hours > 0)
                    {
                        hours = (DateTime.Now - channel.StopTime).Hours;
                    }
                    if (channel.PlayInterval.Value.Minutes > 0)
                    {
                        min = (DateTime.Now - channel.StopTime).Minutes;
                    }
                    if (hours >= channel.PlayInterval.Value.Hours || min >= channel.PlayInterval.Value.Minutes)
                    {
                        channel.StopTime = DateTime.Now;
                        CommandDto commandDto = new CommandDto()
                        {
                            FactoryCode = channel.FactoryCode,
                            ChanneId = channel.Id,
                            CommandType = CommandTypeEnum.Stop,
                            PullAddress = pullList[channel.PullIndex].Url,
                            PushAddress = channel.PushUrl
                        };
                        //发送停止播放命令
                        _netMQManage.Publish(commandDto.FactoryCode, commandDto);
                    }

                }
            }
        }

        #region event
        /// <summary>
        /// 订阅消息收到后的回调事件
        /// </summary>
        public event Action<ISocketData> OnReceiveSubscriber;
        protected void OnReceiveSubscriberEvent(ISocketData receiveDto)
        {
            OnReceiveSubscriber?.Invoke(receiveDto);
        }

        /// <summary>
        /// 收到服务消息的事件
        /// </summary>
        public event Action<LogDto> OnResponseReceiveLog;
        protected virtual void OnResponseReceiveLogEvent(LogDto data)
        {
            OnResponseReceiveLog?.Invoke(data);
        }

        /// <summary>
        /// 收到推流消息的事件
        /// </summary>
        public event Action<LogDto> OnResponseReceivePush;
        protected virtual void OnResponseReceivePushEvent(LogDto data)
        {
            OnResponseReceivePush?.Invoke(data);
        }
        #endregion

        public void Cleanup()
        {
            cancellationTokenSource.Cancel();
            _netMQManage.Cleanup();
        }
    }
}
