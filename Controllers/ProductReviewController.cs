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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Bake.Controllers
{
    public class ProductReviewController : Controller
    {
        private readonly BakeContext _context;

        public ProductReviewController(BakeContext context)
        {
            _context = context;
        }

        private int CurrentUserId =>
            int.TryParse(User.FindFirstValue("UserId"), out var userId) ? userId : 0;

        private async Task<string?> GetOwnedProductNameAsync(int userId, int orderId, int productId)
        {
            return await _context.OrderItems
                .AsNoTracking()
                .Where(oi => oi.OrderId == orderId
                          && oi.ProductId == productId
                          && oi.Order.UserId == userId)
                .Select(oi => oi.Product.ProductName)
                .FirstOrDefaultAsync();
        }

        private async Task<bool> HasReviewedAsync(int userId, int orderId, int productId)
        {
            return await _context.ProductReviews
                .AsNoTracking()
                .AnyAsync(r => r.UserId == userId
                            && r.OrderId == orderId
                            && r.ProductId == productId);
        }

        //將平均值回寫入ProductRating 
        private async Task UpdateProductRatingAsync(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                return;
            }

            var avgRating = await _context.ProductReviews
                .Where(r => r.ProductId == productId)
                .Select(r => (decimal?)r.UserRating)
                .AverageAsync();

            product.ProductRating = avgRating.HasValue
                //值,小數點保留到第幾位,直覺的四捨五入
                ? Math.Round(avgRating.Value, 1, MidpointRounding.AwayFromZero)
                : null;
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

        // GET: ProductReview/Create
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create(int productId, int orderId)
        {
            if (CurrentUserId == 0)
            {
                return Challenge();
            }

            var productName = await GetOwnedProductNameAsync(CurrentUserId, orderId, productId);
            if (productName == null)
            {
                return Forbid();
            }

            var alreadyReviewed = await HasReviewedAsync(CurrentUserId, orderId, productId);
            if (alreadyReviewed)
            {
                TempData["ReviewMessage"] = "你已經評論過這個商品囉！";
                return RedirectToAction("Orders", "Me", new { area = "Seller" });
            }

            var vm = new ProductReviewCreateViewModel
            {
                ProductId = productId,
                OrderId = orderId,
                ProductName = productName,
                UserRating = 5
            };

            return View(vm);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductReviewCreateViewModel vm)
        {
            if (CurrentUserId == 0)
            {
                return Challenge();
            }

            var productName = await GetOwnedProductNameAsync(CurrentUserId, vm.OrderId, vm.ProductId);
            if (productName == null)
            {
                return Forbid();  //禁止存取
            }

            vm.ProductName = productName;

            if (string.IsNullOrWhiteSpace(vm.Comment))
            {
                ModelState.AddModelError(nameof(vm.Comment), "請輸入評論內容");
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var alreadyReviewed = await HasReviewedAsync(CurrentUserId, vm.OrderId, vm.ProductId);
            if (alreadyReviewed)
            {
                ModelState.AddModelError(string.Empty, "你已經評論過這個商品囉！");
                return View(vm);
            }

            var entity = new ProductReview
            {
                ProductId = vm.ProductId,
                UserId = CurrentUserId,
                OrderId = vm.OrderId,
                UserRating = vm.UserRating,
                Comment = vm.Comment.Trim()
            };

            _context.ProductReviews.Add(entity);
            await _context.SaveChangesAsync();

            await UpdateProductRatingAsync(vm.ProductId);
            await _context.SaveChangesAsync();

            TempData["ReviewMessage"] = "評論成功！";
            return RedirectToAction("Orders", "Me", new { area = "Seller" });
        }

        /* 尚未用到
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
        */

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
