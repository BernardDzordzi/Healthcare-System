using System;
using System.Collections.Generic;

/// <summary>
/// Generic repository interface for CRUD operations
/// Provides abstraction for data access layer
/// </summary>
/// <typeparam name="T">Entity type, must implement IEntity</typeparam>
public interface IRepository<T> where T : IEntity
{
    /// <summary>Adds a new entity to the repository</summary>
    void Add(T item);

    /// <summary>Updates an existing entity</summary>
    bool Update(T item);

    /// <summary>Removes an entity by predicate</summary>
    bool Remove(Func<T, bool> predicate);

    /// <summary>Gets all entities</summary>
    IReadOnlyList<T> GetAll();

    /// <summary>Gets an entity by ID</summary>
    T? GetById(int id);

    /// <summary>Gets entities matching a predicate</summary>
    IReadOnlyList<T> GetWhere(Func<T, bool> predicate);

    /// <summary>Checks if an entity exists</summary>
    bool Exists(Func<T, bool> predicate);

    /// <summary>Gets the total count of entities</summary>
    int Count();
}
