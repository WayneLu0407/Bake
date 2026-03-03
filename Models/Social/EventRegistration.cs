using Bake.Models.User;
using System;
using System.Collections.Generic;

namespace Bake.Models.Social;

public partial class EventRegistration
{
    public int RegistrationId { get; set; }

    public int EventId { get; set; }

    public int UserId { get; set; }

    public int NumParticipants { get; set; }

    public string? Notes { get; set; }

    public byte RegistStatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual EventDetail Event { get; set; } = null!;

    public virtual RegistStatusLookup RegistStatus { get; set; } = null!;

    public virtual UserProfile User { get; set; } = null!;
}
