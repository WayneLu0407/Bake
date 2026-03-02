using Bake.Models.User;
using System;
using System.Collections.Generic;

namespace Bake.Models.Social;

public partial class PostFavorite
{
    public int UserId { get; set; }

    public int PostId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual UserProfile User { get; set; } = null!;
}
