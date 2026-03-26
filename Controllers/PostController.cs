using Microsoft.AspNetCore.Mvc;

namespace Bake.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PostList()
        {
            return View();
        }

        public IActionResult PostDetail()
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
