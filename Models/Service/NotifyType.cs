using System;
using System.Collections.Generic;

namespace Bake.Models.Service;

public partial class NotifyType
{
    public byte StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<SystemNotify> SystemNotifies { get; set; } = new List<SystemNotify>();
}
