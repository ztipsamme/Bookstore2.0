using Bookstore2._0.Models;

namespace Bookstore2._0.Ui;


public class MainMenu
{
    private readonly Bookstore2Context _db;
    private readonly InventoryManager _inventoryManager;
    private readonly BookManager _booksManager;

    public MainMenu(Bookstore2Context db)
    {
        _db = db;
        _inventoryManager = new InventoryManager(_db);
        _booksManager = new BookManager(_db);
    }

    public void Show()
    {
        List<Menu> menu = new()
        {
            new ("List inventory balance", ListInventoryBalance),
            new ("Handle books", HandleBooks),
        };

        while (true)
        {
            Console.Clear();
            for (int i = 0; i < menu.Count; i++)
                Console.WriteLine($"{i + 1}. {menu[i].Name}");

            string? choice = ConsoleHelper.Choice();
            if (!ConsoleHelper.ValidateChoice(choice, menu))
            {
                int value = int.Parse(choice!);
                menu[value - 1].Method();
            }
            else
            {
                Console.WriteLine("Invalid choice");
                return;
            }
        }
    }

    private void ListInventoryBalance()
    {
        _inventoryManager.ManageInventory();
    }

    private void HandleBooks()
    {
        _booksManager.ManageLibrary();
    }
}
