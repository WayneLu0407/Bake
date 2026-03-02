using Bake.Models.User;
using System;
using System.Collections.Generic;

namespace Bake.Models.Social;

public partial class Follow
{
    public int FollowerId { get; set; }

    public int BefollowedId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual UserProfile Befollowed { get; set; } = null!;

    public virtual UserProfile Follower { get; set; } = null!;
}
