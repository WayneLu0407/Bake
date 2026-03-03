using System;
using System.Collections.Generic;

namespace Bake.Models.Sales;

public partial class Refund
{
    public int RefundId { get; set; }

    public int OrderId { get; set; }

    public string? Reason { get; set; }

    public decimal RefundAmount { get; set; }

    public byte RefundStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual RefundStatusDefinition RefundStatusNavigation { get; set; } = null!;
}
