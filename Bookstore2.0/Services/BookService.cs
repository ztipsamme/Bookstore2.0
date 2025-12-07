using System;
using Bookstore2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore2._0.Services;

public class BookService
{
    private readonly Bookstore2Context _db;
    public BookService(Bookstore2Context db)
    {
        _db = db;
    }

    public async Task<Book> AddBook(Book book)
    {
        _db.Books.Add(book);
        await _db.SaveChangesAsync();
        return book;
    }

    public async Task<Book?> GetBook(long isbn13)
    {
        return await _db.Books.FindAsync(isbn13);
    }

    public async Task<List<Book>> GetAllBooks()
    {
        return await _db.Books.AsNoTracking().Include(b => b.Author).Include(b => b.Publisher).ToListAsync();
    }

    public async Task<bool> BookExists(long isbn13)
    {
        return await _db.Books.AnyAsync(b => b.Isbn13 == isbn13);
    }

    public async Task<Book> UpdateBook(Book book)
    {
        _db.Books.Update(book);
        await _db.SaveChangesAsync();
        return book;
    }

    public async Task<bool> DeleteBook(long isbn13)
    {
        var book = await GetBook(isbn13);
        if (book == null) return false;

        _db.Books.Remove(book);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteBooksByAuthorId(int authorId)
    {
        await _db.Books
        .Where(b => b.AuthorId == authorId)
        .ExecuteDeleteAsync();
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteBooksByPublisherId(int publisherId)
    {
        await _db.Books.Where(a => a.PublisherId == publisherId).ExecuteDeleteAsync();
        await _db.SaveChangesAsync();

        return true;
    }
}
