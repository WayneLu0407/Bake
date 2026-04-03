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

        [HttpGet]
        public IActionResult Info()
        {
            // 從 Session 抓回舊資料
            int? sessionShipping = HttpContext.Session.GetInt32("ShippingFee");
            ViewBag.ShippingFee = sessionShipping ?? 60; // 如果沒抓到，預設 60

            var infoJson = HttpContext.Session.GetString("ReceiverInfo");
            if (!string.IsNullOrEmpty(infoJson))
            {
                var model = JsonSerializer.Deserialize<CheckoutViewModel>(infoJson);
                return View(model); // 把舊資料丟回給 View 顯示
            }

            return View(new CheckoutViewModel()); // 第一次進來，給空的
        }

        


        [HttpPost]
        public IActionResult Info(CheckoutViewModel model, int ShippingFee)
        {
            if (ModelState.IsValid)
            {
                //資料存session
                var infoJson = JsonSerializer.Serialize(model);
                HttpContext.Session.SetString("ReceiverInfo", infoJson);
                HttpContext.Session.SetInt32("ShippingFee", ShippingFee);

                return RedirectToAction("Payment");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Payment()
        {
            // 從Session拿資料
            var infoJson = HttpContext.Session.GetString("ReceiverInfo");
            if (string.IsNullOrEmpty(infoJson)) return RedirectToAction("Info");

            var model = JsonSerializer.Deserialize<CheckoutViewModel>(infoJson);
            int shippingFee = HttpContext.Session.GetInt32("ShippingFee") ?? 60;

            ViewBag.ReceiverName = model.ReceiverName;
            ViewBag.ReceiverPhone = model.ReceiverPhone;
            ViewBag.ReceiverAddress = model.ReceiverAddress;
            ViewBag.ReceiverEmail = model.ReceiverEmail;
            ViewBag.ShippingFee = shippingFee;
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
