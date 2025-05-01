using System.Collections.Concurrent;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Shipwright;

namespace Traits;

[Trait]
[SuppressMessage("\tUsage", "CA1816")]
sealed class TransactionalDataSourceTrait<TRootDataSource, TConnection, TTransaction, TCommand, TDatabaseMetadata> : ITransactionalDataSource, IDisposable, IAsyncDisposable
	where TRootDataSource : class, IRootDataSource, IDataSource, IHasExtensionCache
	where TConnection : DbConnection
	where TTransaction : DbTransaction
	where TCommand : DbCommand, new()
	where TDatabaseMetadata : IDatabaseMetadataCache
{
	/// <summary>
	/// Returns the associated connection.
	/// </summary>
	/// <value>The associated connection.</value>
	[Expose]
	public TConnection AssociatedConnection => m_Connection;

	DbConnection IOpenDataSource.AssociatedConnection => m_Connection;

	/// <summary>
	/// Returns the associated transaction.
	/// </summary>
	/// <value>The associated transaction.</value>
	[Expose]
	public TTransaction AssociatedTransaction => m_Transaction;

	DbTransaction? IOpenDataSource.AssociatedTransaction => m_Transaction;

	/// <summary>
	/// Gets the cache to be used by this data source. The default is .NET's System.Runtime.Caching.MemoryCache.
	/// </summary>
	[Expose(Inheritance = Inheritance.Override)]
	public ICacheAdapter Cache
	{
		get { return m_BaseDataSource.Cache; }
	}

	[Container]
	public ITransactionalDataSource Container { get; set; } = null!;

	/// <summary>
	/// Gets the database metadata.
	/// </summary>
	[Expose(Inheritance = Inheritance.Override)]
	public TDatabaseMetadata DatabaseMetadata
	{
		get { return (TDatabaseMetadata)m_BaseDataSource.DatabaseMetadata; }
	}

	[Container(IsOptional = true)]
	public IHasOnDispose? DisposableContainer { get; set; }

	/// <summary>
	/// The extension cache is used by extensions to store data source specific information.
	/// </summary>
	/// <value>
	/// The extension cache.
	/// </value>
	[Expose(Accessibility = Accessibility.Protected, Inheritance = Inheritance.Override)]
	public ConcurrentDictionary<Type, object> ExtensionCache
	{
		get { return m_BaseDataSource.ExtensionCache; }
	}

	[Expose(Accessibility = Accessibility.Private, Setter = Setter.Init)]
	public TRootDataSource m_BaseDataSource { get; set; } = null!;

	[Expose(Accessibility = Accessibility.Private, Setter = Setter.Init)]
	public TConnection m_Connection { get; set; } = null!;

	[Expose(Accessibility = Accessibility.Private)]
	public bool m_Disposed { get; set; }

	[Expose(Accessibility = Accessibility.Private, Setter = Setter.Init)]
	public TTransaction m_Transaction { get; set; } = null!;

	void IOpenDataSource.Close()
	{
		Dispose();
	}

	void ITransactionalDataSource.Commit()
	{
		ObjectDisposedException.ThrowIf(m_Disposed, m_Transaction);

		m_Transaction.Commit();
		Dispose(true);
	}

	/// <summary>
	/// Commits the transaction and disposes the underlying connection.
	/// </summary>
	[Expose]
	public void Commit()
	{
		ObjectDisposedException.ThrowIf(m_Disposed, m_Transaction);

		m_Transaction.Commit();
		Dispose(true);
	}

	/// <summary>
	/// Commits the transaction and disposes the underlying connection.
	/// </summary>
	[Expose]
	public async Task CommitAsync(CancellationToken cancellationToken = default)
	{
		ObjectDisposedException.ThrowIf(m_Disposed, m_Transaction);

		await m_Transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
		await DisposeAsync(true).ConfigureAwait(false);
	}

	/// <summary>
	/// Closes the current transaction and connection. If not committed, the transaction is rolled back.
	/// </summary>
	[Expose]
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
		GC.SuppressFinalize(Container);
	}

	/// <summary>
	/// Closes the current transaction and connection. If not committed, the transaction is rolled back.
	/// </summary>
	/// <param name="disposing"></param>
	[Expose(Accessibility = Accessibility.Protected, Inheritance = Inheritance.Virtual)]
	public void Dispose(bool disposing)
	{
		if (m_Disposed)
			return;

		if (disposing)
		{
			m_Transaction.Dispose();
			m_Connection.Dispose();
			DisposableContainer?.OnDispose();
			m_Disposed = true;
		}
	}

	/// <summary>
	/// Closes the current transaction and connection. If not committed, the transaction is rolled back.
	/// </summary>
	[Expose]
	public ValueTask DisposeAsync()
	{
		return DisposeAsync(true);
	}

	/// <summary>
	/// Closes the current transaction and connection. If not committed, the transaction is rolled back.
	/// </summary>
	/// <param name="disposing"></param>
	[Expose(Accessibility = Accessibility.Protected, Inheritance = Inheritance.Virtual)]
	public async ValueTask DisposeAsync(bool disposing)
	{
		if (m_Disposed)
			return;

		if (disposing)
		{
			await m_Transaction.DisposeAsync().ConfigureAwait(false);
			await m_Connection.DisposeAsync().ConfigureAwait(false);
			DisposableContainer?.OnDispose();
			m_Disposed = true;
		}
	}

	/// <summary>
	/// Rolls back the transaction to the indicated save point.
	/// </summary>
	/// <param name="savepointName">The name of the savepoint to roll back to.</param>
	[Expose]
	public void Rollback(string savepointName)
	{
		ObjectDisposedException.ThrowIf(m_Disposed, m_Transaction);

		m_Transaction.Rollback(savepointName);
	}

	/// <summary>
	/// Rolls back the transaction and disposes the underlying connection.
	/// </summary>
	[Expose]
	public void Rollback()
	{
		ObjectDisposedException.ThrowIf(m_Disposed, m_Transaction);

		m_Transaction.Rollback();
		Dispose(true);
	}

	/// <summary>
	/// Rolls back the transaction and disposes the underlying connection.
	/// </summary>
	[Expose]
	public async Task RollbackAsync(CancellationToken cancellationToken = default)
	{
		ObjectDisposedException.ThrowIf(m_Disposed, m_Transaction);

		await m_Transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
		await DisposeAsync(true).ConfigureAwait(false);
	}

	/// <summary>
	/// Rolls back the transaction to the indicated save point.
	/// </summary>
	/// <param name="savepointName">The name of the savepoint to roll back to.</param>
	/// <param name="cancellationToken"></param>
	[Expose]
	public async Task RollbackAsync(string savepointName, CancellationToken cancellationToken = default)
	{
		ObjectDisposedException.ThrowIf(m_Disposed, m_Transaction);

		await m_Transaction.RollbackAsync(savepointName, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Creates a savepoint in the transaction. This allows all commands that are executed after the savepoint was established to be rolled back, restoring the transaction state to what it was at the time of the savepoint.
	/// </summary>
	/// <param name="savepointName">The name of the savepoint to be created.</param>
	[Expose]
	public void Save(string savepointName)
	{
		m_Transaction.Save(savepointName);
	}

	/// <summary>
	/// Creates a savepoint in the transaction. This allows all commands that are executed after the savepoint was established to be rolled back, restoring the transaction state to what it was at the time of the savepoint.
	/// </summary>
	/// <param name="savepointName">The name of the savepoint to be created.</param>
	/// <param name="cancellationToken"></param>
	[Expose]
	public async Task SaveAsync(string savepointName, CancellationToken cancellationToken = default)
	{
		await m_Transaction.SaveAsync(savepointName, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Tests the connection.
	/// </summary>
	[Expose(Inheritance = Inheritance.Override)]
	public void TestConnection()
	{
		using (var cmd = new TCommand() { CommandText = "SELECT 1", Connection = m_Connection, Transaction = m_Transaction })
			cmd.ExecuteScalar();
	}

	/// <summary>
	/// Tests the connection asynchronously.
	/// </summary>
	/// <returns></returns>
	[Expose(Inheritance = Inheritance.Override)]
	public async Task TestConnectionAsync()
	{
		using (var cmd = new TCommand() { CommandText = "SELECT 1", Connection = m_Connection, Transaction = m_Transaction })
			await cmd.ExecuteScalarAsync().ConfigureAwait(false);
	}

	bool IOpenDataSource.TryCommit()
	{
		Commit();
		return true;
	}

	bool IOpenDataSource.TryRollback()
	{
		Rollback();
		return true;
	}
}
