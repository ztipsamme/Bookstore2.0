using System;
using Bookstore2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore2._0.Services;

public class AuthorService
{
    private readonly Bookstore2Context _db;
    public AuthorService(Bookstore2Context db)
    {
        _db = db;
    }

    public async Task<Author> AddAuthor(Author author)
    {
        _db.Authors.Add(author);
        await _db.SaveChangesAsync();
        return author;
    }

    public async Task<Author?> GetAuthor(int id)
    {
        return await _db.Authors.FindAsync(id);
    }

    public async Task<List<Author>> GetAllAuthors()
    {
        return await _db.Authors.AsNoTracking().ToListAsync();
    }

    public async Task<bool> AuthorExists(string firstName, string lastName)
    {
        return await _db.Authors.AnyAsync(a => a.FirstName == firstName && a.LastName == lastName);
    }

    public async Task<bool> AuthorExists(int authorId)
    {
        return await _db.Authors.AnyAsync(a => a.AuthorId == authorId);
    }

    public async Task<Author> UpdateAuthor(Author author)
    {
        _db.Authors.Update(author);
        await _db.SaveChangesAsync();
        return author;
    }

    public async Task<bool> DeleteAuthor(int id)
    {
        var author = await GetAuthor(id);
        if (author == null) return false;

        _db.Authors.Remove(author);
        await _db.SaveChangesAsync();
        return true;
    }

}
