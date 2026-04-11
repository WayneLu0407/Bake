namespace Bake.Areas.Seller.ViewModels
{
    public class MemberDashboardEventCardViewModel
    {
        public int PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime? EventTime { get; set; }
        public int LikesCount { get; set; }
    }
}