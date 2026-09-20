using System;

namespace OOPLab2
{
    public class Book
    {
        private string _title;
        private string _author;
        private int _year;

        public string Title
        {
            get => _title;
            set => _title = value;
        }

        public string Author
        {
            get => _author;
            set => _author = value;
        }

        public int Year
        {
            get => _year;
            set
            {
                if (value > DateTime.Now.Year)
                {
                    throw new ArgumentException("Рік видання не може бути в майбутньому!");
                }
                _year = value;
            }
        }

        public Book(string title, string author, int year)
        {
            Title = title;
            Author = author;
            Year = year;
        }

        public Book() : this("Unknown", "Unknown", DateTime.Now.Year)
        {
        }
        public string GetFullInfo()
        {
            return $"Книга: \"{Title}\" | Автор: {Author} | Рік: {Year}";
        }

        ~Book()
        {
            Console.WriteLine($"[Деструктор]: Об'єкт Book \"{Title}\" вилучено з пам'яті.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            CreateAndUseObjects();

            Console.WriteLine("End of Main, preparing for GC");
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        static void CreateAndUseObjects()
        {
            Console.WriteLine(" Creating objects ");

            Book book1 = new Book();
            Console.WriteLine(book1.GetFullInfo());

            Book book2 = new Book("Кобзар", "Тарас Шевченко", 1840);
            Console.WriteLine(book2.GetFullInfo());

            Book book3 = new Book("Тіні забутих предків", "Михайло Коцюбинський", 1911);
            Console.WriteLine(book3.GetFullInfo());

            Console.WriteLine(" Objects created ");
        }
    }
}