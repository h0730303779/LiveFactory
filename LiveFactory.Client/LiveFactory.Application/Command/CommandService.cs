using LiveCommand.Common;
using LiveFactory.Core.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using JFJT.Framework.Domain.Repositories;
using LiveFactory.Core.Models;
using System.Linq;
using NetMQ.Sockets;
using NetMQ;
using Microsoft.Extensions.Configuration;
using JFJT.Framework.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using JFJT.Framework.Utils;
using JFJT.Framework;

namespace LiveFactory.Application.Command
{
    public interface ICommandService
    {
        //ConcurrentDictionary<string, ConcurrentQueue<CommandDto>> CommandDic { get; set; }
        void ExecuteCommand(CommandDto commandDto);

      
    }

    public class CommandService : ICommandService
    {
        IRepository<OperationInfo> _opertionRepository { get; set; }
        ISendCommand _netMQManage;


        public CommandService(IRepository<OperationInfo> opertionRepository, ISendCommand netMQManage)
        {
            _opertionRepository = opertionRepository;
            _netMQManage = netMQManage;
        }

        public void ExecuteCommand(CommandDto command)
        {

            _netMQManage.ExecuteCommand(command);
            _opertionRepository.InsertAsync(new OperationInfo() { Type = OperationType.Command, Remark = "直播云端命令:" + command.CommandType.ToEnumDesc(), ParamValue = command.ToJson() });
        }

      
    }
}
