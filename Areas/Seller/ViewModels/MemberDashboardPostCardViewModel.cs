namespace Bake.Areas.Seller.ViewModels
{
    public class MemberDashboardPostCardViewModel
    {
        public int PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
    }
}