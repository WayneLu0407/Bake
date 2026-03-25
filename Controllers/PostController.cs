using Microsoft.AspNetCore.Mvc;

namespace Bake.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Latest()
        {
            return View();
        }
        public IActionResult Trending()
        {
            return View();
        }
        public IActionResult Events()
        {
            return View();
        }
        public IActionResult Apply()
        {
            return View();
        }
        public IActionResult Confirmed()
        {
            return View();
        }
        public IActionResult UserProfile()
        {
            return View();
        }
    }
}
