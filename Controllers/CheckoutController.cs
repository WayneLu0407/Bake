using Bake.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Bake.Controllers
{
    public class CheckoutController : Controller
    {
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
            TempData.Keep(); //先keepy資料

            ViewBag.ReceiverName = TempData["ReceiverName"] ?? "測試員";
            ViewBag.ReceiverPhone = TempData["ReceiverPhone"];
            ViewBag.ReceiverAddress = TempData["ReceiverAddress"];
            
            return View();
        }

        //HttpPost接收前端傳來的確定購買清單資料 asp-action="ConfirmPayment"
        //完成後redirection 到Success頁 顯示感謝您的訂購，訂單編號為 #XXX
    }
}
