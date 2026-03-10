using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bake.Data;
using Bake.Models.Sales;
using Bake.ViewModel;

namespace Bake.Controllers
{
    public class ProductReviewController : Controller
    {
        private readonly BakeContext _context;

        public ProductReviewController(BakeContext context)
        {
            _context = context;
        }

        // GET: ProductReview
        public async Task<IActionResult> Index()
        {
            var bakeContext = _context.ProductReviews.Include(p => p.User);
            return View(await bakeContext.ToListAsync());
        }

        // GET: ProductReview/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productReview = await _context.ProductReviews
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (productReview == null)
            {
                return NotFound();
            }

            return View(productReview);
        }

        //要記得寫新的Route     [Route("/Customers/{action=Index}/{CustomerID?}")]
        // GET: ProductReview/Create
        [HttpGet]
        public IActionResult Create(int productId, int orderId)  
        {
            var vm = new ProductReviewCreateViewModel
            {
                ProductId = productId,
                OrderId = orderId,
                UserRating = 5 //預設
            };

            return View(vm);
        }
        //使用者寫入(按儲存)會走這邊
        // POST: ProductReview/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Bind，我只收這些的意思
        public async Task<IActionResult> Create(ProductReviewCreateViewModel vm) //本來就不給動別的屬性所以不用Bind
        {
                const int DEMO_USER_ID = 3; // 還未接登入功能，先固定一個userId demo用

            // 未來要做下單才能評，就把檢查加在這裡
            // TODO: 檢查 vm.OrderId 這筆訂單是不是 DEMO_USER_ID 的
            // TODO: 檢查這筆訂單裡是否包含 vm.ProductId
            // TODO: 檢查訂單狀態是否完成/已收貨

            if (vm.ProductId <= 0 || vm.OrderId <= 0) //避免為0
            {
                return BadRequest("productId/orderId 不正確");
            }

            if (vm.UserRating < 1 || vm.UserRating > 5)
            {
                ModelState.AddModelError("UserRating", "評分必須介於 1 到 5");
            }

            if (string.IsNullOrWhiteSpace(vm.Comment))
                ModelState.AddModelError("Comment", "請輸入評論內容");

            if (!ModelState.IsValid) //最後關卡->避免空 comment、rating=0 
            {
                return View(vm);
            }

            // 先檢查"同單同商品"有之前有沒有已經評論過
            var alreadyReviewed = await _context.ProductReviews
            .AsNoTracking()
            .AnyAsync(r => r.ProductId == vm.ProductId
                        && r.UserId == DEMO_USER_ID
                        && r.OrderId == vm.OrderId);

            if (alreadyReviewed)
                {
                ModelState.AddModelError("", "你已經評論過這個商品囉！");
                return View(vm);
            }
            else
            {
                var entity = new ProductReview
                {
                    ProductId = vm.ProductId,
                    UserId = DEMO_USER_ID,
                    OrderId = vm.OrderId,
                    UserRating = vm.UserRating,
                    Comment = vm.Comment
                    // CreatedAt 不填，DB default sysdatetime() 會自動填（符合 NOT NULL）
                };
                _context.ProductReviews.Add(entity);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Products", new { id = vm.ProductId });

        }

        // GET: ProductReview/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productReview = await _context.ProductReviews.FindAsync(id);
            if (productReview == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.UserProfiles, "UserId", "UserId", productReview.UserId);
            return View(productReview);
        }

        // POST: ProductReview/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,ProductReview productReview)  
        {
            if (id != productReview.ReviewId)
            {
                return NotFound();
            }

            if (ModelState.IsValid) 
            {
                try
                {
                    _context.Update(productReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductReviewExists(productReview.ReviewId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.UserProfiles, "UserId", "UserId", productReview.UserId);
            return View(productReview);
        }

        // GET: ProductReview/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productReview = await _context.ProductReviews
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (productReview == null)
            {
                return NotFound();
            }

            return View(productReview);
        }

        // POST: ProductReview/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productReview = await _context.ProductReviews.FindAsync(id);
            if (productReview != null)
            {
                _context.ProductReviews.Remove(productReview);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductReviewExists(int id)
        {
            return _context.ProductReviews.Any(e => e.ReviewId == id);
        }

        [HttpGet]
        public async Task<IActionResult> Panel(int productId)
        {
            var reviews = await _context.ProductReviews
                .AsNoTracking()
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return PartialView("_ProductReviewsPartial", reviews);
        }
    }
}
