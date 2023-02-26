using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.CommandBuilders;

[SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
class GenericDbSqlCall : SqlCallCommandBuilder<DbCommand, DbParameter>
{
	readonly GenericDbDataSource m_DataSource;

	/// <summary>
	/// Initializes a new instance of the <see cref="GenericDbSqlCall"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="sqlStatement">The SQL statement.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <exception cref="ArgumentException">sqlStatement is null or empty.;sqlStatement</exception>
	public GenericDbSqlCall(GenericDbDataSource dataSource, string sqlStatement, object? argumentValue) : base(dataSource, sqlStatement, argumentValue)
	{
		if (string.IsNullOrEmpty(sqlStatement))
			throw new ArgumentException("sqlStatement is null or empty.", nameof(sqlStatement));

		m_DataSource = dataSource;
	}

	/// <summary>
	/// Prepares the command for execution by generating any necessary SQL.
	/// </summary>
	/// <param name="materializer">The materializer.</param>
	/// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
	public override CommandExecutionToken<DbCommand, DbParameter> Prepare(Materializer<DbCommand, DbParameter> materializer)
	{
		var parameters = SqlBuilder.GetParameters(ArgumentValue, _ => m_DataSource.CreateParameter());
		return new CommandExecutionToken<DbCommand, DbParameter>(DataSource, "Raw SQL call", SqlStatement, parameters);
	}
}
