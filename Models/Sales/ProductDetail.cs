using System;
using System.Collections.Generic;

namespace Bake.Models.Sales;

public partial class ProductDetail
{
    public int ProductId { get; set; }

    public decimal ProductPrice { get; set; }

    public decimal? ProductDiscount { get; set; }

    public int ProductQuantity { get; set; }

    public DateTime ExpireDate { get; set; }
}
