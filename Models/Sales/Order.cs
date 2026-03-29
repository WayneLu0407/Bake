using Bake.Models.Platform;
using Bake.Models.User;
using System;
using System.Collections.Generic;

namespace Bake.Models.Sales;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string ShippingAddress { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public byte PaymentMethodId { get; set; }

    public byte StatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }


    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    public virtual ICollection<PlatformEscrowLedger> PlatformEscrowLedgers { get; set; } = new List<PlatformEscrowLedger>();

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    public virtual OrderStatus Status { get; set; } = null!;

    public virtual UserProfile User { get; set; } = null!;
    public virtual PaymentMethod PaymentMethod { get; set; } = null!;
}
