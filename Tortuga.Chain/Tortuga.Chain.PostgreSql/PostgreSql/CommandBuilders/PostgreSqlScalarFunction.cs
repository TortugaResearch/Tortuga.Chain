using Npgsql;
using NpgsqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
	/// <summary>
	/// Use for scalar functions.
	/// </summary>
	/// <seealso cref="ScalarDbCommandBuilder{NpgsqlCommand, NpgsqlParameter}" />
	internal class PostgreSqlScalarFunction : ScalarFunctionCommandBuilder<NpgsqlCommand, NpgsqlParameter>
	{
		readonly ScalarFunctionMetadata<PostgreSqlObjectName, NpgsqlDbType> m_Function;

		/// <summary>
		/// Initializes a new instance of the <see cref="PostgreSqlScalarFunction" /> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="scalarFunctionName">Name of the scalar function.</param>
		/// <param name="functionArgumentValue">The function argument.</param>
		public PostgreSqlScalarFunction(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName scalarFunctionName, object? functionArgumentValue) : base(dataSource, functionArgumentValue)
		{
			m_Function = dataSource.DatabaseMetadata.GetScalarFunction(scalarFunctionName);
		}

		/// <summary>
		/// Gets the data source.
		/// </summary>
		/// <value>The data source.</value>
		public new PostgreSqlDataSourceBase DataSource => (PostgreSqlDataSourceBase)base.DataSource;

		/// <summary>
		/// Prepares the command for execution by generating any necessary SQL.
		/// </summary>
		/// <param name="materializer">The materializer.</param>
		/// <returns>
		/// ExecutionToken&lt;TCommand&gt;.
		/// </returns>
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public override CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
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

			List<NpgsqlParameter> parameters;
			parameters = sqlBuilder.GetParameters();

			return new PostgreSqlCommandExecutionToken(DataSource, "Query Function " + m_Function.Name, sql.ToString(), parameters);
		}
	}
}
