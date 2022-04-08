using Npgsql;
using System.Collections.Concurrent;
using Tortuga.Chain.Core;
using Tortuga.Chain.PostgreSql;

namespace Tortuga.Chain;

/// <summary>
/// Class PostgreSqlDataSource.
/// </summary>
/// <seealso cref="PostgreSqlDataSourceBase" />
public partial class PostgreSqlDataSource : PostgreSqlDataSourceBase
{
	PostgreSqlMetadataCache m_DatabaseMetadata;

	/// <summary>
	/// Initializes a new instance of the <see cref="PostgreSqlDataSource"/> class.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <param name="connectionString">The connection string.</param>
	/// <param name="settings">The settings.</param>
	/// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
	public PostgreSqlDataSource(string? name, string connectionString, PostgreSqlDataSourceSettings? settings = null)
		: base(settings)
	{
		if (string.IsNullOrEmpty(connectionString))
			throw new ArgumentException($"{nameof(connectionString)} is null or empty.", nameof(connectionString));

		m_ConnectionBuilder = new NpgsqlConnectionStringBuilder(connectionString);
		if (string.IsNullOrEmpty(name))
			Name = m_ConnectionBuilder.Database;
		else
			Name = name;

		m_DatabaseMetadata = new PostgreSqlMetadataCache(m_ConnectionBuilder);
		m_ExtensionCache = new ConcurrentDictionary<Type, object>();
		m_Cache = DefaultCache;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="PostgreSqlDataSource"/> class.
	/// </summary>
	/// <param name="connectionString">The connection string.</param>
	/// <param name="settings">The settings.</param>
	public PostgreSqlDataSource(string connectionString, PostgreSqlDataSourceSettings? settings = null)
		: this(null, connectionString, settings)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="PostgreSqlDataSource" /> class.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <param name="connectionBuilder">The connection builder.</param>
	/// <param name="settings">The settings.</param>
	/// <exception cref="ArgumentNullException"></exception>
	public PostgreSqlDataSource(string? name, NpgsqlConnectionStringBuilder connectionBuilder, PostgreSqlDataSourceSettings? settings = null)
		: base(settings)
	{
		m_ConnectionBuilder = connectionBuilder ?? throw new ArgumentNullException(nameof(connectionBuilder), $"{nameof(connectionBuilder)} is null.");
		if (string.IsNullOrEmpty(name))
			Name = m_ConnectionBuilder.Database;
		else
			Name = name;

		m_DatabaseMetadata = new PostgreSqlMetadataCache(m_ConnectionBuilder);
		m_ExtensionCache = new ConcurrentDictionary<Type, object>();
		m_Cache = DefaultCache;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="PostgreSqlDataSource" /> class.
	/// </summary>
	/// <param name="connectionBuilder">The connection builder.</param>
	/// <param name="settings">The settings.</param>
	public PostgreSqlDataSource(NpgsqlConnectionStringBuilder connectionBuilder, PostgreSqlDataSourceSettings? settings = null)
		: this(null, connectionBuilder, settings)
	{
	}

	PostgreSqlDataSource(string? name, NpgsqlConnectionStringBuilder connectionBuilder, PostgreSqlDataSourceSettings settings, PostgreSqlMetadataCache databaseMetadata, ICacheAdapter cache, ConcurrentDictionary<Type, object> extensionCache)
				: base(settings)
	{
		m_ConnectionBuilder = connectionBuilder ?? throw new ArgumentNullException(nameof(connectionBuilder), $"{nameof(connectionBuilder)} is null.");
		if (string.IsNullOrEmpty(name))
			Name = m_ConnectionBuilder.Database;
		else
			Name = name;

		m_DatabaseMetadata = databaseMetadata;
		m_ExtensionCache = extensionCache;
		m_Cache = cache;
	}

	/// <summary>
	/// Gets the database metadata.
	/// </summary>
	/// <value>The database metadata.</value>
	public override PostgreSqlMetadataCache DatabaseMetadata => m_DatabaseMetadata;

	/// <summary>
	/// Creates a new data source with the indicated changes to the settings.
	/// </summary>
	/// <param name="settings">The new settings to use.</param>
	/// <returns></returns>
	/// <remarks>The new data source will share the same database metadata cache.</remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
	public PostgreSqlDataSource WithSettings(PostgreSqlDataSourceSettings? settings)
	{
		var mergedSettings = new PostgreSqlDataSourceSettings()
		{
			DefaultCommandTimeout = settings?.DefaultCommandTimeout ?? DefaultCommandTimeout,
			SuppressGlobalEvents = settings?.SuppressGlobalEvents ?? SuppressGlobalEvents,
			StrictMode = settings?.StrictMode ?? StrictMode,
			SequentialAccessMode = settings?.SequentialAccessMode ?? SequentialAccessMode
		};
		var result = new PostgreSqlDataSource(Name, m_ConnectionBuilder, mergedSettings, m_DatabaseMetadata, m_Cache, m_ExtensionCache);
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
	/// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
	/// <exception cref="ArgumentNullException">executionToken;executionToken is null.
	/// or
	/// implementation;implementation is null.</exception>
	protected override int? Execute(CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> executionToken, CommandImplementation<NpgsqlCommand> implementation, object? state)
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
				using (var cmd = new NpgsqlCommand())
				{
					cmd.Connection = con;
					if (DefaultCommandTimeout.HasValue)
						cmd.CommandTimeout = (int)DefaultCommandTimeout.Value.TotalSeconds;
					cmd.CommandText = executionToken.CommandText;
					cmd.CommandType = executionToken.CommandType;
					foreach (var param in executionToken.Parameters)
						cmd.Parameters.Add(param);

					executionToken.ApplyCommandOverrides(cmd);

					int? rows;

					if (((PostgreSqlCommandExecutionToken)executionToken).DereferenceCursors)
						rows = DereferenceCursors(cmd, implementation);
					else
						rows = implementation(cmd);

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
	/// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
	/// <exception cref="ArgumentNullException">
	/// executionToken;executionToken is null.
	/// or
	/// implementation;implementation is null.
	/// </exception>
	protected override int? Execute(OperationExecutionToken<NpgsqlConnection, NpgsqlTransaction> executionToken, OperationImplementation<NpgsqlConnection, NpgsqlTransaction> implementation, object? state)
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
	/// execute as an asynchronous operation.
	/// </summary>
	/// <param name="executionToken">The execution token.</param>
	/// <param name="implementation">The implementation that handles processing the result of the command.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="state">User supplied state.</param>
	/// <returns>Task.</returns>
	/// <exception cref="ArgumentNullException">
	/// executionToken;executionToken is null.
	/// or
	/// implementation;implementation is null.
	/// </exception>
	protected override async Task<int?> ExecuteAsync(CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> executionToken, CommandImplementationAsync<NpgsqlCommand> implementation, CancellationToken cancellationToken, object? state)
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
				using (var cmd = new NpgsqlCommand())
				{
					cmd.Connection = con;
					if (DefaultCommandTimeout.HasValue)
						cmd.CommandTimeout = (int)DefaultCommandTimeout.Value.TotalSeconds;
					cmd.CommandText = executionToken.CommandText;
					cmd.CommandType = executionToken.CommandType;
					foreach (var param in executionToken.Parameters)
						cmd.Parameters.Add(param);

					executionToken.ApplyCommandOverrides(cmd);

					int? rows;
					if (((PostgreSqlCommandExecutionToken)executionToken).DereferenceCursors)
						rows = await DereferenceCursorsAsync(cmd, implementation).ConfigureAwait(false);
					else
						rows = await implementation(cmd).ConfigureAwait(false);

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
	protected override async Task<int?> ExecuteAsync(OperationExecutionToken<NpgsqlConnection, NpgsqlTransaction> executionToken, OperationImplementationAsync<NpgsqlConnection, NpgsqlTransaction> implementation, CancellationToken cancellationToken, object? state)
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
}
