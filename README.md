# Bookstore2.0

## Description

This is a C#-application for a bookstore with multiple stores, built with Entity Framework Database-First.

The user can:

- List inventory for the stores.
- Add, update and remove books, authors and publishers.
- Add and remove books from inventory.

## Database

All test data is included in `Bookstore2.bacpac`. To import the database:

1. Open SQL Server Management Studio or Azure Data Studio
2. Import `Bookstore2.bacpac` as a new database (e.g., `Bookstore2`)

## Run application

1. Open Bookstore2.sln in your preferred IUD.
2. Build the project with `dotnet build` in the terminal
3. Run the console application with `dotnet run` in the terminal
4. Follow the menus in the console to handle stores, books, authors and publishers

## Comments

- All testdata is located in Bookstore2.bacpac
- The application is a console app but can easily be adjusted to WPF/Avalonia
