using System.Data.OleDb;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;


namespace Tortuga.Chain.Access.CommandBuilders
{
    /// <summary>
    /// Base class that describes a Access database command.
    /// </summary>
    internal abstract class AccessObjectCommand<TArgument> : ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument>
        where TArgument : class
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessObjectCommand{TArgument}" /> class
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        protected AccessObjectCommand(AccessDataSourceBase dataSource, AccessObjectName tableName, TArgument argumentValue)
            : base(dataSource, argumentValue)
        {
            Table = ((AccessDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(tableName);
        }

        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        public TableOrViewMetadata<AccessObjectName, OleDbType> Table { get; }

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
