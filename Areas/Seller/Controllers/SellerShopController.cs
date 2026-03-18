using Bake.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Bake.Areas.Seller.ViewModels;
using Bake.Models.Sales;
using Microsoft.AspNetCore.Hosting;

namespace Bake.Areas.Seller.Controllers

{
    [Area("Seller")]
    public class SellerShopController : Controller
    {
        private readonly BakeContext _context;
        private readonly IWebHostEnvironment _env;

        public SellerShopController(BakeContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        //對應到店鋪資料
        //GET:Seller/SellerShop/Shop
        [HttpGet]
        public IActionResult Shop()
        {
            int? userId = GetCurrentUserIdFromEmailClaim();

            if (userId == null)
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            var shop = _context.Shops.FirstOrDefault(x => x.UserId == userId.Value);

            if (shop == null)
            {
                TempData["SuccessMessage"] = "請先完成店鋪設定後再預覽";
                return RedirectToAction(nameof(Shop_settings));
            }

            return View(shop);
        }

        // Get:/Seller/SellerShop/Shop_settings
        [HttpGet]
        public IActionResult Shop_settings()
        {
            int? userId = GetCurrentUserIdFromEmailClaim();

            if (userId == null)
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            var shop = _context.Shops.FirstOrDefault(x => x.UserId == userId.Value);

            var vm = new SellerShopSettingsReadViewModel();

            if (shop != null)
            {
                vm.ShopName = shop.ShopName;
                vm.ShopDescription = shop.ShopDescription;
                vm.ShopImg = shop.ShopImg;
                vm.StatusId = shop.StatusId;

                vm.FacebookUrl = shop.FacebookUrl;
                vm.InstagramUrl = shop.InstagramUrl;
                vm.YoutubeUrl = shop.YoutubeUrl;
                vm.PinterestUrl = shop.PinterestUrl;
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Shop_settings(SellerShopSettingsReadViewModel vm)
        {
            int? userId = GetCurrentUserIdFromEmailClaim();

            if (userId == null)
            {
                return RedirectToAction("Login", "Home", new { area = "" });
            }

            var shop = _context.Shops.FirstOrDefault(x => x.UserId == userId.Value);

            // 若驗證失敗，補回目前圖片與狀態，避免頁面回來時顯示資料不完整
            if (!ModelState.IsValid)
            {
                if (shop != null)
                {
                    vm.ShopImg = shop.ShopImg;
                    vm.StatusId = shop.StatusId;
                }

                return View(vm);
            }

            // 若找不到就用原本的
            if (shop == null)
            {
                shop = new Shop
                {
                    UserId = userId.Value,
                    ShopName = vm.ShopName.Trim(),
                    ShopDescription = vm.ShopDescription?.Trim(),
                    ShopImg = "ProductPicture/NoImage.jpg",
                    ShopRating = 0m,
                    ShopTime = DateTime.Now,
                    SellerApprovedAt = DateTime.Now,
                    StatusId = 1,
                    FacebookUrl = vm.FacebookUrl?.Trim(),
                    InstagramUrl = vm.InstagramUrl?.Trim(),
                    YoutubeUrl = vm.YoutubeUrl?.Trim(),
                    PinterestUrl = vm.PinterestUrl?.Trim()
                };

                _context.Shops.Add(shop);
            }
            else
            {
                // 更新現有店鋪資料
                shop.ShopName = vm.ShopName.Trim();
                shop.ShopDescription = vm.ShopDescription?.Trim();
                shop.FacebookUrl = vm.FacebookUrl?.Trim();
                shop.InstagramUrl = vm.InstagramUrl?.Trim();
                shop.YoutubeUrl = vm.YoutubeUrl?.Trim();
                shop.PinterestUrl = vm.PinterestUrl?.Trim();
            }

            // 圖片存到 wwwroot/ProductPicture/Shop
            if (vm.ShopImageFile != null && vm.ShopImageFile.Length > 0)
            {
                string? imagePath = SaveShopImage(vm.ShopImageFile);

                if (imagePath == null)
                {
                    ModelState.AddModelError("ShopImageFile", "只允許上傳 jpg、jpeg、png、webp 圖片");
                    vm.ShopImg = shop.ShopImg;
                    vm.StatusId = shop.StatusId;
                    return View(vm);
                }

                shop.ShopImg = imagePath;
            }
            _context.SaveChanges();

            TempData["SuccessMessage"] = "店鋪資料已儲存";
            return RedirectToAction(nameof(Shop_settings));
        }

        // 功用：提供店鋪預覽 modal 載入用的 Partial HTML
        // 這一步只顯示「已儲存」資料，不顯示未儲存表單內容
        [HttpGet]
        public IActionResult PreviewPartial()
        {
            int? userId = GetCurrentUserIdFromEmailClaim();

            if (userId == null)
            {
                return Content("<div class='alert alert-warning mb-0'>請先登入後再預覽。</div>", "text/html");
            }

            var shop = _context.Shops.FirstOrDefault(x => x.UserId == userId.Value);

            if (shop == null)
            {
                return Content("<div class='alert alert-warning mb-0'>目前找不到店鋪資料，請先完成設定並儲存。</div>", "text/html");
            }

            // 明確指定全站共用 Partial 路徑，避免 Area 找 View 時混淆
            return PartialView("~/Views/Shared/_ShopProfilePartial.cshtml", shop);
        }

        public IActionResult Billing_account()
        {
            return View();
        }

        // 用目前登入者Email反查UserId
        private int? GetCurrentUserIdFromEmailClaim()
        {
            string? currentEmail = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrWhiteSpace(currentEmail))
            {
                return null;
            }

            var user = _context.AccountAuths.FirstOrDefault(x => x.Email == currentEmail);

            return user?.UserId;
        }

        // 功用：儲存店鋪頭像到 wwwroot/ProductPicture/Shop
        // 成功時回傳 DB 要存的相對路徑
        // 失敗時回傳 null
        private string? SaveShopImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png"};
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return null;
            }

            // 實體資料夾位置：wwwroot/ProductPicture/Shop
            string folderPath = Path.Combine(_env.WebRootPath, "ProductPicture", "Shop");
            Directory.CreateDirectory(folderPath);

            // 用 Guid 避免重名覆蓋
            string fileName = $"{Guid.NewGuid()}{extension}";
            string fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // DB 存站內相對路徑
            return $"ProductPicture/Shop/{fileName}";
        }
    }
}