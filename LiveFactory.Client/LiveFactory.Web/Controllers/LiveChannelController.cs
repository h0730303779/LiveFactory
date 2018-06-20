using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JFJT.Framework.Application.Dto;
using JFJT.Framework.Domain.Uow;
using LiveFactory.Application;
using LiveFactory.Application.Command;
using LiveFactory.Core.Models.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LiveFactory.Web.Controllers
{
    [Authorize]
    public class LiveChannelController : Controller
    {
        public readonly ILiveChannelService _liveChannelService;
        public readonly IPullInfoService _pullInfoService;
        public readonly IPlayInfoService _playInfoService;


        public LiveChannelController(ILiveChannelService liveChannelService,
            IPullInfoService pullInfoService,
            IPlayInfoService playInfoService)
        {
            _liveChannelService = liveChannelService;
            _pullInfoService = pullInfoService;
            _playInfoService = playInfoService;
        }


        public IActionResult Index()
        {
            return View();
        }

        [UnitOfWork(IsDisabledSaveChange = true)]
        public PageListResult<LiveChannelDto> GetLiveChannelPageList(LiveChannelSearchDto liveChannelSearchDto)
        {
            var result = _liveChannelService.GetLiveChannelPageList(liveChannelSearchDto);
            return result;
        }
        public PageListResult<PullInfoDto> GetPullInfoPageList(PullInfoSearchDto pullInfoSearchDto)
        {
            if (!pullInfoSearchDto.ChannelId.HasValue)
            {
                return new PageListResult<PullInfoDto>();
            }
            var result = _pullInfoService.GetPullInfoPageList(pullInfoSearchDto);
            return result;
        }
        public PageListResult<PlayInfoDto> GetPlayInfoPageList(PlayInfoSearchDto playInfoSearchDto)
        {
            if (!playInfoSearchDto.ChannelId.HasValue)
            {
                return new PageListResult<PlayInfoDto>();
            }
            var result = _playInfoService.GetPlayInfoPageList(playInfoSearchDto);
            return result;
        }

        /// <summary>
        /// 删除频道、同时删除对应的拉流信息、播放信息
        /// </summary>
        /// <param name="id"></param>
        public void DeleteLiveChannelById(Guid id)
        {
            _liveChannelService.Delete(id);
            _pullInfoService.DeleteByChannelId(id);
            _playInfoService.DeleteByChannelId(id);
        }


        public async Task InsertOrUpdate(LiveChannelDto liveChannelDto)
        {
            await _liveChannelService.InsertOrUpdate(liveChannelDto);
        }

        public List<LiveChannelDto> GetLiveChannels(LiveChannelSearchDto liveChannelDto)
        {
            return _liveChannelService.GetLiveChannels(liveChannelDto);
        }

        public async Task<LiveChannelDto> GetModel(Guid id)
        {

            var result = await _liveChannelService.Get(id);
            return result;
        }
        public IActionResult Detail(Guid? id = null)
        {
            ViewBag.Id = id; ;
            return  View();
            //var result = await _liveChannelService.Get(id);
            //return View(result);
        }


        public IActionResult Manager()
        {
            return View();
        }

        public int GetLiveChannelCount(int state)
        {
            if (state == -1)
            {
                return _liveChannelService.GetLiveChannelCount();
            }
            else
            {
                var stateVal = (LiveCommand.Common.LiveStateType)state;
                return LiveChannelStateManage.StateData.Where(m => m.State == stateVal).ToList().Count;
            }
        }


    }
}