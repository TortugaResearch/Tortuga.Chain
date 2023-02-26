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

		m_DefaultStringType = dataSource.DefaultStringType;
		m_DefaultStringLength = dataSource.DefaultStringLength;
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

	SqlParameter CreateParameter(object? value)
	{
		var result = new SqlParameter();
		if (value is string s)
		{
			if (m_DefaultStringType != null)
				result.SqlDbType = m_DefaultStringType.Value;

			if (m_DefaultStringLength == -1)
				result.Size = -1;
			else if (m_DefaultStringLength != null)
				result.Size = Math.Max(m_DefaultStringLength.Value, s.Length);
		}
		return result;
	}

	internal SqlCallCommandBuilder<SqlCommand, SqlParameter> WithStringLength(int? defaultStringLength)
	{
		m_DefaultStringLength = defaultStringLength;
		return this;
	}

	internal SqlCallCommandBuilder<SqlCommand, SqlParameter> WithStringType(SqlDbType? defaultStringType)
	{
		m_DefaultStringType = defaultStringType;
		return this;
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
