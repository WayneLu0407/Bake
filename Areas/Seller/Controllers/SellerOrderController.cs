using Microsoft.AspNetCore.Mvc;

namespace Bake.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class SellerOrderController : Controller
    {
        public IActionResult Orders()
        {
            return View();
        }

        public IActionResult Past()
        {
            return View();
        }
    }
}
