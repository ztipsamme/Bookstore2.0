using System;
using Bookstore2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore2._0.Services;

public class InventoryService
{
    private readonly Bookstore2Context _db;

    public InventoryService(Bookstore2Context db)
    {
        _db = db;
    }

    public async Task<Inventory> AddInventory(Inventory inventory)
    {
        _db.Inventories.Add(inventory);
        await _db.SaveChangesAsync();
        return inventory;
    }

    public async Task<Inventory?> GetBookInInventory(int storeId, long isbn13)
    {
        return await _db.Inventories.FindAsync(storeId, isbn13);
    }

    public async Task<List<Inventory>> GetInventoryByStoreId(int storeId)
    {
        return await _db.Inventories
            .Where(i => i.StoreId == storeId)
            .Include(i => i.Isbn13Navigation)
                .ThenInclude(b => b.Author)
            .ToListAsync();
    }

    public async Task<Inventory> UpdateInventory(Inventory inventory)
    {
        _db.Inventories.Update(inventory);
        await _db.SaveChangesAsync();
        return inventory;
    }

    public async Task<bool> DeleteInventoryRow(int storeId, long isbn13)
    {
        var inventory = await GetBookInInventory(storeId, isbn13);
        if (inventory == null) return false;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAllInventoriesByIsbn13(long isbn13)
    {
        await _db.Inventories
             .Where(i => i.Isbn13 == isbn13)
             .ExecuteDeleteAsync();

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAllInventoriesByAuthorId(int authorId)
    {
        await _db.Inventories
       .Where(i => i.Isbn13Navigation.AuthorId == authorId)
       .ExecuteDeleteAsync();

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAllInventoriesByPublisherId(int publisherId)
    {
        await _db.Inventories.Where(i => i.Isbn13Navigation.PublisherId == publisherId).ExecuteDeleteAsync();
        await _db.SaveChangesAsync();
        return true;
    }
}
