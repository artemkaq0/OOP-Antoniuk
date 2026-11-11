using System;
using System.Collections.Generic;
using System.Linq;

namespace IndependentWork9
{
    public class FileMetadata
    {
        public string FileName { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; }
        
        public override string ToString() => $"{FileName} ({Size} bytes)";
    }

    public class FileMetadataCache
    {
        private readonly Dictionary<string, FileMetadata> _cache = new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);

        public void Add(string filePath, FileMetadata metadata)
        {
            _cache[filePath] = metadata;
        }

        public FileMetadata Get(string filePath)
        {
            if (_cache.TryGetValue(filePath, out var metadata))
                return metadata;
            throw new KeyNotFoundException($"Файл {filePath} не знайдено в кеші.");
        }

        public bool Remove(string filePath) => _cache.Remove(filePath);

        public bool Contains(string filePath) => _cache.ContainsKey(filePath);
    }

    public class TagManager
    {
        private readonly Dictionary<string, HashSet<string>> _articleTags = new Dictionary<string, HashSet<string>>();

        public void AddTag(string articleId, string tag)
        {
            if (!_articleTags.ContainsKey(articleId))
                _articleTags[articleId] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            
            _articleTags[articleId].Add(tag);
        }

        public void RemoveTag(string articleId, string tag)
        {
            if (_articleTags.ContainsKey(articleId))
                _articleTags[articleId].Remove(tag);
        }

        public HashSet<string> GetAllUniqueTags()
        {
            var allTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var tags in _articleTags.Values)
            {
                allTags.UnionWith(tags);
            }
            return allTags;
        }

        public HashSet<string> FindCommonTags(string articleId1, string articleId2)
        {
            if (!_articleTags.ContainsKey(articleId1) || !_articleTags.ContainsKey(articleId2))
                return new HashSet<string>();

            var commonTags = new HashSet<string>(_articleTags[articleId1], StringComparer.OrdinalIgnoreCase);
            commonTags.IntersectWith(_articleTags[articleId2]);
            return commonTags;
        }
    }

    public class SearchIndex
    {
        private readonly Dictionary<string, HashSet<int>> _invertedIndex = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

        public void AddDocument(int docId, string content)
        {
            var words = content.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var word in words)
            {
                if (!_invertedIndex.ContainsKey(word))
                    _invertedIndex[word] = new HashSet<int>();
                
                _invertedIndex[word].Add(docId);
            }
        }

        public HashSet<int> Search(string word)
        {
            return _invertedIndex.TryGetValue(word, out var docs) ? docs : new HashSet<int>();
        }

        public HashSet<int> SearchMultiple(params string[] words)
        {
            if (words.Length == 0) return new HashSet<int>();

            var result = new HashSet<int>(Search(words[0]));

            for (int i = 1; i < words.Length; i++)
            {
                result.IntersectWith(Search(words[i]));
            }

            return result;
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("Демонстрація FileMetadataCache");
            var cache = new FileMetadataCache();
            cache.Add("C:\\file.txt", new FileMetadata { FileName = "file.txt", Size = 1024 });
            Console.WriteLine($"Чи є в кеші C:\\file.txt? {cache.Contains("C:\\file.txt")}");
            
            Console.WriteLine("\nДемонстрація TagManager");
            var tagManager = new TagManager();
            tagManager.AddTag("doc1", "C#");
            tagManager.AddTag("doc1", ".NET");
            tagManager.AddTag("doc2", "C#");
            tagManager.AddTag("doc2", "OOP");
            
            var common = tagManager.FindCommonTags("doc1", "doc2");
            Console.WriteLine($"Спільні теги doc1 та doc2: {string.Join(", ", common)}");
            Console.WriteLine($"Всі унікальні теги: {string.Join(", ", tagManager.GetAllUniqueTags())}");

            Console.WriteLine("\nДемонстрація SearchIndex");
            var index = new SearchIndex();
            index.AddDocument(1, "C# is an OOP language");
            index.AddDocument(2, "Java is also an OOP language");
            index.AddDocument(3, "C# supports LINQ");

            var csharpDocs = index.Search("C#");
            Console.WriteLine($"Документи зі словом 'C#': {string.Join(", ", csharpDocs)}");
            
            var multiSearchDocs = index.SearchMultiple("OOP", "language");
            Console.WriteLine($"Документи зі словами 'OOP' ТА 'language': {string.Join(", ", multiSearchDocs)}");
        }
    }
}