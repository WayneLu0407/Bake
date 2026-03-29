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

            //過去7天訂單數量趨勢圖
            var endDay = DateTime.Today;
            var startDay = endDay.AddDays(-6);

            //抓出需要的訂單數據:近七天的日期、該天的訂單比數
            var orders = await _bakeContext.Orders
                //條件: 訂單符合日期區間(大於startDay)，且產品擁有者id為當前登入帳號尺用者
                .Where(o=>o.CreatedAt >= startDay && o.OrderItems.Any(oi => oi.Product.UserId == sellerId))
                //用創立時間分群
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g=> new { Date = g.Key, count = g.Count()})
                .ToListAsync();
            //用迴圈把1~7天的日期、訂單數放進去
            var last7DaysLable = new List<string>();
            var last7DaysData = new List<int>();
            for (int i = 0; i < 7; i++) 
            {
                var date = startDay.AddDays(i);
                last7DaysLable.Add(date.ToString("MM/dd"));
                last7DaysData.Add(orders.FirstOrDefault(o=>o.Date == date)?.count??0); 
            }

            //類別銷售圓餅圖
            var categorySales = await (from oi in _bakeContext.OrderItems
                                       join p in _bakeContext.Products on oi.ProductId equals p.ProductId
                                       join c in _bakeContext.ProductCategories on p.CategoryId equals c.CategoryId
                                       where p.UserId == sellerId
                                       group oi by c.CategoryName into g
                                       select new { Name = g.Key, Total = g.Sum(x=>x.ItemQuantity*x.UnitPrice)}
                                       ).ToListAsync();

            //低庫存示警
            int lowThreshold = 20;
            var lowInventory = await _bakeContext.ProductDetails
                .Where(pd => pd.ProductQuantity <= lowThreshold
                            && pd.Product.UserId == sellerId)
                .Select(pd => new ProductStockDto { LowPName = pd.Product.ProductName, Stock = pd.ProductQuantity })
                .ToListAsync();

            //熱門產品Top 5
            var topProducts = await _bakeContext.OrderItems
                .Where(oi => oi.Product.UserId == sellerId)
                .GroupBy(oi => new 
                { oi.ProductId, 
                  oi.Product.ProductName,
                  oi.Product.ProductImage,
                  CategoryName = oi.Product.Category.CategoryName
                })
                .Select(tp => new TopProductDto
                {   ProductName = tp.Key.ProductName,
                    CategoryName = tp.Key.CategoryName,
                    TotalSales = tp.Sum(oi => oi.ItemQuantity),
                    Revenue = tp.Sum(oi => oi.ItemQuantity*oi.UnitPrice),
                    ImageUrl = tp.Key.ProductImage
                })
                .OrderByDescending(x => x.TotalSales).Take(5).ToListAsync();

            var dashboardVeiwData = new DashboardViewModel
            {
                TodayRevenue = todayRevenue,
                PendingOrdersCount = pendingOrdersCount,
                TotalProductsCount = productCount,
                TotalOrdersMonth = ordersMonth,
                MonthRevenue = monthRevenue,

                Last7DaysLabels = last7DaysLable,
                Last7DaysData = last7DaysData,
                CategoryLabels = categorySales.Select(x=>x.Name).ToList(),
                CategoryData =  categorySales.Select(x=>x.Total).ToList(),

                LowStock = lowInventory,
                TopProducts = topProducts
            };
        
            return View(dashboardVeiwData);
        }
    }
}
