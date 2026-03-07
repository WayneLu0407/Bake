using Bake.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Bake.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Get([FromBody] List<CartViewModel> data) 
        {

            return Ok();
        }
    }
}
