using Bake.Models.User;
using System;
using System.Collections.Generic;

namespace Bake.Models.Social;

public partial class Post
{
    public int PostId { get; set; }

    public int AuthorId { get; set; }

    public byte TypeId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int? ViewCount { get; set; }

    public int? LikesCount { get; set; }

    public int? FavoriteCount { get; set; }

    public bool? IsPublished { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual UserProfile Author { get; set; } = null!;

    public virtual ICollection<EventDetail> EventDetails { get; set; } = new List<EventDetail>();

    public virtual ICollection<PostAttachment> PostAttachments { get; set; } = new List<PostAttachment>();

    public virtual ICollection<PostFavorite> PostFavorites { get; set; } = new List<PostFavorite>();

    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    public virtual PostTypeLookup Type { get; set; } = null!;

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
