using Bake.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Bake.Areas.Seller.ViewModels;

namespace Bake.Areas.Seller.Controllers

{
    [Area("Seller")]
    public class SellerShopController : Controller
    {
        private readonly BakeContext _context;

        public SellerShopController(BakeContext context)
        {
            _context = context;
        }

        public IActionResult Shop()
        {
            return View();
        }

        // 抓店鋪資料
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
            }

            return View(vm);
        }

        public IActionResult Billing_account()
        {
            return View();
        }

        // 用登入的Cookie反推UserId
        // !!!!!這段是臨時測試用，之後功能穩定後可以刪掉!!!!!!!!!!!!!
        public IActionResult DebugCurrentSeller()
        {
            string? currentEmail = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrWhiteSpace(currentEmail))
            {
                return Content("目前沒有取得登入者 Email，可能尚未登入，或 Cookie 裡沒有 Name Claim。");
            }

            var user = _context.AccountAuths.FirstOrDefault(x => x.Email == currentEmail);

            if (user == null)
            {
                return Content($"有拿到 Claim Email = {currentEmail}，但在 AccountAuths 找不到對應使用者。");
            }

            return Content($"目前登入者 Email = {currentEmail}，UserId = {user.UserId}");
        }

        // 用登入的Cookie反推UserId
        // !!!!!這段是臨時測試用，之後功能穩定後可以刪掉!!!!!!!!!!!!!
        public IActionResult DebugCurrentShop()
        {
            int? userId = GetCurrentUserIdFromEmailClaim();

            if (userId == null)
            {
                return Content("目前無法取得登入者的 UserId。");
            }

            var shop = _context.Shops.FirstOrDefault(x => x.UserId == userId.Value);

            if (shop == null)
            {
                return Content($"目前賣家 UserId = {userId}，但還找不到 Shop 資料。");
            }

            return Content(
                $"目前賣家 UserId = {userId}\n" +
                $"ShopName = {shop.ShopName}\n" +
                $"ShopDescription = {shop.ShopDescription}\n" +
                $"ShopImg = {shop.ShopImg}\n" +
                $"StatusId = {shop.StatusId}"
            );
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
    }
}