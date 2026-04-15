
using Bake.Data;
using Bake.Models.Sales;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Bake.Controllers
{

    public class PayController:Controller
    {
        private readonly IConfiguration _config;
        private readonly BakeContext _bakeContext;
        public PayController(IConfiguration config, BakeContext bakeContext)
        {
            _config = config;
            _bakeContext = bakeContext;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        //callbackreturn
        public IActionResult CallbackReturn(NewebPayResponse model)
        {
            //// 解密訊息
            string HashKey = _config["NewebPayConfig:HashKey"];//API 串接金鑰
            string HashIV = _config["NewebPayConfig:HashIV"];//API 串接密碼
            string TradeInfoDecrypt = DecryptAESHex(model.TradeInfo, HashKey, HashIV);
            Console.WriteLine($"藍星回傳(加密):{TradeInfoDecrypt}");

            //讀取JSON格式的TradeInfo解密後的內容
            var receive = JsonSerializer.Deserialize<NewebPayDecrypted>(TradeInfoDecrypt);

            string merchantOrderNo = receive.Result.MerchantOrderNo;
            string status = receive.Status;

            //SUCCESS代表付款成功，其他則為失敗
            var parts = merchantOrderNo.Split('_');
            if (!string.IsNullOrEmpty(merchantOrderNo) && status == "SUCCESS")
            {
                // 拆解字串取得 OrderId
                if (parts.Length > 0 && int.TryParse(parts[0], out int orderId))
                {
                    // 導向到 CheckoutController 的 Success Action
                    return RedirectToAction("Success", "Checkout", new { id = orderId });
                }
            }
            //付款失敗
            else if (!string.IsNullOrEmpty(merchantOrderNo) && status != "SUCCESS") 
            {
                if (parts.Length > 0 && int.TryParse(parts[0], out int orderId))
                {
                    
                    return RedirectToAction("Fail", "Checkout", new { id = orderId, msg=receive.Message, amount=receive.Result.Amt });
                }
            }
            return RedirectToAction("Index", "Home");

            //return Json(model);
        }

        public class NewebPayResponse
        {
            public string Status { get; set; }      // SUCCESS or errors
            public string MerchantID { get; set; }
            public string Version { get; set; }     // e.g., 2.3
            public string TradeInfo { get; set; }   // AES Encrypted string
            public string TradeSha { get; set; }    // SHA256 Hash for verification
        }

        //接收藍新回傳的 TradeInfo 解密後的內容
        public class NewebPayDecrypted 
        {
            public string Status { get; set; }          // SUCCESS or errors
            public string Message { get; set; }
            public ResultContent Result { get; set; }
        }

        public class ResultContent
        {
            public string MerchantID { get; set; }
            public object Amt { get; set; }
            public string TradeNo { get; set; }
            public string MerchantOrderNo { get; set; }
            public string PaymentType { get; set; }
        }

        /// <summary>
        /// 商店取號網址
        /// </summary>
        /// <returns></returns>
        //public IActionResult CallbackCustomer(NewebPayResponse model)
        //{
        //    // 解密訊息
        //    string HashKey = _config["NewebPayConfig:HashKey"];//API 串接金鑰
        //    string HashIV = _config["NewebPayConfig:HashIV"];//API 串接密碼
        //    string TradeInfoDecrypt = DecryptAESHex(model.TradeInfo, HashKey, HashIV);

        //    //讀取JSON格式的TradeInfo解密後的內容
        //    var receive = JsonSerializer.Deserialize<NewebPayDecrypted>(TradeInfoDecrypt);

        //    string merchantOrderNo = receive.Result.MerchantOrderNo;
        //    string status = receive.Status;

        //    return View();
        //}

        /// <summary>
        /// 支付通知網址
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CallbackNotify(NewebPayResponse model)
        {
            // 解密訊息& 讀取JSON格式的TradeInfo解密後的內容
            string HashKey = _config["NewebPayConfig:HashKey"];//API 串接金鑰
            string HashIV = _config["NewebPayConfig:HashIV"];//API 串接密碼
            string TradeInfoDecrypt = DecryptAESHex(model.TradeInfo, HashKey, HashIV);
            var receive = JsonSerializer.Deserialize<NewebPayDecrypted>(TradeInfoDecrypt);

            //判斷狀態：確認 status == "SUCCESS"。
            if (receive == null || receive.Status != "SUCCESS") return BadRequest();

            //取得訂單資料
            string merchantOrderNo = receive.Result.MerchantOrderNo;
            int orderId = int.Parse(merchantOrderNo.Split('_')[0]);
            var order = await _bakeContext.Orders
                .Include(o=>o.OrderItems)
                .FirstOrDefaultAsync(o=>o.OrderId==orderId);
            if(order == null) return NotFound();

            //防重複處理
            //如果該訂單狀態已經是「已付款」，直接回傳 OK，不要重複扣庫存。
            if(order.StatusId != 0) return Ok("1"); // 0 代表「待付款」，1 代表「已付款」

            //更新訂單狀態：將訂單從「待付款」改為「待出貨」。
            using (var transaction = await _bakeContext.Database.BeginTransactionAsync())
            {
                try
                {
                    //更新訂單狀態
                    order.StatusId = 1; // 1 代表「已付款、待出貨」
                    order.UpdatedAt = DateTime.Now;

                    //庫存管理：遍歷該訂單的 OrderItems，從 ProductDetail 表中扣除對應數量。
                    foreach (var item in order.OrderItems)
                    {
                        var details = await _bakeContext.ProductDetails
                            .Include(pd => pd.Product)
                            .FirstOrDefaultAsync(p=>p.ProductId == item.ProductId);

                        if (details != null)
                        {
                            details.ProductQuantity -= item.ItemQuantity;

                            // 庫存不足，回滾交易並回傳錯誤
                            if (details.ProductQuantity < 0)
                            {
                                await transaction.RollbackAsync();
                                return StatusCode(400, $"產品 {details.Product.ProductName} 庫存不足");
                            }
                        }
                    }
                    await _bakeContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    //回應藍新：藍新需要收到伺服器回覆（通常是回傳字串），它才會停止發送通知
                    return Ok("1"); // 成功處理，回傳 OK 給藍新
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "內部伺服器錯誤"+ex.Message);
                }
            }
        }



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

        public static byte[] DecryptAES(byte[] source, string cryptoKey, string cryptoIV)
        {
            byte[] dataKey = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] dataIV = Encoding.UTF8.GetBytes(cryptoIV);

            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
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
