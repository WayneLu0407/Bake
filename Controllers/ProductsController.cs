using Bake.Data;
using Bake.ViewModel.Products;
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



        /////Products/Details/3
        //[Route("Products/Details/{id}")]
        //public IActionResult Details(int id)
        //{
        //    return View();  // 只負責回傳頁面，資料交給 API 處理
        //}

        [HttpGet]
        [Route("Products/Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            //查商品（依你們關聯需要加 Include）
            var product = await _db.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound();

            //查評論
            var reviews = await _db.ProductReviews
                .AsNoTracking()
                .Where(r => r.ProductId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            //組ViewModel
            var vm = new ProductDetailsViewModel
            {
                Product = product,
                Reviews = reviews
            };

            return View(vm);
        }
    }
}
