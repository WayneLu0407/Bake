using System.ComponentModel.DataAnnotations;

namespace Bake.ViewModel
{
    public class ProductReviewCreateViewModel
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; } = "";

        [Range(1, 5, ErrorMessage = "評分必須介於1到5")]
        public byte UserRating { get; set; } = 5;

        [Required(ErrorMessage = "請輸入評論內容")]
        [StringLength(500, ErrorMessage = "評論內容不可超過500字")]
        public string Comment { get; set; } = "";
    }
}
