using AspNetCoreGeneratedDocument;
using Bake.Areas.Seller.Model;
using Bake.Areas.Seller.ViewModels;
using Bake.Data;
using Bake.Models.Platform;
using Bake.Models.Sales;
using Bake.Models.User;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BuyerOrderListViewModel = Bake.Areas.Seller.ViewModels.BuyerOrderListViewModel;

namespace Bake.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class MeController : Controller
    {
        private readonly BakeContext _context;
        
        private readonly IWebHostEnvironment _env;
        public MeController(BakeContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            int? userId = GetCurrentUserIdFromClaim();

            if (userId == null)
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            var user = await _context.AccountAuths
                .AsNoTracking()
                .Include(x => x.UserProfile)
                .Include(x => x.RoleNavigation)
                .Include(x => x.AccountStatusNavigation)
                .Include(x => x.Shop)
                .FirstOrDefaultAsync(x => x.UserId == userId.Value);

            if (user == null)
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            var vm = new MemberDashboardViewModel
            {
                UserId = user.UserId,
                FullName = user.UserProfile?.FullName ?? user.UserName,
                //RoleName = user.RoleNavigation?.StatusName ?? string.Empty,
                IsEmailConfirmed = user.IsEmailConfirmed,
                Bio = user.UserProfile?.Bio,
                AvatarUrl = user.UserProfile?.AvatarUrl,

                //PostCount = await _context.Posts
                //    .AsNoTracking()
                //    .CountAsync(p => p.AuthorId == user.UserId),

                //FollowersCount = await _context.Follows
                //    .AsNoTracking()
                //    .CountAsync(f => f.BefollowedId == user.UserId),

                //FollowingCount = await _context.Follows
                //    .AsNoTracking()
                //    .CountAsync(f => f.FollowerId == user.UserId),

                //HasShop = user.Shop != null,
                //ShopName = user.Shop?.ShopName
            };

            return View(vm);
        }

        private int? GetCurrentUserIdFromClaim()
        {
            string? userIdClaim = User.FindFirstValue("UserId");

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return null;
            }

            return userId;
        }

        private async Task<string?> SaveAvatarImageAsync(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return null;
            }

            string folderPath = Path.Combine(_env.WebRootPath, "ProfilePicture", "User");
            Directory.CreateDirectory(folderPath);

            string fileName = $"{Guid.NewGuid()}{extension}";
            string fullPath = Path.Combine(folderPath, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"ProfilePicture/User/{fileName}";
        }

        [Authorize]
        [HttpGet]
        public async Task<JsonResult> GetProfileEditJson()
        {
            int? userId = GetCurrentUserIdFromClaim();

            if (userId == null)
            {
                return Json(new { success = false, message = "請重新登入" });
            }

            var user = await _context.AccountAuths
                .AsNoTracking()
                .Include(x => x.UserProfile)
                .FirstOrDefaultAsync(x => x.UserId == userId.Value);

            if (user == null)
            {
                return Json(new { success = false, message = "找不到會員資料" });
            }

            var genderOptions = await _context.UserGenderStatusDefinitions
                .AsNoTracking()
                .OrderBy(x => x.StatusId)
                .Select(x => new
                {
                    value = x.StatusId,
                    text = x.StatusName
                })
                .ToListAsync();

            string avatarUrl = !string.IsNullOrWhiteSpace(user.UserProfile?.AvatarUrl)
                ? (user.UserProfile.AvatarUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                    ? user.UserProfile.AvatarUrl
                    : Url.Content("~/" + user.UserProfile.AvatarUrl.TrimStart('/')))
                : Url.Content("~/seller_assets/images/profile/profile-image.png");

            return Json(new
            {
                success = true,
                data = new
                {
                    userName = user.UserName,
                    fullName = user.UserProfile?.FullName ?? user.UserName,
                    bio = user.UserProfile?.Bio ?? "",
                    avatarUrl = avatarUrl,
                    genderOptions = genderOptions
                }
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateProfileJson(MemberDashboardEditViewModel vm)
        {
            int? userId = GetCurrentUserIdFromClaim();

            if (userId == null)
            {
                return Json(new { success = false, message = "請重新登入" });
            }

            var user = await _context.AccountAuths
                .Include(x => x.UserProfile)
                .FirstOrDefaultAsync(x => x.UserId == userId.Value);

            if (user == null)
            {
                return Json(new { success = false, message = "找不到會員資料" });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "資料格式不正確" : e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, errors = errors });
            }

            user.UserName = vm.FullName.Trim();

            if (user.UserProfile == null)
            {
                user.UserProfile = new UserProfile
                {
                    UserId = user.UserId,
                    FullName = vm.FullName.Trim(),
                    AvatarUrl = "seller_assets/images/profile/profile-image.png",
                    Bio = string.IsNullOrWhiteSpace(vm.Bio) ? null : vm.Bio.Trim(),
                };

                _context.UserProfiles.Add(user.UserProfile);
            }
            else
            {
                user.UserProfile.FullName = vm.FullName.Trim();
                user.UserProfile.Bio = string.IsNullOrWhiteSpace(vm.Bio) ? null : vm.Bio.Trim();
            }

            if (vm.AvatarFile != null && vm.AvatarFile.Length > 0)
            {
                string? avatarPath = await SaveAvatarImageAsync(vm.AvatarFile);

                if (avatarPath == null)
                {
                    return Json(new
                    {
                        success = false,
                        errors = new[] { "只允許上傳 jpg、jpeg、png 圖片" }
                    });
                }

                user.UserProfile!.AvatarUrl = avatarPath;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "會員資料已更新" });
        }

        public IActionResult Favorites()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Orders(int pageNumber = 1, bool isAjax = false)
        {
            int pageSize = 10;
            var userId = User.FindFirst("UserId")?.Value;
            //避免他人抓到別人的訂單
            if (!int.TryParse(userId, out var currentUserId)) return RedirectToAction("Login", "Home", new { area = "" });

            var reviewedSet = (await _context.ProductReviews
            .Where(r => r.UserId == currentUserId)
            .Select(r => $"{r.OrderId}_{r.ProductId}")
            .ToListAsync())
            .ToHashSet(); //不重複抓取

            var query = _context.OrderItems
                .Where(x => x.Order.UserId == int.Parse(userId))
                .GroupBy(o => o.OrderId);

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var orderList = await query
                .OrderByDescending(g => g.Key)
                .Skip((pageNumber -1) * pageSize)
                .Take(pageSize)
                .Select(g => new BuyerOrderListViewModel
                {
                    OrderId = g.Key,
                    // 取分組中第一筆的訂單共用資訊
                    ShippingAddress = g.First().Order.ShippingAddress,
                    TotalAmount = g.First().Order.TotalAmount,
                    PaymentMethod = g.First().Order.PaymentMethod.Name,
                    CreatedAt = g.First().Order.CreatedAt,
                    StatusName = g.First().Order.Status.StatusName,
                    ProductsList = g.First().Order.OrderItems.Select(oi => new Item
                    {
                        ProductId = oi.ProductId, // 評論需要
                        ProductName = oi.Product.ProductName, // 從訂單項目關聯到產品名稱
                        Quantity = oi.ItemQuantity,
                        IsReviewed = false, 
                    }).ToList()
                })
                .ToListAsync();

            foreach (var order in orderList) // 評論需要 不能寫裡面因為SQL讀不到
            {
                foreach (var item in order.ProductsList)
                {
                    
                    if (reviewedSet.Contains($"{order.OrderId}_{item.ProductId}"))
                    {
                        item.IsReviewed = true; 
                    }
                }
            }

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;

            if (isAjax)
            {
                return PartialView("_BuyerOrderTablePartial", orderList);
            }

            return View(orderList);
        }

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
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            
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
                        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
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
        public IActionResult SwitchSeller()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SwitchSellerAsync(BankAccountModel model )
        {
            if (!ModelState.IsValid)   return View(model);
            
            var userId = User.FindFirstValue("UserId");
            var users = await _context.AccountAuths.FindAsync(int.Parse(userId));
            using(SHA256 sha256  = SHA256.Create())
            {
                byte[] InputAccount = Encoding.UTF8.GetBytes(model.BankAccount);
                byte[] HashAccount = sha256.ComputeHash(InputAccount);

                _context.UserPaymentSecrets.Add(new UserPaymentSecret
                {
                    UserId = int.Parse(userId),
                    EncryptedBankAcc = HashAccount 
                });
                
                users.IsSeller = true;
                users.Role = 1;
                await _context.SaveChangesAsync();
            }
            

            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, User.Identity.Name),
                new Claim(ClaimTypes.Role, "Seller")  // 動態新增賣家身分
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // 3. 關鍵：執行 SignInAsync，這會覆蓋舊的 Cookie，讓權限立即生效
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Dashboard", "Me", new { area="Seller"});
            
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult BankAccountChange()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BankAccountChangeAsync(BankAccountChangeModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var userId = User.FindFirstValue("UserId");
            var users = await _context.AccountAuths.Include(a=>a.UserPaymentSecret).FirstOrDefaultAsync(a=>a.UserId == int.Parse(userId));
            
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] InputAccount = Encoding.UTF8.GetBytes(model.BankAccount);
                byte[] HashAccount = sha256.ComputeHash(InputAccount);
                users.UserPaymentSecret.EncryptedBankAcc = HashAccount;
                _context.UserPaymentSecrets.Update(users.UserPaymentSecret);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Dashboard", "Me", new { area = "Seller" });
        }
    }
}




