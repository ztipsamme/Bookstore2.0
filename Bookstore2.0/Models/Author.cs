using System;
using System.Collections.Generic;

namespace Bookstore2._0.Models;

public partial class Author
{
    public int AuthorId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly? Birthday { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
