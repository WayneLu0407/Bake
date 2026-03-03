using System;
using System.Collections.Generic;

namespace Bake.Models.Sales;

public partial class OrderStatus
{
    public byte StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
