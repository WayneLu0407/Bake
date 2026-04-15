using Bake.Areas.Seller.ViewModels;
using Bake.Data;
using Bake.Hubs;
using Bake.Models;
using Bake.Models.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using OpenAI.Graders;
using OpenAI.Images;
using System.Numerics;
using System.Security.Claims;

namespace Bake.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = "Seller")]
    public class CouponController : Controller
    {
        private readonly BakeContext _bakeContext;
        private readonly IHubContext<NotificationHub> _hubContext;
        public CouponController(BakeContext bakeContext, IHubContext<NotificationHub> hubContext) 
        {
            _bakeContext = bakeContext;
            _hubContext = hubContext;
        }

        private int CurrentSellerId
        {
            get
            {
                var claimValue = User.FindFirst("UserId")?.Value
                              ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(claimValue, out int id))
                {
                    return id;
                }
                return 0;
            }
        }

        

        [HttpGet]
        public async Task<IActionResult> CouponManagement()
        {
            return View();
        }

        //Seller/Coupon/GetCoupon
        [HttpGet]
        public async Task<IActionResult> GetCoupon()
        {
            int sellerId = CurrentSellerId;

            var myCoupons =await _bakeContext.Coupons
                .Where(c => c.SellerId == sellerId)
                .OrderByDescending(c=>c.ExpirationDate)
                .ToListAsync();
            
            return Json(myCoupons);
        }

        //產生優惠券
        //Seller/Coupon/CreateCoupon
        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] CouponCreateViewModel model)
        {
            int sellerId = CurrentSellerId;

            if (!ModelState.IsValid) 
            {
                var errorMsg = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(errorMsg) || errorMsg.Contains("field is required"))
                {
                    errorMsg = "請檢查欄位，所有項目皆為必填";
                }

                return Json(new { success = false, message = errorMsg ?? "資料格式錯誤，請檢查輸入內容" });
            }

            

            if (model.DiscountAmount <= 0 || model.MinimumPurchase <= 0)
            {
                return Json(new { success = false, message = "折扣金額與最低消費皆不能為負數或零" });
            }

            if (model.DiscountAmount >= model.MinimumPurchase)
                return Json(new { success = false, message = $"折扣金額({model.DiscountAmount}元)，不得大於最低消費({model.MinimumPurchase})元" });

            if (model.ExpirationDate <= DateTime.Now)
                return Json(new { success = false, message = "到期日必須大於今天" });

            
             
            string couponCode = GenerateRandomCouponCode(8);
            var newCoupon = new Coupon
            {
                Code = couponCode,
                DiscountValue = model.DiscountAmount,
                MinimumPurchase = model.MinimumPurchase,
                ExpirationDate = model.ExpirationDate.Value,
                IsActive = true,
                SellerId = null
            };

            _bakeContext.Coupons.Add(newCoupon);
            await _bakeContext.SaveChangesAsync();

            await SendCouponNotify(newCoupon.CouponId.ToString());

            return Json(new { success = true, data = newCoupon});
        }

        //停用或啟用優惠券
        //Seller/Coupon/ToggleCouponStatus/{id}
        [HttpPost]
        public async Task<IActionResult> ToggleCouponStatus(int id)
        {
            var coupon = await _bakeContext.Coupons
                .FirstOrDefaultAsync(c=>c.CouponId == id && c.SellerId == CurrentSellerId);

            if (coupon == null)
                return Json(new { success = false, message = "查無優惠券" });

            coupon.IsActive = !coupon.IsActive;
            await _bakeContext.SaveChangesAsync();

            return Json(new { success=true, isActive = coupon.IsActive});
        }


        //刪除優惠券
        //Seller/Coupon/DeleteCoupon
        [HttpPost]
        public async Task<IActionResult> DeleteCoupon(int id) 
        {
            var coupon = await _bakeContext.Coupons
                .FirstOrDefaultAsync(c => c.CouponId == id && c.SellerId == CurrentSellerId);

            if(coupon==null)
                return Json(new { success = false, message = "查無優惠券" });

            _bakeContext.Coupons.Remove(coupon);
            await _bakeContext.SaveChangesAsync();

            return Ok();
        }

        //修改優惠券(只能把結束日期往後延)
        //Seller/Coupon/UpdateExpireDate
        [HttpPost]
        public async Task<IActionResult> UpdateExpireDate(int id, [FromBody] ExpirationUpdateModel model) 
        {
            if (model == null || !model.NewDate.HasValue) 
            {
                return Json(new { success = false, message = "請選擇一個有效日期" });
            }

            var coupon = await _bakeContext.Coupons
                .FirstOrDefaultAsync(c => c.CouponId == id && c.SellerId == CurrentSellerId);

            if (coupon == null)
                return Json(new { success = false, message = "找不到優惠券" });

            if (model.NewDate.Value <= DateTime.Now)
                return Json(new { success = false, message = "新的日期必須大於今日" });

            //更新日期
            coupon.ExpirationDate = model.NewDate.Value;
            await _bakeContext.SaveChangesAsync();

            return Json(new { success = true, newDate= coupon.ExpirationDate.ToString("yyyy-MM-dd")});
        }



        private string GenerateRandomCouponCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public async Task SendCouponNotify( string couponId)
        {
            try
            {
                var user = await _bakeContext.AccountAuths.Select(u=>u.UserId).ToListAsync();
                var coupon = await _bakeContext.Coupons.FirstOrDefaultAsync(c => c.CouponId == int.Parse(couponId));
                if (coupon == null) return;
                var couponNotify = user.Select( uid =>  new Notification 
                {
                    UserId = uid, 
                    CouponId = coupon.CouponId, 
                    Title = "優惠券通知", 
                    Content = $"您的優惠券{coupon.Code}已送達，須購買金額達{coupon.MinimumPurchase}元才可折抵{coupon.DiscountValue}元 !", 
                    URL = "/Seller/Me/Orders" 
                }).ToList();
                _bakeContext.Notifications.AddRange(couponNotify);
                await _bakeContext.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("receiveCouponNotification","優惠券通知", $"您的優惠券{coupon.Code}已送達，須購買金額達{coupon.MinimumPurchase}元才可折抵{coupon.DiscountValue}元 !");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"未發送訊息:{ex.Message}");
            }
        }
        
    }

    public class ExpirationUpdateModel
    {
        public DateTime? NewDate { get; set; }
    }
}
