
using Bake.Data;
using Bake.Helper;
using Bake.Hubs;
using Bake.Models;
using Bake.Models.User;
using Humanizer.Bytes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Identity.Client;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;

namespace Bake.Controllers;

public class HomeController : Controller
{
    private readonly BakeContext _context;  //DI 注入
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(BakeContext context, IWebHostEnvironment webHostEnvironment, IHttpClientFactory httpClientFactory)
    {
        _context = context;  //HomeController 建構子 DI注入  
        _webHostEnvironment = webHostEnvironment;
        _httpClientFactory = httpClientFactory;

    }
    

    public IActionResult Index()
    {
        return View();
    }
    //GET : /Home/ExchangeRate
    [HttpGet]
    public async Task<IActionResult> ExchangeRate()
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync(
            "https://open.er-api.com/v6/latest/USD");

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var ntdRate = doc.RootElement
                .GetProperty("rates")
                .GetProperty("TWD")
                .GetDecimal();

            return Ok(ntdRate);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(503, new
            {
                error = "無法連線到外部 API",
                type = ex.GetType().Name,
                message = ex.Message,
                inner = ex.InnerException?.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "未預期的錯誤",
                type = ex.GetType().Name,
                message = ex.Message,
                inner = ex.InnerException?.Message,
                stackTrace = ex.StackTrace
            });
        }
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
        
        var user = _context.AccountAuths.Include(u=>u.RoleNavigation).FirstOrDefault(x => x.Email == model.Account);  //把資料庫的資料找出來做比對

        if (ModelState.IsValid)
        {
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash)) // 如果沒有資料為Null 則return 回登入畫面  使用Bcrypt套件 做雜湊比對
            {
                TempData["AlertMessage"] = "密碼輸入錯誤!";
                return View();
            }

            if (user.IsEmailConfirmed)
            {
                var userProfile = _context.UserProfiles.FirstOrDefault(x => x.UserId == user.UserId);

                var claims = new List<Claim>    //網站會員的身分證
            {
                new Claim("UserId",user.UserId.ToString()),
                new Claim(ClaimTypes.Name,model.Account),
                new Claim(ClaimTypes.Role,user.RoleNavigation.StatusName),


            };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal); //將身分證發給user

                return RedirectToAction("index", "home");   // 登入後 回首頁
            }
            else
            {
                TempData["EmailConfirm"] = "請到Email信箱收取驗證信!";
            }
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
        TempData.Remove("SuccessMessage");
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]  
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
                var newAccount = new AccountAuth { UserName = model.Name, Email = model.Email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password) };

                    _context.AccountAuths.Add(newAccount);
                    _context.SaveChanges();
                    _context.UserProfiles.Add(new UserProfile { UserId = newAccount.UserId , FullName = model.Name });
                    _context.SaveChanges();

                //encrypt 加密
                var encrypted = AesHelper.Encrypt(model.Email);
                var encodedToken = System.Net.WebUtility.UrlEncode(encrypted);
                var url = "https://team1website-cze8h7ewahcdb5gm.westus3-01.azurewebsites.net/Home/Verifyemail?token=" + encodedToken;
                var sh = new SmtpHelper();

                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Email", "RegisterEmail.html");
                var body = await System.IO.File.ReadAllTextAsync(filePath);
                body = body.Replace("{{VerifyUrl}}", url)
                           .Replace("{{UserEmail}}", model.Email);
                     
                await sh.SendEmailAsync(model.Email, "註冊會員", body);

                TempData["SuccessMessage"] = "註冊成功，請去您的電子信箱查收驗證信以開通帳號。";

                return RedirectToAction("Index", "Home");
            }
            
        }
        return View(model);
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Forgot_passwordAsync(ForgetpasswordModel model)
    {
        if(ModelState .IsValid)
        {
            if(_context.AccountAuths.Any(a=>a.Email == model.Email))
                {
                var encrypted = AesHelper.Encrypt(model.Email);
                var encodedToken = System.Net.WebUtility.UrlEncode(encrypted);
                var url = "https://team1website-cze8h7ewahcdb5gm.westus3-01.azurewebsites.net/Home/ForgotPasswordVerifyEmail?token=" + encodedToken;
                var sh = new SmtpHelper();

                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Email", "ForgotPasswordEmail.html");
                var body = await System.IO.File.ReadAllTextAsync(filePath);
                body = body.Replace("{{VerifyUrl}}", url);
                           

                await sh.SendEmailAsync(model.Email, "更改密碼", body);

                TempData["ForgotPasswordMessage"] = "請至Email信箱收取驗證信，驗證後更改密碼！";

                return RedirectToAction("Index", "Home");
            }
        }
        return View(model);
    }
    public IActionResult ForgotPasswordVerifyEmail(string token)
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
                

                _context.SaveChanges();
                TempData["ResetEmail"] = decryptedEmail;
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

    [Route("Error/{statusCode?}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode)
    {
        // 判斷原始請求路徑
        var originalPath = HttpContext.Features
            .Get<Microsoft.AspNetCore.Diagnostics.IStatusCodeReExecuteFeature>()
            ?.OriginalPath ?? "";

        if (originalPath.StartsWith("/Seller", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Index", "Error", new { area = "Seller", statusCode });
        }
        return statusCode switch
        {
            404 => View("Error404"),
            500 => View("Error500"),
            _ => View("Error404")
        };
    }
}