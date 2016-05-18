using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.SQLite;
using Tortuga.Chain.Core;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.DataSources;

#if !WINDOWS_UWP
using System.Configuration;
using System.Data.Common;
using Nito.AsyncEx;
using System.Collections.Concurrent;
#endif

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
using SQLiteConnection = Microsoft.Data.Sqlite.SqliteConnection;
using SQLiteConnectionStringBuilder = Microsoft.Data.Sqlite.SqliteConnectionStringBuilder;
#endif


namespace Tortuga.Chain
{
    /// <summary>
    /// Class that represents a SQLite Data Source.
    /// </summary>
    public class SQLiteDataSource : SQLiteDataSourceBase, IRootDataSource
    {
        readonly AsyncReaderWriterLock m_SyncLock = new AsyncReaderWriterLock(); //Sqlite is single-threaded for writes. It says otherwise, but it spams the trace window with exceptions.

        readonly SQLiteConnectionStringBuilder m_ConnectionBuilder;
        private SQLiteMetadataCache m_DatabaseMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteDataSource" /> class.
        /// </summary>
        /// <param name="name">The name of the data source.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="settings">Optional settings object.</param>
        /// <exception cref="ArgumentException">Connection string is null or emtpy.;connectionString</exception>
        public SQLiteDataSource(string name, string connectionString, SQLiteDataSourceSettings settings = null) : base(settings)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Connection string is null or emtpy.", "connectionString");

            m_ConnectionBuilder = new SQLiteConnectionStringBuilder(connectionString);
            if (string.IsNullOrEmpty(name))
                Name = m_ConnectionBuilder.DataSource;
            else
                Name = name;

            m_DatabaseMetadata = new SQLiteMetadataCache(m_ConnectionBuilder);
            m_ExtensionCache = new ConcurrentDictionary<Type, object>();
            m_Cache = DefaultCache;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteDataSource" /> class.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="settings">Optional settings object.</param>
        public SQLiteDataSource(string connectionString, SQLiteDataSourceSettings settings = null)
            : this(null, connectionString, settings)
        {
        }

        private SQLiteDataSource(string name, SQLiteConnectionStringBuilder connectionStringBuilder, SQLiteDataSourceSettings settings, SQLiteMetadataCache databaseMetadata, ICacheAdapter cache, ConcurrentDictionary<Type, object> extensionCache) : base(settings)
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
        /// Initializes a new instance of the <see cref="SQLiteDataSource" /> class.
        /// </summary>
        /// <param name="name">The name of the data source.</param>
        /// <param name="connectionStringBuilder">The connection string builder.</param>
        /// <param name="settings">Optional settings object.</param>
        /// <exception cref="ArgumentNullException">connectionStringBuilder;connectionStringBuilder is null.</exception>
        public SQLiteDataSource(string name, SQLiteConnectionStringBuilder connectionStringBuilder, SQLiteDataSourceSettings settings = null) : base(settings)
        {
            if (connectionStringBuilder == null)
                throw new ArgumentNullException("connectionStringBuilder", "connectionStringBuilder is null.");

            m_ConnectionBuilder = connectionStringBuilder;
            if (string.IsNullOrEmpty(name))
                Name = m_ConnectionBuilder.DataSource;
            else
                Name = name;

            m_DatabaseMetadata = new SQLiteMetadataCache(m_ConnectionBuilder);
            m_ExtensionCache = new ConcurrentDictionary<Type, object>();
            m_Cache = DefaultCache;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteDataSource" /> class.
        /// </summary>
        /// <param name="connectionStringBuilder"></param>
        /// <param name="settings">Optional settings object.</param>
        public SQLiteDataSource(SQLiteConnectionStringBuilder connectionStringBuilder, SQLiteDataSourceSettings settings = null)
         : this(null, connectionStringBuilder, settings)
        {
        }

        /// <summary>
        /// This object can be used to lookup database information.
        /// </summary>
        public override SQLiteMetadataCache DatabaseMetadata
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

        internal override AsyncReaderWriterLock SyncLock
        {
            get { return m_SyncLock; }
        }

#if !WINDOWS_UWP
        /// <summary>
        /// Creates a new connection using the connection string in the app.config file.
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public static SQLiteDataSource CreateFromConfig(string connectionName)
        {
            var settings = ConfigurationManager.ConnectionStrings[connectionName];
            if (settings == null)
                throw new InvalidOperationException("The configuration file does not contain a connection named " + connectionName);

            return new SQLiteDataSource(connectionName, settings.ConnectionString);
        }
#endif

        /// <summary>
        /// Creates and opens a new SQLite connection
        /// </summary>
        /// <returns></returns>
        /// <remarks>The caller of this method is responsible for closing the connection.</remarks>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal SQLiteConnection CreateConnection()
        {
            var con = new SQLiteConnection(ConnectionString);
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
        public SQLiteTransactionalDataSource BeginTransaction(IsolationLevel? isolationLevel = null, bool forwardEvents = true)
        {
            IDisposable lockToken = null;
            if (!DisableLocks)
                lockToken = SyncLock.WriterLock();

            var connection = CreateConnection();
            SQLiteTransaction transaction;
            if (isolationLevel == null)
                transaction = connection.BeginTransaction();
            else
                transaction = connection.BeginTransaction(isolationLevel.Value);

            return new SQLiteTransactionalDataSource(this, forwardEvents, connection, transaction, lockToken);
        }

        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <param name="forwardEvents"></param>
        /// <returns></returns>
        /// <remarks>The caller of this method is responsible for closing the transaction.</remarks>
        public async Task<SQLiteTransactionalDataSource> BeginTransactionAsync(IsolationLevel? isolationLevel = null, bool forwardEvents = true)
        {
            IDisposable lockToken = null;
            if (!DisableLocks)
                lockToken = await SyncLock.WriterLockAsync();

            var connection = await CreateConnectionAsync();
            SQLiteTransaction transaction;
            if (isolationLevel == null)
                transaction = connection.BeginTransaction();
            else
                transaction = connection.BeginTransaction(isolationLevel.Value);

            return new SQLiteTransactionalDataSource(this, forwardEvents, connection, transaction, lockToken);
        }

        /// <summary>
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionToken"></param>
        /// <param name="implementation"></param>
        /// <param name="state"></param>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected override int? Execute(CommandExecutionToken<SQLiteCommand, SQLiteParameter> executionToken, CommandImplementation<SQLiteCommand> implementation, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

            var mode = DisableLocks ? LockType.None : (executionToken as SQLiteCommandExecutionToken)?.LockType ?? LockType.Write;

            var startTime = DateTimeOffset.Now;
            OnExecutionStarted(executionToken, startTime, state);

            IDisposable lockToken = null;
            try
            {
                switch (mode)
                {
                    case LockType.Read: lockToken = SyncLock.ReaderLock(); break;
                    case LockType.Write: lockToken = SyncLock.WriterLock(); break;
                }

                using (var con = CreateConnection())
                {
                    using (var cmd = new SQLiteCommand())
                    {
                        cmd.Connection = con;
                        if (DefaultCommandTimeout.HasValue)
                            cmd.CommandTimeout = (int)DefaultCommandTimeout.Value.TotalSeconds;
                        cmd.CommandText = executionToken.CommandText;
                        //TODO: add potential check for this type.
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
            finally
            {
                if (lockToken != null)
                    lockToken.Dispose();
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
        protected override async Task<int?> ExecuteAsync(CommandExecutionToken<SQLiteCommand, SQLiteParameter> executionToken, CommandImplementationAsync<SQLiteCommand> implementation, CancellationToken cancellationToken, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

            var mode = DisableLocks ? LockType.None : (executionToken as SQLiteCommandExecutionToken)?.LockType ?? LockType.Write;

            var startTime = DateTimeOffset.Now;
            OnExecutionStarted(executionToken, startTime, state);

            IDisposable lockToken = null;
            try
            {
                switch (mode)
                {
                    case LockType.Read: lockToken = await SyncLock.ReaderLockAsync().ConfigureAwait(false); break;
                    case LockType.Write: lockToken = await SyncLock.WriterLockAsync().ConfigureAwait(false); break;
                }

                using (var con = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false))
                {
                    using (var cmd = new SQLiteCommand())
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
                if (cancellationToken.IsCancellationRequested) //convert SQLiteException into a OperationCanceledException 
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
            finally
            {
                if (lockToken != null)
                    lockToken.Dispose();
            }
        }

        private async Task<SQLiteConnection> CreateConnectionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var con = new SQLiteConnection(ConnectionString);
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
        public SQLiteDataSource WithSettings(SQLiteDataSourceSettings settings)
        {
            var mergedSettings = new SQLiteDataSourceSettings()
            {
                DefaultCommandTimeout = settings?.DefaultCommandTimeout ?? DefaultCommandTimeout,
                SuppressGlobalEvents = settings?.SuppressGlobalEvents ?? SuppressGlobalEvents,
                StrictMode = settings?.StrictMode ?? StrictMode,
                DisableLocks = settings?.DisableLocks ?? DisableLocks,
            };
            var result = new SQLiteDataSource(Name, m_ConnectionBuilder, mergedSettings, m_DatabaseMetadata, m_Cache, m_ExtensionCache);
            result.m_DatabaseMetadata = m_DatabaseMetadata;
            result.AuditRules = AuditRules;
            result.UserValue = UserValue;
            return result;
        }

        /// <summary>
        /// Creates a new data source with additional audit rules.
        /// </summary>
        /// <param name="additionalRules">The additional rules.</param>
        /// <returns></returns>
        public SQLiteDataSource WithRules(params AuditRule[] additionalRules)
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
        public SQLiteDataSource WithRules(IEnumerable<AuditRule> additionalRules)
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
        public SQLiteDataSource WithUser(object userValue)
        {
            var result = WithSettings(null);
            result.UserValue = userValue;
            return result;
        }


        /// <summary>
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation.</param>
        /// <param name="state">The state.</param>
        /// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
        protected override int? Execute(OperationExecutionToken<SQLiteConnection, SQLiteTransaction> executionToken, OperationImplementation<SQLiteConnection, SQLiteTransaction> implementation, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

            var mode = DisableLocks ? LockType.None : (executionToken as SQLiteOperationExecutionToken)?.LockType ?? LockType.Write;

            var startTime = DateTimeOffset.Now;
            OnExecutionStarted(executionToken, startTime, state);

            IDisposable lockToken = null;
            try
            {
                switch (mode)
                {
                    case LockType.Read: lockToken = SyncLock.ReaderLock(); break;
                    case LockType.Write: lockToken = SyncLock.WriterLock(); break;
                }

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
            finally
            {
                if (lockToken != null)
                    lockToken.Dispose();
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
        protected override async Task<int?> ExecuteAsync(OperationExecutionToken<SQLiteConnection, SQLiteTransaction> executionToken, OperationImplementationAsync<SQLiteConnection, SQLiteTransaction> implementation, CancellationToken cancellationToken, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

            var mode = DisableLocks ? LockType.None : (executionToken as SQLiteOperationExecutionToken)?.LockType ?? LockType.Write;

            var startTime = DateTimeOffset.Now;
            OnExecutionStarted(executionToken, startTime, state);

            IDisposable lockToken = null;
            try
            {
                switch (mode)
                {
                    case LockType.Read: lockToken = await SyncLock.ReaderLockAsync().ConfigureAwait(false); break;
                    case LockType.Write: lockToken = await SyncLock.WriterLockAsync().ConfigureAwait(false); break;
                }

                using (var con = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false))
                {
                    var rows = await implementation(con, null, cancellationToken).ConfigureAwait(false);
                    OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
                    return rows;
                }
            }
            catch (Exception ex)
            {
                if (cancellationToken.IsCancellationRequested) //convert SQLiteException into a OperationCanceledException 
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
            finally
            {
                if (lockToken != null)
                    lockToken.Dispose();
            }
        }

        /// <summary>
        /// Tests the connection.
        /// </summary>
        public override void TestConnection()
        {
            using (var con = CreateConnection())
            using (var cmd = new SQLiteCommand("SELECT 1", con))
                cmd.ExecuteScalar();
        }

        /// <summary>
        /// Tests the connection asynchronously.
        /// </summary>
        /// <returns></returns>
        public override async Task TestConnectionAsync()
        {
            using (var con = await CreateConnectionAsync())
            using (var cmd = new SQLiteCommand("SELECT 1", con))
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
            return new SQLiteOpenDataSource(this, (SQLiteConnection)connection, (SQLiteTransaction)transaction);
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
        public SQLiteDataSource WithCache(ICacheAdapter cache)
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
    }
}
