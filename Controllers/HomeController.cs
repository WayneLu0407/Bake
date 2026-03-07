using Bake.Data;
using Bake.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Bake.Controllers;

public class HomeController : Controller
{
    private readonly BakeContext _context;  //DI 注入

    public HomeController(BakeContext context)
    {
        _context = context;  //HomeController 建構子 DI注入  
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Support()
    {
        return View();
    }
    public IActionResult Cart()
    {
        return View();
    }
    public IActionResult Products()
    {
        return View();
    }
    public IActionResult Me()
    {
        return View();
    }
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    
    public  async Task<IActionResult> LoginAsync(LoginModel model)
    {
        var user = _context.AccountAuths.FirstOrDefault(x => x.Email == model.Account && x.PasswordHash == model.Password);  //把資料庫的資料找出來做比對
        if (user == null) // 如果沒有資料為Null 則return 回登入畫面
        {
            return View();
        }
        var claims = new List<Claim>    //網站會員的身分證
        {
            new Claim(ClaimTypes.Name,model.Account),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("Name","Wayne")
            
        };
        var identity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
        var claimPrincipal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal); //將身分證發給user

        return RedirectToAction("index","home");   // 登入後 回首頁
    }
    public IActionResult Register()
    {
        return View();
    }
    public IActionResult Forgot_password()
    {
        return View();
    }
    public IActionResult Posts()
    {
        return View();
    }
    public IActionResult Checkout()
    {
        return View();
    }
    public IActionResult Shop()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
