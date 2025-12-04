using System;

namespace Bookstore2._0;

public static class ConsoleHelper
{
    public static string? Choice()
    {
        Console.Write("\nChoice: ");
        return Console.ReadLine();
    }

    public static bool IsValidChoice<T>(string? choice, List<T> list)
        => int.TryParse(choice, out int value) && value > 0 && value <= list.Count;

    public static void PressAnyKeyToContinue()
    {
        Console.Write("\nPress any key to continue...");
        Console.ReadKey();
    }

    public static T AskUntilValid<T>(string prompt, string errMessage, Func<string, bool>? validate = null, Func<string, T>? convert = null)
    {
        int startRow = GetStartRow;

        while (true)
        {
            Console.Write($"{prompt}: ");

            string input = Console.ReadLine() ?? "";

            bool isValid = validate?.Invoke(input) ?? !string.IsNullOrWhiteSpace(input);

            if (isValid)
            {
                if (convert != null) return convert(input);
                return (T)(object)input;
            }

            Console.WriteLine($"{errMessage} Try again.");
            Thread.Sleep(1600);

            ClearBelow(startRow - 1);
        }
    }

    public static int GetStartRow => Console.GetCursorPosition().Top;
    public static void ClearRow(int? startRow = null)
    {
        int row = startRow ?? GetStartRow;
        Console.SetCursorPosition(0, row);
        Console.Write(new string(' ', Console.WindowWidth));
    }
    public static void ClearBelow(int startRow = 0)
    {
        for (int i = startRow; i < Console.WindowHeight; i++)
        {
            ClearRow(i);
        }

        Console.SetCursorPosition(0, startRow);
    }
}