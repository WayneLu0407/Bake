namespace Bake.Areas.Seller.ViewModels
{
    public class ProductListViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string? ProductDescription { get; set; }
        public string CategoryName { get; set; }
        public int ProductQuantity { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal? ProductDiscount { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}