using System;
using Bookstore2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore2._0.Services;

public class StoreService
{
    private readonly Bookstore2Context _db;

    public StoreService(Bookstore2Context db)
    {
        _db = db;
    }

    public async Task<Store> GetStore(int id)
    {
        return await _db.Stores.Include(s => s.Inventories).ThenInclude(i => i.Isbn13Navigation).FirstAsync(s => s.StoreId == id);
    }

    public async Task<List<Store>> GetAllStores()
    {
        return await _db.Stores.AsNoTracking().ToListAsync();
    }

    public async Task<Store> UpdateStore(Store store)
    {
        _db.Stores.Update(store);
        await _db.SaveChangesAsync();
        return store;
    }

    public async Task<bool> DeleteStore(int id)
    {
        var store = await GetStore(id);
        if (store == null) return false;

        _db.Stores.Remove(store);
        await _db.SaveChangesAsync();
        return true;
    }
}
