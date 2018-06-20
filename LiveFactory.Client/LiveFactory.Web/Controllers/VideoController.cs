using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiveFactory.Web.Models;
using LiveFactory.Application;
using LiveFactory.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Qiniu.Http;
using Microsoft.Extensions.Options;
using LiveFactory.Core.Models.Config;
using NLog;
using JFJT.Framework.Utils;
using System.IO;
using JFJT.Framework.Extensions;

namespace LiveFactory.Web.Controllers
{
    [Authorize]
    public class VideoController : Controller
    {
        public readonly IVideoService _IVideoService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IOptions<AppSettings> _appSettings;
        private QiniuManage qinniu;
        public VideoController(IVideoService videoService, IHostingEnvironment hostingEnvironment, IOptions<AppSettings> appSettings)
        {
            _IVideoService = videoService;
            _hostingEnvironment = hostingEnvironment;
            _appSettings = appSettings;
            qinniu = new QiniuManage(_appSettings);
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetList(VideoSearchDto videoSearch)
        {
            var result = _IVideoService.GetVideoPageList(videoSearch);
            return Json(result);
        }
        public async Task InsertOrUpdate(VideoDto videoDto)
        {
            if (videoDto.IsUpload)
            {
                bool up = UploadQiniu(videoDto);
                videoDto.IsUpload = up;
            }
            await _IVideoService.InsertOrUpdate(videoDto);
        }

        public async Task DeleteVideoById(Guid id)
        {
            var videoData = await _IVideoService.GetVideo(id);
            if (videoData != null)
            {
                qinniu.DelFile(videoData.Name);
                //string url=qinniu.GetUrl(videoData.Name);
            }
            _IVideoService.DeleteById(id);
        }
        public bool UploadQiniu(VideoDto videoDto)
        {
            LogHelper.Debug("123123213213123");
            //var videoData = await _IVideoService.GetVideo(id);
            var isUpload = false;
            if (videoDto != null)
            {
                LogHelper.Debug("videoDto");
                var WebRootPath = _hostingEnvironment.ContentRootPath;
                HttpResult result = qinniu.UploadData(videoDto.Name, Path.Combine(WebRootPath, "wwwroot" )+ videoDto.Url);
                //详细状态码查看https://developer.qiniu.com/fusion/kb/1352/the-http-request-return-a-status-code，200执行成功
                LogHelper.Debug(result.Text + " ---------------------1------------------" + result.ToJson());
                if (result.Code == (int)HttpCode.OK)
                {
                    isUpload = true;
                    LogHelper.Debug(result.Text + " -------------------2--------------------" + result.RefText);
                }
            }
            LogHelper.Debug("123123213213123");
            return isUpload;
        }

        public List<VideoDto> GetVideo(VideoSearchDto videoDto)
        {
            return _IVideoService.GetVideo(videoDto);
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}