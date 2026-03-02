using System;
using System.Collections.Generic;

namespace Bake.Models.Social;

public partial class PostTypeLookup
{
    public byte TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
