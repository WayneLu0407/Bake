namespace Bake.Areas.Seller.ViewModels
{
    public class MemberDashboardViewModel
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        //public string RoleName { get; set; } = string.Empty;  //保留如果需要身分辨識
        public bool IsEmailConfirmed { get; set; }

        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; } 

        //public int PostCount { get; set; }
        //public int FollowersCount { get; set; }
        //public int FollowingCount { get; set; }

        //public bool HasShop { get; set; }
        //public string? ShopName { get; set; }
    }
}