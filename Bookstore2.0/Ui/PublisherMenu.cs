using System;
using Bookstore2._0.Services;

namespace Bookstore2._0.Ui;

public class PublisherMenu
{
    private readonly PublisherFlow _publisherFlow;

    public PublisherMenu(DbService dbService)
    {
        _publisherFlow
         = new PublisherFlow(dbService);
    }

    public async Task Show()
    {
        List<MenuItem> menu = new()
            {
                new ("List all publishers",  () => _publisherFlow
                .ListAllPublishersFlow()),
                new ("Add publisher", ()=> _publisherFlow.AddNewPublisherToDbFlow()),
                new ("Edit publisher", ()=> _publisherFlow.UpdatePublisher()),
                new ("Delete author", ()=> _publisherFlow.DeletePublisherFlow()),
            };

        await ConsoleHelper.ChooseFromMenu("Publisher menu", menu);

    }
}