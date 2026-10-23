using System;
using System.Collections.Generic;

namespace lab13v1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Робота з int");
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
            
            Console.WriteLine("Оригінальний список чисел:");
            FunctionalUtils.ForEach(numbers, n => Console.Write(n + " "));
            Console.WriteLine("\n");

            List<int> squared = FunctionalUtils.Map(numbers, n => n * n);
            Console.WriteLine("Map (квадрати чисел):");
            FunctionalUtils.ForEach(squared, n => Console.Write(n + " "));
            Console.WriteLine("\n");

            List<int> evens = FunctionalUtils.Filter(numbers, n => n % 2 == 0);
            Console.WriteLine("Filter (парні числа):");
            FunctionalUtils.ForEach(evens, n => Console.Write(n + " "));
            Console.WriteLine("\n");

            int sum = FunctionalUtils.Reduce(numbers, 0, (acc, n) => acc + n);
            Console.WriteLine($"Reduce (сума чисел): {sum}");

            bool hasNegative = FunctionalUtils.Any(numbers, n => n < 0);
            Console.WriteLine($"Any (чи є від'ємні числа?): {hasNegative}\n");

            Console.WriteLine("Робота з string");
            List<string> words = new List<string> { "apple", "banana", "kiwi", "pear" };
            
            List<int> wordLengths = FunctionalUtils.Map(words, w => w.Length);
            Console.WriteLine("Map (довжини слів):");
            FunctionalUtils.ForEach(wordLengths, len => Console.Write(len + " "));
            Console.WriteLine("\n");

            string concatenated = FunctionalUtils.Reduce(words, "", (acc, w) => acc == "" ? w : acc + ", " + w);
            Console.WriteLine($"Reduce (об'єднання в рядок): {concatenated}\n");

            Console.WriteLine("Робота з класом Product");
            List<Product> products = new List<Product>
            {
                new Product("Ноутбук", 35000),
                new Product("Мишка", 800),
                new Product("Клавіатура", 1500),
                new Product("Монітор", 8000)
            };

            Console.WriteLine("Всі продукти (ForEach):");
            FunctionalUtils.ForEach(products, p => Console.WriteLine(p));

            List<Product> expensiveProducts = FunctionalUtils.Filter(products, p => p.Price > 5000);
            Console.WriteLine("\nFilter (дорожче 5000 грн):");
            FunctionalUtils.ForEach(expensiveProducts, p => Console.WriteLine(p.Name));

            bool hasBudgetMouse = FunctionalUtils.Any(products, p => p.Name == "Мишка" && p.Price < 1000);
            Console.WriteLine($"\nAny (чи є мишка дешевше 1000 грн?): {hasBudgetMouse}");
        }
    }
}