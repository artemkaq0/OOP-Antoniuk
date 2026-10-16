using System;
using System.Collections.Generic;

namespace DocumentProcessingLab
{
    abstract class DocumentProcessor
    {
        public string FileName { get; set; }
        public bool IsProcessedSuccessfully { get; protected set; }

        protected DocumentProcessor(string fileName)
        {
            FileName = fileName;
            IsProcessedSuccessfully = false;
        }

        public void Process()
        {
            Console.WriteLine($"\nПочаток обробки документа: {FileName}");
            try
            {
                Open();
                Parse();
                
                if (Validate())
                {
                    Transform();
                    Save();
                    IsProcessedSuccessfully = true;
                }
                else
                {
                    Console.WriteLine("[Помилка]: Валідація не пройдена. Обробку зупинено.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Критична помилка]: {ex.Message}");
            }
            finally
            {
                Close();
                Console.WriteLine($"Завершено. Статус успіху: {IsProcessedSuccessfully}");
            }
        }

        protected abstract void Open();
        protected abstract void Parse();
        protected abstract void Save();

        protected virtual bool Validate()
        {
            Console.WriteLine("[Базова Валідація]: Перевірка наявності файлу та базових прав доступу");
            return true;
        }

        protected virtual void Transform()
        {
            Console.WriteLine("[Базова Трансформація]: Нормалізація формату метаданих");
        }

        protected virtual void Close()
        {
            Console.WriteLine("[Базове Закриття]: Закриття файлових дескрипторів.");
        }
    }

    class PDFProcessor : DocumentProcessor
    {
        public bool IsEncrypted { get; set; }

        public PDFProcessor(string fileName, bool isEncrypted = false) : base(fileName)
        {
            IsEncrypted = isEncrypted;
        }

        protected override void Open()
        {
            Console.WriteLine($"[PDF] Відкриття PDF-файлу '{FileName}'{(IsEncrypted ? " (Зашифровано)" : "")}");
        }

        protected override void Parse()
        {
            Console.WriteLine("[PDF] Вилучення текстових блоків, шрифтів та векторних зображень");
        }

        protected override void Save()
        {
            Console.WriteLine("[PDF] Збереження оптимізованого PDF-документа");
        }

        protected override bool Validate()
        {
            base.Validate();
            Console.WriteLine("[PDF Валідація]: Перевірка цілісності цифрового підпису PDF.");
            return true;
        }
    }

    class WordProcessor : DocumentProcessor
    {
        public int WordCount { get; private set; }

        public WordProcessor(string fileName) : base(fileName) { }

        protected override void Open()
        {
            Console.WriteLine($"[Word] Відкриття DOCX документу '{FileName}' через OpenXML");
        }

        protected override void Parse()
        {
            Console.WriteLine("[Word] Зчитування абзаців, стилів та таблиць");
            Console.WriteLine($"[Word] Підраховано слів: {WordCount}");
        }

        protected override void Transform()
        {
            Console.WriteLine("[Word Трансформація]: Оновлення змісту та форматування заголовків");
        }

        protected override void Save()
        {
            Console.WriteLine("[Word] Збереження зміненого DOCX документа");
        }
    }

    class ExcelProcessor : DocumentProcessor
    {
        public int SheetCount { get; set; }

        public ExcelProcessor(string fileName, int sheetCount = 1) : base(fileName)
        {
            SheetCount = sheetCount;
        }

        protected override void Open()
        {
            Console.WriteLine($"[Excel] Відкриття XLSX книги '{FileName}' (Аркушів: {SheetCount})");
        }

        protected override void Parse()
        {
            Console.WriteLine("[Excel] Парсинг комірок, формул та графіків...");
        }

        protected override void Save()
        {
            Console.WriteLine("[Excel] Перерахунок формул та збереження XLSX файлу");
        }

        protected override void Close()
        {
            Console.WriteLine("[Excel Закриття]: Звільнення оперативної пам'яті Excel-рушія.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<DocumentProcessor> documents = new List<DocumentProcessor>
            {
                new PDFProcessor("annual_report.pdf", isEncrypted: true),
                new WordProcessor("lab11_report.docx"),
                new ExcelProcessor("financial_budget.xlsx", sheetCount: 5)
            };

            Console.WriteLine("Лабораторна Робота 11: Template Method");

            foreach (var doc in documents)
            {
                doc.Process();
            }
        }
    }
}