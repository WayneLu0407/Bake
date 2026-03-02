using Microsoft.AspNetCore.Mvc;

namespace Bake.Controllers
{
    public class SupportController : Controller
    {
        public IActionResult Faq()
        {
            return View();
        }
        public IActionResult Order_lookup()
        {
            return View();
        }
    }
}
