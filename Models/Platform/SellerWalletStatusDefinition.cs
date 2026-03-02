using System;
using System.Collections.Generic;

namespace Bake.Models.Platform;

public partial class SellerWalletStatusDefinition
{
    public byte StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<SellerWallet> SellerWallets { get; set; } = new List<SellerWallet>();
}
