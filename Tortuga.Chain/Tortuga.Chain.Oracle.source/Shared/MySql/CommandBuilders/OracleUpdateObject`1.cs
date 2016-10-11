using Oracle.ManagedDataAccess.Client;
using System;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Oracle.CommandBuilders
{
    /// <summary>
    /// Command object that represents an update operation.
    /// </summary>
    internal sealed class OracleUpdateObject<TArgument> : OracleObjectCommand<TArgument>
        where TArgument : class
    {
        readonly UpdateOptions m_Options;


        /// <summary>
        /// Initializes a new instance of the <see cref="OracleUpdateObject{TArgument}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public OracleUpdateObject(OracleDataSourceBase dataSource, OracleObjectName tableName, TArgument argumentValue, UpdateOptions options)
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
