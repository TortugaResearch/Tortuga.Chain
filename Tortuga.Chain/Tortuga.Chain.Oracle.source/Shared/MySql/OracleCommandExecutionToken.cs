using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.Oracle
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="CommandExecutionToken{OracleCommand, OracleParameter}" />
    public class OracleCommandExecutionToken : CommandExecutionToken<OracleCommand, OracleParameter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OracleCommandExecutionToken" /> class.
        /// </summary>
        public OracleCommandExecutionToken(ICommandDataSource<OracleCommand, OracleParameter> dataSource, string operationName, string commandText, IReadOnlyList<OracleParameter> parameters, CommandType commandType = CommandType.Text) : base(dataSource, operationName, commandText, parameters, commandType)
        {
        }


    }
}