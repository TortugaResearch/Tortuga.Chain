using System.Collections.Concurrent;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Access;
using Tortuga.Chain.Core;

namespace Tortuga.Chain;

/// <summary>
/// Class that represents a Access Data Source.
/// </summary>
public partial class AccessDataSource : AccessDataSourceBase
{
	AccessMetadataCache m_DatabaseMetadata;

	/// <summary>
	/// Initializes a new instance of the <see cref="AccessDataSource" /> class.
	/// </summary>
	/// <param name="name">The name of the data source.</param>
	/// <param name="connectionString">The connection string.</param>
	/// <param name="settings">Optional settings object.</param>
	/// <exception cref="ArgumentException">Connection string is null or empty.;connectionString</exception>
	public AccessDataSource(string? name, string connectionString, AccessDataSourceSettings? settings = null) : base(settings)
	{
		if (string.IsNullOrEmpty(connectionString))
			throw new ArgumentException($"{nameof(connectionString)} is null or empty.", nameof(connectionString));

		m_ConnectionBuilder = new OleDbConnectionStringBuilder(connectionString);
		if (string.IsNullOrEmpty(name))
			Name = m_ConnectionBuilder.DataSource;
		else
			Name = name;

		m_DatabaseMetadata = new AccessMetadataCache(m_ConnectionBuilder);
		m_ExtensionCache = new ConcurrentDictionary<Type, object>();
		m_Cache = DefaultCache;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AccessDataSource" /> class.
	/// </summary>
	/// <param name="connectionString"></param>
	/// <param name="settings">Optional settings object.</param>
	public AccessDataSource(string connectionString, AccessDataSourceSettings? settings = null)
		: this(null, connectionString, settings)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AccessDataSource" /> class.
	/// </summary>
	/// <param name="name">The name of the data source.</param>
	/// <param name="connectionStringBuilder">The connection string builder.</param>
	/// <param name="settings">Optional settings object.</param>
	/// <exception cref="ArgumentNullException">connectionStringBuilder;connectionStringBuilder is null.</exception>
	public AccessDataSource(string? name, OleDbConnectionStringBuilder connectionStringBuilder, AccessDataSourceSettings? settings = null) : base(settings)
	{
		m_ConnectionBuilder = connectionStringBuilder ?? throw new ArgumentNullException(nameof(connectionStringBuilder), $"{nameof(connectionStringBuilder)} is null.");

		if (string.IsNullOrEmpty(name))
			Name = m_ConnectionBuilder.DataSource;
		else
			Name = name;

		m_DatabaseMetadata = new AccessMetadataCache(m_ConnectionBuilder);
		m_ExtensionCache = new ConcurrentDictionary<Type, object>();
		m_Cache = DefaultCache;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AccessDataSource" /> class.
	/// </summary>
	/// <param name="connectionStringBuilder"></param>
	/// <param name="settings">Optional settings object.</param>
	public AccessDataSource(OleDbConnectionStringBuilder connectionStringBuilder, AccessDataSourceSettings? settings = null)
	 : this(null, connectionStringBuilder, settings)
	{
	}

	AccessDataSource(string? name, OleDbConnectionStringBuilder connectionStringBuilder, AccessDataSourceSettings? settings, AccessMetadataCache databaseMetadata, ICacheAdapter cache, ConcurrentDictionary<Type, object> extensionCache) : base(settings)
	{
		if (connectionStringBuilder == null)
			throw new ArgumentNullException(nameof(connectionStringBuilder), $"{nameof(connectionStringBuilder)} is null.");

		m_ConnectionBuilder = connectionStringBuilder;
		if (string.IsNullOrEmpty(name))
			Name = m_ConnectionBuilder.DataSource;
		else
			Name = name;

		m_DatabaseMetadata = databaseMetadata;
		m_ExtensionCache = extensionCache;
		m_Cache = cache;
	}

	/// <summary>
	/// This object can be used to lookup database information.
	/// </summary>
	public override AccessMetadataCache DatabaseMetadata => m_DatabaseMetadata;

	/// <summary>
	/// Executes the stream.
	/// </summary>
	/// <param name="executionToken">The execution token.</param>
	/// <param name="implementation">The implementation.</param>
	/// <param name="state">The state.</param>
	/// <returns>StreamingCommandCompletionToken.</returns>
	/// <exception cref="System.ArgumentNullException">executionToken</exception>
	/// <exception cref="System.ArgumentNullException">implementation</exception>
	/// <exception cref="System.ArgumentNullException">executionToken - only AccessCommandExecutionToken is supported.</exception>
	/// <exception cref="System.InvalidOperationException">currentToken.ExecutionMode is ExecuteScalarAndForward, but currentToken.ForwardResult is null.</exception>
	public override StreamingCommandCompletionToken ExecuteStream(CommandExecutionToken<OleDbCommand, OleDbParameter> executionToken, StreamingCommandImplementation<OleDbCommand> implementation, object? state)
	{
		if (executionToken == null)
			throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
		if (implementation == null)
			throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");
		var currentToken = executionToken as AccessCommandExecutionToken;
		if (currentToken == null)
			throw new ArgumentNullException(nameof(executionToken), "only AccessCommandExecutionToken is supported.");

		var startTime = DateTimeOffset.Now;

		OleDbConnection? con = null;
		try
		{
			con = CreateConnection();

			OleDbCommand? cmdToReturn = null;
			while (currentToken != null)
			{
				OnExecutionStarted(currentToken, startTime, state);
				var cmd = new OleDbCommand();

				cmd.Connection = con;
				currentToken.PopulateCommand(cmd, DefaultCommandTimeout);

				if (currentToken.ExecutionMode == AccessCommandExecutionMode.Materializer)
				{
					implementation(cmd);
					cmdToReturn = cmd;
				}
				else if (currentToken.ExecutionMode == AccessCommandExecutionMode.ExecuteScalarAndForward)
				{
					if (currentToken.ForwardResult == null)
						throw new InvalidOperationException("currentToken.ExecutionMode is ExecuteScalarAndForward, but currentToken.ForwardResult is null.");

					currentToken.ForwardResult(cmd.ExecuteScalar());
				}
				else
					cmd.ExecuteNonQuery();

				currentToken = currentToken.NextCommand;
			}

			return new StreamingCommandCompletionToken(this, executionToken, startTime, state, cmdToReturn, con);
		}
		catch (Exception ex)
		{
			con?.Dispose();
			OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
			throw;
		}
	}

	/// <summary>
	/// Execute stream as an asynchronous operation.
	/// </summary>
	/// <param name="executionToken">The execution token.</param>
	/// <param name="implementation">The implementation.</param>
	/// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
	/// <param name="state">The state.</param>
	/// <returns>A Task&lt;StreamingCommandCompletionToken&gt; representing the asynchronous operation.</returns>
	/// <exception cref="System.ArgumentNullException">executionToken</exception>
	/// <exception cref="System.ArgumentNullException">implementation</exception>
	/// <exception cref="System.ArgumentNullException">executionToken - only AccessCommandExecutionToken is supported.</exception>
	/// <exception cref="System.InvalidOperationException">currentToken.ExecutionMode is ExecuteScalarAndForward, but currentToken.ForwardResult is null.</exception>
	public override async Task<StreamingCommandCompletionToken> ExecuteStreamAsync(CommandExecutionToken<OleDbCommand, OleDbParameter> executionToken, StreamingCommandImplementationAsync<OleDbCommand> implementation, CancellationToken cancellationToken, object? state)
	{
		if (executionToken == null)
			throw new ArgumentNullException(nameof(executionToken), $"{nameof(executionToken)} is null.");
		if (implementation == null)
			throw new ArgumentNullException(nameof(implementation), $"{nameof(implementation)} is null.");
		var currentToken = executionToken as AccessCommandExecutionToken;
		if (currentToken == null)
			throw new ArgumentNullException(nameof(executionToken), "only AccessCommandExecutionToken is supported.");

		var startTime = DateTimeOffset.Now;

		OleDbConnection? con = null;
		try
		{
			con = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

			OleDbCommand? cmdToReturn = null;
			while (currentToken != null)
			{
				OnExecutionStarted(currentToken, startTime, state);
				using (var cmd = new OleDbCommand())
				{
					cmd.Connection = con;
					currentToken.PopulateCommand(cmd, DefaultCommandTimeout);

					if (currentToken.ExecutionMode == AccessCommandExecutionMode.Materializer)
					{
						await implementation(cmd).ConfigureAwait(false);
						cmdToReturn = cmd;
					}
					else if (currentToken.ExecutionMode == AccessCommandExecutionMode.ExecuteScalarAndForward)
					{
						if (currentToken.ForwardResult == null)
							throw new InvalidOperationException("currentToken.ExecutionMode is ExecuteScalarAndForward, but currentToken.ForwardResult is null.");

						currentToken.ForwardResult(await cmd.ExecuteScalarAsync().ConfigureAwait(false));
					}
					else
						await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
				}
				currentToken = currentToken.NextCommand;
			}

			return new StreamingCommandCompletionToken(this, executionToken, startTime, state, cmdToReturn, con);
		}
		catch (Exception ex)
		{
#if NET6_0_OR_GREATER
		if (con != null)
		    await con.DisposeAsync().ConfigureAwait(false);
#else
			con?.Dispose();
#endif

			if (cancellationToken.IsCancellationRequested) //convert Exception into a OperationCanceledException
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
	/// Creates a new data source with the indicated changes to the settings.
	/// </summary>
	/// <param name="settings">The new settings to use.</param>
	/// <returns></returns>
	/// <remarks>The new data source will share the same database metadata cache.</remarks>
	[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
	public AccessDataSource WithSettings(AccessDataSourceSettings? settings)
	{
		var mergedSettings = new AccessDataSourceSettings()
		{
			DefaultCommandTimeout = settings?.DefaultCommandTimeout ?? DefaultCommandTimeout,
			SuppressGlobalEvents = settings?.SuppressGlobalEvents ?? SuppressGlobalEvents,
			StrictMode = settings?.StrictMode ?? StrictMode,
			SequentialAccessMode = settings?.SequentialAccessMode ?? SequentialAccessMode
		};
		var result = new AccessDataSource(Name, m_ConnectionBuilder, mergedSettings, m_DatabaseMetadata, m_Cache, m_ExtensionCache);
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
	/// <param name="executionToken"></param>
	/// <param name="implementation"></param>
	/// <param name="state"></param>
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

		try
		{
			using (var con = CreateConnection())
			{
				int? rows = null;
				while (currentToken != null)
				{
					OnExecutionStarted(currentToken, startTime, state);
					using (var cmd = new OleDbCommand())
					{
						cmd.Connection = con;
						currentToken.PopulateCommand(cmd, DefaultCommandTimeout);

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
	/// Executes the specified operation asynchronously.
	/// </summary>
	/// <param name="executionToken"></param>
	/// <param name="implementation"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="state"></param>
	/// <returns></returns>
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

		try
		{
			using (var con = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false))
			{
				int? rows = null;
				while (currentToken != null)
				{
					OnExecutionStarted(currentToken, startTime, state);
					using (var cmd = new OleDbCommand())
					{
						cmd.Connection = con;
						currentToken.PopulateCommand(cmd, DefaultCommandTimeout);

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
		}
		catch (Exception ex)
		{
			if (cancellationToken.IsCancellationRequested) //convert Exception into a OperationCanceledException
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
