using System;
using System.Collections.Generic;

namespace Bake.Models.Social;

public partial class EventTypeLookup
{
    public byte EventTypeId { get; set; }

    public string EventTypeName { get; set; } = null!;

    public virtual ICollection<EventDetail> EventDetails { get; set; } = new List<EventDetail>();
}
