using System.ComponentModel.DataAnnotations;

namespace Bake.Models
{
    public class ForgetpasswordModel
    {
        [Required(ErrorMessage = "請輸入Email")]
        [EmailAddress(ErrorMessage = "Email格式不正確")]
        [StringLength(254, ErrorMessage = "Email長度過長")]
        public string Email { get; set; }
        
    }
}