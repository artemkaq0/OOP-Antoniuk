using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace robota13
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Дослідження продуктивності PLINQ\n");
            
            int[] sizes = { 1_000_000, 5_000_000, 10_000_000 };

            foreach (var size in sizes)
            {
                RunPerformanceTest(size);
            }

            Console.WriteLine("\nДослідження проблем безпеки (побічні ефекти)\n");
            RunSecurityAndSideEffectsTest();
        }

        static void RunPerformanceTest(int size)
        {
            Console.WriteLine($"Тест для {size:N0} елементів");
            
            var data = GenerateData(size);
            var sw = new Stopwatch();

            sw.Start();
            var linqResult = data
                .Where(x => x > 0.5)
                .Select(HeavyComputation) 
                .ToList();
            sw.Stop();
            long linqTime = sw.ElapsedMilliseconds;

            // PLINQ
            sw.Restart();
            var plinqResult = data
                .AsParallel()
                .Where(x => x > 0.5)
                .Select(HeavyComputation)
                .ToList();
            sw.Stop();
            long plinqTime = sw.ElapsedMilliseconds;

            Console.WriteLine($"LINQ час:  {linqTime} мс");
            Console.WriteLine($"PLINQ час: {plinqTime} мс");
            
            if (plinqTime < linqTime)
            {
                double speedup = (double)linqTime / plinqTime;
                Console.WriteLine($"PLINQ швидший у {speedup:F2} разів.\n");
            }
            else
            {
                Console.WriteLine("PLINQ виявився повільнішим (або таким самим).\n");
            }
        }

        static void RunSecurityAndSideEffectsTest()
        {
            int size = 1_000_000;
            var data = GenerateData(size);
            
            int unsafeCounter = 0;
            data.AsParallel().ForAll(x =>
            {
                if (x > 0.5)
                {
                    unsafeCounter++; 
                }
            });

            int safeCounterLock = 0;
            object locker = new object();
            data.AsParallel().ForAll(x =>
            {
                if (x > 0.5)
                {
                    lock (locker)
                    {
                        safeCounterLock++;
                    }
                }
            });

            int safeCounterInterlocked = 0;
            data.AsParallel().ForAll(x =>
            {
                if (x > 0.5)
                {
                    Interlocked.Increment(ref safeCounterInterlocked);
                }
            });

            int expectedCount = data.Count(x => x > 0.5);

            Console.WriteLine($"Очікуваний результат: {expectedCount}");
            Console.WriteLine($"Небезпечний лічильник (без синхронізації): {unsafeCounter} (ВТРАТА ДАНИХ!)");
            Console.WriteLine($"Безпечний лічильник (lock): {safeCounterLock}");
            Console.WriteLine($"Безпечний лічильник (Interlocked): {safeCounterInterlocked}");
        }

        static List<double> GenerateData(int size)
        {
            var random = new Random(42);
            var list = new List<double>(size);
            for (int i = 0; i < size; i++)
            {
                list.Add(random.NextDouble());
            }
            return list;
        }

        static double HeavyComputation(double x)
        {
            double result = x;
            for (int i = 0; i < 50; i++)
            {
                result = Math.Sqrt(result + Math.Sin(i));
            }
            return result;
        }
    }
}