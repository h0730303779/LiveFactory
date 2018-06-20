using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveCommand.Common;
using LiveCommand.Common.NetMQ;
using LiveFactory.Application;
using LiveFactory.Application.Command;
using LiveFactory.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LiveFactory.Web.Controllers
{
    public class LiveCommandController : Controller
    {

        ICommandService _commandService;
        ILiveChannelService _channelService;
        IPullInfoService _pullInfoService;
        //ILiveChannelManager _liveChannelManager;
        public LiveCommandController(ICommandService commandService, ILiveChannelService channelService, IPullInfoService pullInfoService
            //, ILiveChannelManager liveChannelManager
            )
        {
            _commandService = commandService;
            _channelService = channelService;
            _pullInfoService = pullInfoService;
            //_liveChannelManager = liveChannelManager;

            //_liveChannelManager.Data.AddRange(new List<string>() { "A", "B" });

        }
        ICommandService _commandServicetest;
        public ActionResult Test()
        {
            var a = _commandServicetest.ToString();
            return View();
        }
        public async Task<string> Command(ExecuteCommandRequest request)
        {
            var chandnel = await _channelService.Get(request.Id);
            if (chandnel == null)
                return "命令发送失败";
            List<CommandDto> data = new List<CommandDto>();
            if (request.CommandType == CommandTypeEnum.Stop)
            {
                data.Add(new CommandDto { ChanneId = request.Id, CommandType = request.CommandType, FactoryCode = chandnel.FactoryCode });
            }
            else
            {
                data = _pullInfoService.GetPullInfoPageList(new PullInfoSearchDto { ChannelId = request.Id, ResultCount = 999 })
                                                  .Items.Select(s => new CommandDto
                                                  {
                                                      ChanneId = request.Id,
                                                      CommandType = request.CommandType,
                                                      FactoryCode = chandnel.FactoryCode,
                                                      PullAddress = s.Url,
                                                      PushAddress = chandnel.PushUrl
                                                  }).ToList();

            }
            if (data.Count > 0)
            {
                var model = LiveChannelManager.Data.FirstOrDefault(m => m.Id == request.Id);
                if (model != null)
                {
                    _commandService.ExecuteCommand(data[model.PullIndex]);
                }
                else
                {
                    _commandService.ExecuteCommand(data.FirstOrDefault());
                }
                return "命令已发送";
            }
            else
            {
                return "命令发送失败,为查询到对应的数据";
            }

        }

        //public async Task UpdateState(LogDto logDto)
        //{
        //    await _channelService.UpdateState(logDto);
        //}
    }
}