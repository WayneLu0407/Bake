using Microsoft.AspNetCore.Mvc;

namespace Bake.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
