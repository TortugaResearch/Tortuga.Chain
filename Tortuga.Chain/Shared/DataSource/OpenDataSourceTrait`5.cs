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
	class OpenDataSourceTrait<TRootDataSource, TOpenDataSource, TConnection, TTransaction, TCommand, TDatabaseMetadata> : IOpenDataSource
		where TRootDataSource : class, IRootDataSource, IDataSource, IHasExtensionCache
		where TOpenDataSource : class, IDataSource
		where TConnection : DbConnection
		where TTransaction : DbTransaction
		where TCommand : DbCommand, new()
		where TDatabaseMetadata : IDatabaseMetadataCache
	{

		[Container]
		public IOpenDataSource Container { get; set; } = null!;

		[Container(IsOptional = true)]
		public IHasOnDispose? DisposableContainer { get; set; }

		[Expose(Accessibility = Accessibility.Private, Setter = Setter.Init)]
		public TConnection m_Connection { get; set; } = null!;

		[Expose(Accessibility = Accessibility.Private, Setter = Setter.Init)]
		public TTransaction? m_Transaction { get; set; } = null!;

		[Expose(Accessibility = Accessibility.Private, Setter = Setter.Init)]
		public TRootDataSource m_BaseDataSource { get; set; } = null!;

		DbConnection IOpenDataSource.AssociatedConnection => m_Connection;

		DbTransaction? IOpenDataSource.AssociatedTransaction => m_Transaction;




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
		/// Gets or sets the cache to be used by this data source. The default is .NET's System.Runtime.Caching.MemoryCache.
		/// </summary>
		[Expose(Inheritance = Inheritance.Override)]
		public ICacheAdapter Cache
		{
			get { return m_BaseDataSource.Cache; }
		}

		/// <summary>
		/// Gets the database metadata.
		/// </summary>
		[Expose(Inheritance = Inheritance.Override)]
		public TDatabaseMetadata DatabaseMetadata
		{
			get { return (TDatabaseMetadata)m_BaseDataSource.DatabaseMetadata; }
		}


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
		/// Tries the rollback the transaction associated with this data source.
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
		public TTransaction? AssociatedTransaction => m_Transaction;

		[Partial("additionalRules,userValue")]
		public Func<IEnumerable<AuditRule>?, object?, TOpenDataSource> OnOverride { get; set; } = null!;



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
