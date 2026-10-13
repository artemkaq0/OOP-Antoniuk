using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Lab10V1
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public override string ToString() => $"[ID: {Id}] Ім'я: {Name,-15} | Email: {Email}";
    }

    public interface IRepository<T>
    {
        T GetById(int id);
        void Add(T item);
        void Update(T item);
        void Delete(int id);
        List<T> GetAll();
    }

    public class InMemoryUserRepository : IRepository<User>
    {
        private readonly List<User> _users = new List<User>();

        public void Add(User item)
        {
            _users.Add(item);
            Console.WriteLine($"[InMemoryRepo] Додано користувача: {item.Name}");
        }

        public User GetById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public void Update(User item)
        {
            var existing = GetById(item.Id);
            if (existing != null)
            {
                existing.Name = item.Name;
                existing.Email = item.Email;
                Console.WriteLine($"[InMemoryRepo] Оновлено дані користувача ID {item.Id}");
            }
        }

        public void Delete(int id)
        {
            var user = GetById(id);
            if (user != null)
            {
                _users.Remove(user);
                Console.WriteLine($"[InMemoryRepo] Видалено користувача ID {id}");
            }
        }

        public List<User> GetAll() => _users;
    }

    public class FileUserRepository : IRepository<User>
    {
        private readonly string _filePath = "users.json";

        private List<User> LoadFromFile()
        {
            if (!File.Exists(_filePath)) return new List<User>();
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        private void SaveToFile(List<User> users)
        {
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public void Add(User item)
        {
            var users = LoadFromFile();
            users.Add(item);
            SaveToFile(users);
            Console.WriteLine($"[FileRepo] Записано у файл {_filePath}: {item.Name}");
        }

        public User GetById(int id)
        {
            return LoadFromFile().FirstOrDefault(u => u.Id == id);
        }

        public void Update(User item)
        {
            var users = LoadFromFile();
            var index = users.FindIndex(u => u.Id == item.Id);
            if (index != -1)
            {
                users[index] = item;
                SaveToFile(users);
                Console.WriteLine($"[FileRepo] Оновлено у файлі ID {item.Id}");
            }
        }

        public void Delete(int id)
        {
            var users = LoadFromFile();
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                users.Remove(user);
                SaveToFile(users);
                Console.WriteLine($"[FileRepo] Видалено з файлу ID {id}");
            }
        }

        public List<User> GetAll() => LoadFromFile();
    }

    public abstract class DocumentProcessor
    {
        protected string LoadedContent { get; set; } = string.Empty;
        protected string ProcessedContent { get; set; } = string.Empty;

        public abstract void LoadDocument(string path);
        public abstract void ProcessContent();

        public void SaveDocument(string path)
        {
            File.WriteAllText(path, ProcessedContent);
            Console.WriteLine($"[DocumentProcessor] Збережено результат у файл: {path}");
        }
    }

    public class TextDocumentProcessor : DocumentProcessor
    {
        public override void LoadDocument(string path)
        {
            if (File.Exists(path))
            {
                LoadedContent = File.ReadAllText(path);
                Console.WriteLine($"[TextProcessor] Успішно завантажено TXT з '{path}'");
            }
            else
            {
                LoadedContent = "Прикладовий текстовий вміст для обробки.";
                Console.WriteLine($"[TextProcessor] Файл не знайдено. Згенеровано демо-вміст.");
            }
        }

        public override void ProcessContent()
        {
            ProcessedContent = $"ТЕКСТОВИЙ ДОКУМЕНТ (ОБРОБЛЕНО)\n{LoadedContent.ToUpper()}";
            Console.WriteLine("[TextProcessor] Вміст переведено у верхній регістр.");
        }
    }

    public class XmlDocumentProcessor : DocumentProcessor
    {
        public override void LoadDocument(string path)
        {
            if (File.Exists(path))
            {
                LoadedContent = File.ReadAllText(path);
                Console.WriteLine($"[XmlProcessor] Успішно завантажено XML з '{path}'");
            }
            else
            {
                LoadedContent = "Данні документу в форматі XML";
                Console.WriteLine($"[XmlProcessor] Файл не знайдено. Згенеровано демо-вміст.");
            }
        }

        public override void ProcessContent()
        {
            ProcessedContent = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<document>\n  <content>{LoadedContent}</content>\n  <status>Processed</status>\n</document>";
            Console.WriteLine("[XmlProcessor] Вміст обгорнуто у структуру XML.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("    ЧАСТИНА 1: ПОЛІМОРФІЗМ ЧЕРЕЗ ІНТЕРФЕЙСИ      ");
            List<IRepository<User>> repositories = new List<IRepository<User>>
            {
                new InMemoryUserRepository(),
                new FileUserRepository()
            };

            User user1 = new User { Id = 1, Name = "Іван Петренко", Email = "ivan@example.com" };
            User user2 = new User { Id = 2, Name = "Олена Коваль", Email = "olena@example.com" };

            foreach (var repo in repositories)
            {
                Console.WriteLine($"-> Робота з репозиторієм: {repo.GetType().Name}");
                repo.Add(user1);
                repo.Add(user2);

                var found = repo.GetById(1);
                Console.WriteLine($"     Отримано по ID 1: {found?.Name}");

                user1.Email = "updated_ivan@example.com";
                repo.Update(user1);

                Console.WriteLine("     Усі користувачі в репозиторії:");
                foreach (var u in repo.GetAll())
                {
                    Console.WriteLine($"       {u}");
                }
                Console.WriteLine(new string('-', 50));
            }

            Console.WriteLine("  ЧАСТИНА 2: ПОЛІМОРФІЗМ ЧЕРЕЗ АБСТРАКТНИЙ КЛАС  ");

            List<DocumentProcessor> processors = new List<DocumentProcessor>
            {
                new TextDocumentProcessor(),
                new XmlDocumentProcessor()
            };

            int docIndex = 1;
            foreach (var processor in processors)
            {
                Console.WriteLine($"---> Обробка за допомогою: {processor.GetType().Name}");

                processor.LoadDocument($"input_{docIndex}.txt");
                processor.ProcessContent();
                processor.SaveDocument($"output_{docIndex}.txt");

                Console.WriteLine(new string('-', 50));
                docIndex++;
            }

            Console.WriteLine("\nНатисніть Enter для завершення");
            Console.ReadLine();
        }
    }
}