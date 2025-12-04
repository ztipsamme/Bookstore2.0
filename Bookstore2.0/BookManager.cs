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
                new ("Add new book to the database", AddNewBook),
                new ("Edit book", ()=> {}),
                new ("Delete book from the database", ()=> {}),
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
        Console.WriteLine("=== List all books ===\n");
        Console.WriteLine("All books in the database\n");

        var books = _db.Books.Include(b => b.Author).Include(b => b.Publisher).ToList();

        foreach (var book in books)
        {
            var author = book.Author;
            var publisher = book.Publisher;
            Console.WriteLine($"{author.LastName}, {author.FirstName} ({book.PublishingDate.Year}). {book.Title} ({book.Isbn13}), {book.TotalPages} s. {publisher.Name}");
        }

    }

    private void AddNewBook()
    {
        Console.Clear();
        Console.WriteLine("=== Add new book to the database ===\n");

        var author = SelectAuthor();
        if (ConsoleHelper.IsActionCanceled(author.author)) return;

        var publisher = SelectPublisher();
        if (ConsoleHelper.IsActionCanceled(publisher.publisher)) return;

        CreateBook(author.author.AuthorId, publisher.publisher.PublisherId);
    }

    private (Author? author, bool isCanceled) SelectAuthor()
    {
        Console.WriteLine("\n--- Select or create a new author ---\n");

        List<Author> authors = _db.Authors.ToList();
        int CreateNewAuthorIdx = 1;

        Console.WriteLine($"{CreateNewAuthorIdx}. Create new author");
        for (int i = 0; i < authors.Count; i++)
        {
            var author = authors[i];
            Console.WriteLine($"{i + 2}. {author.FirstName} {author.LastName}");
        }

        int idx = ConsoleHelper.AskUntilValid("\nChoice",
        "Invalid input. Please select from the list above.",
        input => int.TryParse(input, out int value) && value > 0 && value <= authors.Count + 1,
        input => int.Parse(input));

        if (ConsoleHelper.IsActionCanceled(idx)) return (null, true);

        if (idx == CreateNewAuthorIdx)
        {
            Author? newAuthor = CreateNewAuthor();
            return (newAuthor, false);
        }

        return (authors[idx - 1], false);
    }

    private Author? CreateNewAuthor()
    {
        Console.WriteLine("\n--- Add new author ---n");

        string? firstName = ConsoleHelper.AskUntilValid<string>("First name",
          "Invalid first name.");

        if (ConsoleHelper.IsActionCanceled(firstName)) return null;

        string? lastName = ConsoleHelper.AskUntilValid<string>("Last name",
        "Invalid Last name.");

        if (ConsoleHelper.IsActionCanceled(lastName)) return null;

        if (_db.Authors.FirstOrDefault(a => a.FirstName == firstName && a.LastName == lastName) == null)
        {
            Console.WriteLine("Author already exists.");
            return null;
        }

        DateOnly? birthday = ConsoleHelper.AskUntilValid("Birthday",
        "Invalid birthday",
        input => DateOnly.TryParse(input, out var value) && value <= DateOnly.FromDateTime(DateTime.Today),
        input => DateOnly.Parse(input));

        if (ConsoleHelper.IsActionCanceled(birthday)) return null;

        var author = new Author();
        author.FirstName = firstName;
        author.LastName = lastName;
        author.Birthday = birthday;

        try
        {
            _db.Authors.Add(author);
            _db.SaveChanges();
            Console.WriteLine("New author was successfully added");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }

        return author;
    }

    private Publisher? CreateNewPublisher()
    {
        Console.WriteLine("\n--- Add new publisher ---n");

        string? name = ConsoleHelper.AskUntilValid<string>("Name",
          "Invalid name.");

        if (ConsoleHelper.IsActionCanceled(name)) return null;

        if (_db.Publishers.FirstOrDefault(p => p.Name == name) != null)
        {
            Console.WriteLine("Publisher already exists.");
            return null;
        }

        var publisher = new Publisher();
        publisher.Name = name;

        try
        {
            _db.Publishers.Add(publisher);
            _db.SaveChanges();
            Console.WriteLine("New publisher was successfully added");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }

        return publisher;
    }

    private (Publisher? publisher, bool isCanceled) SelectPublisher()
    {
        Console.WriteLine("\n--- Select or create a new publisher ---\n");

        List<Publisher> publishers = _db.Publishers.ToList();
        int CreateNewAuthorIdx = 1;

        Console.WriteLine($"{CreateNewAuthorIdx}. Create new publisher");
        for (int i = 0; i < publishers.Count; i++)
        {
            var publisher = publishers[i];
            Console.WriteLine($"{i + 2}. {publisher.Name}");
        }

        int idx = ConsoleHelper.AskUntilValid("\nChoice",
        "Invalid input. Please select from the list above.",
        input => int.TryParse(input, out int value) && value > 0 && value <= publishers.Count + 1,
        input => int.Parse(input));

        if (ConsoleHelper.IsActionCanceled(idx)) return (null, true);

        if (idx == CreateNewAuthorIdx)
        {
            Publisher? newPublisher = CreateNewPublisher();
            return (newPublisher, false);
        }

        return (publishers[idx - 1], false);
    }

    private Book? CreateBook(int authorId, int publisherId)
    {
        Console.WriteLine("\n--- New book info ---\n");

        string? title = ConsoleHelper.AskUntilValid<string>("Title",
        "Invalid Title");

        if (ConsoleHelper.IsActionCanceled(title)) return null;

        long isbn13 = ConsoleHelper.AskUntilValid("ISBN13",
        "Invalid ISBN13. ISBN13 must be unique and contain 13 integers.",
        input => input.Length == 13 && long.TryParse(input, out long value) && !_db.Books.Any(b => b.Isbn13 == value),
        input => long.Parse(input));

        if (ConsoleHelper.IsActionCanceled(isbn13)) return null;

        string? language = ConsoleHelper.AskUntilValid<string>("Language",
               "Invalid language");

        if (ConsoleHelper.IsActionCanceled(language)) return null;

        decimal priceInSek = ConsoleHelper.AskUntilValid("Price",
        "Invalid price, must be a positive number.",
        input => decimal.TryParse(input, out decimal value) && value > 0,
        input => decimal.Parse(input));

        if (ConsoleHelper.IsActionCanceled(priceInSek)) return null;

        DateOnly publishingDate = ConsoleHelper.AskUntilValid("Published",
        "Invalid publishing date",
        input => DateOnly.TryParse(input, out var value) && value <= DateOnly.FromDateTime(DateTime.Today),
        input => DateOnly.Parse(input));

        if (ConsoleHelper.IsActionCanceled(publishingDate)) return null;


        int totalPages = ConsoleHelper.AskUntilValid("Pages",
        "Invalid amount of pages. Must be a positive number.",
        input => int.TryParse(input, out int value) && value > 0,
        input => int.Parse(input));

        if (ConsoleHelper.IsActionCanceled(totalPages)) return null;

        var book = new Book();
        book.Title = title;
        book.Isbn13 = isbn13;
        book.Language = language;
        book.PriceInSek = priceInSek;
        book.PublishingDate = publishingDate;
        book.TotalPages = totalPages;
        book.AuthorId = authorId;
        book.PublisherId = publisherId;

        try
        {
            _db.Books.Add(book);
            _db.SaveChanges();
            Console.WriteLine("New book was successfully added");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }

        return book;
    }
}