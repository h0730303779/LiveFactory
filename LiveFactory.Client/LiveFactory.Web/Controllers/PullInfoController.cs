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
    public class PullInfoController : Controller
    {
        private readonly IPullInfoService _pullInfoService;

        public PullInfoController(IPullInfoService pullInfoService)
        {
            _pullInfoService = pullInfoService;
        }

        public IActionResult Index()
        {
            return View();
        }   

        public PageListResult<PullInfoDto> GetPullInfoPageList(PullInfoSearchDto pullInfoSearchDto)
        {
            var result = _pullInfoService.GetPullInfoPageList(pullInfoSearchDto);
            return result;
        }

        public void DeletePullInfoById(int id)
        {
            _pullInfoService.Delete(id);
        }

        public async Task InsertOrUpdate(PullInfoDto pullInfoDto)
        {
            await _pullInfoService.InsertOrUpdate(pullInfoDto);
        }

    }
}