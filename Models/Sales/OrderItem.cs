using System;
using System.Collections.Generic;

namespace Bake.Models.Sales;

public partial class OrderItem
{
    public int ItemId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int ItemQuantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } //手動補上Autumn
}
