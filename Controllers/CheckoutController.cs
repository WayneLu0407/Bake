using Microsoft.AspNetCore.Mvc;

namespace Bake.Controllers
{
    public class CheckoutController : Controller
    {
        [HttpPost]
        public IActionResult Info()
        {
            return View();
        }
        public IActionResult Payment()
        {
            return View();
        }
    }
}
