using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Core;
using Tortuga.Chain.SqlServer;
namespace Tortuga.Chain
{

    /// <summary>
    /// Class SqlServerDataSource.
    /// </summary>
    /// <seealso cref="SqlServerDataSourceBase" />
    public class SqlServerDataSource : SqlServerDataSourceBase
    {
        private readonly SqlConnectionStringBuilder m_ConnectionBuilder;
        private readonly SqlServerMetadataCache m_DatabaseMetadata;

        /// <summary>
        /// This is used to decide which option overides to set when establishing a connection.
        /// </summary>
        private SqlServerEffectiveSettings m_ServerDefaultSettings;
        private readonly object m_SyncRoot = new object();
        private bool m_IsSqlDependencyActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDataSource" /> class.
        /// </summary>
        /// <param name="name">Name of the data source.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
        public SqlServerDataSource(string name, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("connectionString is null or empty.", "connectionString");

            m_ConnectionBuilder = new SqlConnectionStringBuilder(connectionString);
            if (string.IsNullOrEmpty(name))
                Name = m_ConnectionBuilder.InitialCatalog ?? m_ConnectionBuilder.DataSource;
            else
                Name = name;

            m_DatabaseMetadata = new SqlServerMetadataCache(m_ConnectionBuilder);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDataSource" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
        public SqlServerDataSource(string connectionString)
            : this(null, connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDataSource" /> class.
        /// </summary>
        /// <param name="name">Optional name of the data source.</param>
        /// <param name="connectionStringBuilder">The connection string builder.</param>
        /// <exception cref="ArgumentNullException">connectionStringBuilder;connectionStringBuilder is null.</exception>
        public SqlServerDataSource(string name, SqlConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder == null)
                throw new ArgumentNullException("connectionStringBuilder", "connectionStringBuilder is null.");

            m_ConnectionBuilder = connectionStringBuilder;
            if (string.IsNullOrEmpty(name))
                Name = m_ConnectionBuilder.InitialCatalog ?? m_ConnectionBuilder.DataSource;
            else
                Name = name;

            m_DatabaseMetadata = new SqlServerMetadataCache(m_ConnectionBuilder);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDataSource"/> class.
        /// </summary>
        /// <param name="connectionStringBuilder">The connection string builder.</param>
        public SqlServerDataSource(SqlConnectionStringBuilder connectionStringBuilder)
            : this(null, connectionStringBuilder)
        {
        }



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
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        internal string ConnectionString
        {
            get { return m_ConnectionBuilder.ConnectionString; }
        }

        /// <summary>
        /// Creates a new connection using the connection string settings in the app.config file.
        /// </summary>
        /// <param name="connectionName"></param>
        public static SqlServerDataSource CreateFromConfig(string connectionName)
        {
            var settings = ConfigurationManager.ConnectionStrings[connectionName];
            if (settings == null)
                throw new InvalidOperationException("The configuration file does not contain a connection named " + connectionName);

            return new SqlServerDataSource(connectionName, settings.ConnectionString);
        }

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
        public virtual SqlServerTransactionalDataSource BeginTransaction(string transactionName = null, IsolationLevel? isolationLevel = null, bool forwardEvents = true)
        {
            return new SqlServerTransactionalDataSource(this, transactionName, isolationLevel, forwardEvents);
        }
        /// <summary>
        /// Creates and opens a SQL connection.
        /// </summary>
        /// <returns></returns>
        /// <remarks>The caller of this method is responsible for closing the connection.</remarks>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal SqlConnection CreateConnection()
        {

            var con = new SqlConnection(ConnectionString);
            con.Open();

            if (m_ServerDefaultSettings == null)
            {
                var temp = new SqlServerEffectiveSettings();
                temp.Reload(con, null);
                Thread.MemoryBarrier();
                m_ServerDefaultSettings = temp;
            }

            var sql = BuildConnectionSettingsOverride();

            if (sql.Length > 0)
                using (var cmd = new SqlCommand(sql.ToString(), con))
                    cmd.ExecuteNonQuery();

            return con;
        }

        private string BuildConnectionSettingsOverride()
        {
            var sql = new StringBuilder();

            if (Settings.ArithAbort.HasValue && Settings.ArithAbort != m_ServerDefaultSettings.ArithAbort)
                sql.AppendLine("SET ARITHABORT " + (Settings.ArithAbort.Value ? "ON" : "OFF"));
            if (Settings.XactAbort.HasValue && Settings.XactAbort != m_ServerDefaultSettings.XactAbort)
                sql.AppendLine("SET XACT_ABORT  " + (Settings.XactAbort.Value ? "ON" : "OFF"));

            return sql.ToString();
        }

        /// <summary>
        /// Starts SQL dependency.
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
        /// Stops SQL dependency.
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
        /// Tests the connection.
        /// </summary>
        public void TestConnection()
        {
            using (var con = CreateConnection())
            {
                using (var cmd = new SqlCommand("SELECT 1", con))
                    cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="state">User supplied state.</param>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected override void Execute(ExecutionToken<SqlCommand, SqlParameter> executionToken, Func<SqlCommand, int?> implementation, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

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
                        OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
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
        /// execute the operation asynchronously.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User supplied state.</param>
        /// <returns>Task.</returns>
        protected override async Task ExecuteAsync(ExecutionToken<SqlCommand, SqlParameter> executionToken, Func<SqlCommand, Task<int?>> implementation, CancellationToken cancellationToken, object state)
        {
            var startTime = DateTimeOffset.Now;
            OnExecutionStarted(executionToken, startTime, state);

            try
            {
                using (var con = await CreateSqlConnectionAsync(cancellationToken).ConfigureAwait(false))
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
                        OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
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
        /// Creates and opens a SQL connection.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <remarks>
        /// The caller of this method is responsible for closing the connection.
        /// </remarks>
        private async Task<SqlConnection> CreateSqlConnectionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var con = new SqlConnection(ConnectionString);
            await con.OpenAsync(cancellationToken).ConfigureAwait(false);

            if (m_ServerDefaultSettings == null)
            {
                var temp = new SqlServerEffectiveSettings();
                await temp.ReloadAsync(con, null);
                Thread.MemoryBarrier();
                m_ServerDefaultSettings = temp;
            }

            var sql = BuildConnectionSettingsOverride();

            if (sql.Length > 0)
                using (var cmd = new SqlCommand(sql.ToString(), con))
                    await cmd.ExecuteNonQueryAsync();

            return con;
        }

        /// <summary>
        /// Gets the options set by the client. If an option is null, the server-defined defaults will be in effect.
        /// </summary>
        /// <value>The options.</value>
        public SqlServerSettings Settings { get; } = new SqlServerSettings();

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
            using (var con = await CreateSqlConnectionAsync())
                await result.ReloadAsync(con, null);
            return result;
        }

    }


}
