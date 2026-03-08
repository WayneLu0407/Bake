using Bake.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake.Controllers
{
    public class ProductsController : Controller
    {
        private readonly BakeContext _db;

        public ProductsController(BakeContext db)
        {
            this._db = db;
        }

        [HttpGet]
        // 1. /Products/Index
        // 2. /Products/Index/apple
        [Route("Products/Index/{keyword?}")]
        public IActionResult Index(string? keyword)
        {
            if (!string.IsNullOrEmpty(keyword)) 
            {
                ViewBag.Keyword = keyword;
                // 執行搜尋邏輯...
            }

            return View();
        }



        //Products/Details
        public IActionResult Details()
        {
            return View();
        }

        //<!- Demo ->之後要合併到商品頁去
        public async Task<IActionResult> ReviewDemo(int productId = 1)
        {
            var reviews = await _db.ProductReviews
                .AsNoTracking()
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            ViewBag.ProductId = productId;
            return View(reviews);
        }
    }
}
