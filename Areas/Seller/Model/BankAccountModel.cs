using System.ComponentModel.DataAnnotations;

namespace Bake.Areas.Seller.Model
{
    public class BankAccountModel
    {

        [Required(ErrorMessage = "請輸入銀行帳號")]
        [Display(Name = "銀行帳戶")]
        // 正規表示式解釋：
        // ^\d{3}      : 開頭必須是 3 位數字 (銀行代碼)
        // -           : 中間必須有一個橫槓
        // \d{6,14}$   : 後面接 6 到 14 位數字 (帳號通常長度在此區間)，並以此結尾
        [RegularExpression(@"^\d{3}-\d{6,14}$", ErrorMessage = "格式錯誤，請輸入：銀行代碼(3碼)-帳號，例如：007-1234567890")]
        public String BankAccount {  get; set; }
    }
}