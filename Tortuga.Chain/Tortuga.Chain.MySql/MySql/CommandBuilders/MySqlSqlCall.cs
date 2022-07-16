using MySqlConnector;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.MySql.CommandBuilders;

/// <summary>
/// Class MySqlSqlCall
/// </summary>
public class MySqlSqlCall : DbSqlCall<MySqlCommand, MySqlParameter>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MySqlSqlCall"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="sqlStatement">The SQL statement.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <exception cref="ArgumentException">sqlStatement is null or empty.;sqlStatement</exception>
	public MySqlSqlCall(MySqlDataSourceBase dataSource, string sqlStatement, object? argumentValue) : base(dataSource, sqlStatement, argumentValue)
	{
		if (string.IsNullOrEmpty(sqlStatement))
			throw new ArgumentException($"{nameof(sqlStatement)} is null or empty.", nameof(sqlStatement));
	}

	/// <summary>
	/// Prepares the command for execution by generating any necessary SQL.
	/// </summary>
	/// <param name="materializer">The materializer.</param>
	/// <returns>
	/// ExecutionToken&lt;TCommand&gt;.
	/// </returns>
	public override CommandExecutionToken<MySqlCommand, MySqlParameter> Prepare(Materializer<MySqlCommand, MySqlParameter> materializer)
	{
		return new MySqlCommandExecutionToken(DataSource, "Raw SQL call", SqlStatement, SqlBuilder.GetParameters<MySqlParameter>(ArgumentValue));
	}
}
