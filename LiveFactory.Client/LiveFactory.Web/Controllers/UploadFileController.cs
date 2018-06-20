using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JFJT.Framework.Application.Dto;
using LiveFactory.Application;
using Microsoft.AspNetCore.Mvc;
using LiveFactory.Core;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using LiveFactory.Core.Models.Config;
using Microsoft.Extensions.Options;
using JFJT.Framework;

namespace LiveFactory.Web.Controllers
{
    [Authorize]
    public class UploadFileController : Controller
    {
        private FilesBaseHelper filesBaseHelper;
        //HttpPostedFileBase
        private readonly IOptions<AppSettings> _appSettings;
        public UploadFileController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            filesBaseHelper = new FilesBaseHelper(_appSettings);
        }
        public async Task<JsonResult> Index(IFormFile file, FileType filetype)
        {
            var validate = filesBaseHelper.Validate(file.FileName, file.Length, filetype, out string error);
            if (!validate)
            {
                throw new UserException(error);
            }

            var saveResult = filesBaseHelper.GetSaveFilePath(filetype, file.FileName);

            using (FileStream fs = System.IO.File.Create(saveResult.SaveDirectory))
            {
                await file.CopyToAsync(fs);
                fs.Flush();
            }

            return Json(new FileInfoResult() { FileName = saveResult.SaveFileName, FileType = filetype });
        }
    }
}