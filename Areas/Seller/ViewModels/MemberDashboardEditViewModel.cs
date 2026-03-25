using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Bake.Areas.Seller.ViewModels
{
    public class MemberDashboardEditViewModel
    {

        [Required(ErrorMessage = "請輸入顯示名稱")]
        [StringLength(50, ErrorMessage = "真實姓名最多 50 字")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "個人介紹最多 1000 字")]
        public string? Bio { get; set; }

        public IFormFile? AvatarFile { get; set; }
    }
}