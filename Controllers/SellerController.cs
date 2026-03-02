using Microsoft.AspNetCore.Mvc;

namespace Bake.Controllers
{
    public class SellerController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Orders()
        {
            return View();
        }
        public IActionResult Products()
        {
            return View();
        }
        public IActionResult Shop_settings()
        {
            return View();
        }
        public IActionResult Shop()
        {
            return View();
        }
    }
}
