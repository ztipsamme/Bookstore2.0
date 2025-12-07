using System;
using Bookstore2._0.Flows;
using Bookstore2._0.Models;
using Bookstore2._0.Services;

namespace Bookstore2._0.Ui;

public class BookMenu
{
    private readonly BookFlow _bookFlow;

    public BookMenu(DbService dbService)
    {
        _bookFlow = new BookFlow(dbService);
    }


    public async Task Show()
    {
        List<MenuItem> menu = new()
            {
                new ("List all books",  () => _bookFlow.ListAllBooksFlow()),
                new ("Add new book to the database", ()=> _bookFlow.AddNewBookToDbFlow()),
                new ("Edit book",  () => _bookFlow.UpdateBookFlow()),
                new ("Delete book from the database", ()=> _bookFlow.DeleteBookFlow()),
            };

        await ConsoleHelper.ChooseFromMenu("Book menu", menu);

    }
}
