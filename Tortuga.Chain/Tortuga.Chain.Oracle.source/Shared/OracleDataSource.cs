using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Oracle;

namespace Tortuga.Chain
{
    /// <summary>
    /// Class OracleDataSource.
    /// </summary>
    /// <seealso cref="OracleDataSourceBase" />
    public class OracleDataSource : OracleDataSourceBase, IRootDataSource
    {
        readonly OracleConnectionStringBuilder m_ConnectionBuilder;
        private OracleMetadataCache m_DatabaseMetadata;

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="forwardEvents">if set to <c>true</c> [forward events].</param>
        /// <returns></returns>
        public OracleTransactionalDataSource BeginTransaction(IsolationLevel? isolationLevel = null, bool forwardEvents = true)
        {
            return new OracleTransactionalDataSource(this, isolationLevel, forwardEvents);
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="forwardEvents">if set to <c>true</c> [forward events].</param>
        /// <returns></returns>
        public async Task<OracleTransactionalDataSource> BeginTransactionAsync(IsolationLevel? isolationLevel = null, bool forwardEvents = true)
        {
            var connection = await CreateConnectionAsync();
            OracleTransaction transaction;
            if (isolationLevel.HasValue)
                transaction = connection.BeginTransaction(isolationLevel.Value);
            else
                transaction = connection.BeginTransaction();
            return new OracleTransactionalDataSource(this, forwardEvents, connection, transaction);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDataSource"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
        public OracleDataSource(string name, string connectionString, OracleDataSourceSettings settings = null)
            : base(settings)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("connectionString is null or empty.", "connectionString");

            m_ConnectionBuilder = new OracleConnectionStringBuilder(connectionString);
            if (string.IsNullOrEmpty(name))
                Name = m_ConnectionBuilder.DataSource;
            else
                Name = name;

            m_DatabaseMetadata = new OracleMetadataCache(m_ConnectionBuilder);
            m_ExtensionCache = new ConcurrentDictionary<Type, object>();
            m_Cache = DefaultCache;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDataSource"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="settings">The settings.</param>
        public OracleDataSource(string connectionString, OracleDataSourceSettings settings = null)
            : this(null, connectionString, settings)
        {
        }

        private OracleDataSource(string name, OracleConnectionStringBuilder connectionBuilder, OracleDataSourceSettings settings, OracleMetadataCache databaseMetadata, ICacheAdapter cache, ConcurrentDictionary<Type, object> extensionCache)
            : base(settings)
        {
            if (connectionBuilder == null)
                throw new ArgumentNullException(nameof(connectionBuilder), $"{nameof(connectionBuilder)} is null.");

            m_ConnectionBuilder = connectionBuilder;
            if (string.IsNullOrEmpty(name))
                Name = m_ConnectionBuilder.DataSource;
            else
                Name = name;

            m_DatabaseMetadata = databaseMetadata;
            m_ExtensionCache = extensionCache;
            m_Cache = cache;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDataSource" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="connectionBuilder">The connection builder.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public OracleDataSource(string name, OracleConnectionStringBuilder connectionBuilder, OracleDataSourceSettings settings = null)
            : base(settings)
        {
            if (connectionBuilder == null)
                throw new ArgumentNullException(nameof(connectionBuilder), $"{nameof(connectionBuilder)} is null.");

            m_ConnectionBuilder = connectionBuilder;
            if (string.IsNullOrEmpty(name))
                Name = m_ConnectionBuilder.DataSource;
            else
                Name = name;

            m_DatabaseMetadata = new OracleMetadataCache(m_ConnectionBuilder);
            m_ExtensionCache = new ConcurrentDictionary<Type, object>();
            m_Cache = DefaultCache;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDataSource" /> class.
        /// </summary>
        /// <param name="connectionBuilder">The connection builder.</param>
        /// <param name="settings">The settings.</param>
        public OracleDataSource(OracleConnectionStringBuilder connectionBuilder, OracleDataSourceSettings settings = null)
            : this(null, connectionBuilder, settings)
        {
        }

        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        /// <value>The database metadata.</value>
        public override OracleMetadataCache DatabaseMetadata
        {
            get { return m_DatabaseMetadata; }
        }

        /// <summary>
        /// Tests the connection.
        /// </summary>
        public override void TestConnection()
        {
            using (var con = CreateConnection())
            using (var cmd = new OracleCommand("SELECT 1", con))
                cmd.ExecuteScalar();
        }

        /// <summary>
        /// Tests the connection asynchronously.
        /// </summary>
        /// <returns></returns>
        public override async Task TestConnectionAsync()
        {
            using (var con = await CreateConnectionAsync())
            using (var cmd = new OracleCommand("SELECT 1", con))
                await cmd.ExecuteScalarAsync();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal OracleConnection CreateConnection()
        {
            var con = new OracleConnection(ConnectionString);
            con.Open();

            //TODO: Research server settings.

            return con;
        }

        private async Task<OracleConnection> CreateConnectionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var con = new OracleConnection(ConnectionString);
            await con.OpenAsync(cancellationToken).ConfigureAwait(false);

            //TODO: Research server settings.

            return con;
        }

        /// <summary>
        /// Creates a new data source with the indicated changes to the settings.
        /// </summary>
        /// <param name="settings">The new settings to use.</param>
        /// <returns></returns>
        /// <remarks>The new data source will share the same database metadata cache.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public OracleDataSource WithSettings(OracleDataSourceSettings settings)
        {
            var mergedSettings = new OracleDataSourceSettings()
            {
                DefaultCommandTimeout = settings?.DefaultCommandTimeout ?? DefaultCommandTimeout,
                SuppressGlobalEvents = settings?.SuppressGlobalEvents ?? SuppressGlobalEvents,
                StrictMode = settings?.StrictMode ?? StrictMode
            };
            var result = new OracleDataSource(Name, m_ConnectionBuilder, mergedSettings, m_DatabaseMetadata, m_Cache, m_ExtensionCache);
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
        public OracleDataSource WithRules(params AuditRule[] additionalRules)
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
        public OracleDataSource WithRules(IEnumerable<AuditRule> additionalRules)
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
        public OracleDataSource WithUser(object userValue)
        {
            var result = WithSettings(null);
            result.UserValue = userValue;
            return result;
        }

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
        public static OracleDataSource CreateFromConfig(string connectionName)
        {
            var settings = ConfigurationManager.ConnectionStrings[connectionName];
            if (settings == null)
                throw new InvalidOperationException("The configuration file does not contain a connection named " + connectionName);

            return new OracleDataSource(connectionName, settings.ConnectionString);
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
        protected override int? Execute(CommandExecutionToken<OracleCommand, OracleParameter> executionToken, CommandImplementation<OracleCommand> implementation, object state)
        {
            throw new NotImplementedException();
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
        protected override Task<int?> ExecuteAsync(CommandExecutionToken<OracleCommand, OracleParameter> executionToken, CommandImplementationAsync<OracleCommand> implementation, CancellationToken cancellationToken, object state)
        {
            throw new NotImplementedException();
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
        protected override int? Execute(OperationExecutionToken<OracleConnection, OracleTransaction> executionToken, OperationImplementation<OracleConnection, OracleTransaction> implementation, object state)
        {
            throw new NotImplementedException();
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
        protected override Task<int?> ExecuteAsync(OperationExecutionToken<OracleConnection, OracleTransaction> executionToken, OperationImplementationAsync<OracleConnection, OracleTransaction> implementation, CancellationToken cancellationToken, object state)
        {
            throw new NotImplementedException();
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
            return new OracleOpenDataSource(this, (OracleConnection)connection, (OracleTransaction)transaction);
        }

        ITransactionalDataSource IRootDataSource.BeginTransaction()
        {
            return BeginTransaction();
        }

        async Task<ITransactionalDataSource> IRootDataSource.BeginTransactionAsync()
        {
            return await BeginTransactionAsync();
        }
#endif

        /// <summary>
        /// Creates a new data source with the provided cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <returns></returns>
        public OracleDataSource WithCache(ICacheAdapter cache)
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
        /// Creates an open data source using the supplied connection and optional transaction.
        /// </summary>
        /// <param name="connection">The connection to wrap.</param>
        /// <param name="transaction">The transaction to wrap.</param>
        /// <returns>IOpenDataSource.</returns>
        public OracleOpenDataSource CreateOpenDataSource(OracleConnection connection, OracleTransaction transaction = null)
        {
            return new OracleOpenDataSource(this, connection, transaction);
        }

        IOpenDataSource IRootDataSource.CreateOpenDataSource(IDbConnection connection, IDbTransaction transaction)
        {
            return new OracleOpenDataSource(this, (OracleConnection)connection, (OracleTransaction)transaction);
        }
    }
}
