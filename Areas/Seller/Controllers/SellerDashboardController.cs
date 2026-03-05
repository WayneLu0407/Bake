using Microsoft.AspNetCore.Mvc;

namespace Bake.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class SellerDashboardController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
