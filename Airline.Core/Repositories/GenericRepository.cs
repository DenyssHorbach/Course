using Airline.Core.Interfaces;

namespace Airline.Core.Repositories;

public class GenericRepository<T> : IRepository<T> where T : class
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
    }

    public bool Remove(T item) => _items.Remove(item);

    public List<T> GetAll() => new(_items);

    public List<T> Find(Predicate<T> match) => _items.FindAll(match);

    public void ReplaceAll(List<T> items)
    {
        _items.Clear();
        _items.AddRange(items);
    }
}