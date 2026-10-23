namespace lab12v1;

public class InMemoryRepository<T> : IRepository<T> where T : IEntity
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        if (_items.Any(i => i.Id == item.Id))
            throw new ArgumentException($"Елемент з ID {item.Id} вже існує.");
        
        _items.Add(item);
    }

    public T GetById(int id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item == null) throw new KeyNotFoundException($"Елемент з ID {id} не знайдено.");
        return item;
    }

    public IEnumerable<T> GetAll() => _items;

    public void Update(T item)
    {
        var existingItem = GetById(item.Id);
        var index = _items.IndexOf(existingItem);
        _items[index] = item;
    }

    public void Delete(int id)
    {
        var item = GetById(id);
        _items.Remove(item);
    }
}