using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace robota12
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class Sale
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
        public int CustomerId { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Генерація тестових даних...");
            var (customers, products, sales) = GenerateData();
            Console.WriteLine($"Згенеровано: {customers.Count} клієнтів, {products.Count} продуктів, {sales.Count} продажів.\n");

            RunPipeline1(sales, products);
            RunPipeline2(sales, products, customers);
            RunPipeline3(sales, products);
            RunPipeline4(sales, products, customers);
        }

        static void RunPipeline1(List<Sale> sales, List<Product> products)
        {
            Console.WriteLine("Пайплайн 1: Найпопулярніші продукти за категоріями");
            var sw = Stopwatch.StartNew();

            var query = sales
                .GroupBy(s => s.ProductId)
                .Select(g => new { ProductId = g.Key, TotalQuantity = g.Sum(s => s.Quantity) })
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { p.Name, p.Category, s.TotalQuantity })
                .GroupBy(x => x.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TopProducts = g.OrderByDescending(x => x.TotalQuantity).Take(3)
                })
                .ToList();

            sw.Stop();
            
            foreach (var category in query)
            {
                Console.WriteLine($"Категорія: {category.Category}");
                foreach (var prod in category.TopProducts)
                {
                    Console.WriteLine($"  - {prod.Name} (Продано: {prod.TotalQuantity})");
                }
            }
            Console.WriteLine($"[Час виконання: {sw.ElapsedMilliseconds} мс / {sw.ElapsedTicks} тиків]\n");
        }

        static void RunPipeline2(List<Sale> sales, List<Product> products, List<Customer> customers)
        {
            Console.WriteLine("Пайплайн 2: Загальний дохід за містами клієнтів");
            var sw = Stopwatch.StartNew();

            var query = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.CustomerId, s.Quantity, p.UnitPrice })
                .Join(customers, sp => sp.CustomerId, c => c.Id, (sp, c) => new { c.City, Revenue = sp.Quantity * sp.UnitPrice })
                .GroupBy(x => x.City)
                .Select(g => new { City = g.Key, TotalRevenue = g.Sum(x => x.Revenue) })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            sw.Stop();

            foreach (var item in query)
            {
                Console.WriteLine($"{item.City}: {item.TotalRevenue:C2}");
            }
            Console.WriteLine($"[Час виконання: {sw.ElapsedMilliseconds} мс / {sw.ElapsedTicks} тиків]\n");
        }
        static void RunPipeline3(List<Sale> sales, List<Product> products)
        {
            Console.WriteLine("Пайплайн 3: Продажі за місяцями та роками");
            var sw = Stopwatch.StartNew();

            var query = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.SaleDate, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
                .GroupBy(x => new { x.SaleDate.Year, x.SaleDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            sw.Stop();

            foreach (var item in query)
            {
                Console.WriteLine($"{item.Year}-{item.Month:D2}: {item.TotalQuantity} шт., Дохід: {item.TotalRevenue:C2}");
            }
            Console.WriteLine($"[Час виконання: {sw.ElapsedMilliseconds} мс / {sw.ElapsedTicks} тиків]\n");
        }

        static void RunPipeline4(List<Sale> sales, List<Product> products, List<Customer> customers)
        {
            Console.WriteLine("Пайплайн 4: Топ-5 VIP клієнтів (які купували товари з >= 2 різних категорій)");
            var sw = Stopwatch.StartNew();

            var query = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.CustomerId, p.Category, Revenue = s.Quantity * p.UnitPrice })
                .Join(customers, sp => sp.CustomerId, c => c.Id, (sp, c) => new { c.Name, sp.Category, sp.Revenue })
                .GroupBy(x => x.Name)
                .Select(g => new
                {
                    CustomerName = g.Key,
                    TotalSpent = g.Sum(x => x.Revenue),
                    DistinctCategories = g.Select(x => x.Category).Distinct().Count()
                })
                .Where(x => x.DistinctCategories >= 2)
                .OrderByDescending(x => x.TotalSpent)
                .Take(5)
                .ToList();

            sw.Stop();

            foreach (var item in query)
            {
                Console.WriteLine($"{item.CustomerName} | Витрачено: {item.TotalSpent:C2} | Категорій: {item.DistinctCategories}");
            }
            Console.WriteLine($"[Час виконання: {sw.ElapsedMilliseconds} мс / {sw.ElapsedTicks} тиків]\n");
        }

        static (List<Customer>, List<Product>, List<Sale>) GenerateData()
        {
            var rnd = new Random(42);
            var cities = new[] { "Київ", "Львів", "Одеса", "Дніпро", "Харків" };
            var categories = new[] { "Електроніка", "Одяг", "Книги", "Їжа", "Іграшки" };

            var customers = Enumerable.Range(1, 100)
                .Select(i => new Customer { Id = i, Name = $"Customer {i}", City = cities[rnd.Next(cities.Length)] })
                .ToList();

            var products = Enumerable.Range(1, 100)
                .Select(i => new Product { Id = i, Name = $"Product {i}", Category = categories[rnd.Next(categories.Length)], UnitPrice = rnd.Next(10, 5000) })
                .ToList();

            var sales = Enumerable.Range(1, 1000)
                .Select(i => new Sale
                {
                    Id = i,
                    ProductId = rnd.Next(1, 101),
                    CustomerId = rnd.Next(1, 101),
                    Quantity = rnd.Next(1, 10),
                    SaleDate = new DateTime(2023, 1, 1).AddDays(rnd.Next(365))
                })
                .ToList();

            return (customers, products, sales);
        }
    }
}