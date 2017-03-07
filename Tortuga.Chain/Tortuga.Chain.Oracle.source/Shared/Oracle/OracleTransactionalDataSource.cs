using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.Oracle
{
    /// <summary>
    /// Class OracleTransactionalDataSource 
    /// </summary>
    /// <seealso cref="OracleDataSourceBase" />
    /// <seealso cref="IDisposable" />
    public class OracleTransactionalDataSource : OracleDataSourceBase, IDisposable, ITransactionalDataSource
    {
        private readonly OracleConnection m_Connection;
        private readonly OracleDataSource m_BaseDataSource;
        private readonly OracleTransaction m_Transaction;
        private bool m_Disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleTransactionalDataSource"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="forwardEvents">if set to <c>true</c> [forward events].</param>
        public OracleTransactionalDataSource(OracleDataSource dataSource, IsolationLevel? isolationLevel, bool forwardEvents)
            : base(new OracleDataSourceSettings { DefaultCommandTimeout = dataSource.DefaultCommandTimeout, StrictMode = dataSource.StrictMode, SuppressGlobalEvents = dataSource.SuppressGlobalEvents || forwardEvents })
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
        /// Initializes a new instance of the <see cref="OracleTransactionalDataSource" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="forwardEvents">if set to <c>true</c> [forward events].</param>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        internal OracleTransactionalDataSource(OracleDataSource dataSource, bool forwardEvents, OracleConnection connection, OracleTransaction transaction)
            : base(new OracleDataSourceSettings { DefaultCommandTimeout = dataSource.DefaultCommandTimeout, StrictMode = dataSource.StrictMode, SuppressGlobalEvents = dataSource.SuppressGlobalEvents || forwardEvents })
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
        public override OracleMetadataCache DatabaseMetadata
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
        protected override int? Execute(CommandExecutionToken<OracleCommand, OracleParameter> executionToken, CommandImplementation<OracleCommand> implementation, object state)
        {
            throw new NotImplementedException();
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
        /// <returns></returns>
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
        protected override Task<int?> ExecuteAsync(OperationExecutionToken<OracleConnection, OracleTransaction> executionToken, OperationImplementationAsync<OracleConnection, OracleTransaction> implementation, CancellationToken cancellationToken, object state)
        {
            throw new NotImplementedException();
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
            using (var cmd = new OracleCommand("SELECT 1", m_Connection))
                cmd.ExecuteScalar();
        }

        /// <summary>
        /// Tests the connection asynchronously.
        /// </summary>
        /// <returns></returns>
        public override async Task TestConnectionAsync()
        {
            using (var cmd = new OracleCommand("SELECT 1", m_Connection))
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