using System.ComponentModel.DataAnnotations;

namespace Bake.Areas.Seller.Model
{
    public class ReSetPasswordModel
    {
        public string? Email { get; set; }
        [Required(ErrorMessage = "請輸入密碼")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "請輸入二次密碼")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "密碼與確認密碼不一致")]
        public string ConfirmPassword { get; set; }
    }
}