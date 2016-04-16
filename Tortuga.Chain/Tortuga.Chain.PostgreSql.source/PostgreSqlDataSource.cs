using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Chain.PostgreSql;

namespace Tortuga.Chain
{
    /// <summary>
    /// Class PostgreSqlDataSource.
    /// </summary>
    /// <seealso cref="PostgreSqlDataSourceBase" />
    public class PostgreSqlDataSource : PostgreSqlDataSourceBase
    {
        private readonly NpgsqlConnectionStringBuilder m_ConnectionBuilder;
        private PostgreSqlMetadataCache m_DatabaseMetadata;

        public PostgreSqlTransactionalDataSource BeginTransaction()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDataSource"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
        public PostgreSqlDataSource(string name, string connectionString, PostgreSqlDataSourceSettings settings = null)
            : base(settings)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("connectionString is null or empty.", "connectionString");

            m_ConnectionBuilder = new NpgsqlConnectionStringBuilder(connectionString);
            if (string.IsNullOrEmpty(name))
                Name = m_ConnectionBuilder.Database;
            else
                Name = name;

            m_DatabaseMetadata = new PostgreSqlMetadataCache(m_ConnectionBuilder);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDataSource"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="settings">The settings.</param>
        public PostgreSqlDataSource(string connectionString, PostgreSqlDataSourceSettings settings = null)
            : this(null, connectionString, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDataSource"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
        public PostgreSqlDataSource(string name, NpgsqlConnectionStringBuilder connectionBuilder, PostgreSqlDataSourceSettings settings = null)
            : base(settings)
        {
            if (connectionBuilder == null)
                throw new ArgumentNullException(nameof(connectionBuilder), $"{nameof(connectionBuilder)} is null.");

            m_ConnectionBuilder = connectionBuilder;
            if (string.IsNullOrEmpty(name))
                Name = m_ConnectionBuilder.Database;
            else
                Name = name;

            m_DatabaseMetadata = new PostgreSqlMetadataCache(m_ConnectionBuilder);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDataSource"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="settings">The settings.</param>
        public PostgreSqlDataSource(NpgsqlConnectionStringBuilder connectionBuilder, PostgreSqlDataSourceSettings settings = null)
            : this(null, connectionBuilder, settings)
        {
        }

        public override PostgreSqlMetadataCache DatabaseMetadata
        {
            get { return m_DatabaseMetadata; }
        }

        protected override void Execute(ExecutionToken<NpgsqlCommand, NpgsqlParameter> executionToken, Func<NpgsqlCommand, int?> implementation, object state)
        {
            throw new NotImplementedException();
        }

        protected override Task ExecuteAsync(ExecutionToken<NpgsqlCommand, NpgsqlParameter> executionToken, Func<NpgsqlCommand, Task<int?>> implementation, CancellationToken cancellationToken, object state)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new data source with the indicated changes to the settings.
        /// </summary>
        /// <param name="settings">The new settings to use.</param>
        /// <returns></returns>
        /// <remarks>The new data source will share the same database metadata cache.</remarks>
        public PostgreSqlDataSource WithSettings(PostgreSqlDataSourceSettings settings)
        {
            var mergedSettings = new PostgreSqlDataSourceSettings()
            {
                DefaultCommandTimeout = settings?.DefaultCommandTimeout ?? DefaultCommandTimeout,
                SuppressGlobalEvents = settings?.SuppressGlobalEvents ?? SuppressGlobalEvents,
                StrictMode = settings?.StrictMode ?? StrictMode
            };
            var result = new PostgreSqlDataSource(Name, m_ConnectionBuilder, mergedSettings);
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
        public PostgreSqlDataSource WithRules(params AuditRule[] additionalRules)
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
        public PostgreSqlDataSource WithRules(IEnumerable<AuditRule> additionalRules)
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
        public PostgreSqlDataSource WithUser(object userValue)
        {
            var result = WithSettings(null);
            result.UserValue = userValue;
            return result;
        }

#if !WINDOWS_UWP
        /// <summary>
        /// Creates a new connection using the connection string in the app.config file.
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public static PostgreSqlDataSource CreateFromConfig(string connectionName)
        {
            var settings = ConfigurationManager.ConnectionStrings[connectionName];
            if (settings == null)
                throw new InvalidOperationException("The configuration file does not contain a connection named " + connectionName);

            return new PostgreSqlDataSource(connectionName, settings.ConnectionString);
        }
#endif
    }
}
