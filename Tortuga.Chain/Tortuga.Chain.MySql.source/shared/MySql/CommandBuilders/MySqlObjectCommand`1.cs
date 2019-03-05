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
            Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
        }

        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        public TableOrViewMetadata<MySqlObjectName, MySqlDbType> Table { get; }

        /// <summary>
        /// Called when ObjectDbCommandBuilder needs a reference to the associated table or view.
        /// </summary>
        /// <returns></returns>
        protected override TableOrViewMetadata OnGetTable() => Table;
    }
}
