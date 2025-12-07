using System;
using Bookstore2._0.Flows;
using Bookstore2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore2._0.Services;

public class PublisherFlow : FlowBase
{
    public PublisherFlow(DbService dbService) : base(dbService)
    {
    }

    public async Task ListAllPublishersFlow()
    {
        Console.Clear();
        Console.WriteLine("=== List all publishers ===\n");
        Console.WriteLine("All publishers in the database\n");

        var publishers = await _dbService.Publishers.GetAllPublishers();

        foreach (var publisher in publishers)
        {
            Console.WriteLine($"{publisher.Name} ({publisher.PublisherId})");
        }
    }

    private async Task<Publisher?> AskPublisherDetails(Publisher publisher)
    {
        string? name = ConsoleHelper.AskUntilValid("Name",
        "Invalid name.", publisher.Name);

        if (name == null || ConsoleHelper.IsActionCanceled(name)) return null;


        if (await _dbService.Publishers.PublisherExists(name))
        {
            Console.WriteLine("Publisher already exists.");
            return null;
        }

        publisher.Name = name;

        return publisher;
    }

    public async Task<Publisher?> AddNewPublisherToDbFlow()
    {
        Console.WriteLine("=== Add new publisher to the database ===\n");

        Publisher? publisher = new Publisher();
        publisher = await AskPublisherDetails(publisher);
        if (publisher == null) return null;

        try
        {
            await _dbService.Publishers.AddPublisher(publisher);
            Console.WriteLine("New publisher was successfully added");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }

        return publisher;
    }

    public async Task UpdatePublisher()
    {
        Console.WriteLine("=== Edit publisher ===\n");

        int selectPublisherId = await ConsoleHelper.AskUntilValid(
            "Select publisher to edit by publisher id",
            "Invalid publisher id",
            async input => await _dbService.Publishers.PublisherExists(int.Parse(input)),
            input => int.Parse(input));

        var publisher = await _dbService.Publishers.GetPublisher(selectPublisherId);

        if (publisher == null)
        {
            Console.WriteLine("Couldn't fetch publisher;");
            return;
        }

        if (ConsoleHelper.IsActionCanceled(selectPublisherId)) return;

        publisher = await AskPublisherDetails(publisher);
        if (publisher == null) return;

        try
        {
            await _dbService.Publishers.UpdatePublisher(publisher);
            Console.WriteLine("Publisher was successfully added");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task DeletePublisherFlow()
    {
        Console.WriteLine("\n--- Delete publisher from database ---\n");

        int selectPublisherId = await ConsoleHelper.AskUntilValid(
            "Select publisher to delete by publisher id",
            "Invalid publisher id",
            async input => await _dbService.Publishers.PublisherExists(int.Parse(input)),
            input => int.Parse(input));

        if (ConsoleHelper.IsActionCanceled(selectPublisherId)) return;

        Console.WriteLine("!Warning: To delete a publisher you must delete all instance where the publisher is referenced. This includes:" +
            "- Deletion of all books by the publisher in the inventory" +
            "- Deletion of all books by the publisher in the database");

        bool deletePublisher = ConsoleHelper.AskUntilValid("Are you sure that you want to delete the publisher? (y/n): ",
        "Invalid input",
        input => input.ToLower() == "y" || input.ToLower() == "n",
        input => input.ToLower() == "y" ? true : false
        );

        try
        {
            await _dbService.Inventory.DeleteAllInventoriesByPublisherId(selectPublisherId);
            await _dbService.Books.DeleteBooksByPublisherId(selectPublisherId);
            await _dbService.Publishers.DeletePublisher(selectPublisherId);
            Console.WriteLine("Publisher was successfully deleted");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Delete failed because the publisher is referenced somewhere.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine("Internal EF error: " + ex.Message);
            Console.WriteLine("Tip: You may be deleting an entity while another instance with the same key is tracked.");
        }
    }

    public async Task<(Publisher? publisher, bool isCanceled)> SelectPublisherFlow(int? currentPublisherId = null)
    {
        Console.WriteLine("\n--- Select or create a new publisher ---\n");

        List<Publisher> publishers = await _dbService.Publishers.GetAllPublishers();
        int CreateNewPublisherIdx = 1;

        Console.WriteLine($"{CreateNewPublisherIdx}. Create new publisher");
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

        if (idx == CreateNewPublisherIdx)
        {
            Publisher? newPublisher = await AddNewPublisherToDbFlow();
            return (newPublisher, false);
        }

        return (publishers[idx - 2], false);
    }

}
