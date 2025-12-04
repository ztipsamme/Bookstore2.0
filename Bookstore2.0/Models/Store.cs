using System;
using System.Collections.Generic;

namespace Bookstore2._0.Models;

public partial class Store
{
    public int StoreId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
