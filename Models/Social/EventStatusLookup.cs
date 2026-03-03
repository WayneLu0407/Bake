using System;
using System.Collections.Generic;

namespace Bake.Models.Social;

public partial class EventStatusLookup
{
    public byte StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<EventDetail> EventDetails { get; set; } = new List<EventDetail>();
}
