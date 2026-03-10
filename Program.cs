using Bake.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) //驗證身分證的關卡
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, option =>
{
    option.Cookie.Name = "UserLoginCookie";
    option.LoginPath = "/Home/Login";
});

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


app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
