using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders;

/// <summary>
/// Use for scalar functions.
/// </summary>
/// <seealso cref="ScalarDbCommandBuilder{SqlCommand, SqlParameter}" />
sealed internal class SqlServerScalarFunction : ScalarFunctionCommandBuilder<SqlCommand, SqlParameter>
{
	readonly ScalarFunctionMetadata<SqlServerObjectName, SqlDbType> m_Function;

	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerTableFunction" /> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="scalarFunctionName">Name of the scalar function.</param>
	/// <param name="functionArgumentValue">The function argument.</param>
	public SqlServerScalarFunction(SqlServerDataSourceBase dataSource, SqlServerObjectName scalarFunctionName, object? functionArgumentValue) : base(dataSource, functionArgumentValue)
	{
		m_Function = dataSource.DatabaseMetadata.GetScalarFunction(scalarFunctionName);
	}

	/// <summary>
	/// Gets the data source.
	/// </summary>
	/// <value>The data source.</value>
	public new SqlServerDataSourceBase DataSource
	{
		get { return (SqlServerDataSourceBase)base.DataSource; }
	}

	/// <summary>
	/// Prepares the command for execution by generating any necessary SQL.
	/// </summary>
	/// <param name="materializer">The materializer.</param>
	/// <returns>
	/// ExecutionToken&lt;TCommand&gt;.
	/// </returns>
	[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
	public override CommandExecutionToken<SqlCommand, SqlParameter> Prepare(Materializer<SqlCommand, SqlParameter> materializer)
	{
		if (materializer == null)
			throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

		var sqlBuilder = m_Function.CreateSqlBuilder(StrictMode);
		sqlBuilder.ApplyRulesForSelect(DataSource);

		if (FunctionArgumentValue != null)
			sqlBuilder.ApplyArgumentValue(DataSource, FunctionArgumentValue);

		var sql = new StringBuilder();
		sqlBuilder.BuildFromFunctionClause(sql, $"SELECT {m_Function.Name.ToQuotedString()} (", " )");
		sql.Append(';');

		List<SqlParameter> parameters;
		parameters = sqlBuilder.GetParameters();

		return new SqlServerCommandExecutionToken(DataSource, "Query Function " + m_Function.Name, sql.ToString(), parameters);
	}
}
