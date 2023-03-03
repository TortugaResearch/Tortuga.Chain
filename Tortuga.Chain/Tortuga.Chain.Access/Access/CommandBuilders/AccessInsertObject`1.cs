using System.Data.OleDb;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Access.CommandBuilders;

/// <summary>
/// Class that represents a Access Insert.
/// </summary>
internal sealed class AccessInsertObject<TArgument> : AccessObjectCommand<TArgument>
	where TArgument : class
{
	readonly InsertOptions m_Options;

	/// <summary>
	/// Initializes a new instance of <see cref="AccessInsertObject{TArgument}" /> class.
	/// </summary>
	/// <param name="dataSource">The data source.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="options">The options.</param>
	public AccessInsertObject(AccessDataSourceBase dataSource, AccessObjectName tableName, TArgument argumentValue, InsertOptions options)
		: base(dataSource, tableName, argumentValue)
	{
		m_Options = options;
	}

	/// <summary>
	/// Prepares the command for execution by generating any necessary SQL.
	/// </summary>
	/// <param name="materializer"></param>
	/// <returns><see cref="AccessCommandExecutionToken" /></returns>
	public override CommandExecutionToken<OleDbCommand, OleDbParameter> Prepare(Materializer<OleDbCommand, OleDbParameter> materializer)
	{
		if (materializer == null)
			throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

		var desiredColumns = materializer.DesiredColumns();
		var identityInsert = m_Options.HasFlag(InsertOptions.IdentityInsert);

		var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
		sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
		sqlBuilder.ApplyDesiredColumns(desiredColumns);

		if (KeyColumns.Count > 0)
			sqlBuilder.OverrideKeys(KeyColumns);

		var sql = new StringBuilder();
		sqlBuilder.BuildInsertClause(sql, $"INSERT INTO {Table.Name.ToQuotedString()} (", null, ")", identityInsert);
		sqlBuilder.BuildValuesClause(sql, " VALUES (", ")", identityInsert);
		sql.Append(";");

		var result = new AccessCommandExecutionToken(DataSource, "Insert into " + Table.Name, sql.ToString(), sqlBuilder.GetParameters(DataSource));
		if (desiredColumns == Materializer.AutoSelectDesiredColumns)
		{
			result.ExecutionMode = AccessCommandExecutionMode.NonQuery;
			result.NextCommand = new AccessCommandExecutionToken(DataSource, "Fetch autonumber", "SELECT @@IDENTITY", new List<OleDbParameter>());
		}
		else if (desiredColumns.Count > 0)
		{
			result.ExecutionMode = AccessCommandExecutionMode.NonQuery;
			result.NextCommand = new AccessCommandExecutionToken(DataSource, "Fetch autonumber", "SELECT @@IDENTITY", new List<OleDbParameter>());
			result.NextCommand.ExecutionMode = AccessCommandExecutionMode.ExecuteScalarAndForward;
			result.NextCommand.ForwardResult = value => { result.NextCommand.NextCommand = PrepareNext(desiredColumns, value); };
		}

		return result;
	}

	AccessCommandExecutionToken PrepareNext(IReadOnlyList<string> desiredColumns, object? previousValue)
	{
		var primaryKeys = Table.PrimaryKeyColumns;
		if (primaryKeys.Count != 1)
			throw new MappingException($"Insert operation cannot return any values for {Table.Name} because it doesn't have a single primary key.");

		var columnMetadata = primaryKeys.Single();
		var where = columnMetadata.SqlName + " = " + columnMetadata.SqlVariableName;

		var parameters = new List<OleDbParameter>();

		var param = Utilities.CreateParameter(columnMetadata, columnMetadata.SqlVariableName, previousValue);
		parameters.Add(param);

		var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
		sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
		sqlBuilder.ApplyDesiredColumns(desiredColumns);

		var sql = new StringBuilder();
		sqlBuilder.BuildSelectClause(sql, "SELECT ", null, null);
		sql.Append(" FROM " + Table.Name.ToQuotedString());
		sql.Append(" WHERE " + where);
		sql.Append(";");

		return new AccessCommandExecutionToken(DataSource, "Select after insert into " + Table.Name, sql.ToString(), parameters);
	}
}