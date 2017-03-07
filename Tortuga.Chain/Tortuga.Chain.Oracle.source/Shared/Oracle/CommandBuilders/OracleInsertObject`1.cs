using Oracle.ManagedDataAccess.Client;
using System;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Oracle.CommandBuilders
{
    /// <summary>
    /// Class that represents a Oracle Insert.
    /// </summary>
    internal sealed class OracleInsertObject<TArgument> : OracleObjectCommand<TArgument>
        where TArgument : class
    {
        readonly InsertOptions m_Options;


        /// <summary>
        /// Initializes a new instance of the <see cref="OracleInsertObject{TArgument}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public OracleInsertObject(OracleDataSourceBase dataSource, OracleObjectName tableName, TArgument argumentValue, InsertOptions options)
            : base(dataSource, tableName, argumentValue)
        {
            m_Options = options;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="materializer"></param>
        /// <returns><see cref="OracleCommandExecutionToken" /></returns>
        public override CommandExecutionToken<OracleCommand, OracleParameter> Prepare(Materializer<OracleCommand, OracleParameter> materializer)
        {
            throw new NotImplementedException();
        }

    }
}

