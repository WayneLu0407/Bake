using System.ComponentModel.DataAnnotations;

namespace Bake.Areas.Seller.ViewModels
{
    public class SettingsViewModel
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Email 為必填")]
        [EmailAddress(ErrorMessage = "請輸入正確的 Email 格式")]
        public string Email {  get; set; }
        [Required(ErrorMessage = "請輸入密碼")]
        [MinLength(6, ErrorMessage = "密碼最少需要 6 個字元")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "請輸入姓名")]
        [StringLength(50,MinimumLength =2,ErrorMessage ="最少需要2個字元")]
        public string Name {  get; set; }
        
    }
}