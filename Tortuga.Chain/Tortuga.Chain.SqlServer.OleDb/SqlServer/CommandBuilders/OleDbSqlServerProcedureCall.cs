using System.Data.OleDb;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders;

/// <summary>
/// Class OleDbSqlServerProcedureCall.
/// </summary>
internal sealed class OleDbSqlServerProcedureCall : ProcedureDbCommandBuilder<OleDbCommand, OleDbParameter>
{
	readonly StoredProcedureMetadata<SqlServerObjectName, OleDbType> m_Procedure;

	/// <summary>
	/// Initializes a new instance of the <see cref="OleDbSqlServerProcedureCall"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="procedureName">Name of the procedure.</param>
	/// <param name="argumentValue">The argument value.</param>
	internal OleDbSqlServerProcedureCall(OleDbSqlServerDataSourceBase dataSource, SqlServerObjectName procedureName, object? argumentValue) : base(dataSource, argumentValue)
	{
		if (procedureName == SqlServerObjectName.Empty)
			throw new ArgumentException($"{nameof(procedureName)} is empty", nameof(procedureName));

		m_Procedure = DataSource.DatabaseMetadata.GetStoredProcedure(procedureName);
	}

	/// <summary>
	/// Gets the data source.
	/// </summary>
	/// <value>The data source.</value>
	public new OleDbSqlServerDataSourceBase DataSource
	{
		get { return (OleDbSqlServerDataSourceBase)base.DataSource; }
	}

	/// <summary>
	/// Prepares the command for execution by generating any necessary SQL.
	/// </summary>
	/// <param name="materializer">The materializer.</param>
	/// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
	public override CommandExecutionToken<OleDbCommand, OleDbParameter> Prepare(Materializer<OleDbCommand, OleDbParameter> materializer)
	{
		if (materializer == null)
			throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

		List<OleDbParameter> parameters;

		if (ArgumentValue is IEnumerable<OleDbParameter>)
		{
			parameters = ((IEnumerable<OleDbParameter>)ArgumentValue).ToList();
		}
		else
		{
			var sqlBuilder = m_Procedure.CreateSqlBuilder(StrictMode);
			sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue);
			parameters = sqlBuilder.GetParameters();
		}

		return new OleDbCommandExecutionToken(DataSource, m_Procedure.Name.ToString(), m_Procedure.Name.ToQuotedString(), parameters, CommandType.StoredProcedure);
	}
}
