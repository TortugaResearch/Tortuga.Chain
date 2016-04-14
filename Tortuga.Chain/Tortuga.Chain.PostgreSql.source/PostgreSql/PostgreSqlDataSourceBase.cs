using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.PostgreSql.PostgreSql
{
    /// <summary>
    /// Class PostgreSqlDataSourceBase.
    /// </summary>
    public abstract class PostgreSqlDataSourceBase : DataSource<NpgsqlCommand, NpgsqlParameter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDataSourceBase"/> class.
        /// </summary>
        /// <param name="settings">Optional settings object.</param>
        public PostgreSqlDataSourceBase(DataSourceSettings settings) : base(settings)
        {
        }

        public abstract PostgreSqlMetadataCache DatabaseMetadata { get; }

        //TODO: implement ClassXMetadata
    }
}
