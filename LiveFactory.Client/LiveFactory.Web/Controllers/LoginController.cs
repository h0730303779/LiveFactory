using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JFJT.Authorize.Identity;
using LiveFactory.Application;
using LiveFactory.Application.Base;
using LiveFactory.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using JFJT.Authorize.IJwtTokenManager;

namespace LiveFactory.Web.Controllers
{
    public class LoginController : Controller
    {
        public readonly IUserService _IUserService;
        public readonly IIdentityManager _identityManager;
        private IHttpContextAccessor _accessor;
        public LoginController(IUserService userService, IIdentityManager identityManager, IHttpContextAccessor accessor)
        {
            _IUserService = userService;
            _identityManager = identityManager;
            _accessor = accessor;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ResultDto<UserDto>> Login(UserDto loginModel)
        {
            string IpAddres = _accessor.HttpContext.Connection.LocalIpAddress.ToString();
            loginModel.LastTime = DateTime.Now;
            loginModel.LastIP = IpAddres;
            var result = _IUserService.Login(loginModel);
            if (result.Success)
            {
                await _identityManager.SignInAsync(new IdentityUser() { Id = result.Data.Id.ToString(), UserName = result.Data.Account.ToString(), NickName = result.Data.Account });
            }
            return result;
        }

        public ActionResult LoginOut()
        {
            //_authenticationManager.SignOut();
            _identityManager.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}