using System.Collections.Concurrent;
using System.Data.Common;
using System.Data.OleDb;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.SqlServer
{
	/// <summary>
	/// Class SqlServerOpenDataSource.
	/// </summary>
	public class OleDbSqlServerOpenDataSource : OleDbSqlServerDataSourceBase, IOpenDataSource
	{
		readonly OleDbSqlServerDataSource m_BaseDataSource;
		readonly OleDbConnection m_Connection;
		readonly OleDbTransaction? m_Transaction;

		internal OleDbSqlServerOpenDataSource(OleDbSqlServerDataSource dataSource, OleDbConnection connection, OleDbTransaction? transaction) : base(new SqlServerDataSourceSettings(dataSource))
		{
			if (connection == null)
				throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");

			m_BaseDataSource = dataSource;
			m_Connection = connection;
			m_Transaction = transaction;
		}

		/// <summary>
		/// Returns the associated connection.
		/// </summary>
		public DbConnection AssociatedConnection
		{
			get { return m_Connection; }
		}

		/// <summary>
		/// Returns the associated transaction.
		/// </summary>
		public DbTransaction? AssociatedTransaction
		{
			get { return m_Transaction; }
		}

		/// <summary>
		/// Gets or sets the cache to be used by this data source. The default is .NET's System.Runtime.Caching.MemoryCache.
		/// </summary>
		public override ICacheAdapter Cache
		{
			get { return m_BaseDataSource.Cache; }
		}

		/// <summary>
		/// Gets the database metadata.
		/// </summary>
		/// <value>The database metadata.</value>
		public override OleDbSqlServerMetadataCache DatabaseMetadata
		{
			get { return m_BaseDataSource.DatabaseMetadata; }
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
		/// Closes the connection and transaction associated with this data source.
		/// </summary>
		public void Close()
		{
			if (m_Transaction != null)
				m_Transaction.Dispose();
			m_Connection.Dispose();
		}

		/// <summary>
		/// Tests the connection.
		/// </summary>
		public override void TestConnection()
		{
			using (var cmd = new OleDbCommand("SELECT 1", m_Connection))
			{
				if (m_Transaction != null)
					cmd.Transaction = m_Transaction;
				cmd.ExecuteScalar();
			}
		}

		/// <summary>
		/// Tests the connection asynchronously.
		/// </summary>
		/// <returns></returns>
		public override async Task TestConnectionAsync()
		{
			using (var cmd = new OleDbCommand("SELECT 1", m_Connection))
			{
				if (m_Transaction != null)
					cmd.Transaction = m_Transaction;
				await cmd.ExecuteScalarAsync().ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Tries the commit the transaction associated with this data source.
		/// </summary>
		/// <returns>
		/// True if there was an open transaction associated with this data source, otherwise false.
		/// </returns>
		public bool TryCommit()
		{
			if (m_Transaction == null)
				return false;

			m_Transaction.Commit();
			return true;
		}

		/// <summary>
		/// Modifies this data source with additional audit rules.
		/// </summary>
		/// <param name="additionalRules">The additional rules.</param>
		/// <returns></returns>
		public OleDbSqlServerOpenDataSource WithRules(params AuditRule[] additionalRules)
		{
			AuditRules = new AuditRuleCollection(AuditRules, additionalRules);
			return this;
		}

		/// <summary>
		/// Modifies this data source with additional audit rules.
		/// </summary>
		/// <param name="additionalRules">The additional rules.</param>
		/// <returns></returns>
		public OleDbSqlServerOpenDataSource WithRules(IEnumerable<AuditRule> additionalRules)
		{
			AuditRules = new AuditRuleCollection(AuditRules, additionalRules);
			return this;
		}

		/// <summary>
		/// Modifies this data source to include the indicated user.
		/// </summary>
		/// <param name="userValue">The user value.</param>
		/// <returns></returns>
		/// <remarks>
		/// This is used in conjunction with audit rules.
		/// </remarks>
		public OleDbSqlServerOpenDataSource WithUser(object? userValue)
		{
			UserValue = userValue;
			return this;
		}

		/// <summary>
		/// Executes the specified operation.
		/// </summary>
		/// <param name="executionToken">The execution token.</param>
		/// <param name="implementation">The implementation that handles processing the result of the command.</param>
		/// <param name="state">User supplied state.</param>
		/// <exception cref="ArgumentNullException">
		/// executionToken;executionToken is null.
		/// or
		/// implementation;implementation is null.
		/// </exception>
		protected override int? Execute(CommandExecutionToken<OleDbCommand, OleDbParameter> executionToken, CommandImplementation<OleDbCommand> implementation, object? state)
		{
			if (executionToken == null)
				throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
			if (implementation == null)
				throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

			var startTime = DateTimeOffset.Now;
			OnExecutionStarted(executionToken, startTime, state);

			try
			{
				using (var cmd = new OleDbCommand())
				{
					cmd.Connection = m_Connection;
					if (m_Transaction != null)
						cmd.Transaction = m_Transaction;
					if (DefaultCommandTimeout.HasValue)
						cmd.CommandTimeout = (int)DefaultCommandTimeout.Value.TotalSeconds;
					cmd.CommandText = executionToken.CommandText;
					cmd.CommandType = executionToken.CommandType;
					foreach (var param in executionToken.Parameters)
						cmd.Parameters.Add(param);

					var rows = implementation(cmd);
					executionToken.RaiseCommandExecuted(cmd, rows);
					OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
					return rows;
				}
			}
			catch (Exception ex)
			{
				OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
				throw;
			}
		}

		/// <summary>
		/// Executes the specified operation.
		/// </summary>
		/// <param name="executionToken">The execution token.</param>
		/// <param name="implementation">The implementation.</param>
		/// <param name="state">The state.</param>
		protected override int? Execute(OperationExecutionToken<OleDbConnection, OleDbTransaction> executionToken, OperationImplementation<OleDbConnection, OleDbTransaction> implementation, object? state)
		{
			if (executionToken == null)
				throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
			if (implementation == null)
				throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

			var startTime = DateTimeOffset.Now;
			OnExecutionStarted(executionToken, startTime, state);

			try
			{
				var rows = implementation(m_Connection, m_Transaction);
				OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
				return rows;
			}
			catch (Exception ex)
			{
				OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
				throw;
			}
		}

		/// <summary>
		/// Executes the operation asynchronously.
		/// </summary>
		/// <param name="executionToken">The execution token.</param>
		/// <param name="implementation">The implementation that handles processing the result of the command.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">User supplied state.</param>
		/// <returns>Task.</returns>
		/// <exception cref="NotImplementedException"></exception>
		protected override async Task<int?> ExecuteAsync(CommandExecutionToken<OleDbCommand, OleDbParameter> executionToken, CommandImplementationAsync<OleDbCommand> implementation, CancellationToken cancellationToken, object? state)
		{
			if (executionToken == null)
				throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
			if (implementation == null)
				throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

			var startTime = DateTimeOffset.Now;
			OnExecutionStarted(executionToken, startTime, state);

			try
			{
				using (var cmd = new OleDbCommand())
				{
					cmd.Connection = m_Connection;
					if (m_Transaction != null)
						cmd.Transaction = m_Transaction;
					if (DefaultCommandTimeout.HasValue)
						cmd.CommandTimeout = (int)DefaultCommandTimeout.Value.TotalSeconds;
					cmd.CommandText = executionToken.CommandText;
					cmd.CommandType = executionToken.CommandType;
					foreach (var param in executionToken.Parameters)
						cmd.Parameters.Add(param);
					var rows = await implementation(cmd).ConfigureAwait(false);
					executionToken.RaiseCommandExecuted(cmd, rows);
					OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
					return rows;
				}
			}
			catch (Exception ex)
			{
				if (cancellationToken.IsCancellationRequested) //convert Exception into a OperationCanceledException
				{
					var ex2 = new OperationCanceledException("Operation was canceled.", ex, cancellationToken);
					OnExecutionCanceled(executionToken, startTime, DateTimeOffset.Now, state);
					throw ex2;
				}
				else
				{
					OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
					throw;
				}
			}
		}

		/// <summary>
		/// Execute the operation asynchronously.
		/// </summary>
		/// <param name="executionToken">The execution token.</param>
		/// <param name="implementation">The implementation.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">The state.</param>
		/// <returns>Task.</returns>
		protected override async Task<int?> ExecuteAsync(OperationExecutionToken<OleDbConnection, OleDbTransaction> executionToken, OperationImplementationAsync<OleDbConnection, OleDbTransaction> implementation, CancellationToken cancellationToken, object? state)
		{
			if (executionToken == null)
				throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
			if (implementation == null)
				throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

			var startTime = DateTimeOffset.Now;
			OnExecutionStarted(executionToken, startTime, state);

			try
			{
				var rows = await implementation(m_Connection, m_Transaction, cancellationToken).ConfigureAwait(false);
				OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
				return rows;
			}
			catch (Exception ex)
			{
				if (cancellationToken.IsCancellationRequested) //convert Exception into a OperationCanceledException
				{
					var ex2 = new OperationCanceledException("Operation was canceled.", ex, cancellationToken);
					OnExecutionCanceled(executionToken, startTime, DateTimeOffset.Now, state);
					throw ex2;
				}
				else
				{
					OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
					throw;
				}
			}
		}
	}
}
