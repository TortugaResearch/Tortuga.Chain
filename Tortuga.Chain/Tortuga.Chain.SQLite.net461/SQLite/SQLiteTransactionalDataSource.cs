using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tortuga.Chain.SQLite
{
    /// <summary>
    /// Class SQLiteTransactionalDataSource
    /// </summary>
    public class SQLiteTransactionalDataSource : SQLiteDataSourceBase
    {
        private readonly SQLiteConnection m_Connection;
        private readonly SQLiteDataSource m_Dispatcher;
        private readonly SQLiteTransaction m_Transaction;
        private readonly ReaderWriterLockSlim m_SyncLock = new ReaderWriterLockSlim(); //Sqlite is single-threaded for writes. It says otherwise, but it spams the trace window with exceptions.
        private bool m_Disposed;

        protected internal SQLiteTransactionalDataSource(SQLiteDataSource dataSource, string transactionName, IsolationLevel? isolationLevel, bool forwardEvents)
        {
            Name = dataSource.Name;

            m_Dispatcher = dataSource;
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

        public override SQLiteMetadataCache DatabaseMetadata
        {
            get { return m_Dispatcher.DatabaseMetadata; }
        }

        /// <summary>
        /// Normally we use a reader/writer lock to avoid simutaneous writes to a SQlite database. If you disable this locking, you may see extra noise in your tracing output or unexcepted exceptions.
        /// </summary>
        public bool DisableLocks { get; set; }

        public void Commit()
        {
            if (m_Disposed)
                throw new ObjectDisposedException("Transaction is disposed.");

            m_Transaction.Commit();
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void RollBack()
        {
            if(m_Disposed)
                throw new ObjectDisposedException("Transaction is disposed.");

            m_Transaction.Rollback();
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                m_Transaction.Dispose();
                m_Connection.Dispose();
                m_Disposed = true;
            }
        }

        protected override void Execute(ExecutionToken<SQLiteCommand, SQLiteParameter> executionToken, Func<SQLiteCommand, int?> implementation, object state)
        {
            if(executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

            var mode = DisableLocks ? LockType.None : (executionToken as SQLiteExecutionToken)?.LockType ?? LockType.Write;

            var startTime = DateTimeOffset.Now;
            OnExecutionStarted(executionToken, startTime, state);

            try
            {
                switch(mode)
                {
                    case LockType.Read: m_SyncLock.EnterReadLock(); break;
                    case LockType.Write: m_SyncLock.EnterWriteLock(); break;
                }

                using (var cmd = new SQLiteCommand())
                {
                    cmd.Connection = m_Connection;
                    cmd.Transaction = m_Transaction;
                    cmd.CommandText = executionToken.CommandText;
                    cmd.CommandType = executionToken.CommandType;
                    foreach (var param in executionToken.Parameters)
                        cmd.Parameters.Add(param);

                    var rows = implementation(cmd);
                    OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
                }
            }
            catch (SQLiteException ex)
            {
                ex.Data["DataSource"] = Name;
                ex.Data["Operation"] = executionToken.OperationName;
                ex.Data["CommandText"] = executionToken.CommandText;
                ex.Data["Parameters"] = executionToken.Parameters;
                OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
                throw;
            }
            finally
            {
                switch (mode)
                {
                    case LockType.Read: m_SyncLock.ExitReadLock(); break;
                    case LockType.Write: m_SyncLock.ExitWriteLock(); break;
                }
            }
        }

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
                    case LockType.Read: m_SyncLock.EnterReadLock(); break;
                    case LockType.Write: m_SyncLock.EnterWriteLock(); break;
                }

                using (var cmd = new SQLiteCommand())
                {
                    cmd.Connection = m_Connection;
                    cmd.Transaction = m_Transaction;
                    cmd.CommandText = executionToken.CommandText;
                    cmd.CommandType = executionToken.CommandType;
                    foreach (var param in executionToken.Parameters)
                        cmd.Parameters.Add(param);

                    var rows = await implementation(cmd).ConfigureAwait(false);
                    OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
                }
            }
            catch (SQLiteException ex)
            {
                if (cancellationToken.IsCancellationRequested) //convert SQLiteException into a OperationCanceledException 
                {
                    var ex2 = new OperationCanceledException("Operation was canceled.", ex, cancellationToken);
                    ex2.Data["DataSource"] = Name;
                    ex2.Data["Operation"] = executionToken.OperationName;
                    ex2.Data["CommandText"] = executionToken.CommandText;
                    ex2.Data["Parameters"] = executionToken.Parameters;
                    OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex2, state);
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
            finally
            {
                switch (mode)
                {
                    case LockType.Read: m_SyncLock.ExitReadLock(); break;
                    case LockType.Write: m_SyncLock.ExitWriteLock(); break;
                }
            }
        }
    }
}
