using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.PostgreSql
{
    /// <summary>
    /// Class PostgreSqlDataSource.
    /// </summary>
    /// <seealso cref="Tortuga.Chain.PostgreSql.PostgreSqlDataSourceBase" />
    public class PostgreSqlDataSource : PostgreSqlDataSourceBase
    {
        private readonly NpgsqlConnectionStringBuilder m_ConnectionBuilder;
        private PostgreSqlMetadataCache m_DatabaseMetadata;
        private DataSourceSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDataSource"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentException">connectionString is null or empty.;connectionString</exception>
        public PostgreSqlDataSource(string name, string connectionString, DataSourceSettings settings = null)
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
        public PostgreSqlDataSource(string connectionString, DataSourceSettings settings = null)
            : this(null, connectionString, settings)
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
    }
}
