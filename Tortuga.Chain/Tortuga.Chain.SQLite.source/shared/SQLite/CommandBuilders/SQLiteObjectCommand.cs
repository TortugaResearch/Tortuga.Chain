using System.Data;
using System.Data.SQLite;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

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
        protected SQLiteObjectCommand(SQLiteDataSourceBase dataSource, SQLiteObjectName tableName, TArgument argumentValue)
            : base(dataSource, argumentValue)
        {
            Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
        }

        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        public TableOrViewMetadata<SQLiteObjectName, DbType> Table { get; }

        /// <summary>
        /// Called when ObjectDbCommandBuilder needs a reference to the associated table or view.
        /// </summary>
        /// <returns></returns>
        protected override TableOrViewMetadata OnGetTable() => Table;
    }
}
