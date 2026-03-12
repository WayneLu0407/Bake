using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bake.Models.Sales;

public partial class ProductDetail
{
    [Key, ForeignKey("Product")]
    public int ProductId { get; set; }

    public decimal ProductPrice { get; set; }

    public decimal? ProductDiscount { get; set; }

    public int ProductQuantity { get; set; }

    public DateTime? ExpireDate { get; set; }
    public virtual Product Product { get; set; } = null!;
}


