using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveFactory.Application;
using LiveFactory.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveFactory.Web.Controllers
{
    [Authorize]
    public class UserManageController : Controller
    {
        public readonly IUserService _IUserService;
        public UserManageController(IUserService userService)
        {
            _IUserService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetList(UserSearchDto userSearch)
        {
            var result = _IUserService.GetUserPageList(userSearch);
            return Json(result);
        }
        public async Task InsertOrUpdate(UserDto userDto)
        {
            if (userDto.Id != null) {
                userDto.Password = Crypto.HashPassword(userDto.Password);
            }
            await _IUserService.InsertOrUpdate(userDto);
        }
        public void DeleteVideoById(Guid id)
        {
            _IUserService.DeleteById(id);
        }
    }
}