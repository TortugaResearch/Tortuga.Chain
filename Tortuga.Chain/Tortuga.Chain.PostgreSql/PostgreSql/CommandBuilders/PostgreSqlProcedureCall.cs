using Npgsql;
using NpgsqlTypes;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
	/// <summary>
	/// Class PostgreSqlProcedureCall.
	/// </summary>
	internal sealed class PostgreSqlProcedureCall : ProcedureDbCommandBuilder<NpgsqlCommand, NpgsqlParameter>
	{
		readonly StoredProcedureMetadata<PostgreSqlObjectName, NpgsqlDbType> m_Procedure;

		/// <summary>
		/// Initializes a new instance of the <see cref="PostgreSqlProcedureCall"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="procedureName">Name of the procedure.</param>
		/// <param name="argumentValue">The argument value.</param>
		internal PostgreSqlProcedureCall(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName procedureName, object? argumentValue) : base(dataSource, argumentValue)
		{
			if (procedureName == PostgreSqlObjectName.Empty)
				throw new ArgumentException($"{nameof(procedureName)} is empty", nameof(procedureName));

			m_Procedure = DataSource.DatabaseMetadata.GetStoredProcedure(procedureName);
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
		/// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
		public override CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
		{
			if (materializer == null)
				throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

			List<NpgsqlParameter> parameters;

			if (ArgumentValue is IEnumerable<NpgsqlParameter>)
			{
				parameters = ((IEnumerable<NpgsqlParameter>)ArgumentValue).ToList();
			}
			else
			{
				var sqlBuilder = m_Procedure.CreateSqlBuilder(StrictMode);
				sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue);
				parameters = sqlBuilder.GetParameters();
			}

			return new PostgreSqlCommandExecutionToken(DataSource, m_Procedure.Name.ToString(), m_Procedure.Name.ToQuotedString(), parameters, CommandType.StoredProcedure, true);
		}
	}
}
