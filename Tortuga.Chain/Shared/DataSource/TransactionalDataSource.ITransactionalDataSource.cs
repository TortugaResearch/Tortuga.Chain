using System.Collections.Concurrent;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

#if SQL_SERVER_SDS

using AbstractConnection = System.Data.SqlClient.SqlConnection;
using AbstractTransaction = System.Data.SqlClient.SqlTransaction;
using AbstractCommand = System.Data.SqlClient.SqlCommand;
using AbstractParameter = System.Data.SqlClient.SqlParameter;

#elif SQL_SERVER_MDS

using AbstractConnection = Microsoft.Data.SqlClient.SqlConnection;
using AbstractTransaction = Microsoft.Data.SqlClient.SqlTransaction;
using AbstractCommand = Microsoft.Data.SqlClient.SqlCommand;
using AbstractParameter = Microsoft.Data.SqlClient.SqlParameter;

#elif SQLITE

using AbstractConnection = System.Data.SQLite.SQLiteConnection;
using AbstractTransaction = System.Data.SQLite.SQLiteTransaction;
using AbstractCommand = System.Data.SQLite.SQLiteCommand;
using AbstractParameter = System.Data.SQLite.SQLiteParameter;

#elif MYSQL

using AbstractConnection = MySqlConnector.MySqlConnection;
using AbstractTransaction = MySqlConnector.MySqlTransaction;
using AbstractCommand = MySqlConnector.MySqlCommand;
using AbstractParameter = MySqlConnector.MySqlParameter;

#elif POSTGRESQL

using AbstractConnection =  Npgsql.NpgsqlConnection;
using AbstractTransaction = Npgsql.NpgsqlTransaction;
using AbstractCommand = Npgsql.NpgsqlCommand;
using AbstractParameter = Npgsql.NpgsqlParameter;

#elif ACCESS || SQL_SERVER_OLEDB

using AbstractConnection = System.Data.OleDb.OleDbConnection;
using AbstractTransaction = System.Data.OleDb.OleDbTransaction;
using AbstractCommand = System.Data.OleDb.OleDbCommand;
using AbstractParameter = System.Data.OleDb.OleDbParameter;

#endif

#if SQL_SERVER_SDS || SQL_SERVER_MDS

namespace Tortuga.Chain.SqlServer
{
    partial class SqlServerTransactionalDataSource : IDisposable, ITransactionalDataSource
#elif SQL_SERVER_OLEDB

namespace Tortuga.Chain.SqlServer
{
    partial class OleDbSqlServerTransactionalDataSource : IDisposable, ITransactionalDataSource

#elif SQLITE

namespace Tortuga.Chain.SQLite
{
    partial class SQLiteTransactionalDataSource : IDisposable, ITransactionalDataSource

#elif MYSQL

namespace Tortuga.Chain.MySql
{
	partial class MySqlTransactionalDataSource : IDisposable, ITransactionalDataSource

#elif POSTGRESQL

namespace Tortuga.Chain.PostgreSql
{
    partial class PostgreSqlTransactionalDataSource : IDisposable, ITransactionalDataSource

#elif ACCESS

namespace Tortuga.Chain.Access
{
    partial class AccessTransactionalDataSource : IDisposable, ITransactionalDataSource

#endif
	{
		private readonly AbstractConnection m_Connection;
		private readonly AbstractTransaction m_Transaction;
		private bool m_Disposed;

		/// <summary>
		/// Returns the associated connection.
		/// </summary>
		/// <value>The associated connection.</value>
		public DbConnection AssociatedConnection => m_Connection;

		/// <summary>
		/// Returns the associated transaction.
		/// </summary>
		/// <value>The associated transaction.</value>
		public DbTransaction? AssociatedTransaction => m_Transaction;

		[SuppressMessage("Design", "CA1033")]
		void IOpenDataSource.Close()
		{
			Dispose();
		}

		[SuppressMessage("Design", "CA1033")]
		bool IOpenDataSource.TryCommit()
		{
			Commit();
			return true;
		}

		/// <summary>
		/// The extension cache is used by extensions to store data source specific information.
		/// </summary>
		/// <value>
		/// The extension cache.
		/// </value>
		protected override ConcurrentDictionary<Type, object> ExtensionCache
		{
			get { return m_BaseDataSource.m_ExtensionCache; }
		}

		/// <summary>
		/// Commits the transaction and disposes the underlying connection.
		/// </summary>
		public void Commit()
		{
			if (m_Disposed)
				throw new ObjectDisposedException("Transaction is disposed");

			m_Transaction.Commit();
			Dispose(true);
		}

		/// <summary>
		/// Closes the current transaction and connection. If not committed, the transaction is rolled back.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Gets the extension data.
		/// </summary>
		/// <typeparam name="TTKey">The type of extension data desired.</typeparam>
		/// <returns>T.</returns>
		/// <remarks>Chain extensions can use this to store data source specific data. The key should be a data type defined by the extension.
		/// Transactional data sources should override this method and return the value held by their parent data source.</remarks>
		public override TTKey GetExtensionData<TTKey>()
		{
			return m_BaseDataSource.GetExtensionData<TTKey>();
		}

		/// <summary>
		/// Rolls back the transaction and disposes the underlying connection.
		/// </summary>
		public void Rollback()
		{
			if (m_Disposed)
				throw new ObjectDisposedException("Transaction is disposed");

			m_Transaction.Rollback();
			Dispose(true);
		}

		/// <summary>
		/// Gets or sets the cache to be used by this data source. The default is .NET's System.Runtime.Caching.MemoryCache.
		/// </summary>
		public override ICacheAdapter Cache
		{
			get { return m_BaseDataSource.Cache; }
		}

		/// <summary>
		/// Closes the current transaction and connection. If not committed, the transaction is rolled back.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (m_Disposed)
				return;

			if (disposing)
			{
				m_Transaction.Dispose();
				m_Connection.Dispose();
				AdditionalDispose();
				m_Disposed = true;
			}
		}

		partial void AdditionalDispose();

		/// <summary>
		/// Tests the connection.
		/// </summary>
		public override void TestConnection()
		{
			using (var cmd = new AbstractCommand("SELECT 1", m_Connection) { Transaction = m_Transaction })
				cmd.ExecuteScalar();
		}

		/// <summary>
		/// Tests the connection asynchronously.
		/// </summary>
		/// <returns></returns>
		public override async Task TestConnectionAsync()
		{
			using (var cmd = new AbstractCommand("SELECT 1", m_Connection) { Transaction = m_Transaction })
				await cmd.ExecuteScalarAsync().ConfigureAwait(false);
		}
	}
}
