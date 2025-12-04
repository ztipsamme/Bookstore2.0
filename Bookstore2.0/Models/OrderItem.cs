using System;
using System.Collections.Generic;

namespace Bookstore2._0.Models;

public partial class OrderItem
{
    public int OrderId { get; set; }

    public long Isbn13 { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Book Isbn13Navigation { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
