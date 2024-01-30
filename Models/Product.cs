using System;
using System.Collections.Generic;

namespace FullCartApi.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int? BrandId { get; set; }

    public int? CategoryId { get; set; }

    public int QuantityAvailable { get; set; }

    public string? Image { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Category? Category { get; set; }
}
