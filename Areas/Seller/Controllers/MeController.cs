using Microsoft.AspNetCore.Mvc;

namespace Bake.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class MeController : Controller
    {
        
        public IActionResult Dashboard()
        {
            return View();
        }
        
        public IActionResult Favorites()
        {
            return View();
        }
        
        public IActionResult Orders()
        {
            return View();
        }
        
        public IActionResult Settings()
        {
            return View();
        }
        
        public IActionResult Profile()
        {
            return View();
        }
        
        public IActionResult Password()
        {
            return View();
        }
        
        public IActionResult Verify_email()
        {
            return View();
        }
    }
}
