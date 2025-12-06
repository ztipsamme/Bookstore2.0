using System.Threading.Tasks;
using Bookstore2._0.Models;

namespace Bookstore2._0.Ui;


public class MainMenu
{
    private readonly Bookstore2Context _db;
    private readonly DbService _dbService;

    private readonly InventoryManager _inventoryManager;
    private readonly BookManager _booksManager;

    public MainMenu(Bookstore2Context db, DbService dbService)
    {
        _db = db;
        _dbService = dbService;

        _inventoryManager = new InventoryManager(db, dbService);
        _booksManager = new BookManager(db, dbService);
    }

    public async Task Show()
    {
        List<Menu> menu = new()
        {
            new ("List inventory balance", async () => await ListInventoryBalance()),
            new ("Manage books", HandleBooks),
        };

        while (true)
        {
            Console.Clear();
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

    private async Task ListInventoryBalance()
    {
        await _inventoryManager.ManageInventory();
    }

    private async Task HandleBooks()
    {
        await _booksManager.ManageLibrary();
    }
}
