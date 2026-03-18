using Bake.Data;
using Bake.Models;
using Bake.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Bake.Controllers
{
    public class AccountController : Controller
    {
        private readonly BakeContext _context = new BakeContext();
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Signup()
        {
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}
