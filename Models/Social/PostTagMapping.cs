using System;
using System.Collections.Generic;

namespace Bake.Models.Social;

public partial class PostTagMapping
{
    public int PostId { get; set; }

    public int TagId { get; set; }
}
