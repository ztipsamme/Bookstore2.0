using Bookstore2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore2._0;

public class InventoryManager
{
    private readonly Bookstore2Context _db;

    public InventoryManager(Bookstore2Context db)
    {
        _db = db;
    }

    public void ManageInventory()
    {
        while (true)
        {
            var store = SelectStore();
            if (store == null)
            {
                Console.WriteLine("Invalid choice");
                ConsoleHelper.PressAnyKeyToContinue();
                continue;
            }

            while (true)
            {
                ShowInventory(store);

                var choice = InventoryMenu();
                if (choice == "back") break;

                if (choice == "1")
                    AddBookToStore(store.StoreId);
                else if (choice == "2")
                    RemoveBookFromStore(store.StoreId);
                else
                {
                    Console.WriteLine("Invalid choice");
                    ConsoleHelper.PressAnyKeyToContinue();
                }
            }
        }
    }

    private Store? SelectStore()
    {
        Console.Clear();
        Console.WriteLine("=== Stores ===\n");
        Console.WriteLine("Select a store to view its inventory\n");

        var stores = _db.Stores.ToList();

        for (int i = 0; i < stores.Count; i++)
            Console.WriteLine($"{i + 1}. {stores[i].Name}");
        Console.WriteLine("[back]. Back");

        string? choice = ConsoleHelper.Choice();
        if (choice?.ToLower() == "back") return null;
        if (!ConsoleHelper.IsValidChoice(choice, stores)) return null;

        int index = int.Parse(choice!) - 1;
        return stores[index];
    }

    private void ShowInventory(Store store)
    {
        Console.Clear();

        var items = _db.Inventories
            .Where(i => i.StoreId == store.StoreId)
            .Include(i => i.Isbn13Navigation)
                .ThenInclude(b => b.Author)
            .ToList();

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

    private string InventoryMenu()
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

    private void AddBookToStore(int storeId)
    {
        Console.WriteLine("\n--- Add book to store ---\n");
        Console.WriteLine("Add a book that already exists in books\n");

        long isbn13 = ConsoleHelper.AskUntilValid("ISBN13",
         "Invalid ISBN13, no book matches the value. ISBN13 must contain 13 integers.",
         input => input.Length == 13 && long.TryParse(input, out long value) && _db.Books.Where(b => b.Isbn13 == value) != null,
         input => long.Parse(input));

        if (ConsoleHelper.IsActionCanceled(isbn13)) return;

        int quantity = ConsoleHelper.AskUntilValid("Quantity",
        "Invalid quantity. Quantity must be greater than 0",
        input => int.TryParse(input, out int value) && value > 0,
        input => int.Parse(input));


        if (ConsoleHelper.IsActionCanceled(quantity)) return;

        var inventory = _db.Inventories
        .FirstOrDefault(ls => ls.StoreId == storeId && ls.Isbn13 == isbn13);

        var books = _db.Books.Where(b => b.Isbn13 == isbn13);
        var store = _db.Stores.Include(s => s.Inventories).ThenInclude(i => i.Isbn13Navigation).First(s => s.StoreId == storeId);

        if (inventory == null)
        {
            inventory = new Inventory
            {
                StoreId = storeId,
                Isbn13 = isbn13,
                Quantity = quantity,
            };

            _db.Inventories.Add(inventory);
        }
        else
        {
            inventory.Quantity += quantity;
        }

        try
        {
            _db.SaveChanges();
            Console.WriteLine("Added book(s) successfully.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Database could not be updated.");
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }
    }

    private void RemoveBookFromStore(int storeId)
    {
        Console.WriteLine("\n--- Remove book from store ---\n");

        long isbn13 = ConsoleHelper.AskUntilValid("ISBN13",
        "Invalid ISBN13, no book matches the value. ISBN13 must contain 13 integers.",
        input => input.Length == 13 && long.TryParse(input, out long value) && _db.Books.Where(b => b.Isbn13 == value) != null,
        input => long.Parse(input));

        if (ConsoleHelper.IsActionCanceled(isbn13)) return;

        var inventory = _db.Inventories
            .FirstOrDefault(i => i.StoreId == storeId && i.Isbn13 == isbn13);

        if (inventory == null)
        {
            Console.WriteLine("\nBook does not exist in this store's inventory.");
            ConsoleHelper.PressAnyKeyToContinue();
            return;
        }

        HandleRemovalOrDecrease(inventory);
    }

    private void HandleRemovalOrDecrease(Inventory inventory)
    {
        Console.WriteLine($"\nCurrent quantity: {inventory.Quantity}");

        if (inventory.Quantity == 0)
        {
            AskToDeleteInventoryRow(inventory);
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
            AskToDeleteInventoryRow(inventory);
        }
        else
        {
            Console.WriteLine("\nDecreased quantity successfully.");
            _db.SaveChanges();
            ConsoleHelper.PressAnyKeyToContinue();
        }
    }
    private void AskToDeleteInventoryRow(Inventory inventory)
    {
        Console.WriteLine("\nQuantity is now 0.");
        Console.WriteLine("Would you like to remove this book entirely from the inventory list?");
        Console.WriteLine("Type \"yes\" to confirm or press any key to keep it with quantity 0.");

        string? choice = ConsoleHelper.Choice();

        if (choice?.ToLower() == "yes")
        {
            _db.Inventories.Remove(inventory);
            Console.WriteLine("\nBook removed from inventory entirely.");
        }
        else
        {
            Console.WriteLine("\nInventory entry kept with quantity 0.");
        }

        _db.SaveChanges();
        ConsoleHelper.PressAnyKeyToContinue();
    }
}
