using System;
using System.Collections.Generic;

namespace Bake.Models;

public partial class ProductCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? CategoryDescription { get; set; }

    public int DisplayOrder { get; set; }
}
