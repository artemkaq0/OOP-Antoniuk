using System;
using System.Collections.Generic;
using System.Linq;

namespace lab16v1
{
    public class Chapter
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PageCount { get; set; }
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Chapter> Chapters { get; set; } = new List<Chapter>();
    }

    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();
    }

    public static class ChapterExtensions
    {
        public static int TotalPages(this IEnumerable<Chapter> chapters)
        {
            return chapters.Sum(c => c.PageCount);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var authors = new List<Author>
            {
                new Author { Id = 1, Name = "Тарас Шевченко", Books = new List<Book>
                {
                    new Book { Id = 1, Title = "Кобзар (Видання 1)", Chapters = new List<Chapter> {
                        new Chapter { Id = 1, Title = "Причинна", PageCount = 5 },
                        new Chapter { Id = 2, Title = "Катерина", PageCount = 15 }
                    }},
                    new Book { Id = 2, Title = "Гайдамаки", Chapters = new List<Chapter> {
                        new Chapter { Id = 3, Title = "Вступ", PageCount = 4 },
                        new Chapter { Id = 4, Title = "Галайда", PageCount = 12 }
                    }}
                }},
                new Author { Id = 2, Name = "Іван Франко", Books = new List<Book>
                {
                    new Book { Id = 3, Title = "Зів'яле листя", Chapters = new List<Chapter> {
                        new Chapter { Id = 5, Title = "Перший жмуток", PageCount = 20 },
                        new Chapter { Id = 6, Title = "Другий жмуток", PageCount = 22 }
                    }},
                    new Book { Id = 4, Title = "Мойсей", Chapters = new List<Chapter> {
                        new Chapter { Id = 7, Title = "Пролог", PageCount = 8 },
                        new Chapter { Id = 8, Title = "Пісня 1", PageCount = 10 }
                    }},
                    new Book { Id = 5, Title = "Борислав сміється", Chapters = new List<Chapter> {
                        new Chapter { Id = 9, Title = "Розділ 1", PageCount = 30 }
                    }}
                }},
                 new Author { Id = 3, Name = "Леся Українка", Books = new List<Book>
                {
                    new Book { Id = 6, Title = "Лісова пісня", Chapters = new List<Chapter> {
                        new Chapter { Id = 10, Title = "Дія I", PageCount = 25 },
                        new Chapter { Id = 11, Title = "Дія II", PageCount = 28 }
                    }},
                    new Book { Id = 7, Title = "Бояриня", Chapters = new List<Chapter> {
                        new Chapter { Id = 12, Title = "Частина 1", PageCount = 18 }
                    }}
                }}
            };


            Console.WriteLine("1. Демонстрація SelectMany");
            var allChapters = authors
                .SelectMany(a => a.Books)
                .SelectMany(b => b.Chapters)
                .ToList();

            foreach (var chapter in allChapters)
            {
                Console.WriteLine($"Розділ: {chapter.Title} ({chapter.PageCount} стор.)");
            }


            Console.WriteLine("\n2. Демонстрація Пагінації (по 3 книги на сторінку)");
            var allBooks = authors.SelectMany(a => a.Books).ToList();
            int pageSize = 3;
            int totalPagesCount = (int)Math.Ceiling(allBooks.Count / (double)pageSize);

            for (int pageNumber = 1; pageNumber <= totalPagesCount; pageNumber++)
            {
                Console.WriteLine($"\nСторінка {pageNumber}:");
                
                var booksOnPage = allBooks
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);

                foreach (var book in booksOnPage)
                {
                    Console.WriteLine($"- Книга: {book.Title}");
                }
            }


            Console.WriteLine("\n3. Демонстрація Custom Extension Method");
            var frankoBooks = authors.First(a => a.Name == "Іван Франко").Books;
            var frankoChapters = frankoBooks.SelectMany(b => b.Chapters);
            
            int totalFrankoPages = frankoChapters.TotalPages();
            
            Console.WriteLine($"Загальна кількість сторінок у всіх розділах Івана Франка: {totalFrankoPages}");
        }
    }
}