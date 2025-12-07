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

        var authors = await _dbService.Authors.GetAllAuthors();

        foreach (var author in authors)
        {
            Console.WriteLine($"{author.LastName}, {author.FirstName} ({author.AuthorId}) – Born {author.Birthday}");
        }
    }

    private async Task<Author?> AskAuthorDetails(Author author)
    {
        string? firstName = ConsoleHelper.AskUntilValid("First name",
        "Invalid first name.", author.FirstName);

        if (firstName == null || ConsoleHelper.IsActionCanceled(firstName)) return null;

        string? lastName = ConsoleHelper.AskUntilValid("Last name",
        "Invalid Last name.", author.LastName);

        if (lastName == null || ConsoleHelper.IsActionCanceled(lastName)) return null;

        if (await _dbService.Authors.AuthorExists(firstName, lastName))
        {
            Console.WriteLine("Author already exists.");
            return null;
        }

        DateOnly birthday = ConsoleHelper.AskUntilValid("Birthday (yyyy/mm/dd)",
        "Invalid birthday",
        input => DateOnly.TryParse(input, out var value) && value <= DateOnly.FromDateTime(DateTime.Today),
        input => DateOnly.Parse(input), author.Birthday.ToString() ?? "");

        if (ConsoleHelper.IsActionCanceled(birthday)) return null;

        author.FirstName = firstName;
        author.LastName = lastName;
        author.Birthday = birthday;

        return author;
    }

    public async Task<Author?> AddNewAuthorToDbFlow()
    {
        Console.WriteLine("=== Add new author to the database ===\n");

        Author? author = new Author();

        author = await AskAuthorDetails(author);
        if (author == null) return null;

        try
        {
            await _dbService.Authors.AddAuthor(author);
            Console.WriteLine("New author was successfully added");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }

        return author;
    }

    public async Task UpdateAuthor()
    {
        Console.WriteLine("=== Edit author ===\n");

        int selectAuthorId = await ConsoleHelper.AskUntilValid(
            "Select author to edit by author id",
            "Invalid author id",
            async input => await _dbService.Authors.AuthorExists(int.Parse(input)),
            input => int.Parse(input));

        var author = await _dbService.Authors.GetAuthor(selectAuthorId);

        if (author == null)
        {
            Console.WriteLine("Couldn't fetch author;");
            return;
        }

        if (ConsoleHelper.IsActionCanceled(selectAuthorId)) return;

        author = await AskAuthorDetails(author);
        if (author == null) return;

        try
        {
            await _dbService.Authors.UpdateAuthor(author);
            Console.WriteLine("Author was successfully added");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }
    }
    public async Task DeleteAuthorFlow()
    {
        Console.WriteLine("\n--- Delete author from database ---\n");

        int selectAuthorId = await ConsoleHelper.AskUntilValid(
            "Select author to delete by author id",
            "Invalid author id",
            async input => await _dbService.Authors.AuthorExists(int.Parse(input)),
            input => int.Parse(input));

        if (ConsoleHelper.IsActionCanceled(selectAuthorId)) return;

        Console.WriteLine("!Warning: To delete an author you must delete all instance where the author is referenced. This includes:" +
            "- Deletion of all books by the author in the inventory" +
            "- Deletion of all books by the author in the database");

        bool deleteAuthor = ConsoleHelper.AskUntilValid("Are you sure that you want to delete the author? (y/n): ",
        "Invalid input",
        input => input.ToLower() == "y" || input.ToLower() == "n",
        input => input.ToLower() == "y" ? true : false
        );

        try
        {
            await _dbService.Inventory.DeleteAllInventoriesByAuthorId(selectAuthorId);
            await _dbService.Books.DeleteBooksByAuthorId(selectAuthorId);
            await _dbService.Authors.DeleteAuthor(selectAuthorId);
            Console.WriteLine("Author was successfully deleted");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Delete failed because the author is referenced somewhere.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine("Internal EF error: " + ex.Message);
            Console.WriteLine("Tip: You may be deleting an entity while another instance with the same key is tracked.");
        }
    }

    public async Task<(Author? author, bool isCanceled)> SelectAuthorFlow(int? currentAuthorId = null)
    {
        Console.WriteLine("\n--- Select or create a new author ---\n");

        List<Author> authors = await _dbService.Authors.GetAllAuthors();
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

