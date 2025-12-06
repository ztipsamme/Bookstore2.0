using Bookstore2._0;
using Bookstore2._0.Models;
using Bookstore2._0.Ui;

using var context = new Bookstore2Context();

var dbService = new DbService(context);
var mainMenu = new MainMenu(context, dbService);

await mainMenu.Show();