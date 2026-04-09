using Bake.Models.Social;
using Bake.Models.User;

namespace Bake.Areas.Seller.ViewModels
{
    public class FavoritePostViewModel
    {
        public List<Post> MySavePosts { get; set; } = new List<Post>();
        public List<UserProfile> MyFollowers { get; set; } = new List<UserProfile>();

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int SavedCount { get; set; }
        public int FollowersCount => MyFollowers.Count;
    }
}
