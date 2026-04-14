using Bake.Data;
using Bake.Models;
using Bake.Models.Sales;
using Bake.ViewModel;
using Bake.ViewModel.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

namespace Bake.Controllers
{
    public class ProductsController : Controller
    {
        private readonly BakeContext _db;

        public ProductsController(BakeContext db)
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
                .Include(r => r.User)
                .Where(r => r.ProductId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            //組ViewModel
            var vm = new ProductDetailsViewModel
            {
                Product = product,
                Reviews = reviews,

            };

            return View(vm);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Favorite()   // (查詢)
        {
            var user = CurrentUserId();
            if(user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            
            var myFavorite = await _db.FavoriteProducts
                .Where(u =>u.UserId == user.Value)
                .Include(i=> i.Product)
                    .ThenInclude(p => p.ProductDetail)
                .Select(w => new WishListViewModel
                {
                    ProductId = w.ProductId,
                    Name = w.Product.ProductName,
                    Price = Math.Round((decimal)(w.Product.ProductDetail.ProductPrice * (1 - w.Product.ProductDetail.ProductDiscount))),
                    ImagePath =w.Product.ProductImage
                })
                .ToListAsync();
            return View(myFavorite);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int productId)   // (新增+刪除)
        {
            var user = CurrentUserId();
            if(user == null)
            {
                return Json(new { success = false, message = "請先登入!" });
            }

            var productExists = await _db.Products.AnyAsync(p => p.ProductId == productId);
            if (!productExists)
            {
                return Json(new { success = false, message = "找不到該產品!" });
            }
            try
            {
                var favorite = await _db.FavoriteProducts.FirstOrDefaultAsync(f =>    // 判斷有沒有收藏
                    f.UserId == user.Value && f.ProductId == productId);
                if (favorite == null)       // 如果還沒收藏   新增至收藏表
                {
                    var newFavorite = new FavoriteProduct
                    {
                        UserId = user.Value,
                        ProductId = productId,
                        CreatedAt = DateTime.Now,
                    };
                    _db.FavoriteProducts.Add(newFavorite);
                    await _db.SaveChangesAsync();
                    return Json(new { success = true, isFavorited = true, message = "已加入收藏清單!" });
                }
                else                    //如果已經收藏  移除收藏清單
                {
                    _db.FavoriteProducts.Remove(favorite);
                    await _db.SaveChangesAsync();
                    return Json(new { success = true, isFavorited = false, message = "已移除收藏清單" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "伺服器發生錯誤" });
            }
        }
        
    }
}
