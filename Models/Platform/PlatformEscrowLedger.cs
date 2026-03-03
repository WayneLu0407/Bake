using Bake.Models.Sales;
using System;
using System.Collections.Generic;

namespace Bake.Models.Platform;

public partial class PlatformEscrowLedger
{
    public int LedgerId { get; set; }

    public int OrderId { get; set; }

    public decimal HeldAmount { get; set; }

    public byte PaymentStatus { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual PaymentStatusDefinition PaymentStatusNavigation { get; set; } = null!;
}
