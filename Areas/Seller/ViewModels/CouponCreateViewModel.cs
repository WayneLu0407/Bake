using System.ComponentModel.DataAnnotations;

namespace Bake.Areas.Seller.ViewModels
{
    public class CouponCreateViewModel
    {
        [Required(ErrorMessage = "請輸入折扣金額")]
        [Range(1,500, ErrorMessage = "折扣金額必須在1~500之間")]
        public decimal DiscountAmount { get; set; }

        [Required(ErrorMessage = "請輸入最低消費門檻")]
        [Range(1,100000,ErrorMessage = "最低消費不能為負數")]
        public decimal MinimumPurchase { get; set; }

        [Required(ErrorMessage ="請輸入到期日")]
        public DateTime ExpirationDate { get; set; }
    }
}