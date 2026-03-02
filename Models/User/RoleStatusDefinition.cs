using System;
using System.Collections.Generic;

namespace Bake.Models.User;

public partial class RoleStatusDefinition
{
    public byte StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<AccountAuth> AccountAuths { get; set; } = new List<AccountAuth>();
}
