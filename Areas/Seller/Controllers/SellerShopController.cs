using Microsoft.AspNetCore.Mvc;

namespace Bake.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class SellerShopController : Controller
    {
        public IActionResult Shop()
        {
            return View();
        }

        public IActionResult Shop_settings()
        {
            return View();
        }
        public IActionResult Billing_account()
        {
            return View();
        }
    }
}
