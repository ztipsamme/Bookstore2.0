using System;
using Bookstore2._0.Services;

namespace Bookstore2._0.Ui;

public class AuthorMenu
{
    private readonly AuthorFlow _authorFlow;

    public AuthorMenu(DbService dbService)
    {
        _authorFlow = new AuthorFlow(dbService);
    }

    public async Task Show()
    {
        List<MenuItem> menu = new()
            {
                new ("List all authors",  () => _authorFlow.ListAllAuthorsFlow()),
                new ("Add author", ()=> _authorFlow.AddNewAuthorToDbFlow()),
                new ("Delete author", ()=> _authorFlow.DeleteAuthorFlow()),
            };

        await ConsoleHelper.ChooseFromMenu("Author menu", menu);

    }
}
