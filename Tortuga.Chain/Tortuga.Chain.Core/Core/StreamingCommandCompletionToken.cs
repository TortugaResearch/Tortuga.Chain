using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.Core;

/// <summary>
/// This is used to close resources after completing a streaming operation.
/// </summary>
public sealed class StreamingCommandCompletionToken : IDisposable, IAsyncDisposable
{
	readonly DbCommand? m_Command;
	readonly DbConnection? m_Connection;
	readonly DataSource m_DataSource;

	readonly ExecutionToken m_ExecutionToken;

	readonly DateTimeOffset m_StartTime;

	readonly object? m_State;

	/// <summary>
	/// Initializes a new instance of the <see cref="StreamingCommandCompletionToken" /> class.
	/// </summary>
	/// <param name="dataSource">The data source. Used to fire completed events.</param>
	/// <param name="executionToken">The execution token.</param>
	/// <param name="startTime">The start time.</param>
	/// <param name="state">The state.</param>
	/// <param name="command">The command.</param>
	/// <param name="connection">If the data source wants the caller to close the DbConnection, it will be provided here.</param>
	public StreamingCommandCompletionToken(DataSource dataSource, ExecutionToken executionToken, DateTimeOffset startTime, object? state, DbCommand? command, DbConnection? connection = null)
	{
		m_DataSource = dataSource;
		m_StartTime = startTime;
		m_ExecutionToken = executionToken;
		m_State = state;
		m_Command = command;
		m_Connection = connection;
	}

	/// <summary>
	/// If the data source wants the caller to release the write lock, it will be provided here.
	/// </summary>
	/// <value>The lock token.</value>
	[SuppressMessage("Design", "CA1044:Properties should not be write only", Justification = "<Pending>")]
	public IDisposable? LockToken { private get; init; }

	/// <summary>
	/// Gets or sets the number of rows affected.
	/// </summary>
	/// <value>The rows affected.</value>
	/// <remarks>Set this before calling Dispose or DisposeAsync.</remarks>
	public int? RowsAffected { get; set; }

	/// <summary>
	/// If the data source wants the caller to commit the DbTransaction, it will be provided here.
	/// </summary>
	/// <value>The transaction.</value>
	[SuppressMessage("Design", "CA1044:Properties should not be write only", Justification = "<Pending>")]
	public DbTransaction? Transaction { private get; init; }

	/// <summary>
	/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	public void Dispose()
	{
		try
		{
			LockToken?.Dispose(); //this MUST be released or the database will deadlock.

			if (m_Command != null)
			{
				m_ExecutionToken.RaiseCommandExecuted(m_Command, RowsAffected);
				m_Command.Dispose();
			}

			Transaction?.Commit();
			m_DataSource.OnExecutionFinished(new ExecutionEventArgs(m_ExecutionToken, m_StartTime, DateTimeOffset.Now, RowsAffected, m_State));
			m_Connection?.Dispose();
		}
		catch (Exception ex)
		{
			m_DataSource.OnExecutionError(new ExecutionEventArgs(m_ExecutionToken, m_StartTime, DateTimeOffset.Now, ex, m_State));
			throw;
		}
	}

	/// <summary>
	/// Disposes the asynchronous.
	/// </summary>
	/// <returns>ValueTask.</returns>
	public async ValueTask DisposeAsync()
	{
		try
		{
			LockToken?.Dispose(); //this MUST be released or the database will deadlock.

			if (m_Command != null)
			{
				m_ExecutionToken.RaiseCommandExecuted(m_Command, RowsAffected);
				await m_Command.DisposeAsync().ConfigureAwait(false);
			}

			if (Transaction != null)
				await Transaction.CommitAsync().ConfigureAwait(false);

			m_DataSource.OnExecutionFinished(new ExecutionEventArgs(m_ExecutionToken, m_StartTime, DateTimeOffset.Now, RowsAffected, m_State));

			if (m_Connection != null)
				await m_Connection.DisposeAsync().ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			m_DataSource.OnExecutionError(new ExecutionEventArgs(m_ExecutionToken, m_StartTime, DateTimeOffset.Now, ex, m_State));
			throw;
		}
	}
}
