using Tortuga.Chain.Core;
using Tortuga.Shipwright;

namespace Tortuga.Chain.SqlServer;

/// <summary>
/// Class SqlServerTransactionalDataSource.
/// </summary>
[UseTrait(typeof(Traits.TransactionalDataSourceTrait<SqlServerDataSource, SqlConnection, SqlTransaction, SqlCommand, SqlServerMetadataCache>))]
public partial class SqlServerTransactionalDataSource : SqlServerDataSourceBase
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerTransactionalDataSource"/> class.
	/// </summary>
	/// <param name="dataSource">The parent connection.</param>
	/// <param name="transactionName">Name of the transaction.</param>
	/// <param name="isolationLevel">The isolation level. If not supplied, will use the database default.</param>
	/// <param name="forwardEvents">If true, logging events are forwarded to the parent connection.</param>
	public SqlServerTransactionalDataSource(SqlServerDataSource dataSource, string? transactionName, IsolationLevel? isolationLevel, bool forwardEvents) : base(new SqlServerDataSourceSettings(dataSource, forwardEvents))
	{
		Name = dataSource.Name;

		m_BaseDataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");
		m_Connection = dataSource.CreateConnection();
		TransactionName = transactionName;

		if (isolationLevel == null)
			m_Transaction = m_Connection.BeginTransaction(transactionName);
		else
			m_Transaction = m_Connection.BeginTransaction(isolationLevel.Value, transactionName);

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
	/// Initializes a new instance of the <see cref="SqlServerTransactionalDataSource" /> class.
	/// </summary>
	/// <param name="dataSource">The parent connection.</param>
	/// <param name="transactionName">Name of the transaction.</param>
	/// <param name="forwardEvents">If true, logging events are forwarded to the parent connection.</param>
	/// <param name="connection">The connection.</param>
	/// <param name="transaction">The transaction.</param>
	internal SqlServerTransactionalDataSource(SqlServerDataSource dataSource, string? transactionName, bool forwardEvents, SqlConnection connection, SqlTransaction transaction) : base(new SqlServerDataSourceSettings(dataSource, forwardEvents))
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
	/// Executes the specified implementation.
	/// </summary>
	/// <param name="executionToken">The execution token.</param>
	/// <param name="implementation">The implementation.</param>
	/// <param name="state">The state.</param>
	/// <returns>The caller is expected to use the StreamingCommandCompletionToken to close any lingering connections and fire appropriate events.</returns>
	/// <exception cref="System.NotImplementedException"></exception>
	public override StreamingCommandCompletionToken ExecuteStream(CommandExecutionToken<SqlCommand, SqlParameter> executionToken, StreamingCommandImplementation<SqlCommand> implementation, object? state)
	{
		if (executionToken == null)
			throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
		if (implementation == null)
			throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

		var startTime = DateTimeOffset.Now;
		OnExecutionStarted(executionToken, startTime, state);

		try
		{
			var cmd = new SqlCommand();

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
	public override async Task<StreamingCommandCompletionToken> ExecuteStreamAsync(CommandExecutionToken<SqlCommand, SqlParameter> executionToken, StreamingCommandImplementationAsync<SqlCommand> implementation, CancellationToken cancellationToken, object? state)
	{
		if (executionToken == null)
			throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
		if (implementation == null)
			throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

		var startTime = DateTimeOffset.Now;
		OnExecutionStarted(executionToken, startTime, state);

		try
		{
			var cmd = new SqlCommand();

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
	protected override int? Execute(CommandExecutionToken<SqlCommand, SqlParameter> executionToken, CommandImplementation<SqlCommand> implementation, object? state)
	{
		if (executionToken == null)
			throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
		if (implementation == null)
			throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

		var startTime = DateTimeOffset.Now;
		OnExecutionStarted(executionToken, startTime, state);

		try
		{
			using (var cmd = new SqlCommand())
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
	protected override int? Execute(OperationExecutionToken<SqlConnection, SqlTransaction> executionToken, OperationImplementation<SqlConnection, SqlTransaction> implementation, object? state)
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
	protected override async Task<int?> ExecuteAsync(CommandExecutionToken<SqlCommand, SqlParameter> executionToken, CommandImplementationAsync<SqlCommand> implementation, CancellationToken cancellationToken, object? state)
	{
		if (executionToken == null)
			throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
		if (implementation == null)
			throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

		var startTime = DateTimeOffset.Now;
		OnExecutionStarted(executionToken, startTime, state);

		try
		{
			using (var cmd = new SqlCommand())
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
	/// <returns>Task.</returns>
	protected override async Task<int?> ExecuteAsync(OperationExecutionToken<SqlConnection, SqlTransaction> executionToken, OperationImplementationAsync<SqlConnection, SqlTransaction> implementation, CancellationToken cancellationToken, object? state)
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

	/// <summary>
	/// Gets the default type of string parameters. This is used when the query builder cannot determine the best parameter type.
	/// </summary>
	/// <remarks>Set this if encountering performance issues from type conversions in the execution plan.</remarks>
	public override SqlDbType? DefaultStringType => m_BaseDataSource.DefaultStringType;

	/// <summary>
	/// Gets the default length of string parameters. This is used when the query builder cannot determine the best parameter type and the parameter's actual length is smaller than the default length.
	/// </summary>
	/// <remarks>Set this if encountering an excessive number of execution plans that only differ by the length of a string .</remarks>
	public override int? DefaultStringLength => m_BaseDataSource.DefaultStringLength;
}
