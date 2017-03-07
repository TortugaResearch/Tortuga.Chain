using Oracle.ManagedDataAccess.Client;
using System;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Oracle.CommandBuilders
{
    /// <summary>
    /// Command object that represents a delete operation.
    /// </summary>
    internal sealed class OracleDeleteObject<TArgument> : OracleObjectCommand<TArgument>
        where TArgument : class
    {
        readonly DeleteOptions m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDeleteObject{TArgument}"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="table">The table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        public OracleDeleteObject(OracleDataSourceBase dataSource, OracleObjectName table, TArgument argumentValue, DeleteOptions options)
            : base(dataSource, table, argumentValue)
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
