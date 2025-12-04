using System;
using System.Collections.Generic;

namespace Bookstore2._0.Models;

public partial class Book
{
    public long Isbn13 { get; set; }

    public string Title { get; set; } = null!;

    public string Language { get; set; } = null!;

    public decimal PriceInSek { get; set; }

    public DateOnly PublishingDate { get; set; }

    public int TotalPages { get; set; }

    public int AuthorId { get; set; }

    public int PublisherId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Publisher Publisher { get; set; } = null!;
}
