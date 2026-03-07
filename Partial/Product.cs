using Bake.Models.User;
using System;
using System.Collections.Generic;

namespace Bake.Models.Sales;

public partial class Product
{
    public virtual ProductDetail ProductDetail { get; set; }
}
