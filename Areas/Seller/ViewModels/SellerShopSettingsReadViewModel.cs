namespace Bake.Areas.Seller.ViewModels
{

    public class SellerShopSettingsReadViewModel
    {
        public string ShopName { get; set; } = string.Empty;

        public string? ShopDescription { get; set; }

        public string? ShopImg { get; set; }

        public byte? StatusId { get; set; }
    }
}