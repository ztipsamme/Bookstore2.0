using System;
using Bookstore2._0.Models;
using Bookstore2._0.Services;
using Microsoft.EntityFrameworkCore;

namespace Bookstore2._0.Flows;

public class BookFlow : FlowBase
{
    private readonly AuthorFlow _authorService;
    private readonly PublisherFlow _publisherService;

    public BookFlow(DbService dbService) : base(dbService)
    {
        _authorService = new AuthorFlow(dbService);
        _publisherService = new PublisherFlow(dbService);
    }

    public async Task ListAllBooksFlow()
    {
        Console.Clear();
        Console.WriteLine("=== List all books ===\n");
        Console.WriteLine("All books in the database\n");

        var books = await _dbService.Books.GetAllBooks();

        foreach (var book in books)
        {
            var author = book.Author;
            var publisher = book.Publisher;
            Console.WriteLine($"{author.LastName}, {author.FirstName} ({book.PublishingDate.Year}). {book.Title} ({book.Isbn13}), {book.TotalPages} s. {publisher.Name}");
        }
    }

    private Book? AskBookDetails(Book book)
    {
        string? title = ConsoleHelper.AskUntilValid("Title",
        "Invalid Title", defaultInput: book.Title);

        if (title == null || ConsoleHelper.IsActionCanceled(title)) return null;

        string? language = ConsoleHelper.AskUntilValid("Language",
               "Invalid language", defaultInput: book.Language);

        if (language == null || ConsoleHelper.IsActionCanceled(language)) return null;

        decimal priceInSek = ConsoleHelper.AskUntilValid("Price",
        "Invalid price, must be a positive number.",
        input => decimal.TryParse(input, out decimal value) && value > 0,
        input => decimal.Parse(input), book.PriceInSek.ToString());

        if (ConsoleHelper.IsActionCanceled(priceInSek)) return null;

        DateOnly publishingDate = ConsoleHelper.AskUntilValid("Published (yyyy/mm/dd)",
        "Invalid publishing date",
        input => DateOnly.TryParse(input, out var value) && value <= DateOnly.FromDateTime(DateTime.Today),
        input => DateOnly.Parse(input), book.PublishingDate.ToString());

        if (ConsoleHelper.IsActionCanceled(publishingDate)) return null;

        int totalPages = ConsoleHelper.AskUntilValid("Pages",
        "Invalid amount of pages. Must be a positive number.",
        input => int.TryParse(input, out int value) && value > 0,
        input => int.Parse(input), book.TotalPages.ToString());

        if (ConsoleHelper.IsActionCanceled(totalPages)) return null;

        book.Title = title;
        book.Language = language;
        book.PriceInSek = priceInSek;
        book.PublishingDate = publishingDate;
        book.TotalPages = totalPages;

        return book;
    }

    public async Task<Book?> AddNewBookToDbFlow()
    {
        Console.Clear();
        Console.WriteLine("=== Add new book to the database ===\n");

        var author = await _authorService.SelectAuthorFlow();
        if (author.author == null || ConsoleHelper.IsActionCanceled(author.author)) return null;

        var publisher = await _publisherService.SelectPublisherFlow();
        if (publisher.publisher == null || ConsoleHelper.IsActionCanceled(publisher.publisher)) return null;

        Console.WriteLine("\n--- New book info ---\n");

        long isbn13 = await ConsoleHelper.AskUntilValid(
            "ISBN13",
            "Invalid ISBN13. ISBN13 must be unique and contain 13 integers.",
            async input =>
                input.Length == 13 &&
                long.TryParse(input, out long value) &&
                !await _dbService.Books.BookExists(value),
            input => long.Parse(input)
        );

        if (ConsoleHelper.IsActionCanceled(isbn13)) return null;

        Book? book = new Book();

        book.AuthorId = author.author.AuthorId;
        book.PublisherId = publisher.publisher.PublisherId;
        book.Isbn13 = isbn13;

        book = AskBookDetails(book);

        if (book == null) return null;

        try
        {
            await _dbService.Books.AddBook(book);
            Console.WriteLine("New book was successfully added");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }

        return book;
    }

    public async Task UpdateBookFlow()
    {
        Console.WriteLine("\n--- Update book info ---\n");

        long selectedIsbn13 = await ConsoleHelper.AskUntilValidUniqueIsbn13(_dbService, "Select book to edit by ISBN13",
        "Invalid ISBN13. ISBN13 must be unique and contain 13 integers.");

        if (ConsoleHelper.IsActionCanceled(selectedIsbn13)) return;

        var book = await _dbService.Books.GetBook(selectedIsbn13);

        if (book == null)
        {
            Console.WriteLine("Couldn't fetch book;");
            return;
        }

        var author = await _authorService.SelectAuthorFlow(book.AuthorId);
        if (author.author == null || ConsoleHelper.IsActionCanceled(author.author)) return;

        var publisher = await _publisherService.SelectPublisherFlow(book.PublisherId);
        if (publisher.publisher == null || ConsoleHelper.IsActionCanceled(publisher.publisher)) return;

        long isbn13 = await ConsoleHelper.AskUntilValid("ISBN13",
        "Invalid ISBN13. ISBN13 must be unique and contain 13 integers.",
        async input =>
        {
            if (input.Length != 13) return false;
            if (!long.TryParse(input, out long value)) return false;

            if (value == book.Isbn13) return true;

            return !await _dbService.Books.BookExists(value);
        },
        input => long.Parse(input), defaultInput: book.Isbn13.ToString());

        if (ConsoleHelper.IsActionCanceled(isbn13)) return;

        book.AuthorId = author.author.AuthorId;
        book.PublisherId = publisher.publisher.PublisherId;
        book.Isbn13 = isbn13;

        book = AskBookDetails(book);
        if (book == null) return;

        try
        {
            await _dbService.Books.UpdateBook(book);
            Console.WriteLine("Book was successfully added");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task DeleteBookFlow()
    {
        Console.WriteLine("\n--- Delete book from database ---\n");

        long selectedIsbn13 = await ConsoleHelper.AskUntilValidUniqueIsbn13(_dbService, "Select book to delete by ISBN13",
        "Invalid ISBN13. ISBN13 must an existing ISBN13.");

        if (ConsoleHelper.IsActionCanceled(selectedIsbn13)) return;

        Console.WriteLine("!Warning: Once you've deleted a book you can't restore it.\nWhen deleting a book it will also delete any record of it in the stores inventories.");

        bool deleteBook = ConsoleHelper.AskUntilValid("Are you sure that you want to delete the book? (y/n): ",
        "Invalid ISBN13. ISBN13 must be unique and contain 13 integers.",
        input => input.ToLower() == "y" || input.ToLower() == "n",
        input => input.ToLower() == "y" ? true : false
       );

        if (ConsoleHelper.IsActionCanceled(!deleteBook))
        {
            Console.WriteLine("Canceled deletion of book.");
            return;
        }

        try
        {
            await _dbService.Inventory.DeleteAllInventoriesByIsbn13(selectedIsbn13);
            await _dbService.Books.DeleteBook(selectedIsbn13);
            Console.WriteLine("Book was successfully deleted");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Delete failed because the book is referenced somewhere.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine("Internal EF error: " + ex.Message);
            Console.WriteLine("Tip: You may be deleting an entity while another instance with the same key is tracked.");
        }

    }
}