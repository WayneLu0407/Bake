using Microsoft.AspNetCore.Mvc;

namespace Bake.Controllers
{
    public class CheckoutController : Controller
    {
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
