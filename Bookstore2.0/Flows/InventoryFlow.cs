using System.Threading.Tasks;
using Bookstore2._0.Flows;
using Bookstore2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore2._0;

public class InventoryFlow : FlowBase
{
    public InventoryFlow(DbService dbService) : base(dbService)
    {

    }

    public async Task ShowInventory(Store store)
    {
        Console.Clear();

        var items = await _dbService.GetStoreInventory(store.StoreId);

        Console.WriteLine($"\nInventory for {store.Name}:\n");

        if (items.Count == 0)
        {
            Console.WriteLine("Inventory is empty\n");

            Console.WriteLine("Invalid choice");
            ConsoleHelper.PressAnyKeyToContinue();
            return;
        }

        foreach (var item in items)
        {
            var book = item.Isbn13Navigation;
            var author = book.Author;

            Console.WriteLine(
                $"{book.Title} ({book.Isbn13}) – " +
                $"{author.FirstName} {author.LastName} – " +
                $"Qty: {item.Quantity} – Price: {book.PriceInSek} kr"
            );
        }
    }

    public string InventoryMenu()
    {
        Console.WriteLine($"\n--- Inventory Menu ---\n");
        Console.WriteLine("1. Add book to store");
        Console.WriteLine("2. Remove book from store");
        Console.WriteLine("[back]. Back");

        string? choice = ConsoleHelper.Choice();
        if (choice == null)
        {
            return "";
        }
        return choice.ToLower();
    }

    public async Task AddBookToStore(int storeId)
    {
        Console.WriteLine("\n--- Add book to store ---\n");
        Console.WriteLine("Add a book that already exists in books\n");

        long isbn13 = await ConsoleHelper.AskUntilValidUniqueIsbn13(_dbService, "ISBN13", "Invalid ISBN13. ISBN13 must be unique and contain 13 integers.");

        if (ConsoleHelper.IsActionCanceled(isbn13)) return;

        int quantity = ConsoleHelper.AskUntilValid("Quantity",
        "Invalid quantity. Quantity must be greater than 0",
        input => int.TryParse(input, out int value) && value > 0,
        input => int.Parse(input));


        if (ConsoleHelper.IsActionCanceled(quantity)) return;

        var inventory = await _dbService.GetBookInInventory(storeId, isbn13);

        var books = await _dbService.GetBook(isbn13);
        var store = await _dbService.GetStore(storeId);

        try
        {
            if (inventory == null)
            {
                inventory = new Inventory
                {
                    StoreId = storeId,
                    Isbn13 = isbn13,
                    Quantity = quantity,
                };

                await _dbService.AddInventory(inventory);
            }
            else
            {
                inventory.Quantity += quantity;
            }
            Console.WriteLine("Added book(s) successfully.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task RemoveBookFromStore(int storeId)
    {
        Console.WriteLine("\n--- Remove book from store ---\n");

        long isbn13 = await ConsoleHelper.AskUntilValidUniqueIsbn13(_dbService, "ISBN13", "Invalid ISBN13. ISBN13 must an existing ISBN13.");

        if (ConsoleHelper.IsActionCanceled(isbn13)) return;

        var inventory = await _dbService.GetBookInInventory(storeId, isbn13);

        if (inventory == null)
        {
            Console.WriteLine("\nBook does not exist in this store's inventory.");
            ConsoleHelper.PressAnyKeyToContinue();
            return;
        }

        await HandleRemovalOrDecrease(inventory);
    }

    private async Task HandleRemovalOrDecrease(Inventory inventory)
    {
        Console.WriteLine($"\nCurrent quantity: {inventory.Quantity}");

        if (inventory.Quantity == 0)
        {
            await AskToDeleteInventoryRow(inventory);
            return;
        }

        Console.Write("Quantity to remove: ");

        if (!int.TryParse(Console.ReadLine(), out int amount) || amount < 1)
        {
            Console.WriteLine("Invalid quantity.");
            ConsoleHelper.PressAnyKeyToContinue();
            return;
        }

        if (amount > inventory.Quantity)
        {
            Console.WriteLine("\nCannot decrease below zero. Try again.");
            ConsoleHelper.PressAnyKeyToContinue();
            return;
        }

        inventory.Quantity -= amount;

        if (inventory.Quantity == 0)
        {
            await AskToDeleteInventoryRow(inventory);
        }
        else
        {
            Console.WriteLine("\nDecreased quantity successfully.");
            ConsoleHelper.PressAnyKeyToContinue();
        }
    }
    private async Task AskToDeleteInventoryRow(Inventory inventory)
    {
        Console.WriteLine("\nQuantity is now 0.");
        Console.WriteLine("Would you like to remove this book entirely from the inventory list?");
        Console.WriteLine("Type \"yes\" to confirm or press any key to keep it with quantity 0.");

        string? choice = ConsoleHelper.Choice();

        if (choice?.ToLower() == "yes")
        {
            await _dbService.DeleteRowInInventory(inventory.StoreId, inventory.Isbn13);
            Console.WriteLine("\nBook removed from inventory entirely.");
        }
        else
        {
            Console.WriteLine("\nInventory entry kept with quantity 0.");
        }

        ConsoleHelper.PressAnyKeyToContinue();
    }
}
