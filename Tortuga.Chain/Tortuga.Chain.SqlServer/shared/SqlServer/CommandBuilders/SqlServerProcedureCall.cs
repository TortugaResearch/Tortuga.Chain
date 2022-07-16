using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.Materializers;

namespace Tortuga.Chain.SqlServer.CommandBuilders;

/// <summary>
/// Class SqlServerProcedureCall.
/// </summary>
internal sealed partial class SqlServerProcedureCall : ProcedureDbCommandBuilder<SqlCommand, SqlParameter>
{
	readonly StoredProcedureMetadata<SqlServerObjectName, SqlDbType> m_Procedure;

	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerProcedureCall"/> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="procedureName">Name of the procedure.</param>
	/// <param name="argumentValue">The argument value.</param>
	internal SqlServerProcedureCall(SqlServerDataSourceBase dataSource, SqlServerObjectName procedureName, object? argumentValue) : base(dataSource, argumentValue)
	{
		if (procedureName == SqlServerObjectName.Empty)
			throw new ArgumentException($"{nameof(procedureName)} is empty", nameof(procedureName));

		m_Procedure = DataSource.DatabaseMetadata.GetStoredProcedure(procedureName);
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
	/// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
	public override CommandExecutionToken<SqlCommand, SqlParameter> Prepare(Materializer<SqlCommand, SqlParameter> materializer)
	{
		if (materializer == null)
			throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

		List<SqlParameter> parameters;

		if (ArgumentValue is IEnumerable<SqlParameter>)
		{
			parameters = ((IEnumerable<SqlParameter>)ArgumentValue).ToList();
		}
		else
		{
			var sqlBuilder = m_Procedure.CreateSqlBuilder(StrictMode);
			sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue);
			parameters = sqlBuilder.GetParameters();
		}

		return new SqlServerCommandExecutionToken(DataSource, m_Procedure.Name.ToString(), m_Procedure.Name.ToQuotedString(), parameters, CommandType.StoredProcedure);
	}
}

partial class SqlServerProcedureCall : ISupportsChangeListener
{
	SqlServerCommandExecutionToken ISupportsChangeListener.Prepare(Materializer<SqlCommand, SqlParameter> materializer)
	{
		return (SqlServerCommandExecutionToken)Prepare(materializer);
	}

	/// <summary>
	/// Waits for change in the data that is returned by this operation.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="state">User defined state, usually used for logging.</param>
	/// <returns>Task that can be waited for.</returns>
	/// <remarks>This requires the use of SQL Dependency</remarks>
	public Task WaitForChange(CancellationToken cancellationToken, object? state = null)
	{
		return WaitForChangeMaterializer.GenerateTask(this, cancellationToken, state);
	}
}
