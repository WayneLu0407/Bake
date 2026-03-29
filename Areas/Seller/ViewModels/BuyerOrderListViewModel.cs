using Bake.Models.Sales;
namespace Bake.Areas.Seller.ViewModels
{
    public class BuyerOrderListViewModel
    {
        public int OrderId { get; set; }

        public List<Item> ProductsList { get; set; } = new List<Item>();
        public string ShippingAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
        public string StatusName { get; set; }
    }
   public class Item
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        
        public bool IsReviewed { get; set; }
    }

    public class OrderPagedListViewModel
    {
        public IEnumerable<BuyerOrderListViewModel> Orders { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

    }
}
