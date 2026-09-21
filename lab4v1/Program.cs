using System;

namespace GeometryApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("1. Створення об'єктів та статичний член");
            Point p1 = new Point(10, 20);
            Point p2 = new Point(5, -15);
            Point origin = Point.Origin;

            Console.WriteLine($"Точка p1: {p1}");
            Console.WriteLine($"Точка p2: {p2}");
            Console.WriteLine($"Початок координат (Point.Origin): {origin}");

            Console.WriteLine("\n2. Робота з індексатором");
            Console.WriteLine($"p1[0] (X): {p1[0]}, p1[1] (Y): {p1[1]}");
            p1[0] = 50;
            Console.WriteLine($"Після p1[0] = 50 -> p1: {p1}");

            Console.WriteLine("\n3. Перевантаження операторів");
            Point sum = p1 + p2;
            Console.WriteLine($"Додавання (p1 + p2): {sum}");

            Point scaled = p2 * 3;
            Console.WriteLine($"Множення на скаляр (p2 * 3): {scaled}");

            Console.WriteLine("\n4. Порівняння об'єктів (==, !=, Equals)");
            Point p3 = new Point(50, 20);
            Console.WriteLine($"p1 ({p1}) == p3 ({p3}): {p1 == p3}");
            Console.WriteLine($"p1 ({p1}) == p2 ({p2}): {p1 == p2}");
            Console.WriteLine($"p1.Equals(p3): {p1.Equals(p3)}");
            Console.WriteLine($"GetHashCode p1: {p1.GetHashCode()}, p3: {p3.GetHashCode()}");

            Console.WriteLine("\n5. Перевірка валідації властивостей");
            try
            {
                Point invalidPoint = new Point(1500, 0);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Успішно перехоплено помилку валідації: {ex.Message}");
            }
        }
    }
}