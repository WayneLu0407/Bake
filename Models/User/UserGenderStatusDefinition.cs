using System;
using System.Collections.Generic;

namespace Bake.Models.User;

public partial class UserGenderStatusDefinition
{
    public byte StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}
