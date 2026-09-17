using System;

namespace lab1v1
{
    class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }

        public Book(string title, string author, int year)
        {
            Title = title;
            Author = author;
            Year = year;
        }

        public string GetInfo()
        {
            return $"Книга: \"{Title}\", Автор: {Author}, Рік видання: {Year}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Book book1 = new Book("1984", "Джордж Орвелл", 1949);
            Book book2 = new Book("Кобзар", "Тарас Шевченко", 1840);
            Book book3 = new Book("Гаррі Поттер і філософський камінь", "Дж. К. Роулінг", 1997);

            Console.WriteLine(book1.GetInfo());
            Console.WriteLine(book2.GetInfo());
            Console.WriteLine(book3.GetInfo());
        }
    }
}