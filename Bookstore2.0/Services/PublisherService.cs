using System;
using Bookstore2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore2._0.Services;

public class PublisherService
{
    private readonly Bookstore2Context _db;

    public PublisherService(Bookstore2Context db)
    {
        _db = db;
    }

    public async Task<Publisher> AddPublisher(Publisher publisher)
    {
        _db.Publishers.Add(publisher);
        await _db.SaveChangesAsync();
        return publisher;
    }

    public async Task<Publisher?> GetPublisher(int id)
    {
        return await _db.Publishers.FindAsync(id);
    }

    public async Task<List<Publisher>> GetAllPublishers()
    {
        return await _db.Publishers.AsNoTracking().ToListAsync();
    }

    public async Task<bool> PublisherExists(string name)
    {
        return await _db.Publishers.AnyAsync(p => p.Name == name);
    }

    public async Task<bool> PublisherExists(int publisherId)
    {
        return await _db.Publishers.AnyAsync(p => p.PublisherId == publisherId);
    }

    public async Task<Publisher> UpdatePublisher(Publisher publisher)
    {
        _db.Publishers.Update(publisher);
        await _db.SaveChangesAsync();
        return publisher;
    }

    public async Task<bool> DeletePublisher(int id)
    {
        var publisher = await GetPublisher(id);
        if (publisher == null) return false;

        _db.Publishers.Remove(publisher);
        await _db.SaveChangesAsync();
        return true;
    }
}
