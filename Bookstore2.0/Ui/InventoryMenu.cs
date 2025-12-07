using System;

namespace Bookstore2._0.Ui;

public class InventoryMenu
{
    public string Show()
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
}
