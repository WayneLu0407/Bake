namespace Bake.Areas.Seller.ViewModels
{
    public class MemberDashboardViewModel
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public bool IsEmailConfirmed { get; set; }

        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }

        public int PostCount { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }

        public List<MemberDashboardPostCardViewModel> MyPosts { get; set; } = new();
        public List<MemberDashboardEventCardViewModel> ActiveHostedEvents { get; set; } = new();
        public List<MemberDashboardEventCardViewModel> PastHostedEvents { get; set; } = new();

        public List<MemberDashboardEventCardViewModel> ActiveEvents { get; set; } = new ();
        public List<MemberDashboardEventCardViewModel> PastActiveEvents { get; set; } = new();
    }
}