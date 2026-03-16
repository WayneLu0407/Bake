using Bake.Data;
using Bake.Models.Sales;
using Bake.ViewModel;
using Microsoft.AspNetCore.Mvc;
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

        //HttpPost接收前端傳來的確定購買清單資料 asp-action="ConfirmPayment"
        //完成後redirection 到Success頁 顯示感謝您的訂購，訂單編號為 #XXX
        public async Task<IActionResult> ConfirmPayment(CheckoutViewModel checkoutViewModel, string PaymentMethod)
        {
            //1. 建立Order物件實體，並將checkoutdata資料填入Order物件中
            var order = new Order
            {
                UserId = 1,
                ShippingAddress = checkoutViewModel.ReceiverAddress,
                TotalAmount = 0, //這裡先設為0，實際金額應該從購物車計算
                PaymentMethod = byte.Parse(PaymentMethod),
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

            return RedirectToAction("Success");
        }

        public IActionResult Success(int id)
        {
            ViewBag.OrderId = id; //這裡應該傳入剛剛建立的Order的ID，實際上需要從資料庫取得
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
            HttpContext.Session.Remove("Cart");
        }
    }
}
