using Microsoft.AspNetCore.Mvc;

namespace Bake.Controllers
{
    public class UserController : Controller
    {
        public IActionResult UserDetail(int id)
        {
            return View();
        }
        public IActionResult Posts()
        {
            return View();
        }
        public IActionResult Joined()
        {
            return View();
        }
        public IActionResult Hosted()
        {
            return View();
        }
        public IActionResult Events()
        {
            return View();
        }
    }
}
