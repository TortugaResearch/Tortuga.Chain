using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;
using System.Data.Common;
using Nito.AsyncEx;
using System.Collections.Concurrent;

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
using SQLiteConnection = Microsoft.Data.Sqlite.SqliteConnection;
using SQLiteTransaction = Microsoft.Data.Sqlite.SqliteTransaction;
#endif

namespace Tortuga.Chain.SQLite
{
    /// <summary>
    /// Class SQLiteOpenDataSource.
    /// </summary>
    /// <seealso cref="SQLiteDataSourceBase" />
    public class SQLiteOpenDataSource : SQLiteDataSourceBase,IOpenDataSource
    {
        readonly SQLiteConnection m_Connection;
        readonly SQLiteDataSource m_BaseDataSource;
        readonly SQLiteTransaction m_Transaction;


        internal SQLiteOpenDataSource(SQLiteDataSource dataSource, SQLiteConnection connection, SQLiteTransaction transaction) : base(new SQLiteDataSourceSettings() { DefaultCommandTimeout = dataSource.DefaultCommandTimeout, StrictMode = dataSource.StrictMode, SuppressGlobalEvents = dataSource.SuppressGlobalEvents, DisableLocks = dataSource.DisableLocks })
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");

            m_BaseDataSource = dataSource;
            m_Connection = connection;
            m_Transaction = transaction;
        }

        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        /// <value>The database metadata.</value>
        public override SQLiteMetadataCache DatabaseMetadata
        {
            get { return m_BaseDataSource.DatabaseMetadata; }
        }

        DbConnection IOpenDataSource.AssociatedConnection
        {
            get { return m_Connection; }
        }

        DbTransaction IOpenDataSource.AssociatedTransaction
        {
            get { return m_Transaction; }
        }

        internal override AsyncReaderWriterLock SyncLock
        {
            get { return m_BaseDataSource.SyncLock; }
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

                using (var cmd = new SQLiteCommand())
                {
                    cmd.Connection = m_Connection;
                    if (m_Transaction != null)
                        cmd.Transaction = m_Transaction;
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
        /// Executes the operation asynchronously.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User supplied state.</param>
        /// <returns>Task.</returns>
        /// <exception cref="NotImplementedException"></exception>
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

                using (var cmd = new SQLiteCommand())
                {
                    cmd.Connection = m_Connection;
                    if (m_Transaction != null)
                        cmd.Transaction = m_Transaction;
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
        /// Gets the extension data.
        /// </summary>
        /// <typeparam name="TTKey">The type of extension data desired.</typeparam>
        /// <returns>T.</returns>
        /// <remarks>Chain extensions can use this to store data source specific data. The key should be a data type defined by the extension.
        /// Transactional data sources should override this method and return the value held by their parent data source.</remarks>
        public override TTKey GetExtensionData<TTKey>()
        {
            return m_BaseDataSource.GetExtensionData<TTKey>();
        }

        /// <summary>
        /// Modifies this data source to include the indicated user.
        /// </summary>
        /// <param name="userValue">The user value.</param>
        /// <returns></returns>
        /// <remarks>
        /// This is used in conjunction with audit rules.
        /// </remarks>
        public SQLiteOpenDataSource WithUser(object userValue)
        {
            UserValue = userValue;
            return this;
        }

        /// <summary>
        /// Modifies this data source with additional audit rules.
        /// </summary>
        /// <param name="additionalRules">The additional rules.</param>
        /// <returns></returns>
        public SQLiteOpenDataSource WithRules(params AuditRule[] additionalRules)
        {
            AuditRules = new AuditRuleCollection(AuditRules, additionalRules);
            return this;
        }

        /// <summary>
        /// Modifies this data source with additional audit rules.
        /// </summary>
        /// <param name="additionalRules">The additional rules.</param>
        /// <returns></returns>
        public SQLiteOpenDataSource WithRules(IEnumerable<AuditRule> additionalRules)
        {
            AuditRules = new AuditRuleCollection(AuditRules, additionalRules);
            return this;
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

                var rows = implementation(m_Connection, m_Transaction);
                OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
                return rows;

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

                var rows = await implementation(m_Connection, m_Transaction, cancellationToken).ConfigureAwait(false);
                OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
                return rows;

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
            using (var cmd = new SQLiteCommand("SELECT 1", m_Connection))
                cmd.ExecuteScalar();
        }

        /// <summary>
        /// Tests the connection asynchronously.
        /// </summary>
        /// <returns></returns>
        public override async Task TestConnectionAsync()
        {
            using (var cmd = new SQLiteCommand("SELECT 1", m_Connection))
                await cmd.ExecuteScalarAsync();
        }

        bool IOpenDataSource.TryCommit()
        {
            if (m_Transaction == null)
                return false;
            m_Transaction.Commit();
            return true;
        }

        void IOpenDataSource.Close()
        {
            if (m_Transaction != null)
                m_Transaction.Dispose();
            m_Connection.Dispose();
        }

        /// <summary>
        /// Gets or sets the cache to be used by this data source. The default is .NET's System.Runtime.Caching.MemoryCache.
        /// </summary>
        public override ICacheAdapter Cache
        {
            get { return m_BaseDataSource.Cache; }
        }

        /// <summary>
        /// The extension cache is used by extensions to store data source specific information.
        /// </summary>
        /// <value>
        /// The extension cache.
        /// </value>
        protected override ConcurrentDictionary<Type, object> ExtensionCache
        {
            get { return m_BaseDataSource.m_ExtensionCache; }
        }

    }
}
