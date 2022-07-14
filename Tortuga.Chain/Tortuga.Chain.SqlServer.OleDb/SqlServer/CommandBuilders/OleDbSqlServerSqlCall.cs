using System.Data.OleDb;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SqlServer.CommandBuilders;

/// <summary>
/// Class OleDbSqlServerSqlCall.
/// </summary>
internal sealed class OleDbSqlServerSqlCall : DbSqlCall<OleDbCommand, OleDbParameter>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="OleDbSqlServerSqlCall" /> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="sqlStatement">The SQL statement.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <exception cref="ArgumentException">sqlStatement is null or empty.;sqlStatement</exception>
	public OleDbSqlServerSqlCall(OleDbSqlServerDataSourceBase dataSource, string sqlStatement, object? argumentValue) : base(dataSource, sqlStatement, argumentValue)
	{
		if (string.IsNullOrEmpty(sqlStatement))
			throw new ArgumentException($"{nameof(sqlStatement)} is null or empty.", nameof(sqlStatement));
	}

	/// <summary>
	/// Prepares the command for execution by generating any necessary SQL.
	/// </summary>
	/// <param name="materializer">The materializer.</param>
	/// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
	public override CommandExecutionToken<OleDbCommand, OleDbParameter> Prepare(Materializer<OleDbCommand, OleDbParameter> materializer)
	{
		return new OleDbCommandExecutionToken(DataSource, "Raw SQL call", SqlStatement, SqlBuilder.GetParameters<OleDbParameter>(ArgumentValue));
	}
}
