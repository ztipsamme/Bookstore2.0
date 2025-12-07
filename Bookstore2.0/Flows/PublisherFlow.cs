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

        var publishers = await _dbService.GetAllPublishers();

        foreach (var publisher in publishers)
        {
            Console.WriteLine($"{publisher.Name}");
        }
    }

    public async Task<Publisher?> AddNewPublisherToDbFlow()
    {
        Console.WriteLine("=== Add new publisher to the database ===\n");


        string? name = ConsoleHelper.AskUntilValid("Name",
          "Invalid name.");

        if (name == null || ConsoleHelper.IsActionCanceled(name)) return null;

        if (await _dbService.PublisherExists(name))
        {
            Console.WriteLine("Publisher already exists.");
            return null;
        }

        var publisher = new Publisher();
        publisher.Name = name;

        try
        {
            await _dbService.AddPublisher(publisher);
            Console.WriteLine("New publisher was successfully added");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }

        return publisher;
    }

    public async Task<(Publisher? publisher, bool isCanceled)> SelectPublisherFlow(int? currentPublisherId = null)
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
            Publisher? newPublisher = await AddNewPublisherToDbFlow();
            return (newPublisher, false);
        }

        return (publishers[idx - 2], false);
    }

}
