using Npgsql;
using NpgsqlTypes;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
    /// <summary>
    /// Base class that describes a PostgreSql database command.
    /// </summary>
    internal abstract class PostgreSqlObjectCommand<TArgument> : ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument>
        where TArgument : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlObjectCommand{TArgument}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        protected PostgreSqlObjectCommand(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableName, TArgument argumentValue)
            : base(dataSource, argumentValue)
        {
            Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
        }

        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        public TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> Table { get; }

        /// <summary>
        /// Called when ObjectDbCommandBuilder needs a reference to the associated table or view.
        /// </summary>
        /// <returns></returns>
        protected override TableOrViewMetadata OnGetTable() => Table;
    }
}
