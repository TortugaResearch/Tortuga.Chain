using Nito.AsyncEx;
using System.Data.SQLite;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Shipwright;
using Traits;

namespace Tortuga.Chain.SQLite
{
	/// <summary>
	/// Class SQLiteOpenDataSource.
	/// </summary>
	/// <seealso cref="SQLiteDataSourceBase" />
	[UseTrait(typeof(OpenDataSourceTrait<SQLiteDataSource, SQLiteOpenDataSource, SQLiteConnection, SQLiteTransaction, SQLiteCommand, SQLiteMetadataCache>))]
	public partial class SQLiteOpenDataSource : SQLiteDataSourceBase
	{
		internal SQLiteOpenDataSource(SQLiteDataSource dataSource, SQLiteConnection connection, SQLiteTransaction? transaction) : base(new SQLiteDataSourceSettings(dataSource))
		{
			if (connection == null)
				throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");

			m_BaseDataSource = dataSource;
			m_Connection = connection;
			m_Transaction = transaction;
		}

		internal override AsyncReaderWriterLock SyncLock
		{
			get { return m_BaseDataSource.SyncLock; }
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
		protected override int? Execute(CommandExecutionToken<SQLiteCommand, SQLiteParameter> executionToken, CommandImplementation<SQLiteCommand> implementation, object? state)
		{
			if (executionToken == null)
				throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
			if (implementation == null)
				throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

			var mode = DisableLocks ? LockType.None : (executionToken as SQLiteCommandExecutionToken)?.LockType ?? LockType.Write;

			var startTime = DateTimeOffset.Now;
			OnExecutionStarted(executionToken, startTime, state);

			IDisposable? lockToken = null;
			try
			{
				switch (mode)
				{
					case LockType.Read: lockToken = SyncLock.ReaderLock(); break;
					case LockType.Write: lockToken = SyncLock.WriterLock(); break;
				}

				using (var cmd = new SQLiteCommand())
				{
					cmd.Connection = m_Connection;
					if (m_Transaction != null)
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
			finally
			{
				if (lockToken != null)
					lockToken.Dispose();
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

			var mode = DisableLocks ? LockType.None : (executionToken as SQLiteOperationExecutionToken)?.LockType ?? LockType.Write;

			var startTime = DateTimeOffset.Now;
			OnExecutionStarted(executionToken, startTime, state);

			IDisposable? lockToken = null;
			try
			{
				switch (mode)
				{
					case LockType.Read: lockToken = SyncLock.ReaderLock(); break;
					case LockType.Write: lockToken = SyncLock.WriterLock(); break;
				}

				var rows = implementation(m_Connection, m_Transaction);
				OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
				return rows;
			}
			catch (Exception ex)
			{
				OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
				throw;
			}
			finally
			{
				if (lockToken != null)
					lockToken.Dispose();
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
		protected override async Task<int?> ExecuteAsync(CommandExecutionToken<SQLiteCommand, SQLiteParameter> executionToken, CommandImplementationAsync<SQLiteCommand> implementation, CancellationToken cancellationToken, object? state)
		{
			if (executionToken == null)
				throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
			if (implementation == null)
				throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

			var mode = DisableLocks ? LockType.None : (executionToken as SQLiteCommandExecutionToken)?.LockType ?? LockType.Write;

			var startTime = DateTimeOffset.Now;
			OnExecutionStarted(executionToken, startTime, state);

			IDisposable? lockToken = null;
			try
			{
				switch (mode)
				{
					case LockType.Read: lockToken = await SyncLock.ReaderLockAsync().ConfigureAwait(false); break;
					case LockType.Write: lockToken = await SyncLock.WriterLockAsync().ConfigureAwait(false); break;
				}

				using (var cmd = new SQLiteCommand())
				{
					cmd.Connection = m_Connection;
					if (m_Transaction != null)
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
			finally
			{
				if (lockToken != null)
					lockToken.Dispose();
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

			IDisposable? lockToken = null;
			try
			{
				switch (mode)
				{
					case LockType.Read: lockToken = await SyncLock.ReaderLockAsync().ConfigureAwait(false); break;
					case LockType.Write: lockToken = await SyncLock.WriterLockAsync().ConfigureAwait(false); break;
				}

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
			finally
			{
				if (lockToken != null)
					lockToken.Dispose();
			}
		}

		private partial SQLiteOpenDataSource OnOverride(IEnumerable<AuditRule>? additionalRules, object? userValue)
		{
			if (userValue != null)
				UserValue = userValue;
			if (additionalRules != null)
				AuditRules = new AuditRuleCollection(AuditRules, additionalRules);

			return this;
		}
	}
}