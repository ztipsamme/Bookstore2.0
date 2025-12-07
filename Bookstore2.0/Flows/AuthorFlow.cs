using System;
using Bookstore2._0.Flows;
using Bookstore2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore2._0.Services;

public class AuthorFlow : FlowBase
{
    public AuthorFlow(DbService dbService) : base(dbService)
    {
    }

    public async Task ListAllAuthorsFlow()
    {
        Console.Clear();
        Console.WriteLine("=== List all authors ===\n");
        Console.WriteLine("All authors in the database\n");

        var authors = await _dbService.GetAllAuthors();

        foreach (var author in authors)
        {
            Console.WriteLine($"{author.LastName}, {author.FirstName} – Born {author.Birthday}");
        }
    }

    public async Task<Author?> AddNewAuthorToDbFlow()
    {
        Console.WriteLine("=== Add new author to the database ===\n");


        string? firstName = ConsoleHelper.AskUntilValid("First name",
          "Invalid first name.");

        if (firstName == null || ConsoleHelper.IsActionCanceled(firstName)) return null;

        string? lastName = ConsoleHelper.AskUntilValid("Last name",
        "Invalid Last name.");

        if (lastName == null || ConsoleHelper.IsActionCanceled(lastName)) return null;

        if (await _dbService.AuthorExists(firstName, lastName))
        {
            Console.WriteLine("Author already exists.");
            return null;
        }

        DateOnly birthday = ConsoleHelper.AskUntilValid("Birthday (yyyy/mm/dd)",
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
            await _dbService.AddAuthor(author);
            Console.WriteLine("New author was successfully added");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }

        return author;
    }

    public async Task<(Author? author, bool isCanceled)> SelectAuthorFlow(int? currentAuthorId = null)
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
            Author? newAuthor = await AddNewAuthorToDbFlow();
            return (newAuthor, false);
        }

        return (authors[idx - 2], false);
    }
}

