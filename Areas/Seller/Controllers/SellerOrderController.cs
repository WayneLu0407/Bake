using Bake.Models;
using Bake.Data;
using Bake.Enum;
using Microsoft.AspNetCore.Mvc;
using Bake.Areas.Seller.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Bake.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class SellerOrderController : Controller
    {
        private readonly BakeContext _bakeContext;
        public SellerOrderController(BakeContext bakeContext)
        {
            _bakeContext = bakeContext;
        }

        // POST: Seller/SellerOrder/Ship/id
        [HttpPost]
        public async Task<IActionResult> Ship(int id)
        {
            var order = await _bakeContext.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "找不到該筆訂單" });
            }

            order.StatusId = (int)OrderStatusEnum.Shipped;
            order.UpdatedAt = DateTime.Now; //出貨時間

            try
            {
                await _bakeContext.SaveChangesAsync();
                return Ok(new { success = true, message = "已成功標記為出貨" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "更新訂單狀態失敗", error = ex.Message });
            }

        }
        public async Task<IActionResult> Orders(int? statusId)
        {
            //想想看如何顯示產品名稱，因為OrderItems裡面有ProductId，但Orders裡面沒有，所以需要使用LINQ JOIN來抓取相關資訊
            //使用LINQ JOIN 抓取Orders和Users資料表的相關資訊
            var ordersData = from order in _bakeContext.Orders
                              join status in _bakeContext.OrderStatuses on order.StatusId equals status.StatusId
                              join user in _bakeContext.AccountAuths on order.UserId equals user.UserId
                              join orderItem in _bakeContext.OrderItems on order.OrderId equals orderItem.OrderId
                              join product in _bakeContext.Products on orderItem.ProductId equals product.ProductId
                              select new { order, status, user, orderItem, product };
                              

            //如果有傳入statusId參數，就過濾訂單資料
            if (statusId.HasValue)
            {
                ordersData = ordersData.Where(o => o.order.StatusId == statusId.Value);
            }
            var ordersQuery = await ordersData.ToListAsync();

            //進行GroupBy 整合一筆訂單對多個購買產品 寫入OrderListViewModel
            var orderList = ordersQuery.GroupBy(x=>x.order.OrderId)
                .Select(p=> new OrderListViewModel 
                {
                    OrderId = p.Key,
                    UserName = p.First().user.UserName,
                    Email = p.First().user.Email,
                    StatusName = p.First().status.StatusName,
                    StatusId = p.First().status.StatusId,
                    TotalAmount = p.First().order.TotalAmount,
                    CreatedAt = p.First().order.CreatedAt,

                    Products = p.Select(x=>new OrderItemInfo { 
                        ProductName = x.product.ProductName,
                        Quantity = x.orderItem.ItemQuantity
                    }).ToList(),
                }).OrderByDescending(x => x.CreatedAt).ToList();

            return View(orderList);
        }

        public IActionResult Past()
        {
            return View();
        }
    }
}
