using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Timeout;

namespace robota14
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Polly Resilience Demo\n");

            await RunDatabaseScenarioAsync();
            Console.WriteLine("\n-\n");
            await RunPaymentGatewayScenarioAsync();
        }

        private static int _dbAttempts = 0;

        static async Task RunDatabaseScenarioAsync()
        {
            Console.WriteLine("Сценарій 1: Підключення до БД (Timeout + Retry)");
            
            var pipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<TimeoutRejectedException>().Handle<InvalidOperationException>(),
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Constant,
                    OnRetry = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Retry] Повторна спроба #{args.AttemptNumber + 1} через помилку: {args.Outcome.Exception?.GetType().Name}");
                        return default;
                    }
                })
                .AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromSeconds(2),
                    OnTimeout = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Timeout] Операція перевищила ліміт часу!");
                        return default;
                    }
                })
                .Build();

            try
            {
                await pipeline.ExecuteAsync(async token => await ConnectToDatabaseAsync(token));
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Успішне підключення до БД!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Фатальна помилка] Не вдалося підключитися після всіх спроб: {ex.Message}");
            }
        }

        static async Task ConnectToDatabaseAsync(CancellationToken token)
        {
            _dbAttempts++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Спроба {_dbAttempts}: Підключення до бази даних...");

            if (_dbAttempts == 1)
            {
                await Task.Delay(3000, token); 
            }
            else if (_dbAttempts == 2)
            {
                throw new InvalidOperationException("Deadlock detected in DB.");
            }
            
            await Task.Delay(500, token);
        }

        private static int _paymentAttempts = 0;

        static async Task RunPaymentGatewayScenarioAsync()
        {
            Console.WriteLine("Сценарій 2: Платіжний шлюз (Circuit Breaker + Fallback)");

            var pipeline = new ResiliencePipelineBuilder<string>()
                .AddFallback(new FallbackStrategyOptions<string>
                {
                    ShouldHandle = new PredicateBuilder<string>().Handle<BrokenCircuitException>().Handle<Exception>(),
                    FallbackAction = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Fallback] Шлюз недоступний. Платіж збережено в локальну чергу для обробки пізніше.");
                        return Outcome.FromResultAsValueTask("FALLBACK_SUCCESS");
                    }
                })
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions<string>
                {
                    ShouldHandle = new PredicateBuilder<string>().Handle<Exception>(),
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    MinimumThroughput = 2,
                    BreakDuration = TimeSpan.FromSeconds(5),
                    OnOpened = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Circuit Breaker] Ланцюг РОЗІРВАНО (Open)! Шлюз впав, зупиняємо запити на 5 сек.");
                        return default;
                    },
                    OnClosed = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Circuit Breaker] Ланцюг ВІДНОВЛЕНО (Closed).");
                        return default;
                    }
                })
                .Build();

            for (int i = 1; i <= 4; i++)
            {
                Console.WriteLine($"\nІніціація транзакції #{i}");
                var result = await pipeline.ExecuteAsync(async token => await ProcessPaymentAsync("user123", token));
                Console.WriteLine($"Результат транзакції #{i}: {result}");
                await Task.Delay(1000);
            }
        }

        static async Task<string> ProcessPaymentAsync(string userId, CancellationToken token)
        {
            _paymentAttempts++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Надсилання платежу для {userId}...");
            
            throw new Exception("Payment Gateway 503 Service Unavailable");
        }
    }
}