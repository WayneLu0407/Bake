using Bake.Models.User;
using System;
using System.Collections.Generic;

namespace Bake.Models.Sales;

public partial class Cart
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public byte Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual CartStatus StatusNavigation { get; set; } = null!;

    public virtual UserProfile User { get; set; } = null!;
}
