using System.Collections.Concurrent;
using System.Data.Common;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Shipwright;

namespace Traits
{
	[Trait]
	class TransactionalDataSourceTrait<TRootDataSource, TConnection, TTransaction, TCommand, TDatabaseMetadata> : ITransactionalDataSource, IDisposable
		where TRootDataSource : class, IRootDataSource, IDataSource, IHasExtensionCache
		where TConnection : DbConnection
		where TTransaction : DbTransaction
		where TCommand : DbCommand, new()
		where TDatabaseMetadata : IDatabaseMetadataCache
	{
		[Container]
		public ITransactionalDataSource Container { get; set; } = null!;

		[Container(IsOptional = true)]
		public IHasOnDispose? DisposableContainer { get; set; }

		[Expose(Accessibility = Accessibility.Private, Setter = Setter.Init)]
		public TConnection m_Connection { get; set; } = null!;

		[Expose(Accessibility = Accessibility.Private, Setter = Setter.Init)]
		public TTransaction m_Transaction { get; set; } = null!;

		[Expose(Accessibility = Accessibility.Private, Setter = Setter.Init)]
		public TRootDataSource m_BaseDataSource { get; set; } = null!;

		[Expose(Accessibility = Accessibility.Private)]
		public bool m_Disposed { get; set; }

		/// <summary>
		/// Returns the associated connection.
		/// </summary>
		/// <value>The associated connection.</value>
		[Expose]
		public TConnection AssociatedConnection => m_Connection;

		/// <summary>
		/// Returns the associated transaction.
		/// </summary>
		/// <value>The associated transaction.</value>
		[Expose]
		public TTransaction AssociatedTransaction => m_Transaction;

		DbConnection IOpenDataSource.AssociatedConnection => m_Connection;

		DbTransaction? IOpenDataSource.AssociatedTransaction => m_Transaction;

		void IOpenDataSource.Close()
		{
			Dispose();
		}

		void ITransactionalDataSource.Commit()
		{
			if (m_Disposed)
				throw new ObjectDisposedException("Transaction is disposed");

			m_Transaction.Commit();
			Dispose(true);
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

		/// <summary>
		/// Commits the transaction and disposes the underlying connection.
		/// </summary>
		[Expose]
		public void Commit()
		{
			if (m_Disposed)
				throw new ObjectDisposedException("Transaction is disposed");

			m_Transaction.Commit();
			Dispose(true);
		}

#if NET6_0_OR_GREATER
		/// <summary>
		/// Commits the transaction and disposes the underlying connection.
		/// </summary>
		[Expose]
		public async Task CommitAsync(CancellationToken cancellationToken = default)
		{
			if (m_Disposed)
				throw new ObjectDisposedException("Transaction is disposed");

			await m_Transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
			Dispose(true);
		}

		/// <summary>
		/// Rolls back the transaction and disposes the underlying connection.
		/// </summary>
		[Expose]
		public async Task RollbackAsync(CancellationToken cancellationToken = default)
		{
			if (m_Disposed)
				throw new ObjectDisposedException("Transaction is disposed");

			await m_Transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
			Dispose(true);
		}

		/// <summary>
		/// Rolls back the transaction to the indicated save point.
		/// </summary>
		/// <param name="savepointName">The name of the savepoint to roll back to.</param>
		/// <param name="cancellationToken"></param>
		[Expose]
		public async Task RollbackAsync(string savepointName, CancellationToken cancellationToken = default)
		{
			if (m_Disposed)
				throw new ObjectDisposedException("Transaction is disposed");

			await m_Transaction.RollbackAsync(savepointName).ConfigureAwait(false);
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
		/// Rolls back the transaction to the indicated save point.
		/// </summary>
		/// <param name="savepointName">The name of the savepoint to roll back to.</param>
		[Expose]
		public void Rollback(string savepointName)
		{
			if (m_Disposed)
				throw new ObjectDisposedException("Transaction is disposed");

			m_Transaction.Rollback(savepointName);
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

#endif

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

		/// <summary>
		/// Gets the cache to be used by this data source. The default is .NET's System.Runtime.Caching.MemoryCache.
		/// </summary>
		[Expose(Inheritance = Inheritance.Override)]
		public ICacheAdapter Cache
		{
			get { return m_BaseDataSource.Cache; }
		}

		/// <summary>
		/// Rolls back the transaction and disposes the underlying connection.
		/// </summary>
		[Expose]
		public void Rollback()
		{
			if (m_Disposed)
				throw new ObjectDisposedException("Transaction is disposed");

			m_Transaction.Rollback();
			Dispose(true);
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

		/// <summary>
		/// Gets the database metadata.
		/// </summary>
		[Expose(Inheritance = Inheritance.Override)]
		public TDatabaseMetadata DatabaseMetadata
		{
			get { return (TDatabaseMetadata)m_BaseDataSource.DatabaseMetadata; }
		}
	}
}
