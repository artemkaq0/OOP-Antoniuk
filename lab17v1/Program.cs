using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace lab17v1
{
    public class FileProcessor
    {
        private int _attempts = 0;

        public string ReadFile(string path)
        {
            _attempts++;
            Console.WriteLine($"[FileProcessor] Спроба читання файлу (спроба №{_attempts})");
            
            if (_attempts <= 2)
            {
                throw new FileNotFoundException($"Файл за шляхом {path} не знайдено.");
            }
            
            return $"УСПІХ! Вміст файлу '{path}' успішно прочитано.";
        }
    }

    public class NetworkClient
    {
        private int _attempts = 0;

        public string DownloadData(string url)
        {
            _attempts++;
            Console.WriteLine($"[NetworkClient] Спроба завантаження з (спроба №{_attempts})");
            
            if (_attempts <= 3)
            {
                throw new HttpRequestException($"Помилка з'єднання із сервером {url}");
            }
            
            return $"УСПІХ! Дані за адресою '{url}' успішно завантажено.";
        }
    }

    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation, 
            int retryCount = 3, 
            TimeSpan initialDelay = default, 
            Func<Exception, bool> shouldRetry = null)
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
                    attempt++;

                    bool canRetry = (attempt <= retryCount) && (shouldRetry == null || shouldRetry(ex));

                    if (!canRetry)
                    {
                        Console.WriteLine($"[RetryHelper] ФАТАЛЬНА ПОМИЛКА: Перевищено ліміт спроб або виняток не підлягає повтору. ({ex.GetType().Name})");
                        throw;
                    }

                    Console.WriteLine($"[RetryHelper] ПОМИЛКА: {ex.Message}. Спроба {attempt} з {retryCount}.");

                    double multiplier = Math.Pow(2, attempt - 1);
                    TimeSpan delay = TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * multiplier);
                    
                    Console.WriteLine($"[RetryHelper] ОЧІКУВАННЯ: {delay.TotalSeconds} сек. перед наступною спробою...");
                    Thread.Sleep(delay);
                }
                finally
                {
                    Console.WriteLine("[RetryHelper] Блок finally: завершення ітерації try-catch.\n");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Лабораторна робота №17. Варіант 1\n");

            var fileProcessor = new FileProcessor();
            var networkClient = new NetworkClient();

            Func<Exception, bool> retryPolicy = ex => 
                ex is FileNotFoundException || ex is HttpRequestException;

            Console.WriteLine("СЦЕНАРІЙ 1: Робота з файлом");
            try
            {
                string fileResult = RetryHelper.ExecuteWithRetry(
                    operation: () => fileProcessor.ReadFile("config.json"),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromSeconds(1),
                    shouldRetry: retryPolicy
                );
                Console.WriteLine(fileResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Main] Операція з файлом остаточно провалилася: {ex.Message}");
            }

            Console.WriteLine("СЦЕНАРІЙ 2: Робота з мережею");
            try
            {
                string networkResult = RetryHelper.ExecuteWithRetry(
                    operation: () => networkClient.DownloadData("https://api.example.com/data"),
                    retryCount: 4,
                    initialDelay: TimeSpan.FromSeconds(1),
                    shouldRetry: retryPolicy
                );
                Console.WriteLine(networkResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Main] Мережева операція остаточно провалилася: {ex.Message}");
            }
            
            Console.WriteLine("\nПрограма завершила роботу.");
        }
    }
}