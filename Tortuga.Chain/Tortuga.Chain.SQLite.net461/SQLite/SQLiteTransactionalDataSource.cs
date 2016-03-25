using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Core;
using System.Diagnostics.CodeAnalysis;


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
    /// Class SQLiteTransactionalDataSource
    /// </summary>
    public sealed class SQLiteTransactionalDataSource : SQLiteDataSourceBase, IDisposable
    {
        private readonly SQLiteConnection m_Connection;
        private readonly SQLiteDataSource m_BaseDataSource;
        private readonly SQLiteTransaction m_Transaction;

        private bool m_Disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteTransactionalDataSource"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="forwardEvents">if set to <c>true</c> [forward events].</param>
        internal SQLiteTransactionalDataSource(SQLiteDataSource dataSource, IsolationLevel? isolationLevel, bool forwardEvents)
        {
            Name = dataSource.Name;

            m_BaseDataSource = dataSource;
            m_Connection = dataSource.CreateSQLiteConnection();

            if (isolationLevel == null)
                m_Transaction = m_Connection.BeginTransaction();
            else
                m_Transaction = m_Connection.BeginTransaction(isolationLevel.Value);

            if (forwardEvents)
            {
                ExecutionStarted += (sender, e) => dataSource.OnExecutionStarted(e);
                ExecutionFinished += (sender, e) => dataSource.OnExecutionFinished(e);
                ExecutionError += (sender, e) => dataSource.OnExecutionError(e);
                ExecutionCanceled += (sender, e) => dataSource.OnExecutionCanceled(e);
                SuppressGlobalEvents = true;
            }
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
        /// Commits this instance.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Transaction is disposed.</exception>
        public void Commit()
        {
            if (m_Disposed)
                throw new ObjectDisposedException("Transaction is disposed.");

            m_Transaction.Commit();
            Dispose(true);
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Rolls the back.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Transaction is disposed.</exception>
        public void Rollback()
        {
            if (m_Disposed)
                throw new ObjectDisposedException("Transaction is disposed.");

            m_Transaction.Rollback();
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_Transaction.Dispose();
                m_Connection.Dispose();
                m_Disposed = true;
            }
        }

        /// <summary>
        /// Executes the specified execution token.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation.</param>
        /// <param name="state">The state.</param>
        /// <exception cref="ArgumentNullException">
        /// executionToken;executionToken is null.
        /// or
        /// implementation;implementation is null.
        /// </exception>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
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
    }
}
