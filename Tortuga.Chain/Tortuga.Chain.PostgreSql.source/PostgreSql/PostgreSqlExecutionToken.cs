using Npgsql;
using System.Collections.Generic;
using System.Data;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.PostgreSql
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ExecutionToken{NpgsqlCommand, NpgsqlParameter}" />
    public class PostgreSqlExecutionToken : ExecutionToken<NpgsqlCommand, NpgsqlParameter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlExecutionToken" /> class.
        /// </summary>
        /// <returns>PostgreSqlxecutionToken.</returns>
        public PostgreSqlExecutionToken(DataSource<NpgsqlCommand, NpgsqlParameter> dataSource, string operationName, string commandText, IReadOnlyList<NpgsqlParameter> parameters, CommandType commandType = CommandType.Text) : base(dataSource, operationName, commandText, parameters, commandType)
        {
        }
    }
}