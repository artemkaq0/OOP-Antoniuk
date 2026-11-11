using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace robota11
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public override string ToString() => $"[{Id}] {Title}";
    }

    public static class TextProcessor
    {
        public static IEnumerable<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return Enumerable.Empty<string>();
            return Regex.Matches(text.ToLowerInvariant(), @"\w+").Select(m => m.Value);
        }

        public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
        {
            var stopWords = new HashSet<string>
            {
                "a", "an", "the", "is", "are", "was", "were",
                "in", "on", "at", "to", "for", "of", "and", "or", "with", "by"
            };
            return words.Where(w => !stopWords.Contains(w));
        }
    }

    public class InvertedIndex
    {
        private readonly Dictionary<string, HashSet<int>> _index;
        private readonly Dictionary<int, Document> _documents;

        public InvertedIndex()
        {
            _index = new Dictionary<string, HashSet<int>>();
            _documents = new Dictionary<int, Document>();
        }

        public void AddDocument(Document doc)
        {
            if (!_documents.ContainsKey(doc.Id))
            {
                _documents[doc.Id] = doc;
            }

            var words = TextProcessor.Tokenize(doc.Content);
            words = TextProcessor.RemoveStopWords(words);

            foreach (var word in words)
            {
                if (!_index.ContainsKey(word))
                {
                    _index[word] = new HashSet<int>();
                }
                _index[word].Add(doc.Id);
            }
        }

        public List<Document> SearchSingleWord(string word)
        {
            var normalized = word.ToLowerInvariant();
            if (_index.TryGetValue(normalized, out var docIds))
            {
                return docIds.Select(id => _documents[id]).ToList();
            }
            return new List<Document>();
        }

        public List<Document> SearchMultipleWords(params string[] words)
        {
            if (words == null || words.Length == 0) return new List<Document>();

            var normalizedWords = words.Select(w => w.ToLowerInvariant()).ToList();
            
            if (!_index.TryGetValue(normalizedWords[0], out var initialSet))
                return new List<Document>();

            var resultSet = new HashSet<int>(initialSet);

            for (int i = 1; i < normalizedWords.Count; i++)
            {
                if (_index.TryGetValue(normalizedWords[i], out var nextSet))
                {
                    resultSet.IntersectWith(nextSet);
                }
                else
                {
                    return new List<Document>();
                }
            }

            return resultSet.Select(id => _documents[id]).ToList();
        }

        public List<Document> SearchAnyWord(params string[] words)
        {
            if (words == null || words.Length == 0) return new List<Document>();

            var resultSet = new HashSet<int>();

            foreach (var word in words)
            {
                var normalized = word.ToLowerInvariant();
                if (_index.TryGetValue(normalized, out var set))
                {
                    resultSet.UnionWith(set);
                }
            }

            return resultSet.Select(id => _documents[id]).ToList();
        }

        public int DocumentCount => _documents.Count;
        public int UniqueWords => _index.Keys.Count;
    }

    public class PerformanceTest
    {
        public static void CompareSearchMethods(List<Document> documents, string searchWord)
        {
            Console.WriteLine($"\nТест для {documents.Count} документів");
            var sw = new Stopwatch();
            var normalizedWord = searchWord.ToLowerInvariant();

            sw.Start();
            var linearResults = documents.Where(d => 
                TextProcessor.Tokenize(d.Content).Contains(normalizedWord)).ToList();
            sw.Stop();
            long linearTicks = sw.ElapsedTicks;

            var index = new InvertedIndex();
            foreach (var doc in documents) index.AddDocument(doc);
            
            sw.Restart();
            var indexResults = index.SearchSingleWord(searchWord);
            sw.Stop();
            long indexTicks = sw.ElapsedTicks;

            Console.WriteLine($"Linear search: {linearTicks} ticks");
            Console.WriteLine($"Inverted Index search: {indexTicks} ticks");
            Console.WriteLine($"Знайдено: Лінійний={linearResults.Count}, Індекс={indexResults.Count}");
        }

        public static List<Document> GenerateDocuments(int count)
        {
            var baseDocs = Program.GetBaseDocuments();
            var result = new List<Document>(count);
            for (int i = 0; i < count; i++)
            {
                var template = baseDocs[i % baseDocs.Count];
                result.Add(new Document
                {
                    Id = i + 1,
                    Title = template.Title + $" Part {i}",
                    Content = template.Content
                });
            }
            return result;
        }
    }

    class Program
    {
        public static List<Document> GetBaseDocuments()
        {
            return new List<Document>
            {
                new Document { Id = 1, Title = "Introduction to C#", Content = "C# is a modern object-oriented programming language developed by Microsoft." },
                new Document { Id = 2, Title = "Java Basics", Content = "Java is a popular programming language known for its portability and performance." },
                new Document { Id = 3, Title = "Python Overview", Content = "Python is a high-level programming language with simple syntax." },
                new Document { Id = 4, Title = "C# Advanced Features", Content = "C# supports LINQ, async/await, and many modern programming paradigms." },
                new Document { Id = 5, Title = "Comparing Languages", Content = "C#, Java, and Python are among the most popular programming languages today." }
            };
        }

        static void Main(string[] args)
        {
            Console.WriteLine("ВИМІРЮВАННЯ ПРОДУКТИВНОСТІ:");
            int[] docCounts = { 10, 100, 1000, 10000 };
            foreach (var count in docCounts)
            {
                var docs = PerformanceTest.GenerateDocuments(count);
                PerformanceTest.CompareSearchMethods(docs, "programming");
            }

            Console.WriteLine("\nІНТЕРАКТИВНА ДЕМОНСТРАЦІЯ (5 базових документів):");
            var index = new InvertedIndex();
            foreach (var doc in GetBaseDocuments()) index.AddDocument(doc);
            
            Console.WriteLine($"Loaded {index.DocumentCount} documents");
            Console.WriteLine($"Index contains {index.UniqueWords} unique words\n");

            while (true)
            {
                Console.Write("Enter search query (or 'exit'): ");
                var query = Console.ReadLine();
                if (query == "exit" || string.IsNullOrWhiteSpace(query)) break;

                var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                
                Console.WriteLine("\nSearch Results (AND)");
                var results = index.SearchMultipleWords(words);
                
                foreach (var doc in results)
                {
                    Console.WriteLine(doc);
                    Console.WriteLine($"  {doc.Content.Substring(0, Math.Min(100, doc.Content.Length))}...");
                }
                Console.WriteLine($"\nFound {results.Count} documents\n");
            }
        }
    }
}