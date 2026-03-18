using Bake.Models.Sales;

namespace Bake.Areas.Seller.ViewModels
{
    public class OrderListViewModel
    {
        public int OrderId { get; set; }
        public string UserName { get; set; }
        public List<OrderItemInfo> Products { get; set; } = new List<OrderItemInfo>(); //訂單中的產品列表，用一個List來裝
        public string Email { get; set; }
        public string StatusName { get; set; }
        public int StatusId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class OrderItemInfo
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
