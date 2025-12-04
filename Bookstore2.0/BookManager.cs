using System;
using Bookstore2._0.Models;
using Microsoft.EntityFrameworkCore;

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
            Console.Clear();
            Console.WriteLine("=== Manage books ===");

            List<Menu> menu = new()
            {
                new ("List all books", ListAllBooks),
                new ("Add new book", ()=> {}),
                new ("Edit book", ()=> {}),
                new ("Delete book", ()=> {}),
            };

            for (int i = 0; i < menu.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {menu[i].Name}");
            }
            Console.WriteLine("[back]. Back");

            string? choice = ConsoleHelper.Choice();
            if (choice?.ToLower() == "back") return;

            if (ConsoleHelper.IsValidChoice(choice, menu))
            {
                int value = int.Parse(choice!);
                menu[value - 1].Method();
            }
            else
            {
                Console.WriteLine("Invalid choice");
            }
            ConsoleHelper.PressAnyKeyToContinue();
        }
    }

    private void ListAllBooks()
    {
        Console.Clear();
        Console.WriteLine("=== List all books ===");

        var books = _db.Books.Include(b => b.Author).ToList();

        foreach (var book in books)
        {
            var author = book.Author;
            Console.WriteLine($"{author.LastName}, {author.FirstName} ({book.PublishingDate.Year}). {book.Title} ({book.Isbn13}), {book.TotalPages} s. {book.Publisher}");
        }

    }

}