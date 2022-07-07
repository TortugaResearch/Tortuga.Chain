using System.Collections.Concurrent;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;
using Tortuga.Shipwright;

namespace Traits;

internal interface IHasExtensionCache
{
	ConcurrentDictionary<Type, object> ExtensionCache { get; }
}

[Trait]
class RootDataSourceTrait<TRootDataSource, TTransactionalDataSource, TOpenDataSource, TConnection, TTransaction, TCommand, TConnectionStringBuilder> : IRootDataSource, IHasExtensionCache
	where TRootDataSource : IRootDataSource
	where TTransactionalDataSource : ITransactionalDataSource
	where TOpenDataSource : IOpenDataSource
	where TConnection : DbConnection
	where TTransaction : DbTransaction
	where TCommand : DbCommand, new()
	where TConnectionStringBuilder : DbConnectionStringBuilder, new()
{
	/// <summary>
	/// Gets the cache to be used by this data source. The default is .NET's System.Runtime.Caching.MemoryCache.
	/// </summary>
	[Expose(Inheritance = Inheritance.Override)]
	public ICacheAdapter Cache => m_Cache;

	/// <summary>
	/// The composed connection string.
	/// </summary>
	/// <remarks>This is created and cached by a ConnectionStringBuilder.</remarks>
	[Expose(Accessibility = Accessibility.Internal)]
	public string ConnectionString => m_ConnectionBuilder.ConnectionString;

	/// <summary>
	/// The extension cache is used by extensions to store data source specific information.
	/// </summary>
	/// <value>
	/// The extension cache.
	/// </value>
	[Expose(Accessibility = Accessibility.Protected, Inheritance = Inheritance.Override)]
	public ConcurrentDictionary<Type, object> ExtensionCache => m_ExtensionCache;

	[Expose(Accessibility = Accessibility.Internal)]
	public ICacheAdapter m_Cache { get; set; } = null!;

	/// <summary>
	/// This object can be used to access the database connection string.
	/// </summary>
	[Expose(Accessibility = Accessibility.Private, Setter = Setter.Init)]
	public TConnectionStringBuilder m_ConnectionBuilder { get; set; } = null!;

	[Expose(Accessibility = Accessibility.Internal)]
	public ConcurrentDictionary<Type, object> m_ExtensionCache { get; set; } = null!;

	[Partial("isolationLevel,forwardEvents")] public Func<IsolationLevel?, bool, TTransactionalDataSource> OnBeginTransaction { get; set; } = null!;

	[Partial("isolationLevel,forwardEvents,cancellationToken")] public Func<IsolationLevel?, bool, CancellationToken, Task<TTransactionalDataSource>> OnBeginTransactionAsync { get; set; } = null!;

	[Partial("cache,additionalRules,userValue")]
	public Func<ICacheAdapter?, IEnumerable<AuditRule>?, object?, TRootDataSource> OnCloneWithOverrides { get; set; } = null!;

	[Partial] public Func<TConnection> OnCreateConnection { get; set; } = null!;

	[Partial("cancellationToken")] public Func<CancellationToken, Task<TConnection>> OnCreateConnectionAsync { get; set; } = null!;

	[Partial("connection,transaction")]
	public Func<TConnection, TTransaction?, TOpenDataSource> OnCreateOpenDataSource { get; set; } = null!;

	ITransactionalDataSource IRootDataSource.BeginTransaction()
	{
		return BeginTransaction();
	}

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

	async Task<ITransactionalDataSource> IRootDataSource.BeginTransactionAsync()
	{
		return await BeginTransactionAsync().ConfigureAwait(false);
	}

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

	[SuppressMessage("Design", "CA1033")]
	DbConnection IRootDataSource.CreateConnection() => OnCreateConnection();

	/// <summary>
	/// Creates and opens a new Access connection
	/// </summary>
	/// <returns></returns>
	/// <remarks>The caller of this method is responsible for closing the connection.</remarks>
	[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
	[Expose]
	public TConnection CreateConnection() => OnCreateConnection();

	[SuppressMessage("Design", "CA1033")]
	async Task<DbConnection> IRootDataSource.CreateConnectionAsync() => await OnCreateConnectionAsync(default).ConfigureAwait(false);

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

	IOpenDataSource IRootDataSource.CreateOpenDataSource() => CreateOpenDataSource();

	IOpenDataSource IRootDataSource.CreateOpenDataSource(DbConnection connection, DbTransaction? transaction)
	{
		return CreateOpenDataSource((TConnection)connection, (TTransaction?)transaction);
	}

	IOpenDataSource IRootDataSource.CreateOpenDataSource(IDbConnection connection, IDbTransaction? transaction)
	{
		return CreateOpenDataSource((TConnection)connection, (TTransaction?)transaction);
	}

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
	/// Tests the connection.
	/// </summary>
	[Expose(Inheritance = Inheritance.Override)]
	public void TestConnection()
	{
		using (var con = CreateConnection())
		using (var cmd = new TCommand() { CommandText = "SELECT 1", Connection = con })
			cmd.ExecuteScalar();
	}

	/// <summary>
	/// Tests the connection asynchronously.
	/// </summary>
	/// <returns></returns>
	[Expose(Inheritance = Inheritance.Override)]
	public async Task TestConnectionAsync()
	{
		using (var con = await CreateConnectionAsync().ConfigureAwait(false))
		using (var cmd = new TCommand() { CommandText = "SELECT 1", Connection = con })
			await cmd.ExecuteScalarAsync().ConfigureAwait(false);
	}

	/// <summary>
	/// Creates a new data source with the provided cache.
	/// </summary>
	/// <param name="cache">The cache.</param>
	/// <returns></returns>
	[Expose]
	public TRootDataSource WithCache(ICacheAdapter cache)
	{
		return OnCloneWithOverrides(cache, null, null);
	}

	/// <summary>
	/// Creates a new data source with additional audit rules.
	/// </summary>
	/// <param name="additionalRules">The additional rules.</param>
	/// <returns></returns>
	[Expose]
	public TRootDataSource WithRules(params AuditRule[] additionalRules)
	{
		return OnCloneWithOverrides(null, additionalRules, null);
	}

	/// <summary>
	/// Creates a new data source with additional audit rules.
	/// </summary>
	/// <param name="additionalRules">The additional rules.</param>
	/// <returns></returns>
	[Expose]
	public TRootDataSource WithRules(IEnumerable<AuditRule> additionalRules)
	{
		return OnCloneWithOverrides(null, additionalRules, null);
	}

	/// <summary>
	/// Creates a new data source with the indicated user.
	/// </summary>
	/// <param name="userValue">The user value.</param>
	/// <returns></returns>
	/// <remarks>
	/// This is used in conjunction with audit rules.
	/// </remarks>
	[Expose]
	public TRootDataSource WithUser(object? userValue)
	{
		return OnCloneWithOverrides(null, null, userValue);
	}
}
