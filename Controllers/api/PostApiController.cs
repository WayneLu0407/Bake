using Bake.Data;
using Bake.Models.Social;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostApiController : ControllerBase
    {
        private readonly BakeContext _db;
        public PostApiController(BakeContext db)
        {
            this._db = db;
        }
        public class PostCardDto
        {
            public int PostId { get; set; }
            public string Title { get; set; } = "";
            public string? Image { get; set; }
            public int? ViewCount { get; set; }
            public int? LikesCount { get; set; }
        }

        // ---------- 共用的「組卡片」查詢 ----------

        // 共用的「基底查詢」— 只做篩選，不做 Select
        private IQueryable<Post> BaseQuery()
        {
            return _db.Posts
                .Where(p => p.IsPublished == true);
        }
        // 共用的「投影成卡片 DTO」— 各 endpoint 排序完再呼叫
        private IQueryable<PostCardDto> ToCard(IQueryable<Post> query)
        {
            return query.Select(p => new PostCardDto
            {
                PostId = p.PostId,
                Title = p.Title,
                Image = p.PostAttachments
                    .Where(a => a.IsCover == true)
                    .Select(a => a.FileUrl)
                    .FirstOrDefault(),
                ViewCount = p.ViewCount,
                LikesCount = p.LikesCount
            });
        }

        // 1) 最新貼文（純文章，不含活動）
        [HttpGet("Latest")]
        public async Task<IActionResult> Latest()
        {
            var list = await ToCard(
                BaseQuery().OrderByDescending(p => p.CreatedAt)
                )
                .Take(10)
                .ToListAsync();
            return Ok(list);
        }

        // 2) 熱門貼文（按瀏覽數 or 按讚數排序）
        [HttpGet("Hot")]
        public async Task<IActionResult> Hot()
        {
            var list = await ToCard(
                        BaseQuery().OrderByDescending(p => p.ViewCount)
                    )
                    .Take(10)
                    .ToListAsync();
            return Ok(list);
        }
        // 3) 熱門活動分類（就是 EventTypeLookup）
        [HttpGet("EventTypes")]
        public async Task<IActionResult> EventTypes()
        {
            var list = await _db.EventTypeLookups
                .Select(t => new {
                    t.EventTypeId,
                    t.EventTypeName,
                    // 如果分類有圖片欄位就用，沒有就先寫死或留空
                })
                .ToListAsync();
            return Ok(list);
        }

        // 4) 揪團中的活動（Post + EventDetail，報名中的）
        [HttpGet("GroupEvents")]
        public async Task<IActionResult> GroupEvents()
        {
            var now = DateTime.Now;
            var list = await _db.Posts
                .Where(p => p.IsPublished == true
                && p.EventDetails.Any(e => e.SignupDeadline > now))
                .Select(p => new
                {
                    p.PostId,
                    p.Title,
                    Image = p.PostAttachments
                        .Where(a => a.IsCover == true)
                        .Select(a => a.FileUrl)
                        .FirstOrDefault(),
                    EventTime = p.EventDetails
                        .Select(e => e.EventTime)
                        .FirstOrDefault(),
                    SingupDeadline = p.EventDetails
                        .Select(e => e.SignupDeadline)
                        .FirstOrDefault(),
                })
                .OrderByDescending(p => p.EventTime)
                .Take(20)
                .ToListAsync();
            return Ok(list);
        }

        // 取單筆商品 → /api/Post/GetById/3
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var prod = _db.Products
                .Include(p => p.ProductDetail)
                .Include(p => p.User).ThenInclude(u => u.Shop)
                .Where(p => p.ProductId == id)
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
                    ShelfLifeNote = p.ProductIngredient.ShelfLifeNote,
                    Ingredient = p.ProductIngredient.Ingredients,
                    NetWeight = p.ProductIngredient.NetWeight,
                    categoryId = p.CategoryId,
                    categoryName = p.Category.CategoryName,
                })
                .FirstOrDefault();

            if (prod == null) return NotFound();
            return Ok(prod);
        }
        //api/Post/Search?keyword = 巧克力
        [HttpGet]
        public IActionResult Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Ok(Array.Empty<object>());
            var results = _db.Products
                .Include(p => p.ProductDetail)
                .Where(p => p.ProductName.Contains(keyword))
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
