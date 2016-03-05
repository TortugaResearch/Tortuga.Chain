using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Threading;
using System.Threading.Tasks;

namespace Tortuga.Chain.SqlServer
{
    /// <summary>
    /// Class SqlServerTransactionalDataSource.
    /// </summary>
    public class SqlServerTransactionalDataSource : SqlServerDataSourceBase, IDisposable
    {

        private readonly SqlConnection m_Connection;
        private readonly SqlServerDataSource m_Dispatcher;
        private readonly SqlTransaction m_Transaction;
        private readonly string m_TransactionName;
        private bool m_Disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerTransactionalDataSource"/> class.
        /// </summary>
        /// <param name="dataSource">The parent connection.</param>
        /// <param name="transactionName">Name of the transaction.</param>
        /// <param name="isolationLevel">The isolation level. If not supplied, will use the database default.</param>
        /// <param name="forwardEvents">If true, logging events are forwarded to the parent connection.</param>
        protected internal SqlServerTransactionalDataSource(SqlServerDataSource dataSource, string transactionName, IsolationLevel? isolationLevel, bool forwardEvents)
        {
            Name = dataSource.Name;

            m_Dispatcher = dataSource;
            m_Connection = dataSource.CreateSqlConnection();
            m_TransactionName = transactionName;

            if (isolationLevel == null)
                m_Transaction = m_Connection.BeginTransaction(transactionName);
            else
                m_Transaction = m_Connection.BeginTransaction(isolationLevel.Value, transactionName);

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
        /// This object can be used to lookup database information.
        /// </summary>
        public override SqlServerMetadataCache DatabaseMetadata
        {
            get { return m_Dispatcher.DatabaseMetadata; }
        }

        /// <summary>
        /// Commits the transaction and disposes the underlying connection.
        /// </summary>
        public void Commit()
        {
            if (m_Disposed)
                throw new ObjectDisposedException("Transaction is disposed");

            m_Transaction.Commit();
            Dispose(true);

        }

        /// <summary>
        /// Closes the current transaction and connection. If not committed, the transaction is rolled back.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Rolls back the transaction and disposes the underlying connection.
        /// </summary>
        public void Rollback()
        {
            if (m_Disposed)
                throw new ObjectDisposedException("Transaction is disposed");

            m_Transaction.Rollback();
            Dispose(true);
        }
        /// <summary>
        /// Closes the current transaction and connection. If not committed, the transaction is rolled back.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_Transaction.Dispose();
                m_Connection.Dispose();
                m_Disposed = true;
            }
        }
        /// <summary>
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="state">User supplied state.</param>
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
                using (var cmd = new SqlCommand())
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
            catch (SqlException ex)
            {
                ex.Data["DataSource"] = Name;
                ex.Data["TransactionName"] = m_TransactionName;
                ex.Data["Operation"] = executionToken.OperationName;
                ex.Data["CommandText"] = executionToken.CommandText;
                ex.Data["Parameters"] = executionToken.Parameters;
                OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
                throw;
            }
            catch (SqlTypeException ex)
            {
                ex.Data["DataSource"] = Name;
                ex.Data["TransactionName"] = m_TransactionName;
                ex.Data["Operation"] = executionToken.OperationName;
                ex.Data["CommandText"] = executionToken.CommandText;
                ex.Data["Parameters"] = executionToken.Parameters;
                OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
                throw;
            }
        }

        /// <summary>
        /// execute as an asynchronous operation.
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
                using (var cmd = new SqlCommand())
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
            catch (SqlException ex)
            {
                if (cancellationToken.IsCancellationRequested) //convert SqlException into a OperationCanceledException 
                {
                    var ex2 = new OperationCanceledException("Operation was canceled.", ex, cancellationToken);
                    ex2.Data["DataSource"] = Name;
                    ex.Data["TransactionName"] = m_TransactionName;
                    ex2.Data["Operation"] = executionToken.OperationName;
                    ex2.Data["CommandText"] = executionToken.CommandText;
                    ex2.Data["Parameters"] = executionToken.Parameters;
                    OnExecutionCanceled(executionToken, startTime, DateTimeOffset.Now, state);
                    throw ex2;
                }
                else
                {
                    ex.Data["DataSource"] = Name;
                    ex.Data["TransactionName"] = m_TransactionName;
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
                ex.Data["TransactionName"] = m_TransactionName;
                ex.Data["Operation"] = executionToken.OperationName;
                ex.Data["CommandText"] = executionToken.CommandText;
                ex.Data["Parameters"] = executionToken.Parameters;
                OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
                throw;
            }
        }


    }
}
