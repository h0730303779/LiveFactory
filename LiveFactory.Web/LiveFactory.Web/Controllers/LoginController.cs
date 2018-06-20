using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveFactory.Web.Common;
using LiveFactory.Web.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LiveFactory.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly LiveDbContext _context;

        IIdentityManager _identityManager;
        public LoginController(LiveDbContext context,IIdentityManager identityManage)
        {
            _identityManager = identityManage;
            _context = context;
        }
        //private IMemoryCache _cache;
        //public LoginController(IMemoryCache memoryCache)
        //{
        //    _cache = memoryCache;
        //}
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Login(Users loginModel)
        {
            var result =  LoginJudgment(loginModel);
            await _identityManager.SignInAsync(new Microsoft.AspNetCore.Identity.IdentityUser() { Id=Guid.NewGuid().ToString(),PasswordHash = result.Data.Password.ToString(), UserName = result.Data.Account.ToString() });
            //if (result.Data != null && result.Success)
            //{
            //    string cacheKey = "key";
            //    Users result1;
            //    if (!_cache.TryGetValue(cacheKey, out result1))
            //    {
            //        result1 = result.Data;
            //        await _cache.Set(cacheKey, result1, new MemoryCacheEntryOptions()
            //        .SetAbsoluteExpiration(TimeSpan.FromDays(1)));
            //    }
            //    //await UserIdentityService.SignIn(new IdentityUserInfo() { Id = result.Data.Id.ToString(), UserName = result.Data.UserName, Name = result.Data.Name });
            //    return Json(result);
            //}
            if (result.Success)
            {
                return Redirect("/Admin/index");
            }
            else
            {
                return Redirect("/Login/index");
            }
        }
        public ResultDto<Users> LoginJudgment(Users param)
        {
            var user = _context.User.FirstOrDefault(b => b.Account == param.Account);
            if (user == null)
                return new ResultDto<Users>("用户不存在");
            
            //if (Crypto.VerifyHashedPassword(user.Password, param.Password)) {
            if (string.Equals(user.Password, param.Password)) {
                ResultDto<Users> result = new ResultDto<Users>();
                result.Data = user;
                result.Success = true;
                return result;
            }
            //return new ResultDto<Users>(user.MapTo<AdminUserDto>());
            else if (user.IsDisabled)
                return new ResultDto<Users>("用户已失效");
            else
                return new ResultDto<Users>("密码错误");
        }
    }
}