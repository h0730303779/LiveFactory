using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveFactory.Web.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveFactory.Web.Controllers
{
    [Authorize]
    public class MobileHomeController : Controller
    {
        private readonly LiveDbContext _context;
        public MobileHomeController(LiveDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.LiveChannels.ToList());
        }
    }
}