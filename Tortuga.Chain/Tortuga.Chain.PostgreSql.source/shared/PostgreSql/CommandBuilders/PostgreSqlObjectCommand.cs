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
            Table = ((PostgreSqlDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(tableName);
        }

        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        public TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> Table { get; }


        /// <summary>
        /// Returns the column associated with the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <remarks>
        /// If the column name was not found, this will return null
        /// </remarks>
        public override ColumnMetadata TryGetColumn(string columnName)
        {
            return Table.Columns.TryGetColumn(columnName);
        }
    }
}
