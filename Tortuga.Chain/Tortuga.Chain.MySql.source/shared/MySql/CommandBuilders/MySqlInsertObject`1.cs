using MySql.Data.MySqlClient;
using System;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.MySql.CommandBuilders
{
    /// <summary>
    /// Class that represents a MySql Insert.
    /// </summary>
    internal sealed class MySqlInsertObject<TArgument> : MySqlObjectCommand<TArgument>
        where TArgument : class
    {
        readonly InsertOptions m_Options;


        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlInsertObject{TArgument}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public MySqlInsertObject(MySqlDataSourceBase dataSource, MySqlObjectName tableName, TArgument argumentValue, InsertOptions options)
            : base(dataSource, tableName, argumentValue)
        {
            m_Options = options;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer"></param>
        /// <returns><see cref="MySqlCommandExecutionToken" /></returns>
        public override CommandExecutionToken<MySqlCommand, MySqlParameter> Prepare(Materializer<MySqlCommand, MySqlParameter> materializer)
        {
            throw new NotImplementedException();
        }

    }
}

