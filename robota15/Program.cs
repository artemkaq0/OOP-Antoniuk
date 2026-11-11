using System;
using System.Threading;

namespace robota15
{
    public class RetryPolicy
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _initialDelay;
        private readonly double _jitterFactor;
        private readonly Random _random;

        public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
        {
            if (maxRetries < 0) throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be >= 0");
            if (jitterFactor < 0 || jitterFactor > 1) throw new ArgumentOutOfRangeException(nameof(jitterFactor), "Jitter factor must be between 0 and 1");

            _maxRetries = maxRetries;
            _initialDelay = initialDelay;
            _jitterFactor = jitterFactor;
            _random = new Random();
        }

        public T Execute<T>(Func<T> operation, Func<Exception, bool> shouldRetry = null)
        {
            int attempt = 0;

            while (true)
            {
                try
                {
                    return operation();
                }
                catch (Exception ex)
                {
                    if (attempt >= _maxRetries || (shouldRetry != null && !shouldRetry(ex)))
                    {
                        Console.WriteLine($"[RetryPolicy] Остаточна невдача після {attempt} повторних спроб. Прокидаємо виняток вище.");
                        throw; 
                    }

                    double delayMilliseconds = _initialDelay.TotalMilliseconds 
                        * Math.Pow(2, attempt) 
                        * (1 + _random.NextDouble() * _jitterFactor * 2 - _jitterFactor);
                    
                    TimeSpan delay = TimeSpan.FromMilliseconds(delayMilliseconds);

                    Console.WriteLine($"[RetryPolicy] Невдача: {ex.Message}. Спроба {attempt + 1} з {_maxRetries}. Затримка: {delay.TotalMilliseconds:F1} мс.");
                    
                    Thread.Sleep(delay);
                    attempt++;
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Демонстрація роботи RetryPolicy");
            
            var retryPolicy = new RetryPolicy(maxRetries: 3, initialDelay: TimeSpan.FromMilliseconds(200), jitterFactor: 0.2);

            Console.WriteLine("\nСценарій 1: Успіх після кількох спроб");
            int attemptCount1 = 0;
            
            Func<string> unreliableNetworkCall = () =>
            {
                attemptCount1++;
                Console.WriteLine($"Виконання операції... (Виклик {attemptCount1})");
                if (attemptCount1 <= 2)
                {
                    throw new TimeoutException("Мережа тимчасово недоступна.");
                }
                return "Дані успішно завантажено!";
            };

            try
            {
                string result1 = retryPolicy.Execute(
                    unreliableNetworkCall, 
                    shouldRetry: ex => ex is TimeoutException
                );
                Console.WriteLine($"Результат: {result1}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }

            Console.WriteLine("\nСценарій 2: Остаточна невдача (завжди падає)");
            int attemptCount2 = 0;

            Func<int> alwaysFailingDbCall = () =>
            {
                attemptCount2++;
                Console.WriteLine($"Виконання БД запиту... (Виклик {attemptCount2})");
                throw new InvalidOperationException("Помилка підключення до бази даних.");
            };

            try
            {
                int result2 = retryPolicy.Execute(alwaysFailingDbCall);
                Console.WriteLine($"Результат: {result2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Отримано очікуваний виняток у Main: {ex.GetType().Name} - {ex.Message}");
            }

            Console.WriteLine("\nРоботу завершено");
        }
    }
}