using System;

namespace isbn_converter
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = args.Length > 0 ? args[0] : null;
            if (input == null)
            {
                Console.WriteLine("Missing argument: ISBN");
                Environment.Exit(1);
            }

            var converter = new ISBNConverter(input);
            if (!converter.IsValid())
            {
                Console.WriteLine($"{converter.ErrorMessage}");
                Environment.Exit(1);
            }

            Console.WriteLine($"ISBN10: {converter.ISBN10}");
            Console.WriteLine($"ISBN13: {converter.ISBN13}");
        }
    }
}