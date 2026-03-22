namespace Bake.Areas.Seller.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TodayRevenue { get; set; } //今日營收
        public int PendingOrdersCount { get; set; } //待出貨訂單數
        public int TotalProductsCount { get; set; } // 在架商品總數
        public int TotalOrdersMonth { get; set; } //本月總訂單數
        public decimal MonthRevenue { get; set; } //本月總營收

        //public int LowStorage { get; set; } //低庫存警示

        //public List<> TopProduct { get; set; } //熱銷產品

    }
}
