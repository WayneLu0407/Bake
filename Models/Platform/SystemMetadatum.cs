using Bake.Models.User;
using System;
using System.Collections.Generic;

namespace Bake.Models.Platform;

public partial class SystemMetadatum
{
    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime LastLoginAt { get; set; }

    public string LastLoginIp { get; set; } = null!;

    public DateTime DeletedAt { get; set; }

    public string RegisterIp { get; set; } = null!;

    public virtual AccountAuth User { get; set; } = null!;
}
