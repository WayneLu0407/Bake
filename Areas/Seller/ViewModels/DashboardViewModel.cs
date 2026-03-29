namespace Bake.Areas.Seller.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TodayRevenue { get; set; } //今日營收
        
        public int PendingOrdersCount { get; set; } //待出貨訂單數
        public int TotalProductsCount { get; set; } // 在架商品總數
        public int TotalOrdersMonth { get; set; } //本月總訂單數
        public decimal MonthRevenue { get; set; } //本月總營收

        public List<string> Last7DaysLabels { get; set; } //過去7天趨勢圖
        public List<int> Last7DaysData { get; set; } //過去7天趨勢圖

        public List<string> CategoryLabels { get; set; } //類別銷售圓餅圖
        public List<decimal> CategoryData { get; set; }//類別銷售圓餅圖

        public List<ProductStockDto> LowStock { get; set; } //低庫存警示
        public List<TopProductDto> TopProducts { get; set; } //熱銷商品
    }

    public class ProductStockDto
    {
        public string LowPName { get; set; }
        public int Stock { get; set; }
    }

    public class TopProductDto 
    {
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public int TotalSales { get; set; }
        public decimal Revenue { get; set; }
        public string ImageUrl { get; set; }
    }
}
