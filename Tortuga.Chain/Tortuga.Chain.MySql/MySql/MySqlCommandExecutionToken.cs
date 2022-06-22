using MySqlConnector;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;

namespace Tortuga.Chain.MySql;

/// <summary>
/// Class MySqlCommandExecutionToken.
/// </summary>
public class MySqlCommandExecutionToken : CommandExecutionToken<MySqlCommand, MySqlParameter>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MySqlCommandExecutionToken" /> class.
	/// </summary>
	public MySqlCommandExecutionToken(ICommandDataSource<MySqlCommand, MySqlParameter> dataSource, string operationName, string commandText, IReadOnlyList<MySqlParameter> parameters, CommandType commandType = CommandType.Text) : base(dataSource, operationName, commandText, parameters, commandType)
	{
	}
}
