using Bookstore2._0;
using Bookstore2._0.Models;
using Bookstore2._0.Ui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var optionsBuilder = new DbContextOptionsBuilder<Bookstore2Context>();
optionsBuilder.UseSqlServer(configuration.GetConnectionString("BookstoreDatabase"));

using var context = new Bookstore2Context(optionsBuilder.Options);

var dbService = new DbService(context);
var bookstore = new BookStore(dbService);

await bookstore.Run();
