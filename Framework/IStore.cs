public interface IStore
{
    Task StoreItem<T, TId>(TId id, T item) where T : class where TId : notnull;
    Task<T?> GetItem<T, TId>(TId id) where T : class where TId : notnull;
    Task<IList<T>> GetItems<T>() where T : class;
    Task RemoveItem<TId>(TId id) where TId : notnull;
}

public class InMemoryStore : IStore
{
    Dictionary<object, object> _items = new();

    public async Task StoreItem<T, TId>(TId id, T item)
        where T : class
        where TId : notnull
    {
        _items[id] = item;
        await Task.CompletedTask;
    }

    public async Task<T?> GetItem<T, TId>(TId id) 
        where T : class
        where TId : notnull
    {
        var value = _items
            .Where(kv => kv.Key is TId)
            .Where(kv => ((TId)kv.Key).Equals(id))
            .Select(kv => kv.Value)
            .FirstOrDefault();

        return await Task.FromResult(value as T);
    }

    public async Task<IList<T>> GetItems<T>() where T : class
    {
        var items = _items
            .Where(kv => kv.Value is T)
            .Select(kv => kv.Value as T)
            .Where(i => i is not null)
            .ToList();

        return await Task.FromResult(items);
    }

    public Task RemoveItem<TId>(TId id)
        where TId : notnull
    {
        _items.Remove(id);
        return Task.CompletedTask;
    }
}