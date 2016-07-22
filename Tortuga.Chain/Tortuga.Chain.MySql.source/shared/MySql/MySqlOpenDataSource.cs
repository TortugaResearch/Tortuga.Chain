using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.MySql
{
    /// <summary>
    /// Class SQLiteOpenDataSource.
    /// </summary>
    public class MySqlOpenDataSource : MySqlDataSourceBase, IOpenDataSource
    {
        readonly MySqlDataSource m_BaseDataSource;
        readonly MySqlConnection m_Connection;
        readonly MySqlTransaction m_Transaction;



        internal MySqlOpenDataSource(MySqlDataSource dataSource, MySqlConnection connection, MySqlTransaction transaction) : base(new MySqlDataSourceSettings() { DefaultCommandTimeout = dataSource.DefaultCommandTimeout, StrictMode = dataSource.StrictMode, SuppressGlobalEvents = dataSource.SuppressGlobalEvents })
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
        public override MySqlMetadataCache DatabaseMetadata
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
        public MySqlOpenDataSource WithUser(object userValue)
        {
            UserValue = userValue;
            return this;
        }

        /// <summary>
        /// Modifies this data source with additional audit rules.
        /// </summary>
        /// <param name="additionalRules">The additional rules.</param>
        /// <returns></returns>
        public MySqlOpenDataSource WithRules(params AuditRule[] additionalRules)
        {
            AuditRules = new AuditRuleCollection(AuditRules, additionalRules);
            return this;
        }

        /// <summary>
        /// Modifies this data source with additional audit rules.
        /// </summary>
        /// <param name="additionalRules">The additional rules.</param>
        /// <returns></returns>
        public MySqlOpenDataSource WithRules(IEnumerable<AuditRule> additionalRules)
        {
            AuditRules = new AuditRuleCollection(AuditRules, additionalRules);
            return this;
        }

        /// <summary>
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="state">User supplied state.</param>
        protected override int? Execute(CommandExecutionToken<MySqlCommand, MySqlParameter> executionToken, CommandImplementation<MySqlCommand> implementation, object state)
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
        /// <returns>Task.</returns>
        protected async override Task<int?> ExecuteAsync(CommandExecutionToken<MySqlCommand, MySqlParameter> executionToken, CommandImplementationAsync<MySqlCommand> implementation, CancellationToken cancellationToken, object state)
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
        protected override int? Execute(OperationExecutionToken<MySqlConnection, MySqlTransaction> executionToken, OperationImplementation<MySqlConnection, MySqlTransaction> implementation, object state)
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
        /// <returns>Task.</returns>
        protected override async Task<int?> ExecuteAsync(OperationExecutionToken<MySqlConnection, MySqlTransaction> executionToken, OperationImplementationAsync<MySqlConnection, MySqlTransaction> implementation, CancellationToken cancellationToken, object state)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tests the connection.
        /// </summary>
        public override void TestConnection()
        {
            using (var cmd = new MySqlCommand("SELECT 1", m_Connection))
                cmd.ExecuteScalar();
        }

        /// <summary>
        /// Tests the connection asynchronously.
        /// </summary>
        /// <returns></returns>
        public override async Task TestConnectionAsync()
        {
            using (var cmd = new MySqlCommand("SELECT 1", m_Connection))
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
