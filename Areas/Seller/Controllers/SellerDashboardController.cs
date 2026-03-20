using Bake.Areas.Seller.ViewModels;
using Bake.Data;
using Bake.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class SellerDashboardController : Controller
    {
        private readonly BakeContext _bakeContext;
        public SellerDashboardController(BakeContext bakeContext) 
        {
            _bakeContext = bakeContext;
        }

        private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            // 身分驗證
            int sellerId = CurrentUserId;
            if(sellerId==0) return RedirectToAction("Login","Home");

            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            //今日營收
            var todayRevenue = await _bakeContext.OrderItems
                .Where(oi => oi.Product.UserId == sellerId && oi.Order.CreatedAt >= today)
                .SumAsync(oi => (decimal?)oi.ItemQuantity * oi.UnitPrice) ?? 0;

            //待出貨訂單數
            var pendingOrdersCount = await _bakeContext.Orders
                .Where(o => o.StatusId == (int)OrderStatusEnum.Paid
                            && o.OrderItems.Any(oi => oi.Product.UserId == sellerId))
                .CountAsync();

            //在架商品總數
            var productCount = await _bakeContext.Products
                .CountAsync(p => p.UserId == sellerId);

            //本月總訂單數
            var ordersMonth = await _bakeContext.Orders
                .Where(o => o.CreatedAt >= firstDayOfMonth
                            && o.OrderItems.Any(oi => oi.Product.UserId == sellerId))
                .CountAsync();

            //本月總營收
            var monthRevenue = await _bakeContext.OrderItems
                .Where(oi => oi.Product.UserId == sellerId && oi.Order.CreatedAt >= firstDayOfMonth)
                .SumAsync(oi => (decimal?)oi.UnitPrice * oi.ItemQuantity) ?? 0;


            var dashboardVeiwData = new DashboardViewModel
            {
                TodayRevenue = todayRevenue,
                PendingOrdersCount = pendingOrdersCount,
                TotalProductsCount = productCount,
                TotalOrdersMonth = ordersMonth,
                MonthRevenue = monthRevenue
            };
        
            return View(dashboardVeiwData);
        }
    }
}
