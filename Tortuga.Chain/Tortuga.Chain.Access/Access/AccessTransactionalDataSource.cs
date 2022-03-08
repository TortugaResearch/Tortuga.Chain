using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Core;

namespace Tortuga.Chain.Access
{
	/// <summary>
	/// Class AccessTransactionalDataSource
	/// </summary>
	public partial class AccessTransactionalDataSource : AccessDataSourceBase
	{
		private readonly AccessDataSource m_BaseDataSource;

		/// <summary>
		/// Initializes a new instance of the <see cref="AccessTransactionalDataSource"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="isolationLevel">The isolation level.</param>
		/// <param name="forwardEvents">if set to <c>true</c> [forward events].</param>
		public AccessTransactionalDataSource(AccessDataSource dataSource, IsolationLevel? isolationLevel, bool forwardEvents) : base(new AccessDataSourceSettings(dataSource, forwardEvents))
		{
			if (dataSource == null)
				throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");

			Name = dataSource.Name;

			m_BaseDataSource = dataSource;
			m_Connection = dataSource.CreateConnection();

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
		/// Initializes a new instance of the <see cref="AccessTransactionalDataSource" /> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="forwardEvents">if set to <c>true</c> [forward events].</param>
		/// <param name="connection">The connection.</param>
		/// <param name="transaction">The transaction.</param>
		/// <exception cref="System.ArgumentNullException">
		/// </exception>
		internal AccessTransactionalDataSource(AccessDataSource dataSource, bool forwardEvents, OleDbConnection connection, OleDbTransaction transaction) : base(new AccessDataSourceSettings(dataSource, forwardEvents))
		{
			if (dataSource == null)
				throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");
			if (connection == null)
				throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");
			if (transaction == null)
				throw new ArgumentNullException(nameof(transaction), $"{nameof(transaction)} is null.");

			Name = dataSource.Name;

			m_BaseDataSource = dataSource;
			m_Connection = connection;
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
		/// Gets the database metadata.
		/// </summary>
		/// <value>The database metadata.</value>
		public override AccessMetadataCache DatabaseMetadata => m_BaseDataSource.DatabaseMetadata;

		/// <summary>
		/// Executes the specified execution token.
		/// </summary>
		/// <param name="executionToken">The execution token.</param>
		/// <param name="implementation">The implementation.</param>
		/// <param name="state">The state.</param>
		/// <exception cref="ArgumentNullException">
		/// executionToken;executionToken is null.
		/// or
		/// implementation;implementation is null.
		/// </exception>
		[SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		protected override int? Execute(CommandExecutionToken<OleDbCommand, OleDbParameter> executionToken, CommandImplementation<OleDbCommand> implementation, object? state)
		{
			if (executionToken == null)
				throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
			if (implementation == null)
				throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");
			var currentToken = executionToken as AccessCommandExecutionToken;
			if (currentToken == null)
				throw new ArgumentNullException(nameof(executionToken), "only AccessCommandExecutionToken is supported.");

			var startTime = DateTimeOffset.Now;
			OnExecutionStarted(executionToken, startTime, state);

			try
			{
				int? rows = null;
				while (currentToken != null)
				{
					OnExecutionStarted(currentToken, startTime, state);
					using (var cmd = new OleDbCommand())
					{
						cmd.Connection = m_Connection;
						cmd.Transaction = m_Transaction;

						if (DefaultCommandTimeout.HasValue)
							cmd.CommandTimeout = (int)DefaultCommandTimeout.Value.TotalSeconds;
						cmd.CommandText = currentToken.CommandText;
						cmd.CommandType = currentToken.CommandType;
						foreach (var param in currentToken.Parameters)
							cmd.Parameters.Add(param);

						currentToken.ApplyCommandOverrides(cmd);

						if (currentToken.ExecutionMode == AccessCommandExecutionMode.Materializer)
							rows = implementation(cmd);
						else if (currentToken.ExecutionMode == AccessCommandExecutionMode.ExecuteScalarAndForward)
						{
							if (currentToken.ForwardResult == null)
								throw new InvalidOperationException("currentToken.ExecutionMode is ExecuteScalarAndForward, but currentToken.ForwardResult is null.");

							currentToken.ForwardResult(cmd.ExecuteScalar());
						}
						else
							rows = cmd.ExecuteNonQuery();
						currentToken.RaiseCommandExecuted(cmd, rows);
						OnExecutionFinished(currentToken, startTime, DateTimeOffset.Now, rows, state);
					}
					currentToken = currentToken.NextCommand;
				}
				return rows;
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
		/// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
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
		/// <param name="implementation">The implementation.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">The state.</param>
		/// <returns>Task.</returns>
		/// <exception cref="ArgumentNullException">
		/// executionToken;executionToken is null.
		/// or
		/// implementation;implementation is null.
		/// </exception>
		protected override async Task<int?> ExecuteAsync(CommandExecutionToken<OleDbCommand, OleDbParameter> executionToken, CommandImplementationAsync<OleDbCommand> implementation, CancellationToken cancellationToken, object? state)
		{
			if (executionToken == null)
				throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
			if (implementation == null)
				throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");
			var currentToken = executionToken as AccessCommandExecutionToken;
			if (currentToken == null)
				throw new ArgumentNullException(nameof(executionToken), "only AccessCommandExecutionToken is supported.");

			var startTime = DateTimeOffset.Now;
			OnExecutionStarted(executionToken, startTime, state);

			try
			{
				int? rows = null;
				while (currentToken != null)
				{
					OnExecutionStarted(currentToken, startTime, state);
					using (var cmd = new OleDbCommand())
					{
						cmd.Connection = m_Connection;
						cmd.Transaction = m_Transaction;
						if (DefaultCommandTimeout.HasValue)
							cmd.CommandTimeout = (int)DefaultCommandTimeout.Value.TotalSeconds;
						cmd.CommandText = currentToken.CommandText;
						cmd.CommandType = currentToken.CommandType;
						foreach (var param in currentToken.Parameters)
							cmd.Parameters.Add(param);

						currentToken.ApplyCommandOverrides(cmd);

						if (currentToken.ExecutionMode == AccessCommandExecutionMode.Materializer)
							rows = await implementation(cmd).ConfigureAwait(false);
						else if (currentToken.ExecutionMode == AccessCommandExecutionMode.ExecuteScalarAndForward)
						{
							if (currentToken.ForwardResult == null)
								throw new InvalidOperationException("currentToken.ExecutionMode is ExecuteScalarAndForward, but currentToken.ForwardResult is null.");

							currentToken.ForwardResult(await cmd.ExecuteScalarAsync().ConfigureAwait(false));
						}
						else
							rows = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
						executionToken.RaiseCommandExecuted(cmd, rows);
						OnExecutionFinished(currentToken, startTime, DateTimeOffset.Now, rows, state);
					}
					currentToken = currentToken.NextCommand;
				}
				return rows;
			}
			catch (Exception ex)
			{
				if (cancellationToken.IsCancellationRequested) //convert AccessException into a OperationCanceledException
				{
					var ex2 = new OperationCanceledException("Operation was canceled.", ex, cancellationToken);
					OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex2, state);
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
		/// execute as an asynchronous operation.
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
				if (cancellationToken.IsCancellationRequested) //convert AccessException into a OperationCanceledException
				{
					var ex2 = new OperationCanceledException("Operation was canceled.", ex, cancellationToken);
					OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex2, state);
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
