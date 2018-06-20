using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiveFactory.Web.Common;
using LiveFactory.Web.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using static LiveFactory.Web.Common.VideoType;

namespace LiveFactory.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly LiveDbContext _context;
        public AdminController(LiveDbContext context)
        {
            _context = context;
        }
        //private IMemoryCache _cache;
        //public AdminController(IMemoryCache memoryCache)
        //{
        //    _cache = memoryCache;
        //}
        public IActionResult Index()
        {

            Dictionary<int, string> dict = new Dictionary<int, string>();
            foreach (PlayerType item in Enum.GetValues(typeof(PlayerType)))
            {
                dict.Add((int)item, description(item));
            }
            //description(PlayerType.flv2)
            ViewBag.dict = dict;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddChannles(LiveChannel channel)
        {
            string editState = Request.Form["editState"];
            var uploadfile = Request.Form.Files["Image"];
            string img = "";
            if (uploadfile != null&& uploadfile.Length>0)
            {
                var now = DateTime.Now;
                var filePath = string.Format("/uploads/images/{0}/", now.ToString("yyyyMMdd"));
                //string bas = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationBasePath; //获取到bin目录
                //string bas = AppContext.BaseDirectory;//获取到bin目录
                string bas = Directory.GetCurrentDirectory() + "/wwwroot";//获取服务器目录
                if (!Directory.Exists(bas + filePath))
                {
                    Directory.CreateDirectory(bas + filePath);
                }
                if (uploadfile != null)
                {//文件后缀
                    var fileExtension = Path.GetExtension(uploadfile.FileName);
                    var strDateTime = DateTime.Now.ToString("yyMMddhhmmssfff"); //取得时间字符串
                    var strRan = Convert.ToString(new Random().Next(100, 999)); //生成三位随机数

                    var saveName = strDateTime + strRan + fileExtension;
                    img = filePath + saveName;
                    using (FileStream fs = System.IO.File.Create(bas + filePath + saveName))
                    {
                        uploadfile.CopyTo(fs);
                        fs.Flush();
                        channel.Image = img;
                    }
                }
            }

            if (editState == "update")
            {
                LiveChannel data = _context.LiveChannels.Where(b => b.Id == channel.Id).FirstOrDefault();
                data.Name = channel.Name;
                data.Url = channel.Url;
                data.VideoType = channel.VideoType;
                if (img.Length > 0)
                {
                    data.Image = img;
                }
            }
            else
            {
                await _context.LiveChannels.AddAsync(channel);
            }
            await _context.SaveChangesAsync();
            return Redirect("/Admin/index");
        }
        public JsonResult GetList()
        {
            //foreach (PlayerType item in Enum.GetValues(typeof(VideoType)))
            //{
            //    //((int)item).Dump();
            //}
            var result = _context.LiveChannels.ToList();
            return Json(result);
        }
        public JsonResult Delete(int id)
        {
            var delQuery = _context.LiveChannels.Where(b => b.Id == id).FirstOrDefault();
            if (delQuery != null)
            {
                _context.LiveChannels.Remove(delQuery);
                _context.SaveChanges();
                return Json("{'Success':'True'}");
            }
            else
            {
                return Json("{'Success':'False'}");
            }
        }

        //public ResultDto<Users> GetUser(string key)
        //{
        //    ResultDto<Users> result = new ResultDto<Users>();
        //    Users timestamp = _cache.Get<Users>(key);
        //    if (timestamp != null)
        //    {
        //        result.Success = true;
        //        result.Data = timestamp;
        //    }
        //    else
        //    {
        //        result.Success = false;
        //        result.Message = "查不到数据！";
        //    }
        //    return result;
        //}
    }
}