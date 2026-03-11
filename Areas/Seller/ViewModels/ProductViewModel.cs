using System.ComponentModel.DataAnnotations;

namespace Bake.Areas.Seller.ViewModels
{
    public class ProductViewModel 
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "請輸入商品名稱")]
        [Display(Name = "商品名稱")]
        public string ProductName { get; set; }


        [Required(ErrorMessage = "請輸入商品描述")]
        [Display(Name = "商品描述")]
        public string ProductDescription { get; set; }


        [Display(Name = "上傳新圖片")]
        public IFormFile? ProductImage { get; set; }

        // 用來存放並顯示資料庫中現有的圖片路徑
        [Display(Name = "目前圖片路徑")]
        public string? ProductImagePath { get; set; }


        [Required(ErrorMessage = "請選擇分類")]
        [Display(Name = "商品分類")]
        public int CategoryId { get; set; }

        [Range(1, 9999999999, ErrorMessage = "價格必須大於1")]
        [Required(ErrorMessage = "請輸入商品價格")]
        [Display(Name = "商品價格")]
        public decimal ProductPrice { get; set; }

        [Range(0, 0.99, ErrorMessage = "折數必須介於 0 到 0.99 之間")]
        [Display(Name = "優惠折數(如: 不打折填 0，九折填 0.1)")]
        public decimal? ProductDiscount { get; set; }

        [Required(ErrorMessage = "請輸入數量")]
        [Display(Name = "商品數量")]
        [Range(1,int.MaxValue, ErrorMessage = "數量必須大於 0")]
        public int ProductQuantity { get; set; }

        [Required(ErrorMessage = "請輸入日期")]
        [Display(Name = "商品過期日")]
        public DateTime ExpireDate { get; set; }= DateTime.Now.AddDays(7);
    }

}