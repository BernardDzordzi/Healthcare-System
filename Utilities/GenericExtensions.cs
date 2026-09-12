using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Generic utility extensions for collections and entities
/// Demonstrates generic methods with constraints
/// </summary>
public static class GenericExtensions
{
    /// <summary>
    /// Finds an element and applies a transformation
    /// Generic method with type parameter for the result
    /// </summary>
    public static TResult? FindAndTransform<T, TResult>(
        this IEnumerable<T> source,
        Func<T, bool> predicate,
        Func<T, TResult> transform) where TResult : class
    {
        var item = source.FirstOrDefault(predicate);
        return item != null ? transform(item) : null;
    }

    /// <summary>
    /// Groups entities by a key selector
    /// Generic method demonstrating Dictionary creation
    /// </summary>
    public static Dictionary<TKey, List<T>> GroupByKey<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> keySelector) where TKey : notnull
    {
        var dictionary = new Dictionary<TKey, List<T>>();
        foreach (var item in source)
        {
            var key = keySelector(item);
            if (!dictionary.ContainsKey(key))
                dictionary[key] = new List<T>();
            dictionary[key].Add(item);
        }
        return dictionary;
    }

    /// <summary>
    /// Filters entities and returns them as a new collection
    /// Generic method with type constraint for IEntity
    /// </summary>
    public static List<T> FilterAndClone<T>(
        this IEnumerable<T> source,
        Func<T, bool> predicate) where T : IEntity
    {
        return source.Where(predicate).ToList();
    }

    /// <summary>
    /// Batch operations on entities
    /// Generic method for processing entities in batches
    /// </summary>
    public static void ForEachInBatches<T>(
        this IEnumerable<T> source,
        int batchSize,
        Action<List<T>> processBatch) where T : class
    {
        var batch = new List<T>(batchSize);
        foreach (var item in source)
        {
            batch.Add(item);
            if (batch.Count >= batchSize)
            {
                processBatch(batch);
                batch.Clear();
            }
        }
        if (batch.Count > 0)
            processBatch(batch);
    }

    /// <summary>
    /// Safely executes an operation on entities with error handling
    /// Generic method demonstrating result handling
    /// </summary>
    public static OperationResult<T> SafeExecute<T>(
        this T entity,
        Action<T> operation) where T : IEntity
    {
        try
        {
            operation(entity);
            return new OperationResult<T>(true, "Operation completed successfully", entity);
        }
        catch (Exception ex)
        {
            return new OperationResult<T>(false, ex.Message, entity);
        }
    }

    /// <summary>
    /// Distinct by a specific key selector
    /// Generic method for unique filtering
    /// </summary>
    public static IEnumerable<T> DistinctBy<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> keySelector) where TKey : notnull
    {
        var seenKeys = new HashSet<TKey>();
        foreach (var item in source)
        {
            if (seenKeys.Add(keySelector(item)))
                yield return item;
        }
    }

    /// <summary>
    /// Converts entities to a dictionary
    /// Generic method with two type parameters
    /// </summary>
    public static Dictionary<TKey, TValue> ToDictionaryGeneric<T, TKey, TValue>(
        this IEnumerable<T> source,
        Func<T, TKey> keySelector,
        Func<T, TValue> valueSelector) where TKey : notnull
    {
        var dictionary = new Dictionary<TKey, TValue>();
        foreach (var item in source)
        {
            dictionary[keySelector(item)] = valueSelector(item);
        }
        return dictionary;
    }
}

/// <summary>
/// Generic result wrapper for operations
/// Demonstrates generic class with constraint
/// </summary>
public class OperationResult<T> where T : IEntity
{
    public bool IsSuccess { get; }
    public string Message { get; }
    public T Entity { get; }

    public OperationResult(bool isSuccess, string message, T entity)
    {
        IsSuccess = isSuccess;
        Message = message;
        Entity = entity;
    }

    public override string ToString()
    {
        return $"Result[Success: {IsSuccess}, Message: {Message}, EntityID: {Entity?.Id}]";
    }
}
