using System;
using System.Collections.Generic;

namespace Bake.Models.Platform;

public partial class PaymentStatusDefinition
{
    public byte StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<PlatformEscrowLedger> PlatformEscrowLedgers { get; set; } = new List<PlatformEscrowLedger>();
}
