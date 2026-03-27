// Models/Social/PostComment.cs

using Bake.Models.User;
using System;

namespace Bake.Models.Social;

public partial class PostComment
{
    public int CommentId { get; set; }

    public int PostId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public int? ParentCommentId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual UserProfile User { get; set; } = null!;

    public virtual PostComment? ParentComment { get; set; }

    public virtual ICollection<PostComment> Replies { get; set; } = new List<PostComment>();
}