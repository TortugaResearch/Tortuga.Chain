using System.Data.OleDb;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Access.CommandBuilders
{
	/// <summary>
	/// Command object that represents an update operation.
	/// </summary>
	internal sealed class AccessUpdateObject<TArgument> : AccessObjectCommand<TArgument>
		where TArgument : class
	{
		readonly UpdateOptions m_Options;

		/// <summary>
		/// Initializes a new instance of the <see cref="AccessUpdateObject{TArgument}"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The options.</param>
		public AccessUpdateObject(AccessDataSourceBase dataSource, AccessObjectName tableName, TArgument argumentValue, UpdateOptions options)
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

			if (!Table.IsTable)
				throw new MappingException($"Cannot perform an update operation on the view {Table.Name}.");

			if (!Table.HasPrimaryKey && !m_Options.HasFlag(UpdateOptions.UseKeyAttribute) && KeyColumns.Count == 0)
				throw new MappingException($"Cannot perform an update operation on {Table.Name} unless UpdateOptions.UseKeyAttribute or .WithKeys() is specified.");

			var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
			sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);
			sqlBuilder.ApplyDesiredColumns(materializer.DesiredColumns());

			if (KeyColumns.Count > 0)
				sqlBuilder.OverrideKeys(KeyColumns);

			var sql = new StringBuilder($"UPDATE {Table.Name.ToQuotedString()}");
			sqlBuilder.BuildSetClause(sql, " SET ", null, null);
			sqlBuilder.BuildWhereClause(sql, " WHERE ", null);
			sql.Append(";");

			var updateCommand = new AccessCommandExecutionToken(DataSource, "Update " + Table.Name, sql.ToString(), sqlBuilder.GetParametersKeysLast()).CheckUpdateRowCount(m_Options);
			updateCommand.ExecutionMode = AccessCommandExecutionMode.NonQuery;

			var desiredColumns = materializer.DesiredColumns();
			if (desiredColumns == Materializer.NoColumns)
				return updateCommand;

			if (m_Options.HasFlag(UpdateOptions.ReturnOldValues))
			{
				var result = PrepareRead(desiredColumns, "before");
				result.NextCommand = updateCommand;
				return result;
			}
			else
			{
				updateCommand.NextCommand = PrepareRead(desiredColumns, "after");
				return updateCommand;
			}
		}

		AccessCommandExecutionToken PrepareRead(IReadOnlyList<string> desiredColumns, string label)
		{
			var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
			sqlBuilder.ApplyDesiredColumns(desiredColumns);
			sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options);

			var sql = new StringBuilder();
			sqlBuilder.BuildSelectClause(sql, "SELECT ", null, null);
			sql.Append(" FROM " + Table.Name.ToQuotedString());
			sqlBuilder.BuildWhereClause(sql, " WHERE ", null);
			sql.Append(";");

			return new AccessCommandExecutionToken(DataSource, $"Query {label} updating " + Table.Name, sql.ToString(), sqlBuilder.GetParameters());
		}
	}
}
