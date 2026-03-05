using System.ComponentModel.DataAnnotations;

namespace Bake.Areas.Seller.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        [Display(Name = "商品名稱")]
        public string? ProductName { get; set; }

        [Display(Name = "商品描述")]
        public string? ProductDescription { get; set; }

        [Display(Name = "商品圖片")]
        public IFormFile? ProductImage { get; set; }
        public string? ProductImagePath { get; set; }

        [Display(Name = "商品分類")]
        public int? CategoryId { get; set; }

        [Display(Name = "商品價格")]
        public decimal? ProductPrice { get; set; }

        [Display(Name = "商品數量")]
        public int? ProductQuantity { get; set; }
    }
}