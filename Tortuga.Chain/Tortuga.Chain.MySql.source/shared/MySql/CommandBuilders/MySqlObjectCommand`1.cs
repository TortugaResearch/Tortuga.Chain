using MySql.Data.MySqlClient;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;



namespace Tortuga.Chain.MySql.CommandBuilders
{
    /// <summary>
    /// Base class that describes a MySql database command.
    /// </summary>
    internal abstract class MySqlObjectCommand<TArgument> : ObjectDbCommandBuilder<MySqlCommand, MySqlParameter, TArgument>
        where TArgument : class
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlObjectCommand{TArgument}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        protected MySqlObjectCommand(MySqlDataSourceBase dataSource, MySqlObjectName tableName, TArgument argumentValue)
            : base(dataSource, argumentValue)
        {
            Table = ((MySqlDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(tableName);
        }

        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        public TableOrViewMetadata<MySqlObjectName, MySqlDbType> Table { get; }


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
