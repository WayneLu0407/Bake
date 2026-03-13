using System.ComponentModel.DataAnnotations;

namespace Bake.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage ="請輸入名稱")]
        [StringLength(20)]
        public string Name { get; set; }
        [Required(ErrorMessage = "請輸入Email")]
        [EmailAddress(ErrorMessage = "Email格式不正確")]
        [StringLength(254, ErrorMessage = "Email長度過長")]
        public string Email { get; set; }
        [Required(ErrorMessage ="請輸入密碼")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage ="請輸入二次密碼")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "密碼與確認密碼不一致")]
        public string PasswordConfirm { get; set; }
        

    }
}