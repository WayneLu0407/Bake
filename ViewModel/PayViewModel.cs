namespace Bake.ViewModel
{
    public class PayViewModel
    {
        //required
        public string MerchantID { get; set; }
        public string TradeInfo { get; set; }
        public string TradeSha { get; set; }
        public string Version { get; set; }
        public string? PayGateWay { get; set; } 
    }
}
