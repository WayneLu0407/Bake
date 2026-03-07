using Microsoft.AspNetCore.Mvc;

namespace Bake.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index(string keyword)
        {
            ViewBag.Keyword = keyword;
            return View();
        }
    }
}
