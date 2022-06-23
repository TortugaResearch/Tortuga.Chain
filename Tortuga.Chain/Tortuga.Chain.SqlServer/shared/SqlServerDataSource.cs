using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.SqlServer;

namespace Tortuga.Chain;

/// <summary>
/// Class SqlServerDataSource.
/// </summary>
/// <seealso cref="SqlServerDataSourceBase" />
public partial class SqlServerDataSource : SqlServerDataSourceBase
{
	readonly object m_SyncRoot = new object();
	SqlServerMetadataCache m_DatabaseMetadata;
	bool m_IsSqlDependencyActive;

	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerDataSource" /> class.
	/// </summary>
	/// <param name="name">Name of the data source.</param>
	/// <param name="connectionString">The connection string.</param>
	/// <param name="settings">Optional settings object.</param>
	/// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
	public SqlServerDataSource(string? name, string connectionString, SqlServerDataSourceSettings? settings = null) : base(settings)
	{
		if (string.IsNullOrEmpty(connectionString))
			throw new ArgumentException("connectionString is null or empty.", nameof(connectionString));

		m_ConnectionBuilder = new SqlConnectionStringBuilder(connectionString);
		if (string.IsNullOrEmpty(name))
			Name = m_ConnectionBuilder.InitialCatalog ?? m_ConnectionBuilder.DataSource;
		else
			Name = name;

		m_DatabaseMetadata = new SqlServerMetadataCache(m_ConnectionBuilder);

		if (settings != null)
		{
			XactAbort = settings.XactAbort;
			ArithAbort = settings.ArithAbort;
		}

		m_ExtensionCache = new ConcurrentDictionary<Type, object>();
		m_Cache = DefaultCache;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerDataSource" /> class.
	/// </summary>
	/// <param name="connectionString">The connection string.</param>
	/// <param name="settings">Optional settings object.</param>
	/// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
	public SqlServerDataSource(string connectionString, SqlServerDataSourceSettings? settings = null)
		: this(null, connectionString, settings)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerDataSource" /> class.
	/// </summary>
	/// <param name="name">Optional name of the data source.</param>
	/// <param name="connectionStringBuilder">The connection string builder.</param>
	/// <param name="settings">Optional settings object.</param>
	/// <exception cref="ArgumentNullException">connectionStringBuilder;connectionStringBuilder is null.</exception>
	public SqlServerDataSource(string? name, SqlConnectionStringBuilder connectionStringBuilder, SqlServerDataSourceSettings? settings = null) : base(settings)
	{
		if (connectionStringBuilder == null)
			throw new ArgumentNullException(nameof(connectionStringBuilder), $"{nameof(connectionStringBuilder)} is null.");

		m_ConnectionBuilder = connectionStringBuilder;
		if (string.IsNullOrEmpty(name))
			Name = m_ConnectionBuilder.InitialCatalog ?? m_ConnectionBuilder.DataSource;
		else
			Name = name;

		m_DatabaseMetadata = new SqlServerMetadataCache(m_ConnectionBuilder);

		if (settings != null)
		{
			XactAbort = settings.XactAbort;
			ArithAbort = settings.ArithAbort;
		}
		m_ExtensionCache = new ConcurrentDictionary<Type, object>();
		m_Cache = DefaultCache;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerDataSource"/> class.
	/// </summary>
	/// <param name="connectionStringBuilder">The connection string builder.</param>
	/// <param name="settings">Optional settings object.</param>
	public SqlServerDataSource(SqlConnectionStringBuilder connectionStringBuilder, SqlServerDataSourceSettings? settings = null)
		: this(null, connectionStringBuilder, settings)
	{
	}

	SqlServerDataSource(string? name, SqlConnectionStringBuilder connectionStringBuilder, SqlServerDataSourceSettings settings, SqlServerMetadataCache databaseMetadata, ICacheAdapter cache, ConcurrentDictionary<Type, object> extensionCache) : base(settings)
	{
		if (connectionStringBuilder == null)
			throw new ArgumentNullException(nameof(connectionStringBuilder), $"{nameof(connectionStringBuilder)} is null.");

		m_ConnectionBuilder = connectionStringBuilder;
		if (string.IsNullOrEmpty(name))
			Name = m_ConnectionBuilder.InitialCatalog ?? m_ConnectionBuilder.DataSource;
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
	/// Terminates a query when an overflow or divide-by-zero error occurs during query execution.
	/// </summary>
	/// <remarks>Microsoft recommends setting ArithAbort=On for all connections. To avoid an additional round-trip to the server, do this at the server level instead of at the connection level.</remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Arith")]
	public bool? ArithAbort { get; }

	/// <summary>
	/// This object can be used to lookup database information.
	/// </summary>
	public override SqlServerMetadataCache DatabaseMetadata
	{
		get { return m_DatabaseMetadata; }
	}

	/// <summary>
	/// Gets a value indicating whether SQL dependency support is active for this dispatcher.
	/// </summary>
	/// <value><c>true</c> if this SQL dependency is active; otherwise, <c>false</c>.</value>
	public bool IsSqlDependencyActive
	{
		get { return m_IsSqlDependencyActive; }
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
	public virtual SqlServerTransactionalDataSource BeginTransaction(string? transactionName, IsolationLevel? isolationLevel = null, bool forwardEvents = true)
	{
		return new SqlServerTransactionalDataSource(this, transactionName, isolationLevel, forwardEvents);
	}

	/// <summary>
	/// Creates a new transaction
	/// </summary>
	/// <param name="transactionName">Name of the transaction.</param>
	/// <param name="isolationLevel">The isolation level.</param>
	/// <param name="forwardEvents">if set to <c>true</c> [forward events].</param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	public async Task<SqlServerTransactionalDataSource> BeginTransactionAsync(string? transactionName = null, IsolationLevel? isolationLevel = null, bool forwardEvents = true, CancellationToken cancellationToken = default)
	{
		var connection = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

		SqlTransaction transaction;
		if (isolationLevel.HasValue)
			transaction = connection.BeginTransaction(isolationLevel.Value);
		else
			transaction = connection.BeginTransaction();
		return new SqlServerTransactionalDataSource(this, transactionName, forwardEvents, connection, transaction);
	}

	/// <summary>
	/// Gets the options that are currently in effect. This takes into account server-defined defaults.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
	public SqlServerEffectiveSettings GetEffectiveSettings()
	{
		var result = new SqlServerEffectiveSettings();
		using (var con = CreateConnection())
			result.Reload(con, null);
		return result;
	}

	/// <summary>
	/// Gets the options that are currently in effect. This takes into account server-defined defaults.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
	public async Task<SqlServerEffectiveSettings> GetEffectiveSettingsAsync()
	{
		var result = new SqlServerEffectiveSettings();
		using (var con = await CreateConnectionAsync().ConfigureAwait(false))
			await result.ReloadAsync(con, null).ConfigureAwait(false);
		return result;
	}

	/// <summary>
	/// Starts SQL dependency on this connection string.
	/// </summary>
	/// <remarks>
	/// true if the listener initialized successfully; false if a compatible listener
	/// already exists.
	/// </remarks>
	public bool StartSqlDependency()
	{
		if (IsSqlDependencyActive)
			throw new InvalidOperationException("SQL Dependency is currently active");

		lock (m_SyncRoot)
		{
			m_IsSqlDependencyActive = true;
			return SqlDependency.Start(ConnectionString);
		}
	}

	/// <summary>
	/// Stops SQL dependency on this connection string.
	/// </summary>
	/// <remarks>
	/// true if the listener was completely stopped; false if the System.AppDomain
	/// was unbound from the listener, but there are is at least one other System.AppDomain
	/// using the same listener.
	/// </remarks>
	public bool StopSqlDependency()
	{
		if (!IsSqlDependencyActive)
			throw new InvalidOperationException("SQL Dependency is not currently active");

		lock (m_SyncRoot)
		{
			m_IsSqlDependencyActive = false;
			return SqlDependency.Stop(ConnectionString);
		}
	}

	/// <summary>
	/// Creates a new data source with the indicated changes to the settings.
	/// </summary>
	/// <param name="settings">The new settings to use.</param>
	/// <returns></returns>
	/// <remarks>The new data source will share the same database metadata cache.</remarks>
	public SqlServerDataSource WithSettings(SqlServerDataSourceSettings? settings)
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
		var result = new SqlServerDataSource(Name, m_ConnectionBuilder, mergedSettings, m_DatabaseMetadata, m_Cache, m_ExtensionCache);
		result.m_DatabaseMetadata = m_DatabaseMetadata;
		result.AuditRules = AuditRules;
		result.UserValue = UserValue;

		result.ExecutionStarted += (sender, e) => OnExecutionStarted(e);
		result.ExecutionFinished += (sender, e) => OnExecutionFinished(e);
		result.ExecutionError += (sender, e) => OnExecutionError(e);
		result.ExecutionCanceled += (sender, e) => OnExecutionCanceled(e);

		return result;
	}

	/// <summary>
	/// Executes the specified operation.
	/// </summary>
	/// <param name="executionToken">The execution token.</param>
	/// <param name="implementation">The implementation that handles processing the result of the command.</param>
	/// <param name="state">User supplied state.</param>
	[SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
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
			using (var con = CreateConnection())
			{
				using (var cmd = new SqlCommand())
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
			using (var con = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false))
			{
				using (var cmd = new SqlCommand())
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
		if (DatabaseMetadata.ServerDefaultSettings == null)
			throw new InvalidOperationException("m_ServerDefaultSettings was not set before building connection settings.");

		if (ArithAbort == null && XactAbort == null)
			return null;

		var sql = new StringBuilder();

		if (ArithAbort.HasValue && ArithAbort != DatabaseMetadata.ServerDefaultSettings.ArithAbort)
			sql.AppendLine("SET ARITHABORT " + (ArithAbort.Value ? "ON" : "OFF"));
		if (XactAbort.HasValue && XactAbort != DatabaseMetadata.ServerDefaultSettings.XactAbort)
			sql.AppendLine("SET XACT_ABORT  " + (XactAbort.Value ? "ON" : "OFF"));

		return sql.ToString();
	}
}