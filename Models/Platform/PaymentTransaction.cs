using Bake.Models.Sales;
using System;
using System.Collections.Generic;

namespace Bake.Models.Platform;

public partial class PaymentTransaction
{
    public int TransactionId { get; set; }

    public int OrdersId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public byte TransactionStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Order Orders { get; set; } = null!;

    public virtual TransactionStatusDefinition TransactionStatusNavigation { get; set; } = null!;
}
