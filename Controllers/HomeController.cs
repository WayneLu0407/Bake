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
    [ValidateAntiForgeryToken]
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
    [HttpPost]
    public async Task<IActionResult> RegisterAsync(RegisterModel model)
    {
        var isRegistered = _context.AccountAuths.Any(a => a.Email == model.Email);
        if (isRegistered)
        {
            return View();
        }
        _context.AccountAuths.Add(new AccountAuth {UserName = model.Name,  Email = model.Email, PasswordHash = model.Password,ConfirmationToken = model.Comfirm });
        _context.SaveChanges();
        
        //encrypt 加密
        var encrypted = AesHelper.Encrypt(model.Email);
        var encodedToken = System.Net.WebUtility.UrlEncode(encrypted);
        var url = "https://localhost:7285/Home/Verifyemail?token=" + encodedToken;
        var sh = new SmtpHelper();

        var body = $@"<!DOCTYPE html>
<html lang=""zh-Hant"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>歡迎加入我們！</title>
    <style>
        /* 響應式調整 */
        @media screen and (max-width: 600px) {{
            .container {{ width: 100% !important; border-radius: 0 !important; }}
            .content {{ padding: 20px !important; }}
            .button {{ width: 100% !important; text-align: center; }}
        }}
    </style>
</head>
<body style=""margin: 0; padding: 0; background-color: #f4f7f9; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;"">
    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
        <tr>
            <td align=""center"" style=""padding: 40px 0;"">
                <!-- 主容器 -->
                <table class=""container"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.1);"">
                    <!-- 頁首：品牌顏色與 Logo -->
                    <tr>
                        <td align=""center"" style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 20px;"">
                            <h1 style=""color: #ffffff; margin: 0; font-size: 28px; letter-spacing: 2px;"">歡迎加入！</h1>
                        </td>
                    </tr>

                    <!-- 主要內容區 -->
                    <tr>
                        <td class=""content"" style=""padding: 40px; color: #333333; line-height: 1.6;"">
                            <h2 style=""margin-top: 0; color: #4a4a4a;"">哈囉，新朋友！</h2>
                            <p style=""font-size: 16px;"">您的帳號已成功建立。我們很高興能邀請您一起開啟這段旅程。以下是您的帳號相關資訊：</p>
                            
                            <!-- 資訊卡片 -->
                            <div style=""background-color: #f8f9fa; border-left: 4px solid #667eea; padding: 15px; margin: 20px 0;"">
                                <p style=""margin: 5px 0;""><strong>註冊信箱：</strong> {model.Email}</p>
                                <p style=""margin: 5px 0;""><strong>會員編號：</strong> #8882024</p>
                            </div>

                            <p style=""font-size: 16px;"">現在您可以點擊下方按鈕，開始探索您的專屬儀表板並設定您的個人檔案。</p>
                            
                            <!-- 行動呼籲按鈕 (CTA) -->
                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""margin-top: 30px;"">
                                <tr>
                                    <td align=""center"" bgcolor=""#667eea"" style=""border-radius: 5px;"">
                                        <a href=""{url}"" target=""_blank"" class=""button"" style=""padding: 15px 30px; color: #ffffff; text-decoration: none; font-size: 18px; font-weight: bold; display: inline-block;"">立即登入帳號</a>
                                    </td>
                                </tr>
                            </table>

                            <hr style=""border: 0; border-top: 1px solid #eeeeee; margin: 40px 0;"">
                            <p style=""font-size: 14px; color: #777777;"">如果您有任何問題，請隨時聯絡我們的支援團隊。我們很樂意為您提供幫助！</p>
                        </td>
                    </tr>

                    <!-- 頁尾 -->
                    <tr>
                        <td align=""center"" style=""background-color: #f1f1f1; padding: 20px; font-size: 12px; color: #999999;"">
                            <p style=""margin: 5px 0;"">© 2026 SweetStack甜點棧. 版權所有</p>
                            <div style=""margin-top: 10px;"">
                                <a href=""#"" style=""color: #667eea; text-decoration: none; margin: 0 10px;"">隱私權政策</a> | 
                                <a href=""#"" style=""color: #667eea; text-decoration: none; margin: 0 10px;"">取消訂閱</a>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>
";
        await sh.SendEmailAsync(model.Email, "註冊會員",body);

        return RedirectToAction("Index", "Home");
        
        
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
                // user.IsEmailConfirmed = true; 

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