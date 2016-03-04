using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly object m_SyncRoot = new object();
        private bool m_IsSqlDependencyActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDataSource" /> class.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
        public SqlServerDataSource(string connectionName, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("connectionString is null or empty.", "connectionString");

            m_ConnectionBuilder = new SqlConnectionStringBuilder(connectionString);
            if (string.IsNullOrEmpty(connectionName))
                Name = m_ConnectionBuilder.InitialCatalog ?? m_ConnectionBuilder.DataSource;
            else
                Name = connectionName;

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
        /// <param name="connectionName">Optional name of the connection.</param>
        /// <param name="connectionStringBuilder">The connection string builder.</param>
        /// <exception cref="ArgumentNullException">connectionStringBuilder;connectionStringBuilder is null.</exception>
        public SqlServerDataSource(string connectionName, SqlConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder == null)
                throw new ArgumentNullException("connectionStringBuilder", "connectionStringBuilder is null.");

            m_ConnectionBuilder = connectionStringBuilder;
            if (string.IsNullOrEmpty(connectionName))
                Name = m_ConnectionBuilder.InitialCatalog ?? m_ConnectionBuilder.DataSource;
            else
                Name = connectionName;

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
        /// When not null, this will set the ArithAbort for new connections.
        /// </summary>
        /// <remarks>Microsoft recommends setting ArithAbort=On for all connections. To avoid an additional round-trip to the server, do this at the server level instead of at the connection level.</remarks>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Arith")]
        public bool? ArithAbort { get; set; }

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
        /// When not null, this will set the XACT_ABORT for new connections.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xact")]
        public bool? XactAbort { get; set; }

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
        public SqlConnection CreateSqlConnection()
        {
            var con = new SqlConnection(ConnectionString);
            con.Open();

            if (ArithAbort.HasValue)
                using (var cmd = new SqlCommand("SET ARITHABORT " + (ArithAbort.Value ? "ON" : "OFF"), con))
                    cmd.ExecuteNonQuery();

            if (XactAbort.HasValue)
                using (var cmd = new SqlCommand("SET XACT_ABORT  " + (XactAbort.Value ? "ON" : "OFF"), con))
                    cmd.ExecuteNonQuery();

            return con;

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
            using (var con = CreateSqlConnection())
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
                using (var con = CreateSqlConnection())
                {
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = executionToken.CommandText;
                        cmd.CommandType = executionToken.CommandType;
                        foreach (var param in executionToken.Parameters)
                            cmd.Parameters.Add(param);

                        var rows = implementation(cmd);
                        OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
                    }
                }
            }
            catch (SqlException ex)
            {
                ex.Data["DataSource"] = Name;
                ex.Data["Operation"] = executionToken.OperationName;
                ex.Data["CommandText"] = executionToken.CommandText;
                ex.Data["Parameters"] = executionToken.Parameters;
                OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
                throw;
            }
            catch (SqlTypeException ex)
            {
                ex.Data["DataSource"] = Name;
                ex.Data["Operation"] = executionToken.OperationName;
                ex.Data["CommandText"] = executionToken.CommandText;
                ex.Data["Parameters"] = executionToken.Parameters;
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
                        cmd.CommandText = executionToken.CommandText;
                        cmd.CommandType = executionToken.CommandType;
                        foreach (var param in executionToken.Parameters)
                            cmd.Parameters.Add(param);
                        var rows = await implementation(cmd).ConfigureAwait(false);
                        OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
                    }
                }
            }
            catch (SqlException ex)
            {
                if (cancellationToken.IsCancellationRequested) //convert SqlException into a OperationCanceledException 
                {
                    var ex2 = new OperationCanceledException("Operation was canceled.", ex, cancellationToken);
                    ex2.Data["DataSource"] = Name;
                    ex2.Data["Operation"] = executionToken.OperationName;
                    ex2.Data["CommandText"] = executionToken.CommandText;
                    ex2.Data["Parameters"] = executionToken.Parameters;
                    OnExecutionCanceled(executionToken, startTime, DateTimeOffset.Now, state);
                    throw ex2;
                }
                else
                {
                    ex.Data["DataSource"] = Name;
                    ex.Data["Operation"] = executionToken.OperationName;
                    ex.Data["CommandText"] = executionToken.CommandText;
                    ex.Data["Parameters"] = executionToken.Parameters;
                    OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
                    throw;
                }
            }
            catch (SqlTypeException ex)
            {
                ex.Data["Dispatcher"] = Name;
                ex.Data["Operation"] = executionToken.OperationName;
                ex.Data["CommandText"] = executionToken.CommandText;
                ex.Data["Parameters"] = executionToken.Parameters;
                OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
                throw;
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
        private async Task<SqlConnection> CreateSqlConnectionAsync(CancellationToken cancellationToken)
        {
            var con = new SqlConnection(ConnectionString);
            await con.OpenAsync(cancellationToken).ConfigureAwait(false);

            if (ArithAbort.HasValue)
                using (var cmd = new SqlCommand("set ARITHABORT " + (ArithAbort.Value ? "ON" : "OFF"), con))
                    await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

            if (XactAbort.HasValue)
                using (var cmd = new SqlCommand("set XACT_ABORT  " + (XactAbort.Value ? "ON" : "OFF"), con))
                    await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

            return con;
        }
    }
}
