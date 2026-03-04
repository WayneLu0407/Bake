using Microsoft.AspNetCore.Mvc;

namespace Bake.Areas.Seller.Controllers
{
    public class SellerController : Controller
    {
        [Area("Seller")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Area("Seller")]
        public IActionResult New()
        {
            return View();
        }

        [Area("Seller")]
        public IActionResult Edit()
        {
            return View();
        }
        [Area("Seller")]
        public IActionResult Orders()
        {
            return View();
        }
        [Area("Seller")]
        public IActionResult Past()
        {
            return View();
        }
        [Area("Seller")]
        public IActionResult Shop_settings()
        {
            return View();
        }
        [Area("Seller")]
        public IActionResult Billing_account()
        {
            return View();
        }
        [Area("Seller")]
        public IActionResult Shop()
        {
            return View();
        }
    }
}
