using System.Threading.Tasks;
using Bookstore2._0.Flows;
using Bookstore2._0.Models;

namespace Bookstore2._0.Ui;


public class BookStore
{
    private readonly DbService _dbService;
    private readonly StoreFlow _storeFlow;

    public BookStore(DbService dbService)
    {
        _dbService = dbService;
        _storeFlow = new StoreFlow(dbService);
    }

    public async Task Run()
    {
        List<MenuItem> menu = new()
        {
            new ("Manage stores", ManageStore),
            new ("Manage books", ManageBooks),
            new ("Manage authors", ManageAuthors),
            new ("Manage publishers", ManagePublishers),
            new ("Quit", QuitProgram),
        };

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Book Store ===\n");

            for (int i = 0; i < menu.Count; i++)
                Console.WriteLine($"{i + 1}. {menu[i].Name}");

            string? choice = ConsoleHelper.Choice();

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

    private async Task ManageStore()
    {
        await _storeFlow.Run();
    }

    private async Task ManageBooks()
    {
        var bookMenu = new BookMenu(_dbService);
        await bookMenu.Show();
    }

    private async Task ManageAuthors()
    {
        var authorMenu = new AuthorMenu(_dbService);
        await authorMenu.Show();
    }

    private async Task ManagePublishers()
    {
        var publisherMenu = new PublisherMenu(_dbService);
        await publisherMenu.Show();
    }

    private Task QuitProgram()
    {
        Console.WriteLine("\nGoodbye, until next time!");
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}
