using System;

namespace lab18v1
{
    public class InsufficientFundsException : Exception
    {
        public decimal RequestedAmount { get; }
        public decimal CurrentBalance { get; }

        public InsufficientFundsException() : base("Недостатньо коштів на рахунку.") { }

        public InsufficientFundsException(string message) : base(message) { }

        public InsufficientFundsException(string message, Exception innerException) : base(message, innerException) { }

        public InsufficientFundsException(decimal requestedAmount, decimal currentBalance) 
            : base($"Недостатньо коштів. Спроба зняти: {requestedAmount:C}, Поточний баланс: {currentBalance:C}")
        {
            RequestedAmount = requestedAmount;
            CurrentBalance = currentBalance;
        }
    }

    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }

        private Result(T value, bool isSuccess, string error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result<T> Success(T value) => new Result<T>(value, true, null);
        
        public static Result<T> Failure(string error) => new Result<T>(default, false, error);
        
        public static Result<T> Failure(Exception ex) => new Result<T>(default, false, ex.Message);
    }

    public class AccountService
    {
        private decimal _balance;

        public AccountService(decimal initialBalance)
        {
            _balance = initialBalance;
        }

        public void Withdraw(decimal amount)
        {
            if (amount > _balance)
            {
                throw new InsufficientFundsException(amount, _balance);
            }
            _balance -= amount;
            Console.WriteLine($"[Withdraw] Успішно знято {amount:C}. Залишок: {_balance:C}");
        }

        public Result<decimal> Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                return Result<decimal>.Failure("Сума поповнення має бути більшою за нуль.");
            }
            _balance += amount;
            return Result<decimal>.Success(_balance);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Лабораторна робота №18. Варіант 1\n");

            var account = new AccountService(1000m);

            Console.WriteLine("Тестування Deposit (Result Pattern)");
            
            var successDeposit = account.Deposit(500m);
            if (successDeposit.IsSuccess)
                Console.WriteLine($"Поповнення успішне. Новий баланс: {successDeposit.Value:C}");
            else
                Console.WriteLine($"Помилка поповнення: {successDeposit.Error}");

            var failDeposit = account.Deposit(-100m);
            if (failDeposit.IsSuccess)
                Console.WriteLine($"Поповнення успішне. Новий баланс: {failDeposit.Value:C}");
            else
                Console.WriteLine($"Помилка поповнення: {failDeposit.Error}");

            Console.WriteLine("\nТестування Withdraw (Exceptions)");
            
            try
            {
                account.Withdraw(300m);
                account.Withdraw(2000m);
            }
            catch (InsufficientFundsException ex)
            {
                Console.WriteLine($"[ПОМИЛКА] {ex.Message}");
                Console.WriteLine($"Деталі: Запитано = {ex.RequestedAmount:C}, Баланс = {ex.CurrentBalance:C}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[НЕВІДОМА ПОМИЛКА] {ex.Message}");
            }

            Console.WriteLine("\nПрограма завершила роботу.");
        }
    }
}