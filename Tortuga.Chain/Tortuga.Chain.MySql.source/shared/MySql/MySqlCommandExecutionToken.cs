using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.MySql
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="CommandExecutionToken{MySqlCommand, MySqlParameter}" />
    public class MySqlCommandExecutionToken : CommandExecutionToken<MySqlCommand, MySqlParameter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlCommandExecutionToken" /> class.
        /// </summary>
        public MySqlCommandExecutionToken(ICommandDataSource<MySqlCommand, MySqlParameter> dataSource, string operationName, string commandText, IReadOnlyList<MySqlParameter> parameters, CommandType commandType = CommandType.Text) : base(dataSource, operationName, commandText, parameters, commandType)
        {
        }


    }
}