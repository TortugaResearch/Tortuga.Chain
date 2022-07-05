using System.Data.SQLite;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SQLite.CommandBuilders
{
	/// <summary>
	/// Command object that represents an update operation.
	/// </summary>
	internal sealed class SQLiteUpdateObject<TArgument> : SQLiteObjectCommand<TArgument>
		where TArgument : class
	{
		readonly UpdateOptions m_Options;

		/// <summary>
		/// Initializes a new instance of the <see cref="SQLiteUpdateObject{TArgument}"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The options.</param>
		public SQLiteUpdateObject(SQLiteDataSourceBase dataSource, SQLiteObjectName tableName, TArgument argumentValue, UpdateOptions options)
			: base(dataSource, tableName, argumentValue)
		{
			m_Options = options;
		}

		/// <summary>
		/// Prepares the command for execution by generating any necessary SQL.
		/// </summary>
		/// <param name="materializer"></param>
		/// <returns><see cref="SQLiteCommandExecutionToken" /></returns>
		public override CommandExecutionToken<SQLiteCommand, SQLiteParameter> Prepare(Materializer<SQLiteCommand, SQLiteParameter> materializer)
		{
			if (materializer == null)
				throw new ArgumentNullException(nameof(materializer), $"{nameof(materializer)} is null.");

			if (!Table.HasPrimaryKey && !m_Options.HasFlag(UpdateOptions.UseKeyAttribute) && KeyColumns.Count == 0)
				throw new MappingException($"Cannot perform an update operation on {Table.Name} unless UpdateOptions.UseKeyAttribute or .WithKeys() is specified.");

			var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
			var desiredColumns = materializer.DesiredColumns();
			sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options, desiredColumns == Materializer.NoColumns);
			sqlBuilder.ApplyDesiredColumns(desiredColumns);

			if (KeyColumns.Count > 0)
				sqlBuilder.OverrideKeys(KeyColumns);

			var sql = new StringBuilder();
			if (m_Options.HasFlag(UpdateOptions.ReturnOldValues))
			{
				sqlBuilder.BuildSelectByKeyStatement(sql, Table.Name.ToQuotedString(), ";");
				sql.AppendLine();
			}
			sqlBuilder.BuildUpdateByKeyStatement(sql, Table.Name.ToQuotedString(), ";");
			if (!m_Options.HasFlag(UpdateOptions.ReturnOldValues))
			{
				sql.AppendLine();
				sqlBuilder.BuildSelectByKeyStatement(sql, Table.Name.ToQuotedString(), ";");
			}

			return new SQLiteCommandExecutionToken(DataSource, "Update " + Table.Name, sql.ToString(), sqlBuilder.GetParameters(), lockType: LockType.Write).CheckUpdateRowCount(m_Options);
		}
	}
}