using System;
using System.Collections.Generic;

namespace Bookstore2._0.Models;

public partial class Inventory
{
    public int StoreId { get; set; }

    public long Isbn13 { get; set; }

    public int Quantity { get; set; }

    public virtual Book Isbn13Navigation { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;
}
