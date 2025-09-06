using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.SqlServer.Materializers;

namespace Tortuga.Chain.SqlServer.CommandBuilders;

/// <summary>
/// Class SqlServerSqlCall.
/// </summary>
internal sealed partial class SqlServerSqlCall : SqlCallCommandBuilder<SqlCommand, SqlParameter>
{
	SqlDbType? m_DefaultStringType;
	int? m_DefaultStringLength;
	readonly SqlServerDataSourceBase m_DataSource;

	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerSqlCall" /> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="sqlStatement">The SQL statement.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <exception cref="ArgumentException">sqlStatement is null or empty.;sqlStatement</exception>
	public SqlServerSqlCall(SqlServerDataSourceBase dataSource, string sqlStatement, object? argumentValue) : base(dataSource, sqlStatement, argumentValue)
	{
		if (string.IsNullOrEmpty(sqlStatement))
			throw new ArgumentException($"{nameof(sqlStatement)} is null or empty.", nameof(sqlStatement));
		m_DataSource = dataSource;
	}

	public SqlServerSqlCall(SqlServerDataSourceBase dataSource, string sqlStatement, object? argumentValue, SqlServerParameterDefaults defaults) : this(dataSource, sqlStatement, argumentValue)
	{
		m_DataSource = dataSource;
		m_DefaultStringType = defaults.StringType;
		m_DefaultStringLength = defaults.StringLength;
	}

	/// <summary>
	/// Prepares the command for execution by generating any necessary SQL.
	/// </summary>
	/// <param name="materializer">The materializer.</param>
	/// <returns>ExecutionToken&lt;TCommand&gt;.</returns>
	public override CommandExecutionToken<SqlCommand, SqlParameter> Prepare(Materializer<SqlCommand, SqlParameter> materializer)
	{
		return new SqlServerCommandExecutionToken(DataSource, "Raw SQL call", SqlStatement, SqlBuilder.GetParameters<SqlParameter>(ArgumentValue, CreateParameter));
	}

	SqlParameter CreateParameter(string parameterName, object? value)
	{
		var result = new SqlParameter(parameterName, value ?? DBNull.Value);
		if (value is string s)
		{
			if (m_DefaultStringType != null)
				result.SqlDbType = m_DefaultStringType.Value;

			if (m_DefaultStringLength.HasValue)
				result.Size = m_DefaultStringLength.Value;
			else if (result.SqlDbType == SqlDbType.VarChar && m_DataSource.DefaultVarCharLength.HasValue)
				result.Size = m_DataSource.DefaultVarCharLength.Value;
			else if (result.SqlDbType == SqlDbType.NVarChar && m_DataSource.DefaultNVarCharLength.HasValue)
				result.Size = m_DataSource.DefaultNVarCharLength.Value;

			//Fix up sizes
			if (result.Size != SqlServerParameterDefaults.Max && result.Size < s.Length)
				result.Size = s.Length;

			//Kick over to max if needed.
			switch (result.SqlDbType)
			{
				case SqlDbType.VarChar:
					if (result.Size > SqlServerParameterDefaults.MaxVarChar)
						result.Size = SqlServerParameterDefaults.Max;
					break;
				case SqlDbType.NVarChar:
					if (result.Size > SqlServerParameterDefaults.MaxNVarChar)
						result.Size = SqlServerParameterDefaults.Max;
					break;
			}



		}
		return result;
	}

}

partial class SqlServerSqlCall : ISupportsChangeListener
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
