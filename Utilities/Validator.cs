using System;
using System.Collections.Generic;

/// <summary>
/// Generic validator class for entity validation
/// Demonstrates generic methods with constraints and validation patterns
/// </summary>
public static class Validator<T> where T : IEntity
{
    /// <summary>Validates an entity is not null</summary>
    public static void ValidateNotNull(T entity, string entityName = "Entity")
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity), $"{entityName} cannot be null");
    }

    /// <summary>Validates entity ID is valid (non-zero)</summary>
    public static void ValidateId(T entity)
    {
        if (entity.Id <= 0)
            throw new ArgumentException($"Invalid entity ID: {entity.Id}");
    }

    /// <summary>Validates a list contains valid entities</summary>
    public static void ValidateList(IList<T> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities), "Entity list cannot be null");

        if (entities.Count == 0)
            throw new ArgumentException("Entity list cannot be empty");

        foreach (var entity in entities)
        {
            ValidateNotNull(entity);
            ValidateId(entity);
        }
    }

    /// <summary>Validates all entities pass a predicate</summary>
    public static bool ValidateAll(IEnumerable<T> entities, Func<T, bool> predicate)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        foreach (var entity in entities)
        {
            if (!predicate(entity))
                return false;
        }
        return true;
    }
}

/// <summary>String validation utilities</summary>
public static class StringValidator
{
    public static void ValidateNotEmpty(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} cannot be empty", nameof(value));
    }

    public static void ValidateLength(string? value, int minLength, int maxLength, string fieldName)
    {
        if (value == null || value.Length < minLength || value.Length > maxLength)
            throw new ArgumentException(
                $"{fieldName} must be between {minLength} and {maxLength} characters", nameof(value));
    }

    public static void ValidateEmail(string? email)
    {
        ValidateNotEmpty(email, "Email");
        if (!email.Contains("@"))
            throw new ArgumentException("Invalid email format", nameof(email));
    }
}
