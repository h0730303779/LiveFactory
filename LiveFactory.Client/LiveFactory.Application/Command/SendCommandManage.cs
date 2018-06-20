using System;
using System.Linq;
using System.Text;
using JFJT.Framework.Extensions;
using JFJT.Framework.Utils;
using NetMQ;
using NetMQ.Sockets;
using Microsoft.Extensions.Configuration;
using LiveCommand.Common;
using System.Threading.Tasks;
using LiveCommand.Common.NetMQ;
using Microsoft.AspNetCore.SignalR;
using LiveFactory.Application.Command.Dto;

namespace LiveFactory.Application.Command
{
    public interface ISendCommand
    {
        void ExecuteCommand(CommandDto command);
    }
    public class SendCommandManage : ISendCommand
    {
        private NetMQService netMQService;
        IHubContext<SignalrHubs> _hubContext;
        ILiveChannelService _liveChannelService;
        public SendCommandManage(ILiveChannelService liveChannelService, IConfiguration configuration, IHubContext<SignalrHubs> hubContext)
        {
            _hubContext = hubContext;
            var publishAddress = configuration["AppSettings:PublishAddress"];
            var responesPort = configuration["AppSettings:ResponsePort"];
            netMQService = new NetMQService($"tcp://{publishAddress}", $"tcp://{responesPort}");
            netMQService.OnResponseReceivePush += NetMQService_OnResponseReceivePush;
            ResponseSocket();
        }

        private void NetMQService_OnResponseReceivePush(LogDto obj)
        {
            var clientdata = new HubSendLiveState() { ChanneId = obj.ChanneId, FactoryCode = obj.FactoryCode, State = obj.LogType };
            var first = LiveChannelStateManage.GetModel(obj.ChanneId);
            if (first == null || first.State != obj.LogType)
                _hubContext.Clients.All.SendAsync("ReceiveCommand", clientdata.ToJson());
            LiveChannelStateManage.UpdateState(obj.ChanneId, obj.LogType);
        }


        public void ExecuteCommand(CommandDto command)
        {
            try
            {
                netMQService.Publish(command.FactoryCode, command);
                LogHelper.Debug("命令已发送:" + command.ToJson());
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        public void ResponseSocket()
        {
            try
            {
                netMQService.ResponseReceive();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }
    }
}
