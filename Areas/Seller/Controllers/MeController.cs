using Microsoft.AspNetCore.Mvc;

namespace Bake.Areas.Seller.Controllers
{
    public class MeController : Controller
    {
        [Area("Seller")]
        public IActionResult Dashboard()
        {
            return View();
        }
        [Area("Seller")]
        public IActionResult Favorites()
        {
            return View();
        }
        [Area("Seller")]
        public IActionResult Orders()
        {
            return View();
        }
        [Area("Seller")]
        public IActionResult Settings()
        {
            return View();
        }
        [Area("Seller")]
        public IActionResult Profile()
        {
            return View();
        }
        [Area("Seller")]
        public IActionResult Password()
        {
            return View();
        }
        [Area("Seller")]
        public IActionResult Verify_email()
        {
            return View();
        }
    }
}
