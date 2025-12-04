using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

    public static T? AskUntilValid<T>(string prompt, string errMessage, Func<string, bool>? validate = null, Func<string, T>? convert = null)
    {
        int startRow = GetStartRow;
        string input = "";

        while (true)
        {
            var res = HandleInput(prompt, input);
            input = res.input;

            if (res.isCanceled) return default;

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

    private static (string input, bool isCanceled) HandleInput(string prompt, string input)
    {
        bool isCanceled = false;

        Console.Write($"{prompt}: {input}");
        ConsoleKeyInfo keyInfo;

        while (true)
        {
            keyInfo = Console.ReadKey(intercept: true);
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (keyInfo.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("\nInput canceled. Returning.");
                Thread.Sleep(1600);
                isCanceled = true;
                break;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input[..^1];
                Console.Write("\b \b");
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                input += keyInfo.KeyChar;
                Console.Write(keyInfo.KeyChar);
            }
        }

        return (input, isCanceled);
    }

    public static bool IsActionCanceled<T>(T? input)
    {
        bool isDefaultNum = (input is long l && l == 0) || (input is int i && i == 0);

        return input == null || isDefaultNum;
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