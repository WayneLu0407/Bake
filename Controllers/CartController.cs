using Bake.Data;
using Bake.Models.Sales;
using Bake.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Bake.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "UserCart";
        private readonly BakeContext _bakeContext;
        public CartController(BakeContext bakeContext)
        {
            _bakeContext = bakeContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        //取得購物車資料 (給 Vue 側邊欄用的 API)
        //對應網址 Cart/GetCartItems
        [HttpGet]
        public IActionResult GetCartItems()
        {
            var cart = GetCartFromSession();
            return Json(cart); //C# 的 CartViewModel 是 Price (大寫 P)。使用 return Json(cart) 時，ASP.NET Core 預設會把它轉成小寫開頭的 JSON。
        }

        // 3. 加入商品到購物車
        // 網址：/Cart/Add (對應fetch)
        [HttpPost]
        public IActionResult Add([FromBody] CartItemRequest request)
        {
            if (request == null || request.ProductId <= 0)
            {
                return BadRequest(new { message = "無效的商品資料" });
            }

            //從 Session 取出目前的購物車資料
            var cart = GetCartFromSession();
            //檢查購物車裡是否已經有這個商品
            var existingItem = cart.FirstOrDefault(c => c.ProductId == request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity; //如果已經有了，就增加數量
            }
            else
            {
                //如果沒有，就新增一筆
                var product = _bakeContext.Products.FirstOrDefault(p => p.ProductId == request.ProductId);
                var productDetails = _bakeContext.ProductDetails.FirstOrDefault(p => p.ProductId == request.ProductId);
                if (product == null || productDetails == null)
                {
                    return NotFound(new { message = "找不到商品" });
                }
                cart.Add(new CartViewModel
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = productDetails.ProductPrice, //從 ProductDetails 取價格
                    Quantity = request.Quantity,
                    ImgUrl = product.ProductImage
                });
            }
            SaveCartToSession(cart); //把更新後的購物車資料存回 Session
            return Ok(new { success = true });
        }

        //接收前端傳來的整串購物車清單
        [HttpPost]
        public IActionResult SaveCart([FromBody] List<CartViewModel> data)
        {
            if (data == null) return BadRequest();

            var cartJsonString = JsonSerializer.Serialize(data);
            HttpContext.Session.SetString(CartSessionKey, cartJsonString);
            return Ok(new { success = true });
        }

        private List<CartViewModel> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(cartJson) ? new List<CartViewModel>() : JsonSerializer.Deserialize<List<CartViewModel>>(cartJson);
        }
        private void SaveCartToSession(List<CartViewModel> cart)
        {
            var cartJsonString = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJsonString);
        }

        public class CartItemRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
