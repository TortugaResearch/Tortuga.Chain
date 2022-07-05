namespace Tortuga.Chain.DataSources;

/// <summary>
/// Used to mark data sources that support additional CRUD operations.
/// </summary>
/// <remarks>Warning: This interface is meant to simulate multiple inheritance and work-around some issues with exposing generic types. Do not implement it in client code, as new methods will be added over time.</remarks>
public interface IAdvancedCrudDataSource : ICrudDataSource, ISupportsTruncate, ISupportsUpsert
{
}
