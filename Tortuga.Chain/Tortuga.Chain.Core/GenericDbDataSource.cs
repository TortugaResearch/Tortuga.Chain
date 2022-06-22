using System.Collections.Concurrent;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain
{
	/// <summary>
	/// The GenericDbDataSource is the most simplistic of all of the data sources. The command builder only supports raw SQL, but you still have access to all of the materializers.
	/// </summary>
	public class GenericDbDataSource : DataSource<DbConnection, DbTransaction, DbCommand, DbParameter>, ISupportsSqlQueries
	{
		readonly ICacheAdapter m_Cache;
		readonly DbConnectionStringBuilder m_ConnectionBuilder;
		readonly ConcurrentDictionary<Type, object> m_ExtensionCache;
		readonly DbProviderFactory? m_Factory;

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericDbDataSource{TConnection, TCommand, TParameter}" /> class.
		/// </summary>
		/// <param name="factory">The factory used to get provider specific objects.</param>
		/// <param name="name">Name of the data source.</param>
		/// <param name="connectionString">The connection string.</param>
		/// <param name="settings">Optional settings object.</param>
		/// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
		/// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
		public GenericDbDataSource(DbProviderFactory factory, string name, string connectionString, DataSourceSettings? settings = null) : base(settings)
		{
			if (string.IsNullOrEmpty(connectionString))
				throw new ArgumentException($"{nameof(connectionString)} is null or empty.", nameof(connectionString));

			m_Factory = factory ?? throw new ArgumentNullException(nameof(factory), $"{nameof(factory)} is null.");
			m_ConnectionBuilder = factory.CreateConnectionStringBuilder() ?? throw new ArgumentException($"{nameof(factory)}.CreateConnectionStringBuilder returned a null.", nameof(factory));
			m_ConnectionBuilder.ConnectionString = connectionString;
			Name = name;
			m_ExtensionCache = new ConcurrentDictionary<Type, object>();
			m_Cache = DefaultCache;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericDbDataSource" /> class.
		/// </summary>
		/// <param name="factory">The factory.</param>
		/// <param name="name">The name.</param>
		/// <param name="connectionStringBuilder">The connection string builder.</param>
		/// <param name="settings">Optional settings object.</param>
		/// <exception cref="ArgumentNullException">factory;factory is null.
		/// or
		/// connectionStringBuilder;connectionStringBuilder is null.</exception>
		public GenericDbDataSource(DbProviderFactory factory, string name, DbConnectionStringBuilder connectionStringBuilder, DataSourceSettings? settings = null) : base(settings)
		{
			m_Factory = factory ?? throw new ArgumentNullException(nameof(factory));
			m_ConnectionBuilder = connectionStringBuilder ?? throw new ArgumentNullException(nameof(connectionStringBuilder));
			Name = name;
			m_ExtensionCache = new ConcurrentDictionary<Type, object>();
			m_Cache = DefaultCache;
		}

		internal GenericDbDataSource(string? name, string connectionString, DataSourceSettings? settings = null) : base(settings)
		{
			if (string.IsNullOrEmpty(connectionString))
				throw new ArgumentException($"{nameof(connectionString)} is null or empty.", nameof(connectionString));

			m_ConnectionBuilder = new DbConnectionStringBuilder() { ConnectionString = connectionString };
			Name = name;
			m_ExtensionCache = new ConcurrentDictionary<Type, object>();
			m_Cache = DefaultCache;
		}

		internal GenericDbDataSource(string? name, DbConnectionStringBuilder connectionStringBuilder, DataSourceSettings? settings = null) : base(settings)
		{
			m_ConnectionBuilder = connectionStringBuilder ?? throw new ArgumentNullException(nameof(connectionStringBuilder));
			Name = name;
			m_ExtensionCache = new ConcurrentDictionary<Type, object>();
			m_Cache = DefaultCache;
		}

		/// <summary>
		/// Gets or sets the cache to be used by this data source. The default is .NET's System.Runtime.Caching.MemoryCache.
		/// </summary>
		public override ICacheAdapter Cache => m_Cache;

		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <value>
		/// The connection string.
		/// </value>
		internal string ConnectionString => m_ConnectionBuilder.ConnectionString;

		/// <summary>
		/// The extension cache is used by extensions to store data source specific information.
		/// </summary>
		/// <value>
		/// The extension cache.
		/// </value>
		protected override ConcurrentDictionary<Type, object> ExtensionCache => m_ExtensionCache;

		/// <summary>
		/// Creates and opens a SQL connection.
		/// </summary>
		/// <returns></returns>
		/// <remarks>The caller of this method is responsible for closing the connection.</remarks>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public DbConnection CreateConnection()
		{
			var con = OnCreateConnection();
			con.ConnectionString = ConnectionString;
			con.Open();

			return con;
		}

		/// <summary>
		/// Creates and opens a SQL connection.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		/// <remarks>
		/// The caller of this method is responsible for closing the connection.
		/// </remarks>
		public async Task<DbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
		{
			var con = OnCreateConnection();
			con.ConnectionString = ConnectionString;
			await con.OpenAsync(cancellationToken).ConfigureAwait(false);
			return con;
		}

		/// <summary>
		/// Creates a operation based on a raw SQL statement.
		/// </summary>
		/// <param name="sqlStatement">The SQL statement.</param>
		/// <returns></returns>
		public MultipleTableDbCommandBuilder<DbCommand, DbParameter> Sql(string sqlStatement) => new GenericDbSqlCall(this, sqlStatement, null);

		/// <summary>
		/// Creates a operation based on a raw SQL statement.
		/// </summary>
		/// <param name="sqlStatement">The SQL statement.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <returns>SqlServerSqlCall.</returns>
		public MultipleTableDbCommandBuilder<DbCommand, DbParameter> Sql(string sqlStatement, object argumentValue) => new GenericDbSqlCall(this, sqlStatement, argumentValue);

		IMultipleTableDbCommandBuilder ISupportsSqlQueries.Sql(string sqlStatement, object argumentValue) => Sql(sqlStatement, argumentValue);

		/// <summary>
		/// Tests the connection.
		/// </summary>
		public override void TestConnection()
		{
			using (var con = CreateConnection())
			using (var cmd = CreateCommand())
			{
				cmd.Connection = con;
				cmd.CommandText = "SELECT 1";
				cmd.ExecuteScalar();
			}
		}

		/// <summary>
		/// Tests the connection asynchronously.
		/// </summary>
		/// <returns></returns>
		public override async Task TestConnectionAsync()
		{
			using (var con = await CreateConnectionAsync().ConfigureAwait(false))
			using (var cmd = CreateCommand())
			{
				cmd.Connection = con;
				cmd.CommandText = "SELECT 1";
				await cmd.ExecuteScalarAsync().ConfigureAwait(false);
			}
		}

		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GenericDbDataSource")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbProviderFactory")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CreateCommand")]
		internal virtual DbCommand CreateCommand()
		{
			if (m_Factory == null)
				throw new InvalidOperationException("Subclasses of GenericDbDataSource that do not provide a DbProviderFactory need to override CreateCommand");
			return m_Factory.CreateCommand() ?? throw new InvalidOperationException($"CreateCommand on factory of type {m_Factory.GetType().Name} returned a null.");
		}

		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GenericDbDataSource")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbProviderFactory")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CreateParameter")]
		internal virtual DbParameter CreateParameter()
		{
			if (m_Factory == null)
				throw new InvalidOperationException("Subclasses of GenericDbDataSource that do not provide a DbProviderFactory need to override CreateParameter");
			return m_Factory.CreateParameter() ?? throw new InvalidOperationException($"CreateParameter on factory of type {m_Factory.GetType().Name} returned a null.");
		}

		/// <summary>
		/// Creates an empty connection to be populated by CreateConnection.
		/// </summary>
		/// <returns>DbConnection.</returns>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "OnCreateConnection")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GenericDbDataSource")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DbProviderFactory")]
		internal virtual DbConnection OnCreateConnection()
		{
			if (m_Factory == null)
				throw new InvalidOperationException("Subclasses of GenericDbDataSource that do not provide a DbProviderFactory need to override OnCreateConnection");
			return m_Factory.CreateConnection() ?? throw new InvalidOperationException($"CreateConnection on factory of type {m_Factory.GetType().Name} returned a null.");
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
		protected internal override int? Execute(CommandExecutionToken<DbCommand, DbParameter> executionToken, CommandImplementation<DbCommand> implementation, object? state)
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
					using (var cmd = CreateCommand())
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
		/// <exception cref="ArgumentNullException">
		/// executionToken;executionToken is null.
		/// or
		/// implementation;implementation is null.
		/// </exception>
		protected internal override int? Execute(OperationExecutionToken<DbConnection, DbTransaction> executionToken, OperationImplementation<DbConnection, DbTransaction> implementation, object? state)
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
		/// Executes the operation asynchronously.
		/// </summary>
		/// <param name="executionToken">The execution token.</param>
		/// <param name="implementation">The implementation that handles processing the result of the command.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">User supplied state.</param>
		/// <returns>Task.</returns>
		protected internal override async Task<int?> ExecuteAsync(CommandExecutionToken<DbCommand, DbParameter> executionToken, CommandImplementationAsync<DbCommand> implementation, CancellationToken cancellationToken, object? state)
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
					using (var cmd = CreateCommand())
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

		/// <summary>
		/// execute as an asynchronous operation.
		/// </summary>
		/// <param name="executionToken">The execution token.</param>
		/// <param name="implementation">The implementation.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="state">The state.</param>
		/// <returns>Task.</returns>
		protected internal override async Task<int?> ExecuteAsync(OperationExecutionToken<DbConnection, DbTransaction> executionToken, OperationImplementationAsync<DbConnection, DbTransaction> implementation, CancellationToken cancellationToken, object? state)
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

		/// <summary>
		/// Called when Database.DatabaseMetadata is invoked.
		/// </summary>
		/// <returns></returns>
		protected override IDatabaseMetadataCache OnGetDatabaseMetadata()
		{
			throw new NotSupportedException("This data source does not expose database metadata");
		}
	}
}
