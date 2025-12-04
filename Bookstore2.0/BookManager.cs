using System;
using Bookstore2._0.Models;

namespace Bookstore2._0;

public class BookManager
{
    private readonly Bookstore2Context _db;

    public BookManager(Bookstore2Context db)
    {
        _db = db;
    }

    public void ManageLibrary()
    {
        while (true)
        {
            List<Menu> menu = new()
            {
                new ("List all books", ()=> {}),
                new ("Add new book", ()=> {}),
                new ("Edit book", ()=> {}),
                new ("Delete book", ()=> {}),
            };
            Console.WriteLine("[back]. Back");

            string? choice = ConsoleHelper.Choice();
            if (choice?.ToLower() == "back") return;
            if (ConsoleHelper.ValidateChoice(choice, menu)) return;
        }
    }
}
