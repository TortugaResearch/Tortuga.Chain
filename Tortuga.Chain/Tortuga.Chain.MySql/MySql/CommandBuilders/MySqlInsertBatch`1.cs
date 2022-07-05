using MySqlConnector;
using System.Text;
using Tortuga.Anchor;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.MySql.CommandBuilders;

/// <summary>
/// Class MySqlInsertBatchTable is when using a values clause with an array of rows.
/// </summary>
internal class MySqlInsertBatch<TObject> : DbCommandBuilder<MySqlCommand, MySqlParameter>
	where TObject : class
{
	readonly InsertOptions m_Options;
	readonly IReadOnlyList<TObject> m_SourceList;
	readonly TableOrViewMetadata<MySqlObjectName, MySqlDbType> m_Table;

	public MySqlInsertBatch(MySqlDataSourceBase dataSource, MySqlObjectName tableName, IEnumerable<TObject> objects, InsertOptions options) : base(dataSource)
	{
		if (dataSource == null)
			throw new ArgumentNullException(nameof(dataSource), $"{nameof(dataSource)} is null.");

		var sourceList = objects.AsReadOnlyList();

		if (sourceList == null || sourceList.Count == 0)
			throw new ArgumentException($"{nameof(objects)} is null or empty.", nameof(objects));

		m_SourceList = sourceList;
		m_Options = options;
		m_Table = dataSource.DatabaseMetadata.GetTableOrView(tableName);
	}

	public override CommandExecutionToken<MySqlCommand, MySqlParameter> Prepare(Materializer<MySqlCommand, MySqlParameter> materializer)
	{
		if (materializer == null)
			throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

		var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
		sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

		//This sets up the sqlBuilder. We're not actually going to build the parameters now
		sqlBuilder.ApplyArgumentValue(DataSource, m_SourceList[0], m_Options);

		var sql = new StringBuilder();

		bool identityInsert = m_Options.HasFlag(InsertOptions.IdentityInsert);

		sqlBuilder.BuildInsertClause(sql, $"INSERT INTO {m_Table.Name.ToQuotedString()} (", null, ")", identityInsert);
		sql.AppendLine("VALUES");

		var parameters = new List<MySqlParameter>();
		for (var i = 0; i < m_SourceList.Count; i++)
		{
			var parameterSuffix = "_" + i;
			var footer = (i == m_SourceList.Count - 1) ? ");" : "),";

			sqlBuilder.OverrideArgumentValue(DataSource, AuditRules.OperationTypes.Insert, m_SourceList[i]);
			sqlBuilder.BuildValuesClause(sql, "(", footer, identityInsert, parameterSuffix, parameters, Utilities.ParameterBuilderCallback);
		}

		var maxParams = DataSource.DatabaseMetadata.MaxParameters!.Value;
		if (parameters.Count > maxParams)
		{
			var parametersPerRow = parameters.Count / m_SourceList.Count;
			var maxRows = maxParams / parametersPerRow;
			throw new InvalidOperationException($"Batch insert exceeds MySql's parameter limit of {DataSource.DatabaseMetadata.MaxParameters}. Break the call into batches of {maxRows} or use InsertMultipleBatch");
		}

		return new MySqlCommandExecutionToken(DataSource, "Insert batch into " + m_Table.Name, sql.ToString(), parameters);
	}

	/// <summary>
	/// Returns the column associated with the column name.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns></returns>
	/// <remarks>
	/// If the column name was not found, this will return null
	/// </remarks>
	public override ColumnMetadata? TryGetColumn(string columnName)
	{
		return m_Table.Columns.TryGetColumn(columnName);
	}

	/// <summary>
	/// Returns a list of columns known to be non-nullable.
	/// </summary>
	/// <returns>
	/// If the command builder doesn't know which columns are non-nullable, an empty list will be returned.
	/// </returns>
	/// <remarks>
	/// This is used by materializers to skip IsNull checks.
	/// </remarks>
	public override IReadOnlyList<ColumnMetadata> TryGetNonNullableColumns() => m_Table.NonNullableColumns;
}