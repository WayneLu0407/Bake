using System.ComponentModel.DataAnnotations;

namespace Bake.Areas.Seller.ViewModels
{
    public class CouponCreateViewModel
    {
        [Required(ErrorMessage = "請輸入折扣金額")]
        [Range(1, 10000, ErrorMessage = "折扣金額不能為負數")]
        public decimal DiscountAmount { get; set; }

        [Required(ErrorMessage = "請輸入最低消費門檻")]
        [Range(1, (double)decimal.MaxValue, ErrorMessage = "最低消費不能為負數")]
        public decimal MinimumPurchase { get; set; }

        [Required(ErrorMessage ="請輸入到期日")]
        public DateTime? ExpirationDate { get; set; }
    }
}