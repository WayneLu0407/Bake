using System;
using System.Collections.Generic;

namespace Bake.Models.Social;

public partial class EventDetail
{
    public int EventId { get; set; }

    public int? PostId { get; set; }

    public byte EventTypeId { get; set; }

    public byte ManualStatusId { get; set; }

    public int? Price { get; set; }

    public int MaxParticipants { get; set; }

    public DateTime SignupStart { get; set; }

    public DateTime SignupDeadline { get; set; }

    public DateTime EventTime { get; set; }

    public DateTime EventEndTime { get; set; }

    public string? LocationCity { get; set; }

    public string? LocationAddress { get; set; }

    public virtual ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();

    public virtual EventTypeLookup EventType { get; set; } = null!;

    public virtual EventStatusLookup ManualStatus { get; set; } = null!;

    public virtual Post? Post { get; set; }
}
