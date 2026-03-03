using System;
using System.Collections.Generic;

namespace Bake.Models.Platform;

public partial class TransactionStatusDefinition
{
    public byte StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
