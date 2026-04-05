using System.ComponentModel.DataAnnotations;

namespace Bake.ViewModel
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "收件人姓名為必填")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "姓名長度需介於2~50字之間")]
        public string ReceiverName { get; set; }

        [Required(ErrorMessage = "聯絡電話為必填")]
        [RegularExpression(@"^09\d{8}$", ErrorMessage = "請輸入正確的台灣手機格式 (如: 0912345678)")]
        public string ReceiverPhone { get; set; }

        [Required(ErrorMessage = "收件地址為必填")]
        public string ReceiverAddress { get; set; }

        [Required(ErrorMessage = "電子信箱為必填")]
        [EmailAddress(ErrorMessage = "請輸入正確的電子信箱格式")]
        public string ReceiverEmail { get; set; }
    }
}
