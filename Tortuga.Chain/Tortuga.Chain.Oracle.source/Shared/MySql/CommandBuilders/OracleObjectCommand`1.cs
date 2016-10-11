using Oracle.ManagedDataAccess.Client;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;



namespace Tortuga.Chain.Oracle.CommandBuilders
{
    /// <summary>
    /// Base class that describes a Oracle database command.
    /// </summary>
    internal abstract class OracleObjectCommand<TArgument> : ObjectDbCommandBuilder<OracleCommand, OracleParameter, TArgument>
        where TArgument : class
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="OracleObjectCommand{TArgument}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        protected OracleObjectCommand(OracleDataSourceBase dataSource, OracleObjectName tableName, TArgument argumentValue)
            : base(dataSource, argumentValue)
        {
            Table = ((OracleDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(tableName);
        }

        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        public TableOrViewMetadata<OracleObjectName, OracleDbType> Table { get; }


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
