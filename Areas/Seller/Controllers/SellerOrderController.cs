using Bake.Models;
using Bake.Data;
using Bake.Enum;
using Microsoft.AspNetCore.Mvc;
using Bake.Areas.Seller.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Bake.Models.User;
using Microsoft.AspNetCore.Authorization;

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

        // 將抓取 ID 的邏輯封裝，全 Controller 通用
        private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

        //修改訂單狀態:未出貨、已出貨，從前端接收fetch並修改資料庫訂單狀態值後，response回前端
        // POST: Seller/SellerOrder/Ship/id
        [HttpPost]
        public async Task<IActionResult> Ship(int id)
        {
            //基本登入檢查
            if (CurrentUserId == 0)
                return RedirectToAction("Login", "Home");

            //身分驗證:檢查這筆訂單裡，是否有屬於目前賣家的產品 使用AnyAsync
            var isMyOrder = await _bakeContext.OrderItems
                .AnyAsync(oi => oi.OrderId == id &&
                                  _bakeContext.Products.Any(p => p.ProductId == oi.ProductId && p.UserId == CurrentUserId));

            if (!isMyOrder) 
            {
                return Forbid();
            }
            
            // 抓取訂單並更新
            var order = await _bakeContext.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "找不到該筆訂單" });
            }

            //執行更新
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

        // 訂單表格呈現
        [Authorize]
        public async Task<IActionResult> Orders(int? statusId)
        {
            // 從 Claims(HomeController) 取得目前登入買家的ID
            if (CurrentUserId == 0)
                return RedirectToAction("Login", "Home");
            
            //因為OrderItems裡面有ProductId，但Orders裡面沒有，所以需要使用LINQ JOIN來抓取相關資訊
            //使用LINQ JOIN 抓取Orders和Users資料表的相關資訊
            var ordersData = from order in _bakeContext.Orders
                             join orderItem in _bakeContext.OrderItems on order.OrderId equals orderItem.OrderId
                             join product in _bakeContext.Products on orderItem.ProductId equals product.ProductId
                             // 只抓產品擁有者是目前登入者的資料
                             where product.UserId == CurrentUserId
                             join status in _bakeContext.OrderStatuses on order.StatusId equals status.StatusId
                             join user in _bakeContext.AccountAuths on order.UserId equals user.UserId
                             select new { order, orderItem, product, status, user,  };
                              

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
