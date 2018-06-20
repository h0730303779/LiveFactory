using System.Linq;
using LiveFactory.Web.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveFactory.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly LiveDbContext _context;
        public HomeController(LiveDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.LiveChannels.ToList());
        }
    }
}
