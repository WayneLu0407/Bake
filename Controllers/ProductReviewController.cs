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
using Bake.Enum;

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

            var access = await GetReviewAccessAsync(CurrentUserId, orderId, productId);

            if (!access.IsOwner)
            {
                return Forbid();
            }

            if (!access.IsCompleted)
            {
                TempData["ReviewMessage"] = "只有已完成訂單才能評論";
                return RedirectToAction("Orders", "Me", new { area = "Seller" });
            }

            if (access.HasReviewed)
            {
                TempData["ReviewMessage"] = "你已經評論過這個商品囉！";
                return RedirectToAction("Orders", "Me", new { area = "Seller" });
            }

            var vm = new ProductReviewCreateViewModel
            {
                ProductId = productId,
                OrderId = orderId,
                ProductName = access.ProductName!,
                UserRating = 5
            };

            if (IsAjaxRequest())
            {
                return PartialView("_CreateFormPartial", vm);
            }

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

            var access = await GetReviewAccessAsync(CurrentUserId, vm.OrderId, vm.ProductId);

            if (!access.IsOwner)
            {
                return Forbid();
            }

            if (!access.IsCompleted)
            {
                TempData["ReviewMessage"] = "只有已完成訂單才能評論";
                return RedirectToAction("Orders", "Me", new { area = "Seller" });
            }

            vm.ProductName = access.ProductName!;

            if (string.IsNullOrWhiteSpace(vm.Comment))
            {
                ModelState.AddModelError(nameof(vm.Comment), "請輸入評論內容");
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            if (access.HasReviewed)
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

            if (IsAjaxRequest())
            {
                return Json(new
                {
                    success = true,
                    message = "評論成功！"
                });
            }

            TempData["ReviewMessage"] = "評論成功！";
            return RedirectToAction("Orders", "Me", new { area = "Seller" });
        }

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

        //判斷訂單使用者/是否已結單Complete/是否評論過統整
        private async Task<(bool IsOwner, bool IsCompleted, bool HasReviewed, string? ProductName)> GetReviewAccessAsync(int userId, int orderId, int productId)
        {
            var row = await _context.OrderItems
                .AsNoTracking()
                .Where(oi => oi.OrderId == orderId
                          && oi.ProductId == productId
                          && oi.Order.UserId == userId)
                .Select(oi => new
                {
                    oi.Product.ProductName,
                    oi.Order.StatusId
                })
                .FirstOrDefaultAsync();

            if (row == null)
            {
                return (false, false, false, null);
            }

            var hasReviewed = await HasReviewedAsync(userId, orderId, productId);
            var isCompleted = row.StatusId != (byte)OrderStatusEnum.Unpaid
                       && row.StatusId != (byte)OrderStatusEnum.Cancelled;

            return (true, isCompleted, hasReviewed, row.ProductName);
        }


        private bool IsAjaxRequest()
        {
            return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
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
