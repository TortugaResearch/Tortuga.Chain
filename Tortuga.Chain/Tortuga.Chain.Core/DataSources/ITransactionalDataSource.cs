namespace Tortuga.Chain.DataSources;

/// <summary>
/// This is data source that is wrapped around a managed connection.
/// </summary>
/// <remarks>
/// This interface is primarily for testing purposes.
/// </remarks>
public interface ITransactionalDataSource : IOpenDataSource, IDisposable
#if NET6_0_OR_GREATER
, IAsyncDisposable
#endif

{
	/// <summary>
	/// Commits this transaction.
	/// </summary>
	void Commit();
}