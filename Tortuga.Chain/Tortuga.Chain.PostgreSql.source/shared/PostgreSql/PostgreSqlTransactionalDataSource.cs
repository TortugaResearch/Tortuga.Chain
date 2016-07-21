using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.PostgreSql
{
    /// <summary>
    /// Class PostgreSqlTransactionalDataSource 
    /// </summary>
    /// <seealso cref="PostgreSqlDataSourceBase" />
    /// <seealso cref="IDisposable" />
    public class PostgreSqlTransactionalDataSource : PostgreSqlDataSourceBase, IDisposable, ITransactionalDataSource
    {
        private readonly NpgsqlConnection m_Connection;
        private readonly PostgreSqlDataSource m_BaseDataSource;
        private readonly NpgsqlTransaction m_Transaction;
        private bool m_Disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlTransactionalDataSource"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="forwardEvents">if set to <c>true</c> [forward events].</param>
        public PostgreSqlTransactionalDataSource(PostgreSqlDataSource dataSource, IsolationLevel? isolationLevel, bool forwardEvents)
            : base(new PostgreSqlDataSourceSettings { DefaultCommandTimeout = dataSource.DefaultCommandTimeout, StrictMode = dataSource.StrictMode, SuppressGlobalEvents = dataSource.SuppressGlobalEvents || forwardEvents })
        {
            Name = dataSource.Name;

            m_BaseDataSource = dataSource;
            m_Connection = dataSource.CreateConnection();

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
            }
            AuditRules = dataSource.AuditRules;
            UserValue = dataSource.UserValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlTransactionalDataSource" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="forwardEvents">if set to <c>true</c> [forward events].</param>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        internal PostgreSqlTransactionalDataSource(PostgreSqlDataSource dataSource, bool forwardEvents, NpgsqlConnection connection, NpgsqlTransaction transaction)
            : base(new PostgreSqlDataSourceSettings { DefaultCommandTimeout = dataSource.DefaultCommandTimeout, StrictMode = dataSource.StrictMode, SuppressGlobalEvents = dataSource.SuppressGlobalEvents || forwardEvents })
        {
            Name = dataSource.Name;

            m_BaseDataSource = dataSource;
            m_Connection = connection;
            m_Transaction = transaction;


            if (forwardEvents)
            {
                ExecutionStarted += (sender, e) => dataSource.OnExecutionStarted(e);
                ExecutionFinished += (sender, e) => dataSource.OnExecutionFinished(e);
                ExecutionError += (sender, e) => dataSource.OnExecutionError(e);
                ExecutionCanceled += (sender, e) => dataSource.OnExecutionCanceled(e);
            }
            AuditRules = dataSource.AuditRules;
            UserValue = dataSource.UserValue;
        }
        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        public override PostgreSqlMetadataCache DatabaseMetadata
        {
            get { return m_BaseDataSource.DatabaseMetadata; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (m_Disposed)
                return;

            if (disposing)
            {
                m_Transaction.Dispose();
                m_Connection.Dispose();
                m_Disposed = true;
            }
        }

        /// <summary>
        /// Commits this transaction instance.
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
        /// Rolls the transaction back.
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
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="state">User supplied state.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// executionToken;executionToken is null.
        /// or
        /// implementation;implementation is null.
        /// </exception>
        protected override int? Execute(CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> executionToken, CommandImplementation<NpgsqlCommand> implementation, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

            var startTime = DateTimeOffset.Now;
            OnExecutionStarted(executionToken, startTime, state);

            try
            {
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = m_Connection;
                    cmd.Transaction = m_Transaction;
                    if (DefaultCommandTimeout.HasValue)
                        cmd.CommandTimeout = (int)DefaultCommandTimeout.Value.TotalSeconds;
                    cmd.CommandText = executionToken.CommandText;
                    cmd.CommandType = executionToken.CommandType;
                    foreach (var param in executionToken.Parameters)
                        cmd.Parameters.Add(param);

                    int? rows;

                    if (((PostgreSqlCommandExecutionToken)executionToken).DereferenceCursors)
                        rows = DereferenceCursors(cmd, implementation);
                    else
                        rows = implementation(cmd);

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
        }


        /// <summary>
        /// Executes the operation asynchronously.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User supplied state.</param>
        /// <returns>
        /// Task.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// executionToken;executionToken is null.
        /// or
        /// implementation;implementation is null.
        /// </exception>
        protected override async Task<int?> ExecuteAsync(CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> executionToken, CommandImplementationAsync<NpgsqlCommand> implementation, CancellationToken cancellationToken, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

            var startTime = DateTimeOffset.Now;
            OnExecutionStarted(executionToken, startTime, state);

            try
            {
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = m_Connection;
                    cmd.Transaction = m_Transaction;
                    if (DefaultCommandTimeout.HasValue)
                        cmd.CommandTimeout = (int)DefaultCommandTimeout.Value.TotalSeconds;
                    cmd.CommandText = executionToken.CommandText;
                    cmd.CommandType = executionToken.CommandType;
                    foreach (var param in executionToken.Parameters)
                        cmd.Parameters.Add(param);

                    int? rows;
                    if (((PostgreSqlCommandExecutionToken)executionToken).DereferenceCursors)
                        rows = await DereferenceCursorsAsync(cmd, implementation).ConfigureAwait(false);
                    else
                        rows = await implementation(cmd).ConfigureAwait(false);

                    executionToken.RaiseCommandExecuted(cmd, rows);
                    OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
                    return rows;
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
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// executionToken;executionToken is null.
        /// or
        /// implementation;implementation is null.
        /// </exception>
        protected override int? Execute(OperationExecutionToken<NpgsqlConnection, NpgsqlTransaction> executionToken, OperationImplementation<NpgsqlConnection, NpgsqlTransaction> implementation, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

            var startTime = DateTimeOffset.Now;
            OnExecutionStarted(executionToken, startTime, state);

            try
            {
                var rows = implementation(m_Connection, m_Transaction);
                OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
                return rows;
            }
            catch (Exception ex)
            {
                OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
                throw;
            }
        }

        /// <summary>
        /// Execute the operation asynchronously.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Task.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// executionToken;executionToken is null.
        /// or
        /// implementation;implementation is null.
        /// </exception>
        protected override async Task<int?> ExecuteAsync(OperationExecutionToken<NpgsqlConnection, NpgsqlTransaction> executionToken, OperationImplementationAsync<NpgsqlConnection, NpgsqlTransaction> implementation, CancellationToken cancellationToken, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");

            var startTime = DateTimeOffset.Now;
            OnExecutionStarted(executionToken, startTime, state);

            try
            {
                var rows = await implementation(m_Connection, m_Transaction, cancellationToken).ConfigureAwait(false);
                OnExecutionFinished(executionToken, startTime, DateTimeOffset.Now, rows, state);
                return rows;
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
        /// Gets the extension data.
        /// </summary>
        /// <typeparam name="TTKey">The type of extension data desired.</typeparam>
        /// <returns>
        /// T.
        /// </returns>
        /// <remarks>
        /// Chain extensions can use this to store data source specific data. The key should be a data type defined by the extension.
        /// Transactional data sources should override this method and return the value held by their parent data source.
        /// </remarks>
        public override TTKey GetExtensionData<TTKey>()
        {
            return m_BaseDataSource.GetExtensionData<TTKey>();
        }

        /// <summary>
        /// Tests the connection.
        /// </summary>
        public override void TestConnection()
        {
            using (var cmd = new NpgsqlCommand("SELECT 1", m_Connection))
                cmd.ExecuteScalar();
        }

        /// <summary>
        /// Tests the connection asynchronously.
        /// </summary>
        /// <returns></returns>
        public override async Task TestConnectionAsync()
        {
            using (var cmd = new NpgsqlCommand("SELECT 1", m_Connection))
                await cmd.ExecuteScalarAsync();
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