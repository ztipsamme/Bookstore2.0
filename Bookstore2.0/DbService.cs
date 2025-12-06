using System;
using Bookstore2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore2._0;

public class DbService
{
    private readonly Bookstore2Context _db;
    public DbService(Bookstore2Context db)
    {
        _db = db;
    }

    public async Task<Book> CreateBook(Book book)
    {
        _db.Books.Add(book);
        await _db.SaveChangesAsync();
        return book;
    }

    public async Task<Author> CreateAuthor(Author author)
    {
        _db.Authors.Add(author);
        await _db.SaveChangesAsync();
        return author;
    }
    public async Task<Publisher> CreatePublisher(Publisher publisher)
    {
        _db.Publishers.Add(publisher);
        await _db.SaveChangesAsync();
        return publisher;
    }

    public async Task<Book?> GetBook(long isbn13)
    {
        return await _db.Books.FindAsync(isbn13);
    }

    public async Task<Author?> GetAuthor(int id)
    {
        return await _db.Authors.FindAsync(id);
    }

    public async Task<Publisher?> GetPublisher(int id)
    {
        return await _db.Publishers.FindAsync(id);
    }

    public async Task<Store> GetStore(int id)
    {
        return await _db.Stores.Include(s => s.Inventories).ThenInclude(i => i.Isbn13Navigation).FirstAsync(s => s.StoreId == id);
    }

    public async Task<Inventory?> GetBookInInventory(int storeId, long isbn13)
    {
        return await _db.Inventories.FindAsync(storeId, isbn13);
    }

    // public async Task<Inventory?> GetBookInInventory(int storeId, long isbn13)
    // {
    //     return await _db.Inventories
    //     .FirstOrDefaultAsync(ls => ls.StoreId == storeId && ls.Isbn13 == isbn13);
    // }

    public async Task<List<Book>> GetAllBooks()
    {
        return await _db.Books.Include(b => b.Author).Include(b => b.Publisher).ToListAsync();
    }

    public async Task<List<Author>> GetAllAuthors()
    {
        return await _db.Authors.ToListAsync();
    }

    public async Task<List<Publisher>> GetAllPublishers()
    {
        return await _db.Publishers.ToListAsync();
    }

    public async Task<List<Store>> GetAllStores()
    {
        return await _db.Stores.ToListAsync();
    }

    public async Task<List<Inventory>> GetStoreInventory(int storeId)
    {
        return await _db.Inventories
            .Where(i => i.StoreId == storeId)
            .Include(i => i.Isbn13Navigation)
                .ThenInclude(b => b.Author)
            .ToListAsync();
    }

    public async Task<Book> UpdateBook(Book book)
    {
        _db.Books.Update(book);
        await _db.SaveChangesAsync();
        return book;
    }

    public async Task<Author> UpdateAuthor(Author author)
    {
        _db.Authors.Update(author);
        await _db.SaveChangesAsync();
        return author;
    }

    public async Task<Publisher> UpdatePublisher(Publisher publisher)
    {
        _db.Publishers.Update(publisher);
        await _db.SaveChangesAsync();
        return publisher;
    }

    public async Task<Store> UpdateStore(Store store)
    {
        _db.Stores.Update(store);
        await _db.SaveChangesAsync();
        return store;
    }

    public async Task<Inventory> UpdateInventory(Inventory inventory)
    {
        _db.Inventories.Update(inventory);
        await _db.SaveChangesAsync();
        return inventory;
    }

    public async Task<bool> DeleteBook(long isbn13)
    {
        var book = await GetBook(isbn13);
        if (book == null) return false;

        _db.Books.Remove(book);
        return true;
    }

    public async Task<bool> DeleteAuthor(int id)
    {
        var author = await GetAuthor(id);
        if (author == null) return false;

        _db.Authors.Remove(author);
        return true;
    }

    public async Task<bool> DeletePublisher(int id)
    {
        var publisher = await GetPublisher(id);
        if (publisher == null) return false;

        _db.Publishers.Remove(publisher);
        return true;
    }

    public async Task<bool> DeleteStore(int id)
    {
        var store = await GetStore(id);
        if (store == null) return false;

        _db.Stores.Remove(store);
        return true;
    }

    public async Task<bool> DeleteInventory(int storeId, long isbn13)
    {
        var inventory = await GetBookInInventory(storeId, isbn13);
        if (inventory == null) return false;

        _db.Inventories.Remove(inventory);
        return true;
    }
}
