using Bake.Models.Platform;
using Bake.Models.User;
using System;
using System.Collections.Generic;

namespace Bake.Models.Sales;

public class PaymentMethod
{
    public byte PaymentMethodId { get; set; }  // PK，對應 0, 1, 2

    public string Name { get; set; } = null!;  // "信用卡", "轉帳", "貨到付款"

    // 導覽屬性：一種付款方式對應多筆訂單
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}