using Bake.Data;
using Bake.Models.Sales;
using Bake.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Bake.Controllers
{
    
    public class CheckoutController : Controller
    {
        private readonly BakeContext _bakeContext;
        public CheckoutController(BakeContext bakeContext)
        {
            _bakeContext = bakeContext;
        }

        // 將抓取 ID 的邏輯封裝，全 Controller 通用
        private int CurrentUserId
        {
            get
            {
                // 嘗試三種抓取方式：自定義、標準 ID、以及 NameIdentifier
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
    }
}
