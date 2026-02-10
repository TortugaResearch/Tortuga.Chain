using Npgsql;
using NpgsqlTypes;
using System.Text;
using Tortuga.Anchor;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql.CommandBuilders
{
	/// <summary>
	/// Class that represents a PostgreSql Insert.
	/// </summary>
	internal sealed class PostgreSqlInsertBatch<TObject> : MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter>
		where TObject : class
	{
		readonly InsertOptions m_Options;
		readonly IReadOnlyList<TObject> m_SourceList;
		readonly TableOrViewMetadata<PostgreSqlObjectName, NpgsqlDbType> m_Table;

		public PostgreSqlInsertBatch(PostgreSqlDataSourceBase dataSource, PostgreSqlObjectName tableName, IEnumerable<TObject> objects, InsertOptions options) : base(dataSource)
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

		/// <summary>
		/// Prepares the command for execution by generating any necessary SQL.
		/// </summary>
		/// <param name="materializer"></param>
		/// <returns><see cref="PostgreSqlCommandExecutionToken" /></returns>
		public override CommandExecutionToken<NpgsqlCommand, NpgsqlParameter> Prepare(Materializer<NpgsqlCommand, NpgsqlParameter> materializer)
		{
			if (materializer == null)
				throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

			var identityInsert = m_Options.HasFlag(InsertOptions.IdentityInsert);

			var sqlBuilder = m_Table.CreateSqlBuilder(StrictMode);
			sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

			//This sets up the sqlBuilder. We're not actually going to build the parameters now
			sqlBuilder.ApplyArgumentValue(DataSource, m_SourceList[0], m_Options);

			var sql = new StringBuilder();

			sqlBuilder.BuildInsertClause(sql, $"INSERT INTO {m_Table.Name.ToQuotedString()} (", null, ")", identityInsert);
			if (identityInsert)
				sql.Append(" OVERRIDING SYSTEM VALUE"); 
			sql.AppendLine(" VALUES");

			var parameters = new List<NpgsqlParameter>();
			for (var i = 0; i < m_SourceList.Count; i++)
			{
				var parameterSuffix = "_" + i;
				var footer = (i == m_SourceList.Count - 1) ? ")" : "),";

				sqlBuilder.OverrideArgumentValue(DataSource, AuditRules.OperationTypes.Insert, m_SourceList[i]);
				sqlBuilder.BuildValuesClause(sql, "(", footer, identityInsert, parameterSuffix, parameters, Utilities.ParameterBuilderCallback);
			}
			sqlBuilder.BuildSelectClause(sql, " RETURNING ", null, ";");

			var maxParams = DataSource.DatabaseMetadata.MaxParameters!.Value;
			if (parameters.Count > maxParams)
			{
				var parametersPerRow = parameters.Count / m_SourceList.Count;
				var maxRows = maxParams / parametersPerRow;
				throw new InvalidOperationException($"Batch insert exceeds PostgreSql's parameter limit of {DataSource.DatabaseMetadata.MaxParameters}. Break the call into batches of {maxRows} or use InsertMultipleBatch");
			}

			return new PostgreSqlCommandExecutionToken(DataSource, "Insert batch into " + m_Table.Name, sql.ToString(), parameters);
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
		/// Returns a list of columns.
		/// </summary>
		/// <returns>If the command builder doesn't know which columns are available, an empty list will be returned.</returns>
		/// <remarks>This is used by materializers to skip exclude columns.</remarks>
		public override IReadOnlyList<ColumnMetadata> TryGetColumns() => m_Table.Columns;

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
}
