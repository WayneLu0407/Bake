using Bake.Models.User;
using System;
using System.Collections.Generic;

namespace Bake.Models.Sales;

public partial class ProductReview
{
    public int ReviewId { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public int OrderId { get; set; }

    public byte UserRating { get; set; }

    public string Comment { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual UserProfile User { get; set; } = null!;
}
