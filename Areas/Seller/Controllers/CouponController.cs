using Bake.Areas.Seller.ViewModels;
using Bake.Data;
using Bake.Models.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Security.Claims;

namespace Bake.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = "Seller")]
    public class CouponController : Controller
    {
        private readonly BakeContext _bakeContext;
        public CouponController(BakeContext bakeContext) 
        {
            _bakeContext = bakeContext;
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
            if (!ModelState.IsValid) return Json(new { success = false, message = "資料格式錯誤，請檢查輸入內容" });
            if (model.ExpirationDate <= DateTime.Now)
                return Json(new { success = false, message = "到期日必須大於今天" });

            string couponCode = GenerateRandomCouponCode(8);
            var newCoupon = new Coupon
            {
                Code = couponCode,
                DiscountValue = model.DiscountAmount,
                MinimumPurchase = model.MinimumPurchase,
                ExpirationDate = model.ExpirationDate,
                IsActive = true,
                SellerId = sellerId
            };

            _bakeContext.Coupons.Add(newCoupon);
            await _bakeContext.SaveChangesAsync();

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
            var coupon = await _bakeContext.Coupons
                .FirstOrDefaultAsync(c => c.CouponId == id && c.SellerId == CurrentSellerId);

            if (coupon == null)
                return Json(new { success = false, message = "找不到優惠券" });

            if (model.NewDate <= DateTime.Now)
                return Json(new { success = false, message = "新的日期必須大於今日" });

            //更新日期
            coupon.ExpirationDate = model.NewDate;
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
    }

    public class ExpirationUpdateModel
    {
        public DateTime NewDate { get; set; }
    }
}
