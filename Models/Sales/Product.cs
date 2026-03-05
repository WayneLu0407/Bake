using Bake.Models.User;
using System;
using System.Collections.Generic;

namespace Bake.Models.Sales;

public partial class Product
{
    public int ProductId { get; set; }

    public int UserId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? ProductImage { get; set; }

    public string ProductMethod { get; set; } = null!;

    public string? ProductDescription { get; set; }

    public decimal? ProductRating { get; set; }

    public DateTime ProductDate { get; set; }

    public int CategoryId { get; set; }

    public virtual ProductCategory Category { get; set; } = null!;

    public virtual AccountAuth User { get; set; } = null!;
}
