using Bake.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var BakeconnectionString = builder.Configuration.GetConnectionString("Bake");
builder.Services.AddDbContext<BakeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Bake")));

// Configure session state with a custom cookie name and a long timeout duration
builder.Services.AddSession(Options => {
    Options.Cookie.Name = "CartSession";
    Options.IOTimeout = TimeSpan.FromDays(10);
    Options.Cookie.IsEssential = true;
    Options.Cookie.HttpOnly = true;
    Options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  //要求cookie必須透過HTTPS連線傳送
});

builder.Services.AddControllersWithViews();

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

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthorization();

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
