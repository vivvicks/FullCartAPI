using System;
using System.Collections.Generic;

namespace FullCartApi.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int? CustomerId { get; set; }

    public int? ItemId { get; set; }

    public int Quantity { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Item? Item { get; set; }
}
