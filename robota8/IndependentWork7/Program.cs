using System;
using System.Collections.Generic;

namespace IndependentWork7
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Демонстрація роботи Cache<TKey, TValue> (Політика FIFO)");
            
            var cache = new Cache<int, string>(3);

            cache.Add(1, "Студент 1");
            cache.Add(2, "Студент 2");
            cache.Add(3, "Студент 3");

            Console.WriteLine($"\nКеш заповнено. Кількість елементів: {cache.Count}");
            Console.WriteLine($"Отримуємо значення за ключем 2: {cache.Get(2)}");

            Console.WriteLine("\nДодаємо 4-й елемент, щоб витіснити найстаріший (ключ 1)...");
            cache.Add(4, "Студент 4");

            Console.WriteLine($"Кількість елементів після додавання: {cache.Count}");
            Console.WriteLine($"Чи існує ключ 1? {cache.ContainsKey(1)}");
            Console.WriteLine($"Чи існує ключ 4? {cache.ContainsKey(4)}");

            Console.WriteLine("\nСпроба отримати видалений елемент (ключ 1):");
            try
            {
                cache.Get(1);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Спіймано виняток: {ex.Message}");
            }
        }
    }
}