using Bake.Models.Sales;
using System;
using System.Collections.Generic;

namespace Bake.Models.Platform;

public partial class SellerWallet
{
    public int PayoutId { get; set; }

    public int UserId { get; set; }

    public decimal SellerAmount { get; set; }

    public decimal Fee { get; set; }

    public byte PayoutStatus { get; set; }

    public string BankRefId { get; set; } = null!;

    public virtual SellerWalletStatusDefinition PayoutStatusNavigation { get; set; } = null!;

    public virtual Shop User { get; set; } = null!;
}
