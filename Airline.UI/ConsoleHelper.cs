using System.Globalization;

namespace Airline.UI;

public static class ConsoleHelper
{
    public static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
                return value;

            Console.WriteLine($"Помилка! Введіть ціле число від {min} до {max}.");
        }
    }

    public static decimal ReadDecimal(string prompt, decimal min, decimal max)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Replace(',', '.');
            if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value)
                && value >= min && value <= max)
                return value;

            Console.WriteLine($"Помилка! Введіть число від {min} до {max}.");
        }
    }

    public static double ReadDouble(string prompt, double min, double max)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Replace(',', '.');
            if (double.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out double value)
                && value >= min && value <= max)
                return value;

            Console.WriteLine($"Помилка! Введіть число від {min} до {max}.");
        }
    }

    public static string ReadNonEmptyString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
                return input.Trim();

            Console.WriteLine("Помилка! Рядок не може бути порожнім.");
        }
    }

    public static DateTime ReadDateTime(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParseExact(
                Console.ReadLine(),
                "dd.MM.yyyy HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date))
                return date;

            Console.WriteLine("Помилка! Використовуйте формат дд.мм.рррр гг:хх (наприклад, 25.12.2026 14:30).");
        }
    }
}