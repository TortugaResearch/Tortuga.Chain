#if !OleDb_Missing
using System.Collections.Generic;
using System.Data.OleDb;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// Class OleDbSqlServerObjectCommand.
    /// </summary>
    /// <typeparam name="TArgument">The type of the argument.</typeparam>
    internal abstract class OleDbSqlServerObjectCommand<TArgument> : ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument>
        where TArgument : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbSqlServerObjectCommand{TArgument}" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        protected OleDbSqlServerObjectCommand(OleDbSqlServerDataSourceBase dataSource, SqlServerObjectName tableName, TArgument argumentValue)
            : base(dataSource, argumentValue)
        {
            Table = DataSource.DatabaseMetadata.GetTableOrView(tableName);
        }

        protected OleDbSqlServerObjectCommand(ICommandDataSource<OleDbCommand, OleDbParameter> dataSource, TArgument argumentValue) : base(dataSource, argumentValue)
        {
        }

        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public SqlServerTableOrViewMetadata<OleDbType> Table { get; }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public new OleDbSqlServerDataSourceBase DataSource
        {
            get { return (OleDbSqlServerDataSourceBase)base.DataSource; }
        }

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

        /// <summary>
        /// Returns a list of columns known to be non-nullable.
        /// </summary>
        /// <returns>
        /// If the command builder doesn't know which columns are non-nullable, an empty list will be returned.
        /// </returns>
        /// <remarks>
        /// This is used by materializers to skip IsNull checks.
        /// </remarks>
        public override IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns() => Table.NullableColumns;
    }
}
#endif