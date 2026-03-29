using Bake.Data;
using Bake.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var BakeconnectionString = builder.Configuration.GetConnectionString("Bake"); // 和appsetting 連線字串相連
builder.Services.AddDbContext<BakeContext>(options =>
    options.UseSqlServer(BakeconnectionString));

// Configure session state with a custom cookie name and a long timeout duration
builder.Services.AddSession(Options => {
    Options.Cookie.Name = "CartSession";
    Options.IOTimeout = TimeSpan.FromDays(10);
    Options.Cookie.IsEssential = true;
    Options.Cookie.HttpOnly = true;
    Options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  //要求cookie必須透過HTTPS連線傳送
});

builder.Services.AddControllersWithViews(options =>
{
    // 更改模型綁定的預設錯誤訊息
    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
        x => $"'{x}' 必須是有效的數字。");
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) //驗證身分證的關卡
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, option =>
{
    option.Cookie.Name = "UserLoginCookie";
    option.LoginPath = "/Home/Login";
});

builder.Services.AddSignalR();//聊天室注入

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>(); // 小鈴鐺通知注入

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseSession(); //啟用Session中介軟體，讓應用程式能夠使用Session功能
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();  //登入驗證
app.UseAuthorization();   //登入授權

app.MapAreaControllerRoute(
    name: "MySellerArea",
    areaName: "Seller",
    pattern: "Seller/{controller=Default}/{action=Index}/{id?}");

app.MapHub<Bake.Hubs.ChatHub>("/chathub"); //聊天室Hubs服務
app.MapHub<NotificationHub>("/notificationHub");

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();



public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        var id = connection.User?.FindFirst("UserId")?.Value;
        
        // 確保這裡回傳的值，跟妳傳給 SendOrderNotify 的 userId 一致
        return id;
    }
}