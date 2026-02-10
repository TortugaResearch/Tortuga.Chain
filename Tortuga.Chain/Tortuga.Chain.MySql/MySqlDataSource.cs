using MySqlConnector;
using System.Collections.Concurrent;
using Tortuga.Chain.Core;
using Tortuga.Chain.MySql;

namespace Tortuga.Chain;

/// <summary>
/// Class MySqlDataSource.
/// </summary>
/// <seealso cref="MySqlDataSourceBase" />
public partial class MySqlDataSource : MySqlDataSourceBase
{
	MySqlMetadataCache m_DatabaseMetadata;

	/// <summary>
	/// Initializes a new instance of the <see cref="MySqlDataSource"/> class.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <param name="connectionString">The connection string.</param>
	/// <param name="settings">The settings.</param>
	/// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
	public MySqlDataSource(string? name, string connectionString, MySqlDataSourceSettings? settings = null)
		: base(settings)
	{
		if (string.IsNullOrEmpty(connectionString))
			throw new ArgumentException($"{nameof(connectionString)} is null or empty.", nameof(connectionString));

		m_ConnectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);

		if (string.IsNullOrEmpty(name))
			Name = m_ConnectionStringBuilder.Database;
		else
			Name = name;

		m_DatabaseMetadata = new MySqlMetadataCache(m_ConnectionStringBuilder);
		m_ExtensionCache = new ConcurrentDictionary<Type, object>();
		m_Cache = DefaultCache;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MySqlDataSource"/> class.
	/// </summary>
	/// <param name="connectionString">The connection string.</param>
	/// <param name="settings">The settings.</param>
	public MySqlDataSource(string connectionString, MySqlDataSourceSettings? settings = null)
		: this(null, connectionString, settings)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MySqlDataSource" /> class.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <param name="connectionBuilder">The connection builder.</param>
	/// <param name="settings">The settings.</param>
	/// <exception cref="ArgumentNullException"></exception>
	public MySqlDataSource(string? name, MySqlConnectionStringBuilder connectionBuilder, MySqlDataSourceSettings? settings = null)
		: base(settings)
	{
		if (connectionBuilder == null)
			throw new ArgumentNullException(nameof(connectionBuilder), $"{nameof(connectionBuilder)} is null.");

		m_ConnectionStringBuilder = connectionBuilder;
		if (string.IsNullOrEmpty(name))
			Name = m_ConnectionStringBuilder.Database;
		else
			Name = name;

		m_DatabaseMetadata = new MySqlMetadataCache(m_ConnectionStringBuilder);
		m_ExtensionCache = new ConcurrentDictionary<Type, object>();
		m_Cache = DefaultCache;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MySqlDataSource" /> class.
	/// </summary>
	/// <param name="connectionBuilder">The connection builder.</param>
	/// <param name="settings">The settings.</param>
	public MySqlDataSource(MySqlConnectionStringBuilder connectionBuilder, MySqlDataSourceSettings? settings = null)
		: this(null, connectionBuilder, settings)
	{
	}

	MySqlDataSource(string? name, MySqlConnectionStringBuilder connectionBuilder, MySqlDataSourceSettings settings, MySqlMetadataCache databaseMetadata, ICacheAdapter cache, ConcurrentDictionary<Type, object> extensionCache)
				: base(settings)
	{
		if (connectionBuilder == null)
			throw new ArgumentNullException(nameof(connectionBuilder), $"{nameof(connectionBuilder)} is null.");

		m_ConnectionStringBuilder = connectionBuilder;
		if (string.IsNullOrEmpty(name))
			Name = m_ConnectionStringBuilder.Database;
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
	public override MySqlMetadataCache DatabaseMetadata
	{
		get { return m_DatabaseMetadata; }
	}

	/// <summary>
	/// Executes the specified implementation.
	/// </summary>
	/// <param name="executionToken">The execution token.</param>
	/// <param name="implementation">The implementation.</param>
	/// <param name="state">The state.</param>
	/// <returns>The caller is expected to use the StreamingCommandCompletionToken to close any lingering connections and fire appropriate events.</returns>
	/// <exception cref="System.NotImplementedException"></exception>
	[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
	public override StreamingCommandCompletionToken ExecuteStream(CommandExecutionToken<MySqlCommand, MySqlParameter> executionToken, StreamingCommandImplementation<MySqlCommand> implementation, object? state)
	{
		if (executionToken == null)
			throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
		if (implementation == null)
			throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

		var startTime = DateTimeOffset.Now;
		OnExecutionStarted(executionToken, startTime, state);

		MySqlConnection? con = null;
		try
		{
			con = CreateConnection();

			var cmd = new MySqlCommand();

			cmd.Connection = con;
			executionToken.PopulateCommand(cmd, DefaultCommandTimeout);

			implementation(cmd);

			return new StreamingCommandCompletionToken(this, executionToken, startTime, state, cmd, con);
		}
		catch (Exception ex)
		{
			con?.Dispose();
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
	[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
	public override async Task<StreamingCommandCompletionToken> ExecuteStreamAsync(CommandExecutionToken<MySqlCommand, MySqlParameter> executionToken, StreamingCommandImplementationAsync<MySqlCommand> implementation, CancellationToken cancellationToken, object? state)
	{
		if (executionToken == null)
			throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
		if (implementation == null)
			throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");

		var startTime = DateTimeOffset.Now;
		OnExecutionStarted(executionToken, startTime, state);

		MySqlConnection? con = null;
		try
		{
			con = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

			var cmd = new MySqlCommand();

			cmd.Connection = con;
			executionToken.PopulateCommand(cmd, DefaultCommandTimeout);

			await implementation(cmd).ConfigureAwait(false);

			return new StreamingCommandCompletionToken(this, executionToken, startTime, state, cmd, con);
		}
		catch (Exception ex)
		{
			if (con != null)
				await con.DisposeAsync().ConfigureAwait(false);

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
	/// Creates a new data source with the indicated changes to the settings.
	/// </summary>
	/// <param name="settings">The new settings to use.</param>
	/// <returns></returns>
	/// <remarks>The new data source will share the same database metadata cache.</remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
	public MySqlDataSource WithSettings(MySqlDataSourceSettings? settings)
	{
		var mergedSettings = new MySqlDataSourceSettings()
		{
			DefaultCommandTimeout = settings?.DefaultCommandTimeout ?? DefaultCommandTimeout,
			SuppressGlobalEvents = settings?.SuppressGlobalEvents ?? SuppressGlobalEvents,
			StrictMode = settings?.StrictMode ?? StrictMode,
			SequentialAccessMode = settings?.SequentialAccessMode ?? SequentialAccessMode
		};
		var result = new MySqlDataSource(Name, m_ConnectionStringBuilder, mergedSettings, m_DatabaseMetadata, m_Cache, m_ExtensionCache);
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
			using (var con = CreateConnection())
			{
				using (var cmd = new MySqlCommand())
				{
					cmd.Connection = con;
					executionToken.PopulateCommand(cmd, DefaultCommandTimeout);

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
	/// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
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
			using (var con = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false))
			{
				using (var cmd = new MySqlCommand())
				{
					cmd.Connection = con;
					executionToken.PopulateCommand(cmd, DefaultCommandTimeout);

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
			using (var con = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false))
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
