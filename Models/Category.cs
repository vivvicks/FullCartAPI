using System;
using System.Collections.Generic;

namespace FullCartApi.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Image { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
