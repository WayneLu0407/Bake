using Bake.Data;
using Bake.Hubs;
using Bake.Models;
using Bake.Models.Sales;
using Bake.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Bake.Controllers
{
    
    public class CheckoutController : Controller
    {
        private readonly BakeContext _bakeContext;
        private readonly IHubContext<NotificationHub> _hubContext;
        public CheckoutController(BakeContext bakeContext, IHubContext<NotificationHub> hubContext)
        {
            _bakeContext = bakeContext;
            _hubContext = hubContext;
        }

        // 將抓取 ID 的邏輯封裝，全 Controller 通用
        private int CurrentUserId
        {
            get
            { 
                var claimValue = User.FindFirst("UserId")?.Value
                              ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(claimValue, out int id))
                {
                    return id;
                }
                return 0;
            }
        }

        public IActionResult Info()
        {
            return View();
        }

        


        [HttpPost]
        public IActionResult Info(CheckoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["ReceiverName"] = model.ReceiverName;
                TempData["ReceiverPhone"] = model.ReceiverPhone;
                TempData["ReceiverAddress"] = model.ReceiverAddress;
                TempData["ReceiverEmail"] = model.ReceiverEmail;
                return RedirectToAction("Payment");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Payment()
        {
            TempData.Keep(); //先keep資料

            ViewBag.ReceiverName = TempData["ReceiverName"] ?? "測試員";
            ViewBag.ReceiverPhone = TempData["ReceiverPhone"];
            ViewBag.ReceiverAddress = TempData["ReceiverAddress"];
            ViewBag.ReceiverEmail = TempData["ReceiverEmail"];

            return View();
        }

        [Authorize]
        //HttpPost接收前端傳來的確定購買清單資料 asp-action="ConfirmPayment"
        //完成後redirection 到Success頁 顯示感謝您的訂購，訂單編號為 #XXX
        public async Task<IActionResult> ConfirmPayment(CheckoutViewModel checkoutViewModel, string PaymentMethod)
        {
            int userId = CurrentUserId;

            // 驗收防呆：如果真的抓不到登入者 ID，不要讓它進資料庫
            if (userId == 0)
            {
                // 應急方案：驗收時如果真的掛了，可以先 return Content("登入資訊遺失，請重新登入");
                return RedirectToAction("Login", "Home");
            }

            //1. 建立Order物件實體，並將checkoutdata資料填入Order物件中
            var order = new Order
            {
                UserId = userId,
                ShippingAddress = checkoutViewModel.ReceiverAddress,
                TotalAmount = 0, //這裡先設為0，實際金額應該從購物車計算
                PaymentMethodId = byte.Parse(PaymentMethod),
                StatusId = 1, //1代表待出貨
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            //2. 建立OrderItem物件實體，並將購物車中的每個商品資料填入OrderItem物件中，並加入Order的OrderItems集合中
            var cartItems = GetCartItemsFromSession();

            decimal totalAmount = 0;
            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    ItemQuantity = item.Quantity,
                    UnitPrice = item.Price,
                    Subtotal = item.Quantity * item.Price
                };

                order.OrderItems.Add(orderItem);
                totalAmount += item.Quantity * item.Price;
            }

            order.TotalAmount = totalAmount + 60; //假設運費固定60元

            //3. 寫入資料庫
            _bakeContext.Orders.Add(order);
            await _bakeContext.SaveChangesAsync();

            ClearCart(); //清空購物車

            


            return RedirectToAction("Success", new { id = order.OrderId});
        }

        

        [HttpGet]
        public async Task<IActionResult> Success(int id)
        {
            // 根據 ID 抓出訂單明細
            var order = await _bakeContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return RedirectToAction("Index", "Home");

            await SendOrderNotify(order.UserId.ToString(), order.OrderId.ToString());

            return View(order);
        }

        private List<CartViewModel> GetCartItemsFromSession()
        {
            var cartJson = HttpContext.Session.GetString("UserCart");

            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartViewModel>();
            }

            return JsonSerializer.Deserialize<List<CartViewModel>>(cartJson);
        }

        private void ClearCart()
        {
            HttpContext.Session.Remove("UserCart");
        }

        public async Task SendOrderNotify(string userId, string orderId)
        {
            try
            {
                var notify = new Notification { UserId = int.Parse(userId), OrderId = int.Parse(orderId), Title = "訂單通知", Content = $"您的訂單 #{orderId} 已成功下單 !", URL = "/Seller/Me/Orders" };
                _bakeContext.Notifications.Add(notify);
                await _bakeContext.SaveChangesAsync();

                await _hubContext.Clients.User(userId).SendAsync("receiveNotification", notify.Title, notify.Content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("未發送訊息");
            }
        }
        [HttpGet]
        public async Task<IActionResult> SystemInfo()
        {
            int userId = CurrentUserId; // 取得目前登入者 ID
            var count = await _bakeContext.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead); // 假設你有 IsRead 欄位
            return Json(count);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            int userid = CurrentUserId;

            var notifucations = await _bakeContext.Notifications.Where(u => u.UserId == userid).OrderByDescending(n => n.CreateAt).Select(n => new
            {
                n.NotificationId,
                n.OrderId,
                n.Title,
                n.Content,
                n.IsRead,
                n.URL
            }).ToListAsync();
            return Json(notifucations);
        }
        [HttpPost]
        [Route("Checkout/NotificationRead")]
        public async Task<IActionResult> NotificationRead(int Id)
        {
            var notification = await _bakeContext.Notifications.FindAsync(Id);
            if(notification != null)
            {
                notification.IsRead = true;
                await _bakeContext.SaveChangesAsync();
                return Ok(new { success = true });
            }
            return NotFound();
        }
    }
}
