using System;

namespace Bookstore2._0;

public static class ConsoleHelper
{
    public static string? Choice()
    {
        Console.Write("\nChoice: ");
        return Console.ReadLine();
    }

    public static bool ValidateChoice<T>(string? choice, List<T> list)
        => !int.TryParse(choice, out int value) || value < 1 || value > list.Count;

    public static void PressAnyKey()
    {
        Console.Write("Press any key to continue...");
        Console.ReadKey();
    }
}