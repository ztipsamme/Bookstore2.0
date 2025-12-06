using System.Threading.Tasks;
using Bookstore2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore2._0;

public class BookManager
{
    private readonly Bookstore2Context _db;
    private readonly DbService _dbService;

    public BookManager(Bookstore2Context db, DbService dbService)
    {
        _db = db;
        _dbService = dbService;
    }

    public async Task ManageLibrary()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Manage books ===");

            List<Menu> menu = new()
            {
                new ("List all books",  () => ListAllBooks()),
                new ("Add new book to the database", ()=> AddNewBook()),
                new ("Edit book",  () => UpdateBook()),
                // new ("Delete book from the database", ()=> {}),
            };

            for (int i = 0; i < menu.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {menu[i].Name}");
            }
            Console.WriteLine("[back]. Back");

            string? choice = ConsoleHelper.Choice();
            if (choice?.ToLower() == "back") return;

            if (!ConsoleHelper.IsValidChoice(choice, menu))
            {
                Console.WriteLine("Invalid choice");
                ConsoleHelper.PressAnyKeyToContinue();
                continue;
            }

            int value = int.Parse(choice!);
            await menu[value - 1].Method();

            ConsoleHelper.PressAnyKeyToContinue();
        }
    }

    private async Task ListAllBooks()
    {
        Console.Clear();
        Console.WriteLine("=== List all books ===\n");
        Console.WriteLine("All books in the database\n");

        var books = await _dbService.GetAllBooks();

        foreach (var book in books)
        {
            var author = book.Author;
            var publisher = book.Publisher;
            Console.WriteLine($"{author.LastName}, {author.FirstName} ({book.PublishingDate.Year}). {book.Title} ({book.Isbn13}), {book.TotalPages} s. {publisher.Name}");
        }
    }

    private async Task AddNewBook()
    {
        Console.Clear();
        Console.WriteLine("=== Add new book to the database ===\n");

        var author = await SelectAuthor();
        if (author.author == null || ConsoleHelper.IsActionCanceled(author.author)) return;

        var publisher = await SelectPublisher();
        if (publisher.publisher == null || ConsoleHelper.IsActionCanceled(publisher.publisher)) return;


        await CreateBook(author.author.AuthorId, publisher.publisher.PublisherId);
    }

    private async Task<(Author? author, bool isCanceled)> SelectAuthor(int? currentAuthorId = null)
    {
        Console.WriteLine("\n--- Select or create a new author ---\n");

        List<Author> authors = await _dbService.GetAllAuthors();
        int CreateNewAuthorIdx = 1;

        Console.WriteLine($"{CreateNewAuthorIdx}. Create new author");
        for (int i = 0; i < authors.Count; i++)
        {
            var author = authors[i];
            Console.WriteLine($"{i + 2}. {author.FirstName} {author.LastName}");
        }

        int currentAuthorIdx = authors.FindIndex(a => a.AuthorId == currentAuthorId) + 2;

        int idx = ConsoleHelper.AskUntilValid("\nChoice",
        "Invalid input. Please select from the list above.",
        input => int.TryParse(input, out int value) && value > 0 && value <= authors.Count + 1,
        input => int.Parse(input), currentAuthorIdx.ToString());

        if (ConsoleHelper.IsActionCanceled(idx)) return (null, true);

        if (idx == CreateNewAuthorIdx)
        {
            Author? newAuthor = await CreateNewAuthor();
            return (newAuthor, false);
        }

        return (authors[idx - 2], false);
    }

    private async Task<Author?> CreateNewAuthor()
    {
        Console.WriteLine("\n--- Add new author ---n");

        string? firstName = ConsoleHelper.AskUntilValid<string>("First name",
          "Invalid first name.");

        if (firstName == null || ConsoleHelper.IsActionCanceled(firstName)) return null;

        string? lastName = ConsoleHelper.AskUntilValid<string>("Last name",
        "Invalid Last name.");

        if (lastName == null || ConsoleHelper.IsActionCanceled(lastName)) return null;

        if (_db.Authors.Any(a => a.FirstName == firstName && a.LastName == lastName))
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
            await _dbService.CreateAuthor(author);
            Console.WriteLine("New author was successfully added");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }

        return author;
    }

    private async Task<Publisher?> CreateNewPublisher()
    {
        Console.WriteLine("\n--- Add new publisher ---n");

        string? name = ConsoleHelper.AskUntilValid<string>("Name",
          "Invalid name.");

        if (name == null || ConsoleHelper.IsActionCanceled(name)) return null;

        if (_db.Publishers.Any(p => p.Name == name))
        {
            Console.WriteLine("Publisher already exists.");
            return null;
        }

        var publisher = new Publisher();
        publisher.Name = name;

        try
        {
            await _dbService.CreatePublisher(publisher);
            Console.WriteLine("New publisher was successfully added");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }

        return publisher;
    }

    private async Task<(Publisher? publisher, bool isCanceled)> SelectPublisher(int? currentPublisherId = null)
    {
        Console.WriteLine("\n--- Select or create a new publisher ---\n");

        List<Publisher> publishers = await _dbService.GetAllPublishers();
        int CreateNewAuthorIdx = 1;

        Console.WriteLine($"{CreateNewAuthorIdx}. Create new publisher");
        for (int i = 0; i < publishers.Count; i++)
        {
            var publisher = publishers[i];
            Console.WriteLine($"{i + 2}. {publisher.Name}");
        }

        int currentPublisherIdx = publishers.FindIndex(p => p.PublisherId == currentPublisherId) + 2;

        int idx = ConsoleHelper.AskUntilValid("\nChoice",
        "Invalid input. Please select from the list above.",
        input => int.TryParse(input, out int value) && value > 0 && value <= publishers.Count + 1,
        input => int.Parse(input), currentPublisherIdx.ToString());

        if (ConsoleHelper.IsActionCanceled(idx)) return (null, true);

        if (idx == CreateNewAuthorIdx)
        {
            Publisher? newPublisher = await CreateNewPublisher();
            return (newPublisher, false);
        }

        return (publishers[idx - 2], false);
    }

    private async Task<Book?> CreateBook(int authorId, int publisherId)
    {
        Console.WriteLine("\n--- New book info ---\n");

        string? title = ConsoleHelper.AskUntilValid<string>("Title",
        "Invalid Title");

        if (title == null || ConsoleHelper.IsActionCanceled(title)) return null;

        long isbn13 = ConsoleHelper.AskUntilValid("ISBN13",
        "Invalid ISBN13. ISBN13 must be unique and contain 13 integers.",
        input => input.Length == 13 && long.TryParse(input, out long value) && !_db.Books.Any(b => b.Isbn13 == value),
        input => long.Parse(input));

        if (ConsoleHelper.IsActionCanceled(isbn13)) return null;

        string? language = ConsoleHelper.AskUntilValid<string>("Language",
               "Invalid language");

        if (language == null || ConsoleHelper.IsActionCanceled(language)) return null;

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
            await _dbService.CreateBook(book);
            Console.WriteLine("New book was successfully added");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }

        return book;
    }

    private async Task UpdateBook()
    {
        Console.WriteLine("\n--- Update book info ---\n");

        long selectedIsbn13 = ConsoleHelper.AskUntilValid("Select book to edit by ISBN13",
        "Invalid ISBN13. ISBN13 must be unique and contain 13 integers.",
        input => input.Length == 13 && long.TryParse(input, out long value) && _db.Books.Any(b => b.Isbn13 == value),
        input => long.Parse(input));

        if (ConsoleHelper.IsActionCanceled(selectedIsbn13)) return;

        var book = await _dbService.GetBook(selectedIsbn13);

        if (book == null)
        {
            Console.WriteLine("Couldn't fetch book;");
            return;
        }

        var author = await SelectAuthor(book.AuthorId);
        if (author.author == null || ConsoleHelper.IsActionCanceled(author.author)) return;

        var publisher = await SelectPublisher(book.PublisherId);
        if (publisher.publisher == null || ConsoleHelper.IsActionCanceled(publisher.publisher)) return;


        string? title = ConsoleHelper.AskUntilValid<string>("Title",
        "Invalid Title", defaultInput: book.Title);

        if (title == null || ConsoleHelper.IsActionCanceled(title)) return;

        long isbn13 = ConsoleHelper.AskUntilValid("ISBN13",
        "Invalid ISBN13. ISBN13 must be unique and contain 13 integers.",
        input =>
        {
            if (input.Length != 13) return false;
            if (!long.TryParse(input, out long value)) return false;

            if (value == book.Isbn13) return true;

            return !_db.Books.Any(b => b.Isbn13 == value);
        },
        input => long.Parse(input), defaultInput: book.Isbn13.ToString());

        if (ConsoleHelper.IsActionCanceled(isbn13)) return;

        string? language = ConsoleHelper.AskUntilValid<string>("Language",
               "Invalid language", defaultInput: book.Language);

        if (language == null || ConsoleHelper.IsActionCanceled(language)) return;

        decimal priceInSek = ConsoleHelper.AskUntilValid("Price",
        "Invalid price, must be a positive number.",
        input => decimal.TryParse(input, out decimal value) && value > 0,
        input => decimal.Parse(input), book.PriceInSek.ToString());

        if (ConsoleHelper.IsActionCanceled(priceInSek)) return;

        DateOnly publishingDate = ConsoleHelper.AskUntilValid("Published",
        "Invalid publishing date",
        input => DateOnly.TryParse(input, out var value) && value <= DateOnly.FromDateTime(DateTime.Today),
        input => DateOnly.Parse(input), book.PublishingDate.ToString());

        if (ConsoleHelper.IsActionCanceled(publishingDate)) return;

        int totalPages = ConsoleHelper.AskUntilValid("Pages",
        "Invalid amount of pages. Must be a positive number.",
        input => int.TryParse(input, out int value) && value > 0,
        input => int.Parse(input), book.TotalPages.ToString());

        if (ConsoleHelper.IsActionCanceled(totalPages)) return;

        book.Title = title;
        book.Isbn13 = isbn13;
        book.Language = language;
        book.PriceInSek = priceInSek;
        book.PublishingDate = publishingDate;
        book.TotalPages = totalPages;
        book.AuthorId = author.author.AuthorId;
        book.PublisherId = publisher.publisher.PublisherId;

        try
        {
            await _dbService.UpdateBook(book);
            Console.WriteLine("Book was successfully added");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }
    }
}