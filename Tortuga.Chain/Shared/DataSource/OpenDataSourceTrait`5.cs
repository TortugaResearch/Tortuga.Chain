using System.Collections.Concurrent;
using System.Data.Common;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Shipwright;

namespace Traits
{
	[Trait]
	sealed class OpenDataSourceTrait<TRootDataSource, TOpenDataSource, TConnection, TTransaction, TCommand, TDatabaseMetadata> : IOpenDataSource
		where TRootDataSource : class, IRootDataSource, IDataSource, IHasExtensionCache
		where TOpenDataSource : class, IDataSource
		where TConnection : DbConnection
		where TTransaction : DbTransaction
		where TCommand : DbCommand, new()
		where TDatabaseMetadata : IDatabaseMetadataCache
	{
		DbConnection IOpenDataSource.AssociatedConnection => m_Connection;

		/// <summary>
		/// Returns the associated connection.
		/// </summary>
		/// <value>The associated connection.</value>
		[Expose]
		public TConnection AssociatedConnection => m_Connection;

		DbTransaction? IOpenDataSource.AssociatedTransaction => m_Transaction;

		/// <summary>
		/// Returns the associated transaction.
		/// </summary>
		/// <value>The associated transaction.</value>
		[Expose]
		public TTransaction? AssociatedTransaction => m_Transaction;

		/// <summary>
		/// Gets the cache to be used by this data source. The default is .NET's System.Runtime.Caching.MemoryCache.
		/// </summary>
		[Expose(Inheritance = Inheritance.Override)]
		public ICacheAdapter Cache
		{
			get { return m_BaseDataSource.Cache; }
		}

		[Container]
		public IOpenDataSource Container { get; set; } = null!;

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

		[Expose(Accessibility = Accessibility.Private, Setter = Setter.Init)]
		public TTransaction? m_Transaction { get; set; } = null!;

		[Partial("additionalRules,userValue")]
		public Func<IEnumerable<AuditRule>?, object?, TOpenDataSource> OnOverride { get; set; } = null!;

		/// <summary>
		/// Closes the connection and transaction associated with this data source.
		/// </summary>
		[Expose]
		public void Close()
		{
			if (m_Transaction != null)
				m_Transaction.Dispose();
			m_Connection.Dispose();
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
		/// Tries the commit the transaction associated with this data source.
		/// </summary>
		/// <returns>True if there was an open transaction associated with this data source, otherwise false.</returns>
		[Expose]
		public bool TryCommit()
		{
			if (m_Transaction == null)
				return false;
			m_Transaction.Commit();
			return true;
		}

		/// <summary>
		/// Tries to commits the transaction and disposes the underlying connection.
		/// </summary>
		/// <returns>True if there was an open transaction associated with this data source, otherwise false.</returns>
		[Expose]
		public async Task<bool> TryCommitAsync(CancellationToken cancellationToken = default)
		{
			if (m_Transaction == null)
				return false;

			await m_Transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

			return true;
		}

		/// <summary>
		/// Tries to rollback the transaction associated with this data source.
		/// </summary>
		/// <returns>True if there was an open transaction associated with this data source, otherwise false.</returns>
		[Expose]
		public bool TryRollback()
		{
			if (m_Transaction == null)
				return false;
			m_Transaction.Rollback();
			return true;
		}

		/// <summary>
		/// Tries to roll back the transaction to the indicated save point.
		/// </summary>
		/// <param name="savepointName">The name of the savepoint to roll back to.</param>
		/// <returns>True if there was an open transaction associated with this data source, otherwise false.</returns>
		[Expose]
		public bool TryRollback(string savepointName)
		{
			if (m_Transaction == null)
				return false;

			m_Transaction.Rollback(savepointName);
			return true;
		}

		/// <summary>
		/// Tries to roll back the transaction.
		/// </summary>
		/// <returns>True if there was an open transaction associated with this data source, otherwise false.</returns>
		[Expose]
		public async Task<bool> TryRollbackAsync(CancellationToken cancellationToken = default)
		{
			if (m_Transaction == null)
				return false;

			await m_Transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);

			return true;
		}

		/// <summary>
		/// Tries to roll back the transaction to the indicated save point.
		/// </summary>
		/// <param name="savepointName">The name of the savepoint to roll back to.</param>
		/// <param name="cancellationToken"></param>
		/// <returns>True if there was an open transaction associated with this data source, otherwise false.</returns>
		[Expose]
		public async Task<bool> TryRollbackAsync(string savepointName, CancellationToken cancellationToken = default)
		{
			if (m_Transaction == null)
				return false;

			await m_Transaction.RollbackAsync(savepointName, cancellationToken).ConfigureAwait(false);
			return true;
		}

		/// <summary>
		/// Tries to create a savepoint in the transaction. This allows all commands that are executed after the savepoint was established to be rolled back, restoring the transaction state to what it was at the time of the savepoint.
		/// </summary>
		/// <param name="savepointName">The name of the savepoint to be created.</param>
		/// <returns>True if there was an open transaction associated with this data source, otherwise false.</returns>
		[Expose]
		public bool TrySave(string savepointName)
		{
			if (m_Transaction == null)
				return false;
			m_Transaction.Save(savepointName);
			return true;
		}

		/// <summary>
		/// Tries to creates a savepoint in the transaction. This allows all commands that are executed after the savepoint was established to be rolled back, restoring the transaction state to what it was at the time of the savepoint.
		/// </summary>
		/// <param name="savepointName">The name of the savepoint to be created.</param>
		/// <param name="cancellationToken"></param>
		/// <returns>True if there was an open transaction associated with this data source, otherwise false.</returns>
		[Expose]
		public async Task<bool> TrySaveAsync(string savepointName, CancellationToken cancellationToken = default)
		{
			if (m_Transaction == null)
				return false;

			await m_Transaction.SaveAsync(savepointName, cancellationToken).ConfigureAwait(false);
			return true;
		}

		/// <summary>
		/// Modifies this data source with additional audit rules.
		/// </summary>
		/// <param name="additionalRules">The additional rules.</param>
		/// <returns></returns>
		[Expose]
		public TOpenDataSource WithRules(params AuditRule[] additionalRules)
		{
			return OnOverride(additionalRules, null);
		}

		/// <summary>
		/// Modifies this data source with additional audit rules.
		/// </summary>
		/// <param name="additionalRules">The additional rules.</param>
		/// <returns></returns>
		[Expose]
		public TOpenDataSource WithRules(IEnumerable<AuditRule> additionalRules)
		{
			return OnOverride(additionalRules, null);
		}

		/// <summary>
		/// Modifies this data source to include the indicated user.
		/// </summary>
		/// <param name="userValue">The user value.</param>
		/// <returns></returns>
		/// <remarks>
		/// This is used in conjunction with audit rules.
		/// </remarks>
		[Expose]
		public TOpenDataSource WithUser(object? userValue)
		{
			return OnOverride(null, userValue);
		}
	}
}
