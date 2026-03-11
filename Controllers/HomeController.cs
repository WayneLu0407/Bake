using Bake.Data;
using Bake.Helper;
using Bake.Models;
using Bake.Models.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using System.Diagnostics;
using System.Security.Claims;
using static System.Collections.Specialized.BitVector32;

namespace Bake.Controllers;

public class HomeController : Controller
{
    private readonly BakeContext _context;  //DI 注入
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HomeController(BakeContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;  //HomeController 建構子 DI注入  
        _webHostEnvironment = webHostEnvironment;
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginAsync(LoginModel model)
    {
        var user = _context.AccountAuths.FirstOrDefault(x => x.Email == model.Account && x.PasswordHash == model.Password);  //把資料庫的資料找出來做比對

        if (ModelState.IsValid)
        {
            if (user == null) // 如果沒有資料為Null 則return 回登入畫面
            {
                return View();
            }



            var claims = new List<Claim>    //網站會員的身分證
        {
            new Claim(ClaimTypes.Name,model.Account),
            new Claim(ClaimTypes.Role, "Admin"),

        };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimPrincipal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal); //將身分證發給user





            return RedirectToAction("index", "home");   // 登入後 回首頁
        }
        return View(model);

    }
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> RegisterAsync(RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.Password == model.PasswordConfirm)
            {
                var isRegistered = _context.AccountAuths.Any(a => a.Email == model.Email);
                if (isRegistered)
                {
                    return View();
                }
                _context.AccountAuths.Add(new AccountAuth { UserName = model.Name, Email = model.Email, PasswordHash = model.Password, ConfirmationToken = model.Comfirm });
                _context.SaveChanges();

                //encrypt 加密
                var encrypted = AesHelper.Encrypt(model.Email);
                var encodedToken = System.Net.WebUtility.UrlEncode(encrypted);
                var url = "https://localhost:7285/Home/Verifyemail?token=" + encodedToken;
                var sh = new SmtpHelper();

                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Email", "RegisterEmail.html");
                var body = await System.IO.File.ReadAllTextAsync(filePath);
                body = body.Replace("{{VerifyUrl}}", url)
                           .Replace("{{UserEmail}}", model.Email);
                        
                await sh.SendEmailAsync(model.Email, "註冊會員", body);

                return RedirectToAction("Index", "Home");
            }
            
        }
        return View();
    }
    [HttpGet]
    public IActionResult Verifyemail(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("無效的驗證請求");
        }

        try
        {
            // 1. 使用你的 AesHelper 解密出 Email
            // 注意：如果網址 token 包含特殊字元，建議加密時使用 WebEncoders.Base64UrlEncode
            string decryptedEmail = AesHelper.Decrypt(token);

            // 2. 到資料庫尋找該使用者
            var user = _context.AccountAuths.FirstOrDefault(a => a.Email == decryptedEmail);

            if (user != null)
            {
                // 3. 修改驗證狀態 (假設你有一個 IsEmailConfirmed 欄位)
                user.IsEmailConfirmed = true; 

                // 或是清空 Token 表示已驗證
                user.ConfirmationToken = "Verified";
                
                _context.SaveChanges();

                ViewBag.Message = "電子郵件驗證成功！現在您可以登入。";
                return View(); // 回傳一個驗證成功的畫面
            }
            else
            {
                return NotFound("找不到該使用者");
            }
        }
        catch (Exception ex)
        {
            // 解密失敗（例如 Token 被亂改）會跳到這裡
            return BadRequest("驗證連結已失效或格式錯誤");
        }
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