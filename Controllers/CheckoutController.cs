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

    [Authorize]
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

        [HttpGet]
        public IActionResult Info()
        {
            // 運費資料
            ViewBag.ShippingFee = HttpContext.Session.GetInt32("ShippingFee") ?? 60;

            // 收件人資料
            var infoJson = TempData.Peek("ReceiverInfo")?.ToString();

            //如果有填過收件人資料，回傳填好的資料；如果沒有填過，回傳空的資料表格
            CheckoutViewModel model = !string.IsNullOrEmpty(infoJson)? 
                JsonSerializer.Deserialize<CheckoutViewModel>(infoJson)
                : new CheckoutViewModel();

            // 優惠券
            var couponCode = Request.Cookies["AppliedCoupon"];
            ViewBag.CouponCode = couponCode ?? string.Empty; // 如果沒抓到，預設空字串

            return View(model);
        }


        [HttpPost]
        public IActionResult Info(CheckoutViewModel model, int ShippingFee)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ViewBag.ShippingFee = HttpContext.Session.GetInt32("ShippingFee") ?? 60;
            ViewBag.CouponCode = Request.Cookies["AppliedCoupon"] ?? string.Empty;

            return View("Payment", model);
        }

        [HttpGet]
        public IActionResult Payment()
        { 
            return View();
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

        [HttpGet]
        public async Task<IActionResult> Fail(int id, string msg, string amount)
        {
            ViewBag.OrderId = id;
            ViewBag.ErrorMessage = msg;
            ViewBag.Amount = amount;
            //await SendOrderNotify(order.UserId.ToString(), order.OrderId.ToString());

            return View();
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
