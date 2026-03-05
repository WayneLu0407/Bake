using Microsoft.AspNetCore.Mvc;

namespace Bake.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
