using System;

namespace OOPLab3
{
    public class FileLogger : IDisposable
    {
        private bool _disposed = false;
        private readonly string _filePath;
        private bool _isFileOpen;

        public string FilePath => _filePath;
        public bool IsFileOpen => _isFileOpen;

        public FileLogger(string filePath)
        {
            _filePath = filePath;
            _isFileOpen = true;
            Console.WriteLine($"[Файл відкрито]: {_filePath}");
        }

        public void Log(string message)
        {
            if (_disposed || !_isFileOpen)
            {
                throw new InvalidOperationException("Помилка: файл закрито або ресурс вже звільнено.");
            }

            Console.WriteLine($"[Запис у {_filePath}]: {message}");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Console.WriteLine($"[Dispose(true)]: Очищення керованих ресурсів для {_filePath}");
                }

                if (_isFileOpen)
                {
                    Console.WriteLine($"[Dispose]: Закриття файлу {_filePath}");
                    _isFileOpen = false;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FileLogger()
        {
            Console.WriteLine($"[Деструктор]: Автоматичне вилучення через GC для {_filePath}");
            Dispose(false);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Сценарій 1: Використання using ===");
            using (var logger1 = new FileLogger("app_using.log"))
            {
                logger1.Log("Запис через блочний using");

            Console.WriteLine();

            Console.WriteLine("=== Сценарій 2: Явний виклик Dispose() ===");
            var logger2 = new FileLogger("app_explicit.log");
            logger2.Log("Запис перед ручним закриттям");
            logger2.Dispose();

            Console.WriteLine();

            Console.WriteLine("=== Сценарій 3: Автоматична фіналізація (GC) ===");
            CreateUnreleasedLogger();

            Console.WriteLine("Запуск збирача сміття...");
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\nПрограму завершено.");
        }

        static void CreateUnreleasedLogger()
        {
            var logger3 = new FileLogger("app_gc.log");
            logger3.Log("Запис об'єктом без Dispose");
        }
    }
    }
}