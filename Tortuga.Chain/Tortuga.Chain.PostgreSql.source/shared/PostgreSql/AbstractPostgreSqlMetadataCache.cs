using NpgsqlTypes;
using System;
using System.Linq;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql
{
    /// <summary>
    /// Class AbstractPostgreSqlMetadataCache.
    /// </summary>
    public abstract class AbstractPostgreSqlMetadataCache : DatabaseMetadataCache<PostgreSqlObjectName, NpgsqlDbType>
    {
        /// <summary>
        /// Gets the metadata for a table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override sealed TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> GetTableOrView(PostgreSqlObjectName tableName)
        {
            return OnGetTableOrView(tableName);
        }

        //C# doesn't allow us to change the return type so we're using this as a thunk.
        internal abstract TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> OnGetTableOrView(PostgreSqlObjectName tableName);
    }
}
