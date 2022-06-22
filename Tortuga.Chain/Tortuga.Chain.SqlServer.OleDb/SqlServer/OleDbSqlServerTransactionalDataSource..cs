using System.Data.OleDb;
using Tortuga.Chain.Core;
using Tortuga.Shipwright;

namespace Tortuga.Chain.SqlServer
{
	/// <summary>
	/// Class SqlServerTransactionalDataSource.
	/// </summary>
	[UseTrait(typeof(Traits.TransactionalDataSourceTrait<OleDbSqlServerDataSource, OleDbConnection, OleDbTransaction, OleDbCommand, OleDbSqlServerMetadataCache>))]
	public partial class OleDbSqlServerTransactionalDataSource : OleDbSqlServerDataSourceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OleDbSqlServerTransactionalDataSource"/> class.
		/// </summary>
		/// <param name="dataSource">The parent connection.</param>
		/// <param name="transactionName">Name of the transaction.</param>
		/// <param name="isolationLevel">The isolation level. If not supplied, will use the database default.</param>
		/// <param name="forwardEvents">If true, logging events are forwarded to the parent connection.</param>
		public OleDbSqlServerTransactionalDataSource(OleDbSqlServerDataSource dataSource, string? transactionName, IsolationLevel? isolationLevel, bool forwardEvents) : base(new SqlServerDataSourceSettings(dataSource, forwardEvents))
		{
			Name = dataSource.Name;

			m_BaseDataSource = dataSource;
			m_Connection = dataSource.CreateConnection();
			TransactionName = transactionName;

			if (isolationLevel == null)
				m_Transaction = m_Connection.BeginTransaction();
			else
				m_Transaction = m_Connection.BeginTransaction(isolationLevel.Value);

			if (forwardEvents)
			{
				ExecutionStarted += (sender, e) => dataSource.OnExecutionStarted(e);
				ExecutionFinished += (sender, e) => dataSource.OnExecutionFinished(e);
				ExecutionError += (sender, e) => dataSource.OnExecutionError(e);
				ExecutionCanceled += (sender, e) => dataSource.OnExecutionCanceled(e);
			}
			AuditRules = dataSource.AuditRules;
			UserValue = dataSource.UserValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OleDbSqlServerTransactionalDataSource" /> class.
		/// </summary>
		/// <param name="dataSource">The parent connection.</param>
		/// <param name="transactionName">Name of the transaction.</param>
		/// <param name="forwardEvents">If true, logging events are forwarded to the parent connection.</param>
		/// <param name="connection">The connection.</param>
		/// <param name="transaction">The transaction.</param>
		internal OleDbSqlServerTransactionalDataSource(OleDbSqlServerDataSource dataSource, string? transactionName, bool forwardEvents, OleDbConnection connection, OleDbTransaction transaction) : base(new SqlServerDataSourceSettings(dataSource, forwardEvents))
		{
			Name = dataSource.Name;

			m_BaseDataSource = dataSource;
			m_Connection = connection;
			TransactionName = transactionName;
			m_Transaction = transaction;

			if (forwardEvents)
			{
				ExecutionStarted += (sender, e) => dataSource.OnExecutionStarted(e);
				ExecutionFinished += (sender, e) => dataSource.OnExecutionFinished(e);
				ExecutionError += (sender, e) => dataSource.OnExecutionError(e);
				ExecutionCanceled += (sender, e) => dataSource.OnExecutionCanceled(e);
			}
			AuditRules = dataSource.AuditRules;
			UserValue = dataSource.UserValue;
		}



		/// <summary>
		/// Gets the name of the transaction.
		/// </summary>
		/// <value>The name of the transaction.</value>
		public string? TransactionName { get; }

		/// <summary>
		/// Executes the specified operation.
		/// </summary>
		/// <param name="executionToken">The execution token.</param>
		/// <param name="implementation">The implementation that handles processing the result of the command.</param>
		/// <param name="state">User supplied state.</param>
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
		/// execute as an asynchronous operation.
		/// </summary>
		/// <param name="executionToken">The execution token.</param>
		/// <param name="implementation">The implementation that handles processing the result of the command.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">User supplied state.</param>
		/// <returns>Task.</returns>
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
