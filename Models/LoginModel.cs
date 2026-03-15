using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;

namespace Bake.Models
{
    public class LoginModel
    {
        
        [Required(ErrorMessage ="請輸入Email")]
        [EmailAddress(ErrorMessage ="Email格式不正確")]
        [StringLength(254,ErrorMessage ="Email長度過長")]
        public string Account { get; set; }
        [Required(ErrorMessage ="請輸入密碼")]
        [MinLength(8, ErrorMessage = "密碼最少需要 8 個字元")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}