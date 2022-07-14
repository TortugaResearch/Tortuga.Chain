using System.Data.OleDb;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Access.CommandBuilders;

/// <summary>
/// Class that represents an operation based on a raw SQL statement.
/// </summary>
internal sealed class AccessSqlCall : DbSqlCall<OleDbCommand, OleDbParameter>
{
	/// <summary>
	/// Creates a new instance of <see cref="AccessSqlCall" />
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="sqlStatement">The SQL statement.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <exception cref="System.ArgumentException">SQL statement is null or empty.;sqlStatement</exception>
	public AccessSqlCall(AccessDataSourceBase dataSource, string sqlStatement, object? argumentValue) :
		base(dataSource, sqlStatement, argumentValue)
	{
		if (string.IsNullOrEmpty(sqlStatement))
			throw new ArgumentException($"{nameof(sqlStatement)} is null or empty.", nameof(sqlStatement));
	}

	/// <summary>
	/// Prepares the command for execution by generating any necessary SQL.
	/// </summary>
	/// <param name="materializer"></param>
	/// <returns></returns>
	public override CommandExecutionToken<OleDbCommand, OleDbParameter> Prepare(Materializer<OleDbCommand, OleDbParameter> materializer)
	{
		return new AccessCommandExecutionToken(DataSource, "Raw SQL call", SqlStatement, SqlBuilder.GetParameters<OleDbParameter>(ArgumentValue));
	}
}
