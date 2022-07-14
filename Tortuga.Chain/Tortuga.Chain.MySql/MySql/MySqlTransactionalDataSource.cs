using MySqlConnector;
using Tortuga.Chain.Core;
using Tortuga.Shipwright;

namespace Tortuga.Chain.MySql;

/// <summary>
/// Class MySqlTransactionalDataSource
/// </summary>
/// <seealso cref="MySqlDataSourceBase" />
/// <seealso cref="IDisposable" />
[UseTrait(typeof(Traits.TransactionalDataSourceTrait<MySqlDataSource, MySqlConnection, MySqlTransaction, MySqlCommand, MySqlMetadataCache>))]
public partial class MySqlTransactionalDataSource : MySqlDataSourceBase
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MySqlTransactionalDataSource"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="isolationLevel">The isolation level.</param>
	/// <param name="forwardEvents">if set to <c>true</c> [forward events].</param>
	public MySqlTransactionalDataSource(MySqlDataSource dataSource, IsolationLevel? isolationLevel, bool forwardEvents)
		: base(new MySqlDataSourceSettings(dataSource, forwardEvents))
	{
		Name = dataSource.Name;

		m_BaseDataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");
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
	/// Initializes a new instance of the <see cref="MySqlTransactionalDataSource" /> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="forwardEvents">if set to <c>true</c> [forward events].</param>
	/// <param name="connection">The connection.</param>
	/// <param name="transaction">The transaction.</param>
	internal MySqlTransactionalDataSource(MySqlDataSource dataSource, bool forwardEvents, MySqlConnection connection, MySqlTransaction transaction)
		: base(new MySqlDataSourceSettings(dataSource, forwardEvents))
	{
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
	/// Executes the specified implementation.
	/// </summary>
	/// <param name="executionToken">The execution token.</param>
	/// <param name="implementation">The implementation.</param>
	/// <param name="state">The state.</param>
	/// <returns>The caller is expected to use the StreamingCommandCompletionToken to close any lingering connections and fire appropriate events.</returns>
	/// <exception cref="System.NotImplementedException"></exception>
	public override StreamingCommandCompletionToken ExecuteStream(CommandExecutionToken<MySqlCommand, MySqlParameter> executionToken, StreamingCommandImplementation<MySqlCommand> implementation, object? state)
	{
		if (executionToken == null)
			throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
		if (implementation == null)
			throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

		var startTime = DateTimeOffset.Now;
		OnExecutionStarted(executionToken, startTime, state);

		try
		{
			var cmd = new MySqlCommand();

			cmd.Connection = m_Connection;
			cmd.Transaction = m_Transaction;
			executionToken.PopulateCommand(cmd, DefaultCommandTimeout);
			implementation(cmd);

			return new StreamingCommandCompletionToken(this, executionToken, startTime, state, cmd);
		}
		catch (Exception ex)
		{
			OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
			throw;
		}
	}

	/// <summary>
	/// Executes the specified implementation asynchronously.
	/// </summary>
	/// <param name="executionToken">The execution token.</param>
	/// <param name="implementation">The implementation.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="state">The state.</param>
	/// <returns>The caller is expected to use the StreamingCommandCompletionToken to close any lingering connections and fire appropriate events.</returns>
	/// <exception cref="System.NotImplementedException"></exception>
	public override async Task<StreamingCommandCompletionToken> ExecuteStreamAsync(CommandExecutionToken<MySqlCommand, MySqlParameter> executionToken, StreamingCommandImplementationAsync<MySqlCommand> implementation, CancellationToken cancellationToken, object? state)
	{
		if (executionToken == null)
			throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
		if (implementation == null)
			throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

		var startTime = DateTimeOffset.Now;
		OnExecutionStarted(executionToken, startTime, state);

		try
		{
			var cmd = new MySqlCommand();

			cmd.Connection = m_Connection;
			cmd.Transaction = m_Transaction;
			executionToken.PopulateCommand(cmd, DefaultCommandTimeout);
			await implementation(cmd).ConfigureAwait(false);

			return new StreamingCommandCompletionToken(this, executionToken, startTime, state, cmd);
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
	/// Executes the specified operation.
	/// </summary>
	/// <param name="executionToken">The execution token.</param>
	/// <param name="implementation">The implementation that handles processing the result of the command.</param>
	/// <param name="state">User supplied state.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">
	/// executionToken;executionToken is null.
	/// or
	/// implementation;implementation is null.
	/// </exception>
	protected override int? Execute(CommandExecutionToken<MySqlCommand, MySqlParameter> executionToken, CommandImplementation<MySqlCommand> implementation, object? state)
	{
		if (executionToken == null)
			throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
		if (implementation == null)
			throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

		var startTime = DateTimeOffset.Now;
		OnExecutionStarted(executionToken, startTime, state);

		try
		{
			using (var cmd = new MySqlCommand())
			{
				cmd.Connection = m_Connection;
				cmd.Transaction = m_Transaction;
				executionToken.PopulateCommand(cmd, DefaultCommandTimeout);
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
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">
	/// executionToken;executionToken is null.
	/// or
	/// implementation;implementation is null.
	/// </exception>
	protected override int? Execute(OperationExecutionToken<MySqlConnection, MySqlTransaction> executionToken, OperationImplementation<MySqlConnection, MySqlTransaction> implementation, object? state)
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
	/// <returns>
	/// Task.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// executionToken;executionToken is null.
	/// or
	/// implementation;implementation is null.
	/// </exception>
	protected override async Task<int?> ExecuteAsync(CommandExecutionToken<MySqlCommand, MySqlParameter> executionToken, CommandImplementationAsync<MySqlCommand> implementation, CancellationToken cancellationToken, object? state)
	{
		if (executionToken == null)
			throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
		if (implementation == null)
			throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

		var startTime = DateTimeOffset.Now;
		OnExecutionStarted(executionToken, startTime, state);

		try
		{
			using (var cmd = new MySqlCommand())
			{
				cmd.Connection = m_Connection;
				cmd.Transaction = m_Transaction;
				executionToken.PopulateCommand(cmd, DefaultCommandTimeout);
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
	/// <returns>
	/// Task.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// executionToken;executionToken is null.
	/// or
	/// implementation;implementation is null.
	/// </exception>
	protected override async Task<int?> ExecuteAsync(OperationExecutionToken<MySqlConnection, MySqlTransaction> executionToken, OperationImplementationAsync<MySqlConnection, MySqlTransaction> implementation, CancellationToken cancellationToken, object? state)
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
