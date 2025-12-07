using System;
using Bookstore2._0.Models;
using Bookstore2._0.Services;
using Microsoft.EntityFrameworkCore;

namespace Bookstore2._0;

public class DbService
{
    public BookService Books { get; }
    public AuthorService Authors { get; }
    public PublisherService Publishers { get; }
    public StoreService Stores { get; }
    public InventoryService Inventory { get; }

    public DbService(Bookstore2Context db)
    {
        Books = new BookService(db);
        Authors = new AuthorService(db);
        Publishers = new PublisherService(db);
        Stores = new StoreService(db);
        Inventory = new InventoryService(db);
    }
}
