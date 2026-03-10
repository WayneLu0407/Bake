using System.ComponentModel.DataAnnotations;

namespace Bake.Areas.Seller.ViewModels
{
    public class ProductViewModel 
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "請輸入商品名稱")]
        [Display(Name = "商品名稱")]
        public string? ProductName { get; set; }


        [Required(ErrorMessage = "請輸入商品描述")]
        [Display(Name = "商品描述")]
        public string? ProductDescription { get; set; }

        [Required(ErrorMessage = "請附上商品圖片")]
        [Display(Name = "商品圖片")]
        public IFormFile? ProductImage { get; set; }
        public string? ProductImagePath { get; set; }


        [Required(ErrorMessage = "請選擇分類")]
        [Display(Name = "商品分類")]
        public int? CategoryId { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "價格必須大於 0")]
        [Required(ErrorMessage = "請輸入商品價格")]
        [Display(Name = "商品價格")]
        public decimal? ProductPrice { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "價格必須大於 0")]
        [Display(Name = "商品優惠價格")]
        public decimal? ProductDiscount { get; set; }

        [Required(ErrorMessage = "請輸入數量")]
        [Display(Name = "商品數量")]
        [Range(1,int.MaxValue, ErrorMessage = "數量必須大於 0")]
        public int? ProductQuantity { get; set; }
    }

}