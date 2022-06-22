using System.Collections.Concurrent;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.SqlServer;

namespace Tortuga.Chain
{
	/// <summary>
	/// Class OleDbSqlServerDataSource.
	/// </summary>
	/// <seealso cref="OleDbSqlServerDataSource" />
	public partial class OleDbSqlServerDataSource : OleDbSqlServerDataSourceBase
	{
		OleDbSqlServerMetadataCache m_DatabaseMetadata;

		/// <summary>
		/// This is used to decide which option overrides to set when establishing a connection.
		/// </summary>
		OleDbSqlServerEffectiveSettings? m_ServerDefaultSettings;

		/// <summary>
		/// Initializes a new instance of the <see cref="OleDbSqlServerDataSource" /> class.
		/// </summary>
		/// <param name="name">Name of the data source.</param>
		/// <param name="connectionString">The connection string.</param>
		/// <param name="settings">Optional settings object.</param>
		/// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
		public OleDbSqlServerDataSource(string? name, string connectionString, SqlServerDataSourceSettings? settings = null) : base(settings)
		{
			if (string.IsNullOrEmpty(connectionString))
				throw new ArgumentException($"{nameof(connectionString)} is null or empty.", nameof(connectionString));

			m_ConnectionBuilder = new OleDbConnectionStringBuilder(connectionString);
			if (string.IsNullOrEmpty(name))
				Name = m_ConnectionBuilder.DataSource;
			else
				Name = name;

			m_DatabaseMetadata = new OleDbSqlServerMetadataCache(m_ConnectionBuilder);

			if (settings != null)
			{
				XactAbort = settings.XactAbort;
				ArithAbort = settings.ArithAbort;
			}

			m_ExtensionCache = new ConcurrentDictionary<Type, object>();
			m_Cache = DefaultCache;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OleDbSqlServerDataSource" /> class.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		/// <param name="settings">Optional settings object.</param>
		/// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
		public OleDbSqlServerDataSource(string connectionString, SqlServerDataSourceSettings? settings = null)
			: this(null, connectionString, settings)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OleDbSqlServerDataSource" /> class.
		/// </summary>
		/// <param name="name">Optional name of the data source.</param>
		/// <param name="connectionStringBuilder">The connection string builder.</param>
		/// <param name="settings">Optional settings object.</param>
		/// <exception cref="ArgumentNullException">connectionStringBuilder;connectionStringBuilder is null.</exception>
		public OleDbSqlServerDataSource(string? name, OleDbConnectionStringBuilder connectionStringBuilder, SqlServerDataSourceSettings? settings = null) : base(settings)
		{
			m_ConnectionBuilder = connectionStringBuilder ?? throw new ArgumentNullException(nameof(connectionStringBuilder), $"{nameof(connectionStringBuilder)} is null.");
			if (string.IsNullOrEmpty(name))
				Name = m_ConnectionBuilder.DataSource;
			else
				Name = name;

			m_DatabaseMetadata = new OleDbSqlServerMetadataCache(m_ConnectionBuilder);

			if (settings != null)
			{
				XactAbort = settings.XactAbort;
				ArithAbort = settings.ArithAbort;
			}
			m_ExtensionCache = new ConcurrentDictionary<Type, object>();
			m_Cache = DefaultCache;
		}

		OleDbSqlServerDataSource(string? name, OleDbConnectionStringBuilder connectionStringBuilder, SqlServerDataSourceSettings settings, OleDbSqlServerMetadataCache databaseMetadata, ICacheAdapter cache, ConcurrentDictionary<Type, object> extensionCache) : base(settings)
		{
			m_ConnectionBuilder = connectionStringBuilder ?? throw new ArgumentNullException(nameof(connectionStringBuilder), $"{nameof(connectionStringBuilder)} is null.");
			if (string.IsNullOrEmpty(name))
				Name = m_ConnectionBuilder.DataSource;
			else
				Name = name;

			m_DatabaseMetadata = databaseMetadata;

			if (settings != null)
			{
				XactAbort = settings.XactAbort;
				ArithAbort = settings.ArithAbort;
			}
			m_ExtensionCache = extensionCache;
			m_Cache = cache;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OleDbSqlServerDataSource"/> class.
		/// </summary>
		/// <param name="connectionStringBuilder">The connection string builder.</param>
		/// <param name="settings">Optional settings object.</param>
		public OleDbSqlServerDataSource(OleDbConnectionStringBuilder connectionStringBuilder, SqlServerDataSourceSettings? settings = null)
			: this(null, connectionStringBuilder, settings)
		{
		}

		/// <summary>
		/// Terminates a query when an overflow or divide-by-zero error occurs during query execution.
		/// </summary>
		/// <remarks>Microsoft recommends setting ArithAbort=On for all connections. To avoid an additional round-trip to the server, do this at the server level instead of at the connection level.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Arith")]
		public bool? ArithAbort { get; }

		/// <summary>
		/// This object can be used to lookup database information.
		/// </summary>
		public override OleDbSqlServerMetadataCache DatabaseMetadata
		{
			get { return m_DatabaseMetadata; }
		}

		/// <summary>
		/// Rolls back a transaction if a Transact-SQL statement raises a run-time error.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xact")]
		public bool? XactAbort { get; }

		/// <summary>
		/// Creates a new transaction
		/// </summary>
		/// <param name="transactionName">optional name of the transaction.</param>
		/// <param name="isolationLevel">the isolation level. if not supplied, will use the database default.</param>
		/// <param name="forwardEvents">if true, logging events are forwarded to the parent connection.</param>
		/// <returns></returns>
		/// <remarks>
		/// the caller of this function is responsible for closing the transaction.
		/// </remarks>
		public virtual OleDbSqlServerTransactionalDataSource BeginTransaction(string? transactionName = null, IsolationLevel? isolationLevel = null, bool forwardEvents = true)
		{
			return new OleDbSqlServerTransactionalDataSource(this, transactionName, isolationLevel, forwardEvents);
		}

		/// <summary>
		/// Creates a new transaction
		/// </summary>
		/// <param name="transactionName">Name of the transaction.</param>
		/// <param name="isolationLevel">The isolation level.</param>
		/// <param name="forwardEvents">if set to <c>true</c> [forward events].</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<OleDbSqlServerTransactionalDataSource> BeginTransactionAsync(string? transactionName = null, IsolationLevel? isolationLevel = null, bool forwardEvents = true, CancellationToken cancellationToken = default)
		{
			var connection = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

			OleDbTransaction transaction;
			if (isolationLevel.HasValue)
				transaction = connection.BeginTransaction(isolationLevel.Value);
			else
				transaction = connection.BeginTransaction();
			return new OleDbSqlServerTransactionalDataSource(this, transactionName, forwardEvents, connection, transaction);
		}

		/// <summary>
		/// Gets the options that are currently in effect. This takes into account server-defined defaults.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public OleDbSqlServerEffectiveSettings GetEffectiveSettings()
		{
			var result = new OleDbSqlServerEffectiveSettings();
			using (var con = CreateConnection())
				result.Reload(con, null);
			return result;
		}

		/// <summary>
		/// Gets the options that are currently in effect. This takes into account server-defined defaults.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public async Task<OleDbSqlServerEffectiveSettings> GetEffectiveSettingsAsync()
		{
			var result = new OleDbSqlServerEffectiveSettings();
			using (var con = await CreateConnectionAsync().ConfigureAwait(false))
				await result.ReloadAsync(con, null).ConfigureAwait(false);
			return result;
		}



		/// <summary>
		/// Executes the specified operation.
		/// </summary>
		/// <param name="executionToken">The execution token.</param>
		/// <param name="implementation">The implementation that handles processing the result of the command.</param>
		/// <param name="state">User supplied state.</param>
		[SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
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
				using (var con = CreateConnection())
				{
					using (var cmd = new OleDbCommand())
					{
						cmd.Connection = con;
						if (DefaultCommandTimeout.HasValue)
							cmd.CommandTimeout = (int)DefaultCommandTimeout.Value.TotalSeconds;
						cmd.CommandText = executionToken.CommandText;
						cmd.CommandType = executionToken.CommandType;
						foreach (var param in executionToken.Parameters)
							cmd.Parameters.Add(param);

						executionToken.ApplyCommandOverrides(cmd);

						var rows = implementation(cmd);
						executionToken.RaiseCommandExecuted(cmd, rows);
						OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
						return rows;
					}
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
				using (var con = CreateConnection())
				{
					var rows = implementation(con, null);
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
				using (var con = CreateConnection())
				{
					var rows = await implementation(con, null, cancellationToken).ConfigureAwait(false);
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
				using (var con = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false))
				{
					using (var cmd = new OleDbCommand())
					{
						cmd.Connection = con;
						if (DefaultCommandTimeout.HasValue)
							cmd.CommandTimeout = (int)DefaultCommandTimeout.Value.TotalSeconds;
						cmd.CommandText = executionToken.CommandText;
						cmd.CommandType = executionToken.CommandType;
						foreach (var param in executionToken.Parameters)
							cmd.Parameters.Add(param);

						executionToken.ApplyCommandOverrides(cmd);

						var rows = await implementation(cmd).ConfigureAwait(false);
						executionToken.RaiseCommandExecuted(cmd, rows);
						OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
						return rows;
					}
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

		string? BuildConnectionSettingsOverride()
		{
			if (m_ServerDefaultSettings == null)
				throw new InvalidOperationException("m_ServerDefaultSettings was not set before building connection settings.");

			if (ArithAbort == null && XactAbort == null)
				return null;

			var sql = new StringBuilder();

			if (ArithAbort.HasValue && ArithAbort != m_ServerDefaultSettings.ArithAbort)
				sql.AppendLine("SET ARITHABORT " + (ArithAbort.Value ? "ON" : "OFF"));
			if (XactAbort.HasValue && XactAbort != m_ServerDefaultSettings.XactAbort)
				sql.AppendLine("SET XACT_ABORT  " + (XactAbort.Value ? "ON" : "OFF"));

			return sql.ToString();
		}



		/// <summary>
		/// Creates a new data source with the indicated changes to the settings.
		/// </summary>
		/// <param name="settings">The new settings to use.</param>
		/// <returns></returns>
		/// <remarks>The new data source will share the same database metadata cache.</remarks>
		public OleDbSqlServerDataSource WithSettings(SqlServerDataSourceSettings? settings)
		{
			var mergedSettings = new SqlServerDataSourceSettings()
			{
				DefaultCommandTimeout = settings?.DefaultCommandTimeout ?? DefaultCommandTimeout,
				SuppressGlobalEvents = settings?.SuppressGlobalEvents ?? SuppressGlobalEvents,
				StrictMode = settings?.StrictMode ?? StrictMode,
				SequentialAccessMode = settings?.SequentialAccessMode ?? SequentialAccessMode,
				XactAbort = settings?.XactAbort ?? XactAbort,
				ArithAbort = settings?.ArithAbort ?? ArithAbort
			};
			var result = new OleDbSqlServerDataSource(Name, m_ConnectionBuilder, mergedSettings, m_DatabaseMetadata, m_Cache, m_ExtensionCache);
			result.m_DatabaseMetadata = m_DatabaseMetadata;
			result.AuditRules = AuditRules;
			result.UserValue = UserValue;

			result.ExecutionStarted += (sender, e) => OnExecutionStarted(e);
			result.ExecutionFinished += (sender, e) => OnExecutionFinished(e);
			result.ExecutionError += (sender, e) => OnExecutionError(e);
			result.ExecutionCanceled += (sender, e) => OnExecutionCanceled(e);

			return result;
		}

	}
}
