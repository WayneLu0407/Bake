using Bake.Models.Sales;

namespace Bake.ViewModel.ShopFront
{
    public class ShopProfilePageViewModel
    {
        public Shop ShopInfo { get; set; } = null!;

        public List<ShopProductCardViewModel> Products { get; set; } = new();
        public List<ShopCategoryFilterItemViewModel> Categories { get; set; } = new();

        public string? Keyword { get; set; }
        public int? CategoryId { get; set; }
        public string Sort { get; set; } = "latest";
        public string PriceRange { get; set; } = "all";

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalCount { get; set; }

        public int TotalPages =>
            PageSize <= 0 ? 1 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    public class ShopProductCardViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string? ProductImage { get; set; }
        public string ShopName { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? Rating { get; set; }
    }

    public class ShopCategoryFilterItemViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public int Count { get; set; }
    }
}