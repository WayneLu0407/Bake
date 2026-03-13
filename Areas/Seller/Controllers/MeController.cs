using AspNetCoreGeneratedDocument;
using Bake.Areas.Seller.Model;
using Bake.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Bake.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class MeController : Controller
    {
        private readonly BakeContext _context;
        public MeController(BakeContext context)
        {
            _context = context;
        }

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
        
        public async Task<IActionResult> ResetPassword(string email)
        {
            var model = await _context.AccountAuths.Select(m => new ReSetPasswordModel
            {
                Email = m.Email
                
            }).FirstOrDefaultAsync(m=>m.Email == email);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ReSetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if(model.Password == model.ConfirmPassword)
                {

                    var email = TempData["ResetEmail"] as string;
                    var user = _context.AccountAuths.FirstOrDefault(a=>a.Email == email);
                    if(user != null)
                    {
                        user.PasswordHash = model.Password;
                        _context.AccountAuths.Update(user);
                        _context.SaveChanges();
                        return RedirectToAction("Login", "Home", new { area = "" });
                    }
                    else
                    {
                        ModelState.AddModelError("", "找不到該帳號。");
                    }
                }
            }
            return View(model);
        }
        public IActionResult Password()
        {
            return View();
        }
        public IActionResult Password(PasswordModel model)
        {
            return View();
        }

        public IActionResult Verify_email()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
