using NpgsqlTypes;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql
{
    /// <summary>
    /// SQL Server specific metadata for an index.
    /// </summary>
    public class PostgreSqlIndexMetadata : IndexMetadata<PostgreSqlObjectName, NpgsqlDbType>
    {
        /// <summary>Initializes a new instance of the <see cref="T:Tortuga.Chain.PostgreSql.PostgreSqlIndexMetadata"/> class.</summary>
        /// <param name="tableName">Name of the table (or view).</param>
        /// <param name="name">The name.</param>
        /// <param name="isPrimaryKey">if set to <c>true</c> is a primary key.</param>
        /// <param name="isUnique">if set to <c>true</c> is a unique index.</param>
        /// <param name="isUniqueConstraint">if set to <c>true</c> is a unique constraint.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="indexSizeKB">Approximate index size in KB</param>
        /// <param name="rowCount">Approximate row count</param>
        /// <param name="indexType">Type of the index.</param>
        public PostgreSqlIndexMetadata(PostgreSqlObjectName tableName, string name, bool isPrimaryKey, bool isUnique, bool isUniqueConstraint, IndexColumnMetadataCollection<NpgsqlDbType> columns, long? indexSizeKB, long? rowCount, PostgreSqlIndexType indexType) : base(tableName, name, isPrimaryKey, isUnique, isUniqueConstraint, columns, indexSizeKB, rowCount)
        {
            IndexType = indexType;
        }

        /// <summary>Gets the type of the index.</summary>
        public PostgreSqlIndexType IndexType { get; }
    }
}
