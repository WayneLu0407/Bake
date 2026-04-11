using Bake.Data;
using Microsoft.AspNetCore.Authorization;
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

        
        private int? CurrentUserId()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            return int.Parse(userId);
        }

        // 取全部商品 → /api/Product/Get
        [HttpGet]
        public IActionResult Get()
        {
            var userId = CurrentUserId();

            var prod = _db.Products
                .Where(p => p.User.Shop != null && p.User.Shop.StatusId == 0)
                .Include(p => p.Category)
                .Include(p => p.ProductDetail)
                .Include(p => p.User).ThenInclude(u => u.Shop)
                .Select(p => new {
                    productId = p.ProductId,
                    userId = p.UserId,
                    productName = p.ProductName,
                    productImage = p.ProductImage,
                    productRating = p.ProductRating,
                    productDate = p.ProductDate,
                    categoryId = p.CategoryId,
                    productQuantity = p.ProductDetail != null ? p.ProductDetail.ProductQuantity : 0,
                    productPrice = p.ProductDetail != null ? p.ProductDetail.ProductPrice : (decimal?)null,
                    productDiscount = p.ProductDetail != null ? p.ProductDetail.ProductDiscount : (decimal?)null,
                    shopName = p.User.Shop != null ? p.User.Shop.ShopName : "未知店家",
                    isFavorited = userId !=null && _db.FavoriteProducts.Any(f => f.ProductId == p.ProductId && f.UserId == userId.Value)
                })
                .ToList();
            return Ok(prod);
        }

        // 取單筆商品 → /api/Product/GetById/3
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var userId = CurrentUserId();

            var prod = _db.Products
                .Include(p => p.ProductDetail)
                .Include(p => p.User).ThenInclude(u => u.Shop)
                .Include(p => p.ProductIngredient)
                .Where(p => p.ProductId == id && p.User.Shop != null && p.User.Shop.StatusId == 0)
                .Select(p => new {
                    productId = p.ProductId,
                    userId = p.UserId,
                    productName = p.ProductName,
                    productImage = p.ProductImage,
                    productRating = p.ProductRating,
                    productPrice = p.ProductDetail.ProductPrice,
                    productDiscount = p.ProductDetail.ProductDiscount,
                    productQuantity = p.ProductDetail.ProductQuantity,
                    expireDate = p.ProductDetail.ExpireDate,
                    shopName = p.User.Shop.ShopName,
                    shopImg = p.User.Shop.ShopImg!= null? "/" + p.User.Shop.ShopImg.TrimStart('/'): null,
                    productDescription = p.ProductDescription,
                    ShelfLifeNote = p.ProductIngredient!= null? p.ProductIngredient.ShelfLifeNote: null,
                    Ingredient = p.ProductIngredient != null ? p.ProductIngredient.Ingredients : null,
                    NetWeight = p.ProductIngredient != null ? p.ProductIngredient.NetWeight : null,
                    categoryId = p.CategoryId,
                    categoryName = p.Category.CategoryName,
                    isFavorited = userId != null && _db.FavoriteProducts.Any(f => f.ProductId == p.ProductId && f.UserId == userId.Value)

                })
                .FirstOrDefault();

            if (prod == null) return NotFound();
            return Ok(prod);
        }
        //api/Product/Search?keyword = 巧克力
        [HttpGet]
        public IActionResult Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Ok(Array.Empty<object>());
            if (keyword.Length > 50)
                return BadRequest();
            var results = _db.Products
                .Include(p => p.ProductDetail)
                .Where(p => p.ProductName.Contains(keyword) && p.User.Shop != null && p.User.Shop.StatusId == 0)
                .Take(8)
                .Select(p => new
                {
                    ProductId=p.ProductId,
                    ProductName=p.ProductName,
                    ProductImage = p.ProductImage,
                    ProductPrice= p.ProductDetail != null ? p.ProductDetail.ProductPrice : (decimal?)null
                })
                .ToList();
            return Ok(results);
        }
    }
}
