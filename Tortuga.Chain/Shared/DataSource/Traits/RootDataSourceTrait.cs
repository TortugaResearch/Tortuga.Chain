using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
class RootDataSourceTrait<TTransactionalDataSource, TOpenDataSource, TConnection, TTransaction> : IRootDataSource
	where TTransactionalDataSource : ITransactionalDataSource
	where TOpenDataSource : IOpenDataSource
	where TConnection : DbConnection
	where TTransaction : DbTransaction
{
	IOpenDataSource IRootDataSource.CreateOpenDataSource() => CreateOpenDataSource();

	IOpenDataSource IRootDataSource.CreateOpenDataSource(DbConnection connection, DbTransaction? transaction)
	{
		return CreateOpenDataSource((TConnection)connection, (TTransaction?)transaction);
	}

	IOpenDataSource IRootDataSource.CreateOpenDataSource(IDbConnection connection, IDbTransaction? transaction)
	{
		return CreateOpenDataSource((TConnection)connection, (TTransaction?)transaction);
	}

	ITransactionalDataSource IRootDataSource.BeginTransaction()
	{
		return BeginTransaction();
	}

	async Task<ITransactionalDataSource> IRootDataSource.BeginTransactionAsync()
	{
		return await BeginTransactionAsync().ConfigureAwait(false);
	}

	[SuppressMessage("Design", "CA1033")]
	DbConnection IRootDataSource.CreateConnection() => OnCreateConnection();

	[SuppressMessage("Design", "CA1033")]
	async Task<DbConnection> IRootDataSource.CreateConnectionAsync() => await OnCreateConnectionAsync(default).ConfigureAwait(false);


	/// <summary>
	/// Creates an open data source using the supplied connection and optional transaction.
	/// </summary>
	/// <param name="connection">The connection to wrap.</param>
	/// <param name="transaction">The transaction to wrap.</param>
	/// <returns>IOpenDataSource.</returns>
	/// <remarks>WARNING: The caller of this method is responsible for closing the connection.</remarks>
	[Expose]
	public TOpenDataSource CreateOpenDataSource(TConnection connection, TTransaction? transaction = null)
	{
		return OnCreateOpenDataSource(connection, transaction);
	}

	/// <summary>
	/// Creates an open data source with a new connection.
	/// </summary>
	/// <remarks>WARNING: The caller of this method is responsible for closing the connection.</remarks>
	[Expose]
	public TOpenDataSource CreateOpenDataSource()
	{
		return OnCreateOpenDataSource(OnCreateConnection(), null);
	}

	/// <summary>
	/// Creates and opens a new Access connection
	/// </summary>
	/// <returns></returns>
	/// <remarks>The caller of this method is responsible for closing the connection.</remarks>
	[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
	[Expose]
	public TConnection CreateConnection() => OnCreateConnection();

	/// <summary>
	/// Creates the connection asynchronous.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns></returns>
	/// <remarks>
	/// The caller of this method is responsible for closing the connection.
	/// </remarks>
	[Expose]
	public Task<TConnection> CreateConnectionAsync(CancellationToken cancellationToken = default(CancellationToken))
		=> OnCreateConnectionAsync(cancellationToken);



	/// <summary>
	/// Creates a new transaction.
	/// </summary>
	/// <returns></returns>
	/// <remarks>The caller of this method is responsible for closing the transaction.</remarks>
	[Expose]
	public TTransactionalDataSource BeginTransaction() => OnBeginTransaction(null, true);


	/// <summary>
	/// Creates a new transaction.
	/// </summary>
	/// <param name="isolationLevel"></param>
	/// <param name="forwardEvents"></param>
	/// <returns></returns>
	/// <remarks>The caller of this method is responsible for closing the transaction.</remarks>
	[Expose]
	public TTransactionalDataSource BeginTransaction(IsolationLevel? isolationLevel = null, bool forwardEvents = true) => OnBeginTransaction(isolationLevel, forwardEvents);

	/// <summary>
	/// Creates a new transaction.
	/// </summary>
	/// <param name="isolationLevel"></param>
	/// <param name="forwardEvents"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	/// <remarks>The caller of this method is responsible for closing the transaction.</remarks>
	[Expose]
	public async Task<TTransactionalDataSource> BeginTransactionAsync(IsolationLevel? isolationLevel = null, bool forwardEvents = true, CancellationToken cancellationToken = default)
		=> await OnBeginTransactionAsync(isolationLevel, forwardEvents, cancellationToken).ConfigureAwait(false);

	/// <summary>
	/// Creates a new transaction.
	/// </summary>
	/// <returns></returns>
	/// <remarks>The caller of this method is responsible for closing the transaction.</remarks>
	[Expose]
	public async Task<TTransactionalDataSource> BeginTransactionAsync()
		=> await OnBeginTransactionAsync(null, true, default).ConfigureAwait(false);


	[Partial("cancellationToken")] public Func<CancellationToken, Task<TConnection>> OnCreateConnectionAsync { get; set; } = null!;
	[Partial] public Func<TConnection> OnCreateConnection { get; set; } = null!;
	[Partial("isolationLevel,forwardEvents,cancellationToken")] public Func<IsolationLevel?, bool, CancellationToken, Task<TTransactionalDataSource>> OnBeginTransactionAsync { get; set; } = null!;
	[Partial("isolationLevel,forwardEvents")] public Func<IsolationLevel?, bool, TTransactionalDataSource> OnBeginTransaction { get; set; } = null!;

	[Partial("connection,transaction")]
	public Func<TConnection, TTransaction?, TOpenDataSource> OnCreateOpenDataSource { get; set; } = null!;
}