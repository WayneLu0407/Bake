using AspNetCoreGeneratedDocument;
using Bake.Data;
using Bake.Models;
using Bake.Models.Sales;
using Bake.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using static Bake.ViewModel.PayViewModel;


namespace Bake.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayController : Controller
    {
        private readonly BakeContext _bakeContext;
        private readonly IConfiguration _config;
        public PayController( BakeContext bakeContext, IConfiguration config)
        {
            _bakeContext = bakeContext;
            _config = config;
        }

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



        [Authorize]
        [HttpPost("GetTradeData")]
        public async Task<IActionResult> GetTradeData([FromForm]CheckoutViewModel checkoutViewModel, [FromForm]string PaymentMethod)
        {
            int userId = CurrentUserId;

            // 驗收防呆：如果真的抓不到登入者 ID，不要讓它進資料庫
            if (userId == 0)
            {
                return RedirectToAction("Login", "Home");
            }

            //從session拿Info資料
            var infoJson = HttpContext.Session.GetString("ReceiverInfo");
            if (string.IsNullOrEmpty(infoJson)) return RedirectToAction("Info");
            var receiverInfo = JsonSerializer.Deserialize<CheckoutViewModel>(infoJson);

            //1. 建立Order物件實體，並將checkoutdata資料填入Order物件中
            var order = new Order
            {
                UserId = userId,
                ShippingAddress = receiverInfo.ReceiverAddress,
                TotalAmount = 0, //這裡先設為0，實際金額應該從購物車計算
                PaymentMethodId = byte.Parse(PaymentMethod),
                StatusId = 0, //0:待付款、1:待出貨
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            //2. 建立OrderItem物件實體，並將購物車中的每個商品資料填入OrderItem物件中，並加入Order的OrderItems集合中
            var cartItems = GetCartItemsFromSession();
            if (!cartItems.Any()) return RedirectToAction("Index", "Cart"); // 沒東西買就回購物車

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


            int shippingFee = HttpContext.Session.GetInt32("ShippingFee") ?? 60;
            order.TotalAmount = totalAmount + shippingFee;

            //3. 寫入資料庫
            _bakeContext.Orders.Add(order);
            await _bakeContext.SaveChangesAsync();

            ClearCart(); //清空購物車

            //4.重新導向: 如果是貨到付款，直接到Success
            if (order.PaymentMethodId == 2)
            {
                return Ok(new { success=true, isCod=true, id = order.OrderId });
            }


            // [ 待修改 ] 用ViewBag拿資料
            string customerEmail = GetEmailFromSession();

            // 金流配置
            string merchantID = _config["NewebPayConfig:MerchantID"];
            string hashKey = _config["NewebPayConfig:HashKey"];
            string hashIV = _config["NewebPayConfig:HashIV"];
            string baseAddress = $"{Request.Scheme}://{Request.Host}";

            string itemDesc = string.Join(",", cartItems.Select(item => item.ProductName.Trim() ?? "烘焙商品"))
                .Replace("&", "").Replace("=", "");
            if (itemDesc.Length > 50) itemDesc = itemDesc.Substring(0, 47) + "...";
            long nowTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            // 金流資訊→組成藍新金流需要的TradeInfo
            var payData = new List<string>
            {
                $"MerchantID={merchantID}",
                $"RespondType=JSON",
                $"TimeStamp={nowTimeStamp}",
                $"Version=2.3",
                $"MerchantOrderNo={order.OrderId}_{nowTimeStamp}",
                $"Amt={(int)order.TotalAmount}",
                $"ItemDesc=testorder",
                $"ExpireDate={DateTime.Now.AddDays(3).ToString("yyyyMMdd")}",
                $"Email={customerEmail}",
                $"ReturnURL={baseAddress}/Pay/CallbackReturn",
                $"NotifyURL={baseAddress}/Pay/CallbackNotify",
                $"CustomerURL={baseAddress}/Pay/CallbackCustomer",
                (order.PaymentMethodId == 0 ? "CREDIT=1" : "CREDIT=0"),
                (order.PaymentMethodId == 1 ? "VACC=1" : "VACC=0"),
            };
            string rawTradeInfo = string.Join("&", payData).Trim();
            Console.WriteLine($"加密前字串:{rawTradeInfo}");

            // 交易資料做 AES加密、SHA256 加密
            string TradeInfoEncrypt = EncryptAESHex(rawTradeInfo, hashKey, hashIV);
            string shaSource = $"HashKey={hashKey}&{TradeInfoEncrypt}&HashIV={hashIV}";
            string TradeSha = EncryptSHA256(shaSource);

            //使用viewmodel包裝要傳給前端的資料
            var response = new PayViewModel
            {
                MerchantID = merchantID,
                TradeInfo = TradeInfoEncrypt,
                TradeSha = TradeSha,
                Version = "2.3",
                PayGateWay = _config["NewebPayConfig:PayGateWay"]
            };

            //檢查用
            //string testDecrypt = DecryptAESHex(TradeInfoEncrypt, hashKey, hashIV);
            //Console.WriteLine($"反向解密結果: {testDecrypt}");
            //Console.WriteLine($"[Debug] Raw: {rawTradeInfo}");
            //string shaSource_ = $"HashKey={hashKey}&{TradeInfoEncrypt.ToUpper()}&HashIV={hashIV}";
            //Console.WriteLine($"[Debug] SHA Source: {shaSource_}");
            return Ok(new { isCod=false, payData = response});

            
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        //callbackreturn
        public IActionResult CallbackReturn(NewebPayResponse model)
        {
            // 接收參數
            //StringBuilder receive = new StringBuilder();
            //foreach (var item in Request.Form)
            //{
            //    receive.AppendLine(item.Key + "=" + item.Value + "<br>");
            //}
            //ViewData["ReceiveObj"] = receive.ToString();

            //// 解密訊息
            //IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            //string HashKey = Config.GetSection("HashKey").Value;//API 串接金鑰
            //string HashIV = Config.GetSection("HashIV").Value;//API 串接密碼

            //string TradeInfoDecrypt = DecryptAESHex(Request.Form["TradeInfo"], HashKey, HashIV);
            //NameValueCollection decryptTradeCollection = HttpUtility.ParseQueryString(TradeInfoDecrypt);
            //receive.Length = 0;
            //foreach (String key in decryptTradeCollection.AllKeys)
            //{
            //    receive.AppendLine(key + "=" + decryptTradeCollection[key] + "<br>");
            //}
            //ViewData["TradeInfo"] = receive.ToString();
            //Console.WriteLine($"藍星回傳:{receive.ToString()}");

            //string merchantOrderNo = decryptTradeCollection["MerchantOrderNo"];
            //string status = decryptTradeCollection["Status"]; // 藍新回傳的狀態碼

            //if (!string.IsNullOrEmpty(merchantOrderNo) && status == "SUCCESS")
            //{
            //    // 2. 拆解字串取得 OrderId
            //    var parts = merchantOrderNo.Split('_');
            //    if (parts.Length > 0 && int.TryParse(parts[0], out int orderId))
            //    {
            //        // 3. 導向到 CheckoutController 的 Success Action
            //        // 參數順序：(Action名稱, Controller名稱, 路由參數)
            //        return RedirectToAction("Success", "Checkout", new { id = orderId });
            //    }
            //}
            //return RedirectToAction("Index", "Home");

            return Json(model);
        }

        /// <summary>
        /// 商店取號網址
        /// </summary>
        /// <returns></returns>
        public IActionResult CallbackCustomer()
        {
            // 接收參數
            StringBuilder receive = new StringBuilder();
            foreach (var item in Request.Form)
            {
                receive.AppendLine(item.Key + "=" + item.Value + "<br>");
            }
            ViewData["ReceiveObj"] = receive.ToString();

            // 解密訊息
            IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            string HashKey = Config.GetSection("HashKey").Value;//API 串接金鑰
            string HashIV = Config.GetSection("HashIV").Value;//API 串接密碼
            string TradeInfoDecrypt = DecryptAESHex(Request.Form["TradeInfo"], HashKey, HashIV);
            NameValueCollection decryptTradeCollection = HttpUtility.ParseQueryString(TradeInfoDecrypt);
            receive.Length = 0;
            foreach (String key in decryptTradeCollection.AllKeys)
            {
                receive.AppendLine(key + "=" + decryptTradeCollection[key] + "<br>");
            }
            ViewData["TradeInfo"] = receive.ToString();
            return View();
        }

        /// <summary>
        /// 支付通知網址
        /// </summary>
        /// <returns></returns>
        public IActionResult CallbackNotify()
        {
            // 接收參數
            StringBuilder receive = new StringBuilder();
            foreach (var item in Request.Form)
            {
                receive.AppendLine(item.Key + "=" + item.Value + "<br>");
            }
            ViewData["ReceiveObj"] = receive.ToString();

            // 解密訊息
            IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            string HashKey = Config.GetSection("HashKey").Value;//API 串接金鑰
            string HashIV = Config.GetSection("HashIV").Value;//API 串接密碼
            string TradeInfoDecrypt = DecryptAESHex(Request.Form["TradeInfo"], HashKey, HashIV);
            NameValueCollection decryptTradeCollection = HttpUtility.ParseQueryString(TradeInfoDecrypt);
            receive.Length = 0;
            foreach (String key in decryptTradeCollection.AllKeys)
            {
                receive.AppendLine(key + "=" + decryptTradeCollection[key] + "<br>");
            }
            ViewData["TradeInfo"] = receive.ToString();

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
        private string GetEmailFromSession()
        {
            var infoJson = HttpContext.Session.GetString("ReceiverInfo");

            if (string.IsNullOrEmpty(infoJson))
            {
                return string.Empty;
            }
            var model = JsonSerializer.Deserialize<CheckoutViewModel>(infoJson);
            return model.ReceiverEmail;
        }

        private void ClearCart()
        {
            HttpContext.Session.Remove("UserCart");
        }

        // ↓↓↓加密解密方法↓↓↓
        /// <summary>
        /// 加密後再轉 16 進制字串
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <param name="cryptoKey">加密金鑰</param>
        /// <param name="cryptoIV">cryptoIV</param>
        /// <returns>加密後的字串</returns>
        public string EncryptAESHex(string source, string cryptoKey, string cryptoIV)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(source))
            {
                var encryptValue = EncryptAES(Encoding.UTF8.GetBytes(source), cryptoKey, cryptoIV);

                if (encryptValue != null)
                {
                    result = BitConverter.ToString(encryptValue)?.Replace("-", string.Empty)?.ToLower();
                }
            }

            return result;
        }

        /// <summary>
        /// 字串加密AES
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <param name="cryptoKey">加密金鑰</param>
        /// <param name="cryptoIV">cryptoIV</param>
        /// <returns>加密後字串</returns>
        public byte[] EncryptAES(byte[] source, string cryptoKey, string cryptoIV)
        {
            byte[] dataKey = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] dataIV = Encoding.UTF8.GetBytes(cryptoIV);

            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
                aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                aes.Key = dataKey;
                aes.IV = dataIV;

                using (var encryptor = aes.CreateEncryptor())
                {
                    return encryptor.TransformFinalBlock(source, 0, source.Length);
                }
            }
        }

        /// <summary>
        /// 字串加密SHA256
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <returns>加密後字串</returns>
        public string EncryptSHA256(string source)
        {
            string result = string.Empty;

            using (System.Security.Cryptography.SHA256 algorithm = System.Security.Cryptography.SHA256.Create())
            {
                var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(source));

                if (hash != null)
                {
                    result = BitConverter.ToString(hash)?.Replace("-", string.Empty)?.ToUpper();
                }

            }
            return result;
        }

        /// <summary>
        /// 16 進制字串解密
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <param name="cryptoKey">加密金鑰</param>
        /// <param name="cryptoIV">cryptoIV</param>
        /// <returns>解密後的字串</returns>
        public string DecryptAESHex(string source, string cryptoKey, string cryptoIV)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(source))
            {
                // 將 16 進制字串 轉為 byte[] 後
                byte[] sourceBytes = ToByteArray(source);

                if (sourceBytes != null)
                {
                    // 使用金鑰解密後，轉回 加密前 value
                    result = Encoding.UTF8.GetString(DecryptAES(sourceBytes, cryptoKey, cryptoIV)).Trim();
                }
            }

            return result;
        }

        /// <summary>
        /// 將16進位字串轉換為byteArray
        /// </summary>
        /// <param name="source">欲轉換之字串</param>
        /// <returns></returns>
        public byte[] ToByteArray(string source)
        {
            byte[] result = null;

            if (!string.IsNullOrWhiteSpace(source))
            {
                var outputLength = source.Length / 2;
                var output = new byte[outputLength];

                for (var i = 0; i < outputLength; i++)
                {
                    output[i] = Convert.ToByte(source.Substring(i * 2, 2), 16);
                }
                result = output;
            }

            return result;
        }

        /// <summary>
        /// 字串解密AES
        /// </summary>
        /// <param name="source">解密前字串</param>
        /// <param name="cryptoKey">解密金鑰</param>
        /// <param name="cryptoIV">cryptoIV</param>
        /// <returns>解密後字串</returns>
        public static byte[] DecryptAES(byte[] source, string cryptoKey, string cryptoIV)
        {
            byte[] dataKey = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] dataIV = Encoding.UTF8.GetBytes(cryptoIV);

            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
                // 智付通無法直接用PaddingMode.PKCS7，會跳"填補無效，而且無法移除。"
                // 所以改為PaddingMode.None並搭配RemovePKCS7Padding
                aes.Padding = System.Security.Cryptography.PaddingMode.None;
                aes.Key = dataKey;
                aes.IV = dataIV;

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] data = decryptor.TransformFinalBlock(source, 0, source.Length);
                    int iLength = data[data.Length - 1];
                    var output = new byte[data.Length - iLength];
                    Buffer.BlockCopy(data, 0, output, 0, output.Length);
                    return output;
                }
            }
        }
    }

    public class NewebPayResponse
    {
        public string Status { get; set; }      // SUCCESS or errors
        public string MerchantID { get; set; }
        public string Version { get; set; }     // e.g., 2.3
        public string TradeInfo { get; set; }   // AES Encrypted string
        public string TradeSha { get; set; }    // SHA256 Hash for verification
    }
}
