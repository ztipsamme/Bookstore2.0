using System;
using Bookstore2._0.Models;

namespace Bookstore2._0.Flows;

public class StoreFlow : FlowBase
{
    private readonly InventoryFlow _inventoryFlow;
    public StoreFlow(DbService dbService) : base(dbService)
    {
        _inventoryFlow = new InventoryFlow(dbService);
    }

    public async Task Run()
    {
        while (true)
        {
            var store = await SelectStore();

            var selectedStore = store.selectedStore;
            if (store.isBack) return;
            if (selectedStore == null) continue;

            while (true)
            {
                await _inventoryFlow.ShowInventory(selectedStore);

                var choice = _inventoryFlow.InventoryMenu();
                if (choice == "back") break;

                if (choice == "1")
                    await _inventoryFlow.AddBookToStore(selectedStore.StoreId);
                else if (choice == "2")
                    await _inventoryFlow.RemoveBookFromStore(selectedStore.StoreId);
                else
                {
                    Console.WriteLine("Invalid choice");
                    ConsoleHelper.PressAnyKeyToContinue();
                }
            }
        }
    }

    public async Task<(Store? selectedStore, bool isBack)> SelectStore()
    {
        Console.Clear();
        Console.WriteLine("=== Stores ===\n");
        Console.WriteLine("Select a store to view it's inventory\n");

        var stores = await _dbService.Stores.GetAllStores();

        for (int i = 0; i < stores.Count; i++)
            Console.WriteLine($"{i + 1}. {stores[i].Name}");
        Console.WriteLine("[back]. Back");

        string? choice = ConsoleHelper.Choice();
        if (choice?.ToLower() == "back")
        {
            return (null, true);
        }
        if (!ConsoleHelper.IsValidChoice(choice, stores))
        {
            Console.WriteLine("Invalid choice");
            ConsoleHelper.PressAnyKeyToContinue();
            return (null, false);
        }

        int index = int.Parse(choice!) - 1;
        return (stores[index], false);
    }
}