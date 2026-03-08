using Bake.Data;
using Microsoft.AspNetCore.Mvc;

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



        ///Products/Details/3
        [Route("Products/Details/{id}")]
        public IActionResult Details(int id)
        {
            return View();  // 只負責回傳頁面，資料交給 API 處理
        }
    }
}
