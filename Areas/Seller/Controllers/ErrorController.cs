using Microsoft.AspNetCore.Mvc;

namespace Bake.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class ErrorController : Controller
    {
        [Route("Seller/Error/{statusCode?}")]
        public IActionResult Index(int? statusCode)
        {
            return statusCode switch
            {
                404 => View("Error404"),
                500 => View("Error500"),
                _ => View("Error404"),
            };
        }
    }
}
