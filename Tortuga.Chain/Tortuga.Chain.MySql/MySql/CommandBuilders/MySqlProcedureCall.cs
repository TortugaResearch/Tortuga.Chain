using MySqlConnector;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.MySql.CommandBuilders;

/// <summary>
/// Class MySqlProcedureCall.
/// </summary>
internal sealed class MySqlProcedureCall : ProcedureDbCommandBuilder<MySqlCommand, MySqlParameter>
{
	private readonly StoredProcedureMetadata<MySqlObjectName, MySqlDbType> m_Procedure;

	/// <summary>
	/// Initializes a new instance of the <see cref="MySqlProcedureCall"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="procedureName">Name of the procedure.</param>
	/// <param name="argumentValue">The argument value.</param>
	internal MySqlProcedureCall(MySqlDataSourceBase dataSource, MySqlObjectName procedureName, object? argumentValue) : base(dataSource, argumentValue)
	{
		if (procedureName == MySqlObjectName.Empty)
			throw new ArgumentException($"{nameof(procedureName)} is empty", nameof(procedureName));

		m_Procedure = DataSource.DatabaseMetadata.GetStoredProcedure(procedureName);
	}

	/// <summary>
	/// Gets the data source.
	/// </summary>
	/// <value>The data source.</value>
	public new MySqlDataSourceBase DataSource
	{
		get { return (MySqlDataSourceBase)base.DataSource; }
	}

	/// <summary>
	/// Prepares the command for execution by generating any necessary SQL.
	/// </summary>
	/// <param name="materializer">The materializer.</param>
	/// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
	public override CommandExecutionToken<MySqlCommand, MySqlParameter> Prepare(Materializer<MySqlCommand, MySqlParameter> materializer)
	{
		if (materializer == null)
			throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

		List<MySqlParameter> parameters;

		if (ArgumentValue is IEnumerable<MySqlParameter>)
		{
			parameters = ((IEnumerable<MySqlParameter>)ArgumentValue).ToList();
		}
		else
		{
			var sqlBuilder = m_Procedure.CreateSqlBuilder(StrictMode);
			sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue);
			parameters = sqlBuilder.GetParameters();
		}

		return new MySqlCommandExecutionToken(DataSource, m_Procedure.Name.ToString(), m_Procedure.Name.ToQuotedString(), parameters, CommandType.StoredProcedure);
	}
}
