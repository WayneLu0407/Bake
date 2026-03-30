using Bake.Data;
using Bake.Models;
using Bake.Models.Sales;
using Bake.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Web;
using static Bake.ViewModel.PayViewModel;


namespace Bake.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayController : ControllerBase
    {
        private readonly BakeContext _bakeContext;
        private readonly IConfiguration _config;
        public PayController( BakeContext bakeContext, IConfiguration config)
        {
            _bakeContext = bakeContext;
            _config = config;
        }

        [HttpGet("GetTradeData/{id}")]
        public IActionResult GetTradeData(int id)
        {
            // 從資料庫拿訂單資料
            var order = _bakeContext.Orders.FirstOrDefault(o => o.OrderId == id);
            if(order == null) return NotFound("找不到訂單");

            // 從session拿Info資料、購物車資料
            string customerEmail = GetEmailFromSession();
            var cartItem = GetCartItemsFromSession();

            // 金流配置
            string merchantID = _config["NewebPayConfig:MerchantID"];
            string hashKey = _config["NewebPayConfig:HashKey"];
            string hashIV = _config["NewebPayConfig:HashIV"];
            string baseAddress = $"{Request.Scheme}://{Request.Host}";

            string itemDesc = string.Join(", ", cartItem.Select(c => c.ProductName));
            if (itemDesc.Length > 50) itemDesc = itemDesc.Substring(0, 47) + "...";
            // 金流資訊→組成藍新金流需要的TradeInfo
            var payData = new List<string>
            {
                $"MerchantID={merchantID}",
                $"RespondType=String",
                $"TimeStamp={DateTimeOffset.Now.ToUnixTimeSeconds()}",
                $"Version=2.0",
                $"MerchantOrderNo={order.OrderId}_{DateTimeOffset.Now.ToUnixTimeSeconds()}",  // 要加上時間戳?
                $"Amt={(int)order.TotalAmount}",
                $"ItemDesc={itemDesc}",
                $"ExpireDate={DateTime.Now.AddDays(3).ToString("yyyyMMdd")}",
                $"Email={customerEmail}",
                $"ReturnURL={baseAddress}/Pay/CallbackReturn",
                $"NotifyURL={baseAddress}/Pay/CallbackNotify",
                $"CustomerURL={baseAddress}/Pay/CallbackCustomer",
                (order.PaymentMethodId == 0 ? "CREDIT=1" : "CREDIT=0"),
                (order.PaymentMethodId == 1 ? "VACC=1" : "VACC=0"),
            };
            string rawTradeInfo = string.Join("&", payData);

            // 執行AES加密
            string TradeInfoEncrypt = EncryptAESHex(rawTradeInfo, hashKey, hashIV);
            //交易資料 SHA256 加密
            string TradeSha = EncryptSHA256($"HashKey={hashKey}&{TradeInfoEncrypt}&HashIV={hashIV}");

            //使用viewmodel包裝要傳給前端的資料
            var response = new PayViewModel
            {
                MerchantID = merchantID,
                TradeInfo = TradeInfoEncrypt,
                TradeSha = TradeSha,
                PayGateWay = _config["NewebPayConfig:PayGateWay"]
            };
            return Ok(response);
        }

        
        /// 支付完成返回網址
        //public IActionResult CallbackReturn()
        //{
        //    // 接收參數
        //    StringBuilder receive = new StringBuilder();
        //    foreach (var item in Request.Form)
        //    {
        //        receive.AppendLine(item.Key + "=" + item.Value + "<br>");
        //    }
        //    //ViewData["ReceiveObj"] = receive.ToString();

        //    // 解密訊息
        //    IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
        //    string HashKey = Config.GetSection("HashKey").Value;//API 串接金鑰
        //    string HashIV = Config.GetSection("HashIV").Value;//API 串接密碼

        //    string TradeInfoDecrypt = DecryptAESHex(Request.Form["TradeInfo"], HashKey, HashIV);
        //    NameValueCollection decryptTradeCollection = HttpUtility.ParseQueryString(TradeInfoDecrypt);
        //    receive.Length = 0;
        //    foreach (String key in decryptTradeCollection.AllKeys)
        //    {
        //        receive.AppendLine(key + "=" + decryptTradeCollection[key] + "<br>");
        //    }
        //    //ViewData["TradeInfo"] = receive.ToString();

        //    return View();
        //}

        ///// 商店取號網址？?
        ///// </summary>
        ///// <returns></returns>
        //public IActionResult CallbackCustomer()
        //{
        //    // 接收參數
        //    StringBuilder receive = new StringBuilder();
        //    foreach (var item in Request.Form)
        //    {
        //        receive.AppendLine(item.Key + "=" + item.Value + "<br>");
        //    }
        //    ViewData["ReceiveObj"] = receive.ToString();

        //    // 解密訊息
        //    IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
        //    string HashKey = Config.GetSection("HashKey").Value;//API 串接金鑰
        //    string HashIV = Config.GetSection("HashIV").Value;//API 串接密碼
        //    string TradeInfoDecrypt = DecryptAESHex(Request.Form["TradeInfo"], HashKey, HashIV);
        //    NameValueCollection decryptTradeCollection = HttpUtility.ParseQueryString(TradeInfoDecrypt);
        //    receive.Length = 0;
        //    foreach (String key in decryptTradeCollection.AllKeys)
        //    {
        //        receive.AppendLine(key + "=" + decryptTradeCollection[key] + "<br>");
        //    }
        //    ViewData["TradeInfo"] = receive.ToString();
        //    return View();
        //}

        
        ///// 支付通知網址 ??????
        ///// </summary>
        ///// <returns></returns>
        //public IActionResult CallbackNotify()
        //{
        //    // 接收參數
        //    StringBuilder receive = new StringBuilder();
        //    foreach (var item in Request.Form)
        //    {
        //        receive.AppendLine(item.Key + "=" + item.Value + "<br>");
        //    }
        //    ViewData["ReceiveObj"] = receive.ToString();

        //    // 解密訊息
        //    IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
        //    string HashKey = Config.GetSection("HashKey").Value;//API 串接金鑰
        //    string HashIV = Config.GetSection("HashIV").Value;//API 串接密碼
        //    string TradeInfoDecrypt = DecryptAESHex(Request.Form["TradeInfo"], HashKey, HashIV);
        //    NameValueCollection decryptTradeCollection = HttpUtility.ParseQueryString(TradeInfoDecrypt);
        //    receive.Length = 0;
        //    foreach (String key in decryptTradeCollection.AllKeys)
        //    {
        //        receive.AppendLine(key + "=" + decryptTradeCollection[key] + "<br>");
        //    }
        //    ViewData["TradeInfo"] = receive.ToString();

        //    return View();
        //}

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
}
