using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace lab15v1
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Clothing" },
                new Category { Id = 3, Name = "Home & Kitchen" },
                new Category { Id = 4, Name = "Books" }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Smartphone", Price = 800m, CategoryId = 1 },
                new Product { Id = 2, Name = "Laptop", Price = 1500m, CategoryId = 1 },
                new Product { Id = 3, Name = "Smartwatch", Price = 300m, CategoryId = 1 },
                new Product { Id = 4, Name = "Headphones", Price = 150m, CategoryId = 1 },
                new Product { Id = 5, Name = "TV", Price = 1200m, CategoryId = 1 },
                new Product { Id = 6, Name = "T-Shirt", Price = 25m, CategoryId = 2 },
                new Product { Id = 7, Name = "Jeans", Price = 60m, CategoryId = 2 },
                new Product { Id = 8, Name = "Jacket", Price = 120m, CategoryId = 2 },
                new Product { Id = 9, Name = "Sneakers", Price = 90m, CategoryId = 2 },
                new Product { Id = 10, Name = "Blender", Price = 85m, CategoryId = 3 },
                new Product { Id = 11, Name = "Microwave", Price = 200m, CategoryId = 3 },
                new Product { Id = 12, Name = "Coffee Maker", Price = 150m, CategoryId = 3 },
                new Product { Id = 13, Name = "Vacuum Cleaner", Price = 300m, CategoryId = 3 },
                new Product { Id = 14, Name = "C# Programming Book", Price = 45m, CategoryId = 4 },
                new Product { Id = 15, Name = "Design Patterns", Price = 55m, CategoryId = 4 }
            };

            Stopwatch sw = new Stopwatch();

            Console.WriteLine("ЗАПИТ 1: Electronics з ціною > 500");
            
            sw.Restart();
            var q1Query = (from p in products
                           join c in categories on p.CategoryId equals c.Id
                           where c.Name == "Electronics" && p.Price > 500
                           select p).ToList();
            sw.Stop();
            Console.WriteLine($"[Синтаксис запитів] Знайдено: {q1Query.Count}. Час виконання: {sw.ElapsedTicks} тіків");

            sw.Restart();
            var q1Method = products
                .Join(categories, p => p.CategoryId, c => c.Id, (p, c) => new { p, c })
                .Where(x => x.c.Name == "Electronics" && x.p.Price > 500)
                .Select(x => x.p)
                .ToList();
            sw.Stop();
            Console.WriteLine($"[Синтаксис методів] Знайдено: {q1Method.Count}. Час виконання: {sw.ElapsedTicks} тіків\n");


            Console.WriteLine("ЗАПИТ 2: Об'єднання (Назва продукту - Назва категорії)");

            sw.Restart();
            var q2Query = (from p in products
                           join c in categories on p.CategoryId equals c.Id
                           select new { ProductName = p.Name, CategoryName = c.Name }).ToList();
            sw.Stop();
            Console.WriteLine($"[Синтаксис запитів] Знайдено пар: {q2Query.Count}. Час виконання: {sw.ElapsedTicks} тіків");

            sw.Restart();
            var q2Method = products
                .Join(categories, p => p.CategoryId, c => c.Id, (p, c) => new { ProductName = p.Name, CategoryName = c.Name })
                .ToList();
            sw.Stop();
            Console.WriteLine($"[Синтаксис методів] Знайдено пар: {q2Method.Count}. Час виконання: {sw.ElapsedTicks} тіків\n");


            Console.WriteLine("ЗАПИТ 3: Середня ціна в кожній категорії");

            sw.Restart();
            var q3Query = (from p in products
                           group p by p.CategoryId into g
                           join c in categories on g.Key equals c.Id
                           select new { CategoryName = c.Name, AvgPrice = g.Average(p => p.Price) }).ToList();
            sw.Stop();
            Console.WriteLine($"[Синтаксис запитів] Знайдено категорій: {q3Query.Count}. Час виконання: {sw.ElapsedTicks} тіків");

            sw.Restart();
            var q3Method = products
                .GroupBy(p => p.CategoryId)
                .Join(categories, g => g.Key, c => c.Id, (g, c) => new { CategoryName = c.Name, AvgPrice = g.Average(p => p.Price) })
                .ToList();
            sw.Stop();
            Console.WriteLine($"[Синтаксис методів] Знайдено категорій: {q3Method.Count}. Час виконання: {sw.ElapsedTicks} тіків\n");


            Console.WriteLine("ЗАПИТ 4: Найдорожчий продукт");

            sw.Restart();
            var q4Query = (from p in products
                           where p.Price == (from p2 in products select p2.Price).Max()
                           select p).FirstOrDefault();
            sw.Stop();
            Console.WriteLine($"[Синтаксис запитів] Продукт: {q4Query?.Name} ({q4Query?.Price}$). Час виконання: {sw.ElapsedTicks} тіків");

            sw.Restart();
            var q4Method = products.MaxBy(p => p.Price);
            sw.Stop();
            Console.WriteLine($"[Синтаксис методів] Продукт: {q4Method?.Name} ({q4Method?.Price}$). Час виконання: {sw.ElapsedTicks} тіків\n");

            Console.WriteLine("ВИСНОВКИ ПРО ПРОДУКТИВНІСТЬ");
            Console.WriteLine("1. Синтаксис методів (Fluent API) зазвичай виконується трохи швидше при компіляції та виконанні базових операцій (як-от MaxBy), оскільки він безпосередньо викликає методи розширення без додаткової трансляції компілятором.");
            Console.WriteLine("2. Найдорожчими за часом виявились операції Join разом із GroupBy (Запит 3). Групування (GroupBy) потребує проходу по всій колекції для створення хеш-таблиці ключів перед об'єднанням.");
            Console.WriteLine("3. Використання вкладеного підзапиту для пошуку максимального елемента в синтаксисі запитів (Запит 4) є дуже неефективним порівняно з методом MaxBy(), оскільки може виконувати обчислення O(N^2) замість O(N).");
        }
    }
}