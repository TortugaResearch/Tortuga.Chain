using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;

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
    public class SQLiteOpenDataSource : SQLiteDataSourceBase
    {
        private readonly SQLiteConnection m_Connection;
        private readonly SQLiteDataSource m_BaseDataSource;
        private readonly SQLiteTransaction m_Transaction;


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
        protected override void Execute(ExecutionToken<SQLiteCommand, SQLiteParameter> executionToken, Func<SQLiteCommand, int?> implementation, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

            var mode = DisableLocks ? LockType.None : (executionToken as SQLiteExecutionToken)?.LockType ?? LockType.Write;

            var startTime = DateTimeOffset.Now;
            OnExecutionStarted(executionToken, startTime, state);

            try
            {
                switch (mode)
                {
                    case LockType.Read: SyncLock.EnterReadLock(); break;
                    case LockType.Write: SyncLock.EnterWriteLock(); break;
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
                    OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
                }
            }
            catch (Exception ex)
            {
                OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
                throw;
            }
            finally
            {
                switch (mode)
                {
                    case LockType.Read: SyncLock.ExitReadLock(); break;
                    case LockType.Write: SyncLock.ExitWriteLock(); break;
                }
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
        protected override async Task ExecuteAsync(ExecutionToken<SQLiteCommand, SQLiteParameter> executionToken, Func<SQLiteCommand, Task<int?>> implementation, CancellationToken cancellationToken, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

            var mode = DisableLocks ? LockType.None : (executionToken as SQLiteExecutionToken)?.LockType ?? LockType.Write;

            var startTime = DateTimeOffset.Now;
            OnExecutionStarted(executionToken, startTime, state);

            try
            {
                switch (mode)
                {
                    case LockType.Read: SyncLock.EnterReadLock(); break;
                    case LockType.Write: SyncLock.EnterWriteLock(); break;
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
                    OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
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
                switch (mode)
                {
                    case LockType.Read: SyncLock.ExitReadLock(); break;
                    case LockType.Write: SyncLock.ExitWriteLock(); break;
                }
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


    }
}
