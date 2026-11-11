using System;
using System.Collections.Generic;

namespace robota10
{
    public static class CollectionUtils
    {
        public static IEnumerable<TResult> Map<TSource, TResult>(
            IEnumerable<TSource> collection, Func<TSource, TResult> mapper)
        {
            var result = new List<TResult>();
            foreach (var item in collection)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        public static T MaxBy<T, TKey>(
            IEnumerable<T> collection, Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            T maxItem = default(T);
            TKey maxKey = default(TKey);
            bool first = true;
            foreach (var item in collection)
            {
                TKey key = keySelector(item);
                if (first || key.CompareTo(maxKey) > 0)
                {
                    maxKey = key;
                    maxItem = item;
                    first = false;
                }
            }
            if (first) throw new InvalidOperationException("Колекція порожня");
            return maxItem;
        }

        public static IEnumerable<T> Filter<T>(
            IEnumerable<T> collection, Predicate<T> predicate)
        {
            var result = new List<T>();
            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static void ForEach<T>(
            IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1. Колекція об'єктів (List<Product>)");
            var products = new List<Product>
            {
                new Product { Name = "Laptop", Price = 25000 },
                new Product { Name = "Mouse", Price = 500 },
                new Product { Name = "Keyboard", Price = 1500 }
            };

            var names = CollectionUtils.Map(products, p => p.Name);
            Console.WriteLine("Names: " + string.Join(", ", names));

            var mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
            Console.WriteLine($"Most expensive: {mostExpensive.Name} - {mostExpensive.Price}");

            var expensive = CollectionUtils.Filter(products, p => p.Price > 1000);
            Console.WriteLine("Expensive products:");
            CollectionUtils.ForEach(expensive, p => Console.WriteLine($" - {p.Name}"));

            Console.WriteLine("\n2. Колекція чисел (List<int>)");
            var numbers = new List<int> { 10, -5, 23, 8, 42, 0 };
            
            var squares = CollectionUtils.Map(numbers, n => n * n);
            Console.WriteLine("Квадрати чисел: " + string.Join(", ", squares));
            
            var positive = CollectionUtils.Filter(numbers, n => n > 0);
            Console.WriteLine("Лише додатні: " + string.Join(", ", positive));
            
            var maxNum = CollectionUtils.MaxBy(numbers, n => n);
            Console.WriteLine($"Максимальне число: {maxNum}");

            Console.WriteLine("\n3. Колекція рядків (List<string>)");
            var words = new List<string> { "apple", "banana", "kiwi", "strawberry" };
            
            var longWords = CollectionUtils.Filter(words, w => w.Length > 5);
            Console.WriteLine("Довгі слова (довжина > 5):");
            CollectionUtils.ForEach(longWords, w => Console.WriteLine(w));
            
            var longestWord = CollectionUtils.MaxBy(words, w => w.Length);
            Console.WriteLine($"Найдовше слово: {longestWord}");
        }
    }
}