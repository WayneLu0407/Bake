namespace Bake.ViewModel
{
    public class PayViewModel
    {
        public class SendToNewebPayOut
        {
            //required
            public string MerchantID { get; set; }
            public string TradeInfo { get; set; }
            public string TradeSha { get; set; }
            public string Version { get; set; }
        }
        public class SendToNewebPayIn
        {
            public string ChannelID { get; set; }
            public string MerchantID { get; set; }
            public string MerchantOrderNo { get; set; } //商店訂單編號
            public string ItemDesc { get; set; } //商品資訊
            public string Amt { get; set; } //訂單金額

            public string ExpireDate { get; set; }
            public string ReturnURL { get; set; }
            public string ClientBackURL { get; set; } //返回商店網址
            public string CustomerURL { get; set; } //商店取號網址
            public string NotifyURL { get; set; } //支付通知網址
            public string Email { get; set; } //付款人電子信箱
        }

        
    }
}
