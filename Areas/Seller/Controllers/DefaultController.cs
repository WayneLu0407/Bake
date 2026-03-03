using Microsoft.AspNetCore.Mvc;

namespace Bake.Areas.Seller.Controllers
{
    public class DefaultController : Controller
    {
        [Area("Seller")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
