using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Generic repository implementation using List&lt;T&gt;
/// Supports CRUD operations with type safety through generic constraints
/// </summary>
/// <typeparam name="T">Entity type, must implement IEntity</typeparam>
public class Repository<T> : IRepository<T> where T : IEntity
{
    private readonly List<T> _items = new List<T>();
    private readonly object _lockObject = new object();

    /// <summary>Adds a new entity to the repository</summary>
    public void Add(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item), "Item cannot be null");

        lock (_lockObject)
        {
            if (_items.Any(x => x.Id == item.Id))
                throw new InvalidOperationException($"Entity with ID {item.Id} already exists");

            _items.Add(item);
        }
    }

    /// <summary>Updates an existing entity</summary>
    public bool Update(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item), "Item cannot be null");

        lock (_lockObject)
        {
            var index = _items.FindIndex(x => x.Id == item.Id);
            if (index >= 0)
            {
                _items[index] = item;
                return true;
            }
        }
        return false;
    }

    /// <summary>Removes an entity by predicate</summary>
    public bool Remove(Func<T, bool> predicate)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate), "Predicate cannot be null");

        lock (_lockObject)
        {
            var item = _items.FirstOrDefault(predicate);
            if (item != null)
            {
                return _items.Remove(item);
            }
        }
        return false;
    }

    /// <summary>Gets all entities</summary>
    public IReadOnlyList<T> GetAll()
    {
        lock (_lockObject)
        {
            return _items.AsReadOnly();
        }
    }

    /// <summary>Gets an entity by ID</summary>
    public T? GetById(int id)
    {
        lock (_lockObject)
        {
            return _items.FirstOrDefault(x => x.Id == id);
        }
    }

    /// <summary>Gets entities matching a predicate</summary>
    public IReadOnlyList<T> GetWhere(Func<T, bool> predicate)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate), "Predicate cannot be null");

        lock (_lockObject)
        {
            return _items.Where(predicate).ToList().AsReadOnly();
        }
    }

    /// <summary>Checks if an entity exists</summary>
    public bool Exists(Func<T, bool> predicate)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate), "Predicate cannot be null");

        lock (_lockObject)
        {
            return _items.Any(predicate);
        }
    }

    /// <summary>Gets the total count of entities</summary>
    public int Count()
    {
        lock (_lockObject)
        {
            return _items.Count;
        }
    }

    /// <summary>Clears all entities (for testing purposes)</summary>
    public void Clear()
    {
        lock (_lockObject)
        {
            _items.Clear();
        }
    }
}
