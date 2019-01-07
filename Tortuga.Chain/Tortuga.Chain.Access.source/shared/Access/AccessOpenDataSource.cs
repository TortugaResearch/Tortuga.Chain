using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;


namespace Tortuga.Chain.Access
{
    /// <summary>
    /// Class AccessOpenDataSource.
    /// </summary>
    /// <seealso cref="AccessDataSourceBase" />
    public class AccessOpenDataSource : AccessDataSourceBase, IOpenDataSource
    {
        readonly AccessDataSource m_BaseDataSource;
        readonly OleDbConnection m_Connection;
        readonly OleDbTransaction m_Transaction;


        internal AccessOpenDataSource(AccessDataSource dataSource, OleDbConnection connection, OleDbTransaction transaction) : base(new AccessDataSourceSettings() { DefaultCommandTimeout = dataSource.DefaultCommandTimeout, StrictMode = dataSource.StrictMode, SuppressGlobalEvents = dataSource.SuppressGlobalEvents })
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");

            m_BaseDataSource = dataSource;
            m_Connection = connection;
            m_Transaction = transaction;
        }

        /// <summary>
        /// Returns the associated connection.
        /// </summary>
        public DbConnection AssociatedConnection => m_Connection;

        /// <summary>
        /// Returns the associated transaction.
        /// </summary>
        public DbTransaction AssociatedTransaction => m_Transaction;

        /// <summary>
        /// Gets or sets the cache to be used by this data source. The default is .NET's System.Runtime.Caching.MemoryCache.
        /// </summary>
        public override ICacheAdapter Cache => m_BaseDataSource.Cache;

        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        /// <value>The database metadata.</value>
        public override AccessMetadataCache DatabaseMetadata => m_BaseDataSource.DatabaseMetadata;
        /// <summary>
        /// The extension cache is used by extensions to store data source specific information.
        /// </summary>
        /// <value>
        /// The extension cache.
        /// </value>
        protected override ConcurrentDictionary<Type, object> ExtensionCache => m_BaseDataSource.m_ExtensionCache;

        /// <summary>
        /// Closes the connection and transaction associated with this data source.
        /// </summary>
        public void Close()
        {
            if (m_Transaction != null)
                m_Transaction.Dispose();
            m_Connection.Dispose();
        }

        /// <summary>
        /// Gets the extension data.
        /// </summary>
        /// <typeparam name="TTKey">The type of extension data desired.</typeparam>
        /// <returns>T.</returns>
        /// <remarks>Chain extensions can use this to store data source specific data. The key should be a data type defined by the extension.
        /// Transactional data sources should override this method and return the value held by their parent data source.</remarks>
        public override TTKey GetExtensionData<TTKey>() => m_BaseDataSource.GetExtensionData<TTKey>();

        /// <summary>
        /// Tests the connection.
        /// </summary>
        public override void TestConnection()
        {
            using (var cmd = new OleDbCommand("SELECT 1", m_Connection))
            {
                if (m_Transaction != null)
                    cmd.Transaction = m_Transaction;
                cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Tests the connection asynchronously.
        /// </summary>
        /// <returns></returns>
        public override async Task TestConnectionAsync()
        {
            using (var cmd = new OleDbCommand("SELECT 1", m_Connection))
            {
                if (m_Transaction != null)
                    cmd.Transaction = m_Transaction;
                await cmd.ExecuteScalarAsync();
            }
        }

        /// <summary>
        /// Tries the commit the transaction associated with this data source.
        /// </summary>
        /// <returns>
        /// True if there was an open transaction associated with this data source, otherwise false.
        /// </returns>
        public bool TryCommit()
        {
            if (m_Transaction == null)
                return false;
            m_Transaction.Commit();
            return true;
        }

        /// <summary>
        /// Modifies this data source with additional audit rules.
        /// </summary>
        /// <param name="additionalRules">The additional rules.</param>
        /// <returns></returns>
        public AccessOpenDataSource WithRules(params AuditRule[] additionalRules)
        {
            AuditRules = new AuditRuleCollection(AuditRules, additionalRules);
            return this;
        }

        /// <summary>
        /// Modifies this data source with additional audit rules.
        /// </summary>
        /// <param name="additionalRules">The additional rules.</param>
        /// <returns></returns>
        public AccessOpenDataSource WithRules(IEnumerable<AuditRule> additionalRules)
        {
            AuditRules = new AuditRuleCollection(AuditRules, additionalRules);
            return this;
        }

        /// <summary>
        /// Modifies this data source to include the indicated user.
        /// </summary>
        /// <param name="userValue">The user value.</param>
        /// <returns></returns>
        /// <remarks>
        /// This is used in conjunction with audit rules.
        /// </remarks>
        public AccessOpenDataSource WithUser(object userValue)
        {
            UserValue = userValue;
            return this;
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
        protected override int? Execute(CommandExecutionToken<OleDbCommand, OleDbParameter> executionToken, CommandImplementation<OleDbCommand> implementation, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");
            var currentToken = executionToken as AccessCommandExecutionToken;
            if (currentToken == null)
                throw new ArgumentNullException("executionToken", "only AccessCommandExecutionToken is supported.");

            var startTime = DateTimeOffset.Now;
            OnExecutionStarted(executionToken, startTime, state);

            try
            {
                int? rows = null;
                while (currentToken != null)
                {
                    OnExecutionStarted(currentToken, startTime, state);
                    using (var cmd = new OleDbCommand())
                    {
                        cmd.Connection = m_Connection;
                        if (m_Transaction != null)
                            cmd.Transaction = m_Transaction;

                        if (DefaultCommandTimeout.HasValue)
                            cmd.CommandTimeout = (int)DefaultCommandTimeout.Value.TotalSeconds;
                        cmd.CommandText = currentToken.CommandText;
                        cmd.CommandType = currentToken.CommandType;
                        foreach (var param in currentToken.Parameters)
                            cmd.Parameters.Add(param);

                        currentToken.ApplyCommandOverrides(cmd);

                        if (currentToken.ExecutionMode == AccessCommandExecutionMode.Materializer)
                            rows = implementation(cmd);
                        else if (currentToken.ExecutionMode == AccessCommandExecutionMode.ExecuteScalarAndForward)
                            currentToken.ForwardResult(cmd.ExecuteScalar());
                        else
                            rows = cmd.ExecuteNonQuery();
                        executionToken.RaiseCommandExecuted(cmd, rows);
                        OnExecutionFinished(currentToken, startTime, DateTimeOffset.Now, rows, state);
                    }
                    currentToken = currentToken.NextCommand;
                }
                return rows;
            }
            catch (Exception ex)
            {
                OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
                throw;
            }

        }

        /// <summary>
        /// Executes the specified operation.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation.</param>
        /// <param name="state">The state.</param>
        /// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
        protected override int? Execute(OperationExecutionToken<OleDbConnection, OleDbTransaction> executionToken, OperationImplementation<OleDbConnection, OleDbTransaction> implementation, object state)
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
        /// Executes the operation asynchronously.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation that handles processing the result of the command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">User supplied state.</param>
        /// <returns>Task.</returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override async Task<int?> ExecuteAsync(CommandExecutionToken<OleDbCommand, OleDbParameter> executionToken, CommandImplementationAsync<OleDbCommand> implementation, CancellationToken cancellationToken, object state)
        {
            if (executionToken == null)
                throw new ArgumentNullException("executionToken", "executionToken is null.");
            if (implementation == null)
                throw new ArgumentNullException("implementation", "implementation is null.");
            var currentToken = executionToken as AccessCommandExecutionToken;
            if (currentToken == null)
                throw new ArgumentNullException("executionToken", "only AccessCommandExecutionToken is supported.");

            var startTime = DateTimeOffset.Now;
            OnExecutionStarted(executionToken, startTime, state);

            try
            {
                int? rows = null;
                while (currentToken != null)
                {
                    OnExecutionStarted(currentToken, startTime, state);
                    using (var cmd = new OleDbCommand())
                    {
                        cmd.Connection = m_Connection;
                        if (m_Transaction != null)
                            cmd.Transaction = m_Transaction;
                        if (DefaultCommandTimeout.HasValue)
                            cmd.CommandTimeout = (int)DefaultCommandTimeout.Value.TotalSeconds;
                        cmd.CommandText = currentToken.CommandText;
                        cmd.CommandType = currentToken.CommandType;
                        foreach (var param in currentToken.Parameters)
                            cmd.Parameters.Add(param);

                        currentToken.ApplyCommandOverrides(cmd);

                        if (currentToken.ExecutionMode == AccessCommandExecutionMode.Materializer)
                            rows = await implementation(cmd);
                        else if (currentToken.ExecutionMode == AccessCommandExecutionMode.ExecuteScalarAndForward)
                            currentToken.ForwardResult(await cmd.ExecuteScalarAsync());
                        else
                            rows = await cmd.ExecuteNonQueryAsync();
                        executionToken.RaiseCommandExecuted(cmd, rows);
                        OnExecutionFinished(currentToken, startTime, DateTimeOffset.Now, rows, state);
                    }
                    currentToken = currentToken.NextCommand;
                }
                return rows;
            }
            catch (Exception ex)
            {
                if (cancellationToken.IsCancellationRequested) //convert AccessException into a OperationCanceledException 
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

        }

        /// <summary>
        /// execute as an asynchronous operation.
        /// </summary>
        /// <param name="executionToken">The execution token.</param>
        /// <param name="implementation">The implementation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="state">The state.</param>
        /// <returns>Task.</returns>
        protected override async Task<int?> ExecuteAsync(OperationExecutionToken<OleDbConnection, OleDbTransaction> executionToken, OperationImplementationAsync<OleDbConnection, OleDbTransaction> implementation, CancellationToken cancellationToken, object state)
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
                    OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex2, state);
                    throw ex2;
                }
                else
                {
                    OnExecutionError(executionToken, startTime, DateTimeOffset.Now, ex, state);
                    throw;
                }
            }

        }
    }
}
