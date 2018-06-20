using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JFJT.Framework.Application.Dto;
using LiveFactory.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveFactory.Web.Controllers
{
    [Authorize]
    public class PlayInfoController : Controller
    {
        private readonly IPlayInfoService _playInfoService;

        public PlayInfoController(IPlayInfoService playInfoService)
        {
            _playInfoService = playInfoService;

        }

        public IActionResult Index()
        {
            return View();
        }

        public PageListResult<PlayInfoDto> GetPlayInfoPageList(PlayInfoSearchDto playInfoSearchDto)
        {
            var result = _playInfoService.GetPlayInfoPageList(playInfoSearchDto);
            return result;
        }

        public void DeletePlayInfoById(int id)
        {
            _playInfoService.Delete(id);
        }

        public async Task InsertOrUpdate(PlayInfoDto playInfoDto)
        {
            await _playInfoService.InsertOrUpdate(playInfoDto);
        }


    }
}