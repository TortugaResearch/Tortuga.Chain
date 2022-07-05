using Nito.AsyncEx;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Core;
using Tortuga.Shipwright;
using Traits;

namespace Tortuga.Chain.SQLite
{
	/// <summary>
	/// Class SQLiteTransactionalDataSource
	/// </summary>

	[UseTrait(typeof(TransactionalDataSourceTrait<SQLiteDataSource, SQLiteConnection, SQLiteTransaction, SQLiteCommand, SQLiteMetadataCache>))]
	public partial class SQLiteTransactionalDataSource : SQLiteDataSourceBase, IHasOnDispose
	{
		[SuppressMessage("Microsoft.Usage", "CA2213")]
		private IDisposable m_LockToken;

		/// <summary>
		/// Initializes a new instance of the <see cref="SQLiteTransactionalDataSource"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="isolationLevel">The isolation level.</param>
		/// <param name="forwardEvents">if set to <c>true</c> [forward events].</param>
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public SQLiteTransactionalDataSource(SQLiteDataSource dataSource, IsolationLevel? isolationLevel, bool forwardEvents) : base(new SQLiteDataSourceSettings(dataSource, forwardEvents))
		{
			if (dataSource == null)
				throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");

			Name = dataSource.Name;

			m_BaseDataSource = dataSource;
			m_Connection = dataSource.CreateConnection();
			m_LockToken = SyncLock.WriterLock();

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
		/// Initializes a new instance of the <see cref="SQLiteTransactionalDataSource" /> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="forwardEvents">if set to <c>true</c> [forward events].</param>
		/// <param name="connection">The connection.</param>
		/// <param name="transaction">The transaction.</param>
		/// <param name="lockToken">The lock token.</param>
		/// <exception cref="ArgumentNullException">
		/// </exception>
		internal SQLiteTransactionalDataSource(SQLiteDataSource dataSource, bool forwardEvents, SQLiteConnection connection, SQLiteTransaction transaction, IDisposable? lockToken) : base(new SQLiteDataSourceSettings(dataSource))
		{
			if (dataSource == null)
				throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");
			if (connection == null)
				throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");
			if (transaction == null)
				throw new ArgumentNullException(nameof(transaction), $"{nameof(transaction)} is null.");
			if (lockToken == null)
				throw new ArgumentNullException(nameof(lockToken), $"{nameof(lockToken)} is null.");

			Name = dataSource.Name;

			m_BaseDataSource = dataSource;
			m_Connection = connection;
			m_Transaction = transaction;
			m_LockToken = lockToken;

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

		internal override AsyncReaderWriterLock SyncLock
		{
			get { return m_BaseDataSource.SyncLock; }
		}

		void IHasOnDispose.OnDispose()
		{
			if (m_LockToken != null)
				m_LockToken.Dispose();
		}

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
		protected override int? Execute(CommandExecutionToken<SQLiteCommand, SQLiteParameter> executionToken, CommandImplementation<SQLiteCommand> implementation, object? state)
		{
			if (executionToken == null)
				throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
			if (implementation == null)
				throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

			var startTime = DateTimeOffset.Now;
			OnExecutionStarted(executionToken, startTime, state);

			try
			{
				using (var cmd = new SQLiteCommand())
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
		/// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
		protected override int? Execute(OperationExecutionToken<SQLiteConnection, SQLiteTransaction> executionToken, OperationImplementation<SQLiteConnection, SQLiteTransaction> implementation, object? state)
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
		protected override async Task<int?> ExecuteAsync(CommandExecutionToken<SQLiteCommand, SQLiteParameter> executionToken, CommandImplementationAsync<SQLiteCommand> implementation, CancellationToken cancellationToken, object? state)
		{
			if (executionToken == null)
				throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
			if (implementation == null)
				throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

			var mode = DisableLocks ? LockType.None : (executionToken as SQLiteCommandExecutionToken)?.LockType ?? LockType.Write;

			var startTime = DateTimeOffset.Now;
			OnExecutionStarted(executionToken, startTime, state);

			try
			{
				using (var cmd = new SQLiteCommand())
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
				if (cancellationToken.IsCancellationRequested) //convert SQLiteException into a OperationCanceledException
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
		protected override async Task<int?> ExecuteAsync(OperationExecutionToken<SQLiteConnection, SQLiteTransaction> executionToken, OperationImplementationAsync<SQLiteConnection, SQLiteTransaction> implementation, CancellationToken cancellationToken, object? state)
		{
			if (executionToken == null)
				throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
			if (implementation == null)
				throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

			var mode = DisableLocks ? LockType.None : (executionToken as SQLiteOperationExecutionToken)?.LockType ?? LockType.Write;

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
				if (cancellationToken.IsCancellationRequested) //convert SQLiteException into a OperationCanceledException
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