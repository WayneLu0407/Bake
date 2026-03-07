using Bake.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake.Controllers.api
{
    [Route("api/Product/[action]")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly BakeContext _db;

        public ProductApiController(BakeContext db)
        {
            this._db = db;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var prod = _db.Products
                .Include(p => p.Category)        // 如果需要分類名稱
                .Include(p => p.User)            // 透過 User 找到 Shop
                    .ThenInclude(u => u.Shop)    // User → Shop
                .Select(p => new {
                    productId = p.ProductId,
                    productName = p.ProductName,
                    productImage = p.ProductImage,
                    productRating = p.ProductRating,
                    // ProductDetail 是 1對1，直接用導覽屬性
                    productPrice = p.ProductDetail.ProductPrice,
                    productDiscount = p.ProductDetail.ProductDiscount,
                    productQuantity = p.ProductDetail.ProductQuantity,
                    // Shop 透過 User 取得
                    shopName = p.User.Shop.ShopName
                })
                .ToList();

            return Ok(prod);
        }
    }
}
