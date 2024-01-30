using System;
using System.Collections.Generic;

namespace FullCartApi.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
