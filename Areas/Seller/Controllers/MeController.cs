using AspNetCoreGeneratedDocument;
using Bake.Areas.Seller.Model;
using Bake.Areas.Seller.ViewModels;
using Bake.Data;
using Bake.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

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

        //public IActionResult Settings()
        //{
        //    return View();
        //}
        [Authorize]
        public async Task<IActionResult> Settings()
        {
            string userAccount = User.Identity.Name ;
            if (string.IsNullOrEmpty(userAccount)) return RedirectToAction("Login", "Home");
            
            var user = _context.AccountAuths.FirstOrDefault(c=>c.Email == userAccount);
            if (user == null) return NotFound();

            var ViewModel = new SettingsViewModel
            {
                Name = user.UserName,
                Email = user.Email
            };
            
            return View(ViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _context.AccountAuths.FirstOrDefaultAsync(c => c.Email == model.Email);
            if( user == null )
            {
                return NotFound();
            }
            user.UserName = model.Name;
            user.Email = model.Email;
            user.PasswordHash = model.Password;
            
            _context.SaveChanges();
            return RedirectToAction("Dashboard","Me", new {area = "Seller"});
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
