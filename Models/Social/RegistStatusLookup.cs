using System;
using System.Collections.Generic;

namespace Bake.Models.Social;

public partial class RegistStatusLookup
{
    public byte RegStatusId { get; set; }

    public string RegStatusName { get; set; } = null!;

    public virtual ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
}
