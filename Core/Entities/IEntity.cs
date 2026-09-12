/// <summary>
/// Base interface for all entities to ensure type safety in generic repository
/// </summary>
public interface IEntity
{
    int Id { get; }
}
