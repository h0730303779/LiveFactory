using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveFactory.Application.Command
{
    public class SignalrHubs : Hub
    {
        public async Task SendCommand(string message)
        {
            //前端主动调用
            await Clients.All.SendAsync("ReceiveCommand","你接受到了我的消息嘛你");
        }
    }
}
