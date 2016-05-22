using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Access;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;



namespace Tortuga.Chain
{
    /// <summary>
    /// Class that represents a Access Data Source.
    /// </summary>
    public class AccessDataSource : AccessDataSourceBase, IRootDataSource
    {
        readonly OleDbConnectionStringBuilder m_ConnectionBuilder;
        private AccessMetadataCache m_DatabaseMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessDataSource" /> class.
        /// </summary>
        /// <param name="name">The name of the data source.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="settings">Optional settings object.</param>
        /// <exception cref="ArgumentException">Connection string is null or emtpy.;connectionString</exception>
        public AccessDataSource(string name, string connectionString, AccessDataSourceSettings settings = null) : base(settings)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Connection string is null or emtpy.", "connectionString");

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
        public AccessDataSource(string connectionString, AccessDataSourceSettings settings = null)
            : this(null, connectionString, settings)
        {
        }

        private AccessDataSource(string name, OleDbConnectionStringBuilder connectionStringBuilder, AccessDataSourceSettings settings, AccessMetadataCache databaseMetadata, ICacheAdapter cache, ConcurrentDictionary<Type, object> extensionCache) : base(settings)
        {
            if (connectionStringBuilder == null)
                throw new ArgumentNullException("connectionStringBuilder", "connectionStringBuilder is null.");

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
        /// Initializes a new instance of the <see cref="AccessDataSource" /> class.
        /// </summary>
        /// <param name="name">The name of the data source.</param>
        /// <param name="connectionStringBuilder">The connection string builder.</param>
        /// <param name="settings">Optional settings object.</param>
        /// <exception cref="ArgumentNullException">connectionStringBuilder;connectionStringBuilder is null.</exception>
        public AccessDataSource(string name, OleDbConnectionStringBuilder connectionStringBuilder, AccessDataSourceSettings settings = null) : base(settings)
        {
            if (connectionStringBuilder == null)
                throw new ArgumentNullException("connectionStringBuilder", "connectionStringBuilder is null.");

            m_ConnectionBuilder = connectionStringBuilder;
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
        public AccessDataSource(OleDbConnectionStringBuilder connectionStringBuilder, AccessDataSourceSettings settings = null)
         : this(null, connectionStringBuilder, settings)
        {
        }

        /// <summary>
        /// This object can be used to lookup database information.
        /// </summary>
        public override AccessMetadataCache DatabaseMetadata
        {
            get { return m_DatabaseMetadata; }
        }

        /// <summary>
        /// This object can be used to access the database connection string.
        /// </summary>
        internal string ConnectionString
        {
            get { return m_ConnectionBuilder.ConnectionString; }
        }



#if !WINDOWS_UWP
        /// <summary>
        /// Creates a new connection using the connection string in the app.config file.
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public static AccessDataSource CreateFromConfig(string connectionName)
        {
            var settings = ConfigurationManager.ConnectionStrings[connectionName];
            if (settings == null)
                throw new InvalidOperationException("The configuration file does not contain a connection named " + connectionName);

            return new AccessDataSource(connectionName, settings.ConnectionString);
        }
#endif

        /// <summary>
        /// Creates and opens a new Access connection
        /// </summary>
        /// <returns></returns>
        /// <remarks>The caller of this method is responsible for closing the connection.</remarks>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal OleDbConnection CreateConnection()
        {
            var con = new OleDbConnection(ConnectionString);
            con.Open();

            //TODO: Research any potential PRAGMA/Rollback options

            return con;
        }

        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <param name="forwardEvents"></param>
        /// <returns></returns>
        /// <remarks>The caller of this method is responsible for closing the transaction.</remarks>
        public AccessTransactionalDataSource BeginTransaction(IsolationLevel? isolationLevel = null, bool forwardEvents = true)
        {
            var connection = CreateConnection();
            OleDbTransaction transaction;
            if (isolationLevel == null)
                transaction = connection.BeginTransaction();
            else
                transaction = connection.BeginTransaction(isolationLevel.Value);

            return new AccessTransactionalDataSource(this, forwardEvents, connection, transaction);
        }

        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <param name="forwardEvents"></param>
        /// <returns></returns>
        /// <remarks>The caller of this method is responsible for closing the transaction.</remarks>
        public async Task<AccessTransactionalDataSource> BeginTransactionAsync(IsolationLevel? isolationLevel = null, bool forwardEvents = true)
        {
            var connection = await CreateConnectionAsync();
            OleDbTransaction transaction;
            if (isolationLevel == null)
                transaction = connection.BeginTransaction();
            else
                transaction = connection.BeginTransaction(isolationLevel.Value);

            return new AccessTransactionalDataSource(this, forwardEvents, connection, transaction);
        }





        private async Task<OleDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var con = new OleDbConnection(ConnectionString);
            await con.OpenAsync(cancellationToken).ConfigureAwait(false);

            //TODO: Add in needed PRAGMA statements

            return con;
        }

        /// <summary>
        /// Creates a new data source with the indicated changes to the settings.
        /// </summary>
        /// <param name="settings">The new settings to use.</param>
        /// <returns></returns>
        /// <remarks>The new data source will share the same database metadata cache.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public AccessDataSource WithSettings(AccessDataSourceSettings settings)
        {
            var mergedSettings = new AccessDataSourceSettings()
            {
                DefaultCommandTimeout = settings?.DefaultCommandTimeout ?? DefaultCommandTimeout,
                SuppressGlobalEvents = settings?.SuppressGlobalEvents ?? SuppressGlobalEvents,
                StrictMode = settings?.StrictMode ?? StrictMode
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
        /// Creates a new data source with additional audit rules.
        /// </summary>
        /// <param name="additionalRules">The additional rules.</param>
        /// <returns></returns>
        public AccessDataSource WithRules(params AuditRule[] additionalRules)
        {
            var result = WithSettings(null);
            result.AuditRules = new AuditRuleCollection(AuditRules, additionalRules);
            return result;
        }

        /// <summary>
        /// Creates a new data source with additional audit rules.
        /// </summary>
        /// <param name="additionalRules">The additional rules.</param>
        /// <returns></returns>
        public AccessDataSource WithRules(IEnumerable<AuditRule> additionalRules)
        {
            var result = WithSettings(null);
            result.AuditRules = new AuditRuleCollection(AuditRules, additionalRules);
            return result;
        }

        /// <summary>
        /// Creates a new data source with the indicated user.
        /// </summary>
        /// <param name="userValue">The user value.</param>
        /// <returns></returns>
        /// <remarks>
        /// This is used in conjunction with audit rules.
        /// </remarks>
        public AccessDataSource WithUser(object userValue)
        {
            var result = WithSettings(null);
            result.UserValue = userValue;
            return result;
        }





        /// <summary>
        /// Tests the connection.
        /// </summary>
        public override void TestConnection()
        {
            using (var con = CreateConnection())
            using (var cmd = new OleDbCommand("SELECT 1", con))
                cmd.ExecuteScalar();
        }

        /// <summary>
        /// Tests the connection asynchronously.
        /// </summary>
        /// <returns></returns>
        public override async Task TestConnectionAsync()
        {
            using (var con = await CreateConnectionAsync())
            using (var cmd = new OleDbCommand("SELECT 1", con))
                await cmd.ExecuteScalarAsync();
        }

        DbConnection IRootDataSource.CreateConnection()
        {
            return CreateConnection();
        }

        async Task<DbConnection> IRootDataSource.CreateConnectionAsync()
        {
            return await CreateConnectionAsync();
        }

        IOpenDataSource IRootDataSource.CreateOpenDataSource(DbConnection connection, DbTransaction transaction)
        {
            return new AccessOpenDataSource(this, (OleDbConnection)connection, (OleDbTransaction)transaction);
        }

        ITransactionalDataSource IRootDataSource.BeginTransaction()
        {
            return BeginTransaction();
        }

        async Task<ITransactionalDataSource> IRootDataSource.BeginTransactionAsync()
        {
            return await BeginTransactionAsync();
        }

        /// <summary>
        /// Craetes a new data source with the provided cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <returns></returns>
        public AccessDataSource WithCache(ICacheAdapter cache)
        {
            var result = WithSettings(null);
            result.m_Cache = cache;
            return result;
        }

        internal ICacheAdapter m_Cache;

        /// <summary>
        /// Gets or sets the cache to be used by this data source. The default is .NET's System.Runtime.Caching.MemoryCache.
        /// </summary>
        public override ICacheAdapter Cache
        {
            get { return m_Cache; }
        }

        internal ConcurrentDictionary<Type, object> m_ExtensionCache;

        /// <summary>
        /// The extension cache is used by extensions to store data source specific information.
        /// </summary>
        /// <value>
        /// The extension cache.
        /// </value>
        protected override ConcurrentDictionary<Type, object> ExtensionCache
        {
            get { return m_ExtensionCache; }
        }

        /// <summary>
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionToken"></param>
        /// <param name="implementation"></param>
        /// <param name="state"></param>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected override int? Execute(CommandExecutionToken<OleDbCommand, OleDbParameter> executionToken, CommandImplementation<OleDbCommand> implementation, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");
            var currentToken = executionToken as AccessCommandExecutionToken;
            if (currentToken == null)
                throw new ArgumentNullException("executionToken", "only AccessCommandExecutionToken is supported.");

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
                            if (DefaultCommandTimeout.HasValue)
                                cmd.CommandTimeout = (int)DefaultCommandTimeout.Value.TotalSeconds;
                            cmd.CommandText = currentToken.CommandText;
                            cmd.CommandType = currentToken.CommandType;
                            foreach (var param in currentToken.Parameters)
                                cmd.Parameters.Add(param);

                            currentToken.ApplyCommandOverrides(cmd);

                            if (currentToken.ExecutionMode == AccessCommandExecutionMode.Materializer)
                                rows = implementation(cmd);
                            else if (currentToken.ExecutionMode == AccessCommandExecutionMode.ExecuteScalarAndForward)
                                currentToken.ForwardResult(cmd.ExecuteScalar());
                            else
                                rows = cmd.ExecuteNonQuery();
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
        protected override int? Execute(OperationExecutionToken<OleDbConnection, OleDbTransaction> executionToken, OperationImplementation<OleDbConnection, OleDbTransaction> implementation, object state)
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
        protected override async Task<int?> ExecuteAsync(CommandExecutionToken<OleDbCommand, OleDbParameter> executionToken, CommandImplementationAsync<OleDbCommand> implementation, CancellationToken cancellationToken, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");
            var currentToken = executionToken as AccessCommandExecutionToken;
            if (currentToken == null)
                throw new ArgumentNullException("executionToken", "only AccessCommandExecutionToken is supported.");

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
                            if (DefaultCommandTimeout.HasValue)
                                cmd.CommandTimeout = (int)DefaultCommandTimeout.Value.TotalSeconds;
                            cmd.CommandText = currentToken.CommandText;
                            cmd.CommandType = currentToken.CommandType;
                            foreach (var param in currentToken.Parameters)
                                cmd.Parameters.Add(param);

                            currentToken.ApplyCommandOverrides(cmd);

                            if (currentToken.ExecutionMode == AccessCommandExecutionMode.Materializer)
                                rows = await implementation(cmd);
                            else if (currentToken.ExecutionMode == AccessCommandExecutionMode.ExecuteScalarAndForward)
                                currentToken.ForwardResult(await cmd.ExecuteScalarAsync());
                            else
                                rows = await cmd.ExecuteNonQueryAsync();
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
        protected override async Task<int?> ExecuteAsync(OperationExecutionToken<OleDbConnection, OleDbTransaction> executionToken, OperationImplementationAsync<OleDbConnection, OleDbTransaction> implementation, CancellationToken cancellationToken, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

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
}
