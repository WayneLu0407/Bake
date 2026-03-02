using System;
using System.Collections.Generic;

namespace Bake.Models.Sales;

public partial class ShopStatus
{
    public byte StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Shop> Shops { get; set; } = new List<Shop>();
}
