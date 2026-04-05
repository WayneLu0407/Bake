using Bake.Data;
using Bake.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace Bake.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponApiController : ControllerBase
    {
        private readonly BakeContext _bakecontext;
        public CouponApiController(BakeContext bakecontext)
        {
            _bakecontext = bakecontext;
        }

        //api/CouponApi/CheckCoupon
        [HttpPost("CheckCoupon")]
        public async Task<IActionResult> CheckCoupon([FromBody] CouponApplyRequest request)
        {
            //從Session取出購物車資料
            var cartJson = HttpContext.Session.GetString("UserCart");
            if (string.IsNullOrEmpty(cartJson))
                return Ok(new CouponResultViewModel { Success = false, Message="購物車是空的"});

            var cartItems = JsonSerializer.Deserialize<List<CartViewModel>>(cartJson);


            //從資料庫取出購物車中商品的相關資訊 (包含賣家資訊)
            var productIds = cartItems.Select(i => i.ProductId).ToList();
            var productsInDb =  await _bakecontext.Products
                .Where(p => productIds.Contains(p.ProductId))
                .Select(p => new {
                    p.ProductId,
                    p.ProductDetail.ProductPrice,
                    p.UserId
                }).ToListAsync();

            //檢查優惠券是否存在
            var coupon = await _bakecontext.Coupons
                .FirstOrDefaultAsync(c => c.Code == request.CouponCode && c.IsActive);

            if(coupon == null)
                return Ok(new { Success = false, Message = "此優惠券無效" });

            //檢查優惠券是否過期
            if(coupon.ExpirationDate < DateTime.Now)
                return Ok(new { Success = false, Message = "此優惠券已過期" });

            //計算金額
            decimal subTotal = 0;  //購物車總金額
            decimal applyAmout = 0;  //符合優惠券條件的金額 (如果有指定賣家，則只計算該賣家的商品金額)

            foreach (var item in cartItems) 
            {
                var dbProduct = productsInDb.FirstOrDefault(p => p.ProductId == item.ProductId);
                if (dbProduct == null) continue;

                decimal itemTotal = dbProduct.ProductPrice * item.Quantity;
                subTotal += itemTotal;

                //全站券走條件1 : 如果優惠券SellerId為null，每個itemTotal都會被加進applyAmount；如果有指定SellerId
                //賣家券走條件2 : 只有當商品的UserId與SellerId相符時，itemTotal才會被加進applyAmount
                if (!coupon.SellerId.HasValue || dbProduct.UserId == coupon.SellerId) 
                {
                    applyAmout += itemTotal;
                }
            }

            //檢查消費門檻
            if (applyAmout < coupon.MinimumPurchase) 
            {
                string scope = coupon.SellerId.HasValue? "指定賣家商品" : "全站商品";
                return Ok(new { Success = false, Message = $"此優惠券為{scope}，需購買金額達{coupon.MinimumPurchase:0}元才可使用" });
            }

            //存入Cookie
            Response.Cookies.Append("AppliedCoupon", request.CouponCode, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddMinutes(30),
                HttpOnly = true,
                Secure = true
            });

            return Ok(new {
                Success = true,
                Message = "套用成功！",
                Discount = coupon.DiscountValue
            });
        }
    }
}
