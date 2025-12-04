using Bookstore2._0.Models;
using Bookstore2._0.Ui;

using var context = new Bookstore2Context();

var mainMenu = new MainMenu(context);

mainMenu.Show();