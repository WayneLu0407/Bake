using System;
using System.Collections.Generic;

namespace Bake.Models.Social;

public partial class PostAttachment
{
    public int ImageId { get; set; }

    public int PostId { get; set; }

    public string FileUrl { get; set; } = null!;

    public string? AltText { get; set; }

    public bool? IsCover { get; set; }

    public int? SortOrder { get; set; }

    public virtual Post Post { get; set; } = null!;
}
