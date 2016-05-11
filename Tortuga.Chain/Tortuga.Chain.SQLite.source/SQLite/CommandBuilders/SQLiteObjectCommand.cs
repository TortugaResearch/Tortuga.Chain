using System.Data;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

#if SDS
using System.Data.SQLite;
#else
using SQLiteCommand = Microsoft.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Microsoft.Data.Sqlite.SqliteParameter;
#endif

namespace Tortuga.Chain.SQLite.CommandBuilders
{
    /// <summary>
    /// Base class that describes a SQLite database command.
    /// </summary>
    internal abstract class SQLiteObjectCommand<TArgument> : ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument>
        where TArgument : class
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteObjectCommand{TArgument}" /> class
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        protected SQLiteObjectCommand(SQLiteDataSourceBase dataSource, string tableName, TArgument argumentValue)
            : base(dataSource, argumentValue)
        {
            Table = ((SQLiteDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(tableName);
        }

        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        public TableOrViewMetadata<string, DbType> Table { get; }

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
