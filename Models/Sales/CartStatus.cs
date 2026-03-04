using System;
using System.Collections.Generic;

namespace Bake.Models.Sales;

public partial class CartStatus
{
    public byte StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
