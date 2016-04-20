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
        /// Initializes a new instance of the <see cref="SQLiteObjectCommand" /> class
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        protected SQLiteObjectCommand(SQLiteDataSourceBase dataSource, string tableName, TArgument argumentValue)
            : base(dataSource, argumentValue)
        {
            TableName = tableName;
            Metadata = ((SQLiteDataSourceBase)DataSource).DatabaseMetadata.GetTableOrView(tableName);
        }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        protected string TableName { get; }

        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        public TableOrViewMetadata<string, DbType> Metadata { get; }

    }
}
