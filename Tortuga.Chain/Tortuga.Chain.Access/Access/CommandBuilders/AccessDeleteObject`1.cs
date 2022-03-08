using System.Data.OleDb;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Access.CommandBuilders
{
	/// <summary>
	/// Command object that represents a delete operation.
	/// </summary>
	internal sealed class AccessDeleteObject<TArgument> : AccessObjectCommand<TArgument>
		where TArgument : class
	{
		readonly DeleteOptions m_Options;

		/// <summary>
		/// Initializes a new instance of the <see cref="AccessDeleteObject{TArgument}"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="tableName">The table.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The options.</param>
		public AccessDeleteObject(AccessDataSourceBase dataSource, AccessObjectName tableName, TArgument argumentValue, DeleteOptions options)
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

			var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
			var desiredColumns = materializer.DesiredColumns();
			sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options, desiredColumns == Materializer.NoColumns);
			sqlBuilder.ApplyDesiredColumns(desiredColumns);

			if (KeyColumns.Count > 0)
				sqlBuilder.OverrideKeys(KeyColumns);

			var sql = new StringBuilder();
			sql.Append("DELETE FROM " + Table.Name.ToQuotedString());
			sqlBuilder.BuildWhereClause(sql, " WHERE ", null);
			sql.Append(";");

			return new AccessCommandExecutionToken(DataSource, "Delete from " + Table.Name, sql.ToString(), sqlBuilder.GetParameters()).CheckDeleteRowCount(m_Options);
		}
	}
}
