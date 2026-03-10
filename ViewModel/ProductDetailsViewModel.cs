using Bake.Models.Sales;

namespace Bake.ViewModel.Products
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; } = null!;
        public List<ProductReview> Reviews { get; set; } = new();
    }
}