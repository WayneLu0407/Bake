using System.ComponentModel.DataAnnotations;

namespace Bake.Areas.Seller.Model
{
    public class BankAccountChangeModel
    {
        [Required(ErrorMessage = "請輸入銀行帳號")]
        // 固定長度為 12，且必須全是數字
        [RegularExpression(@"^\d{12}$", ErrorMessage = "銀行帳號必須為 12 碼數字")]
        public String BankAccount { get; set; }
    }
}