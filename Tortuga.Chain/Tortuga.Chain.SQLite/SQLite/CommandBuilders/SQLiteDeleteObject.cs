﻿using System.Data.SQLite;
using System.Text;
using Tortuga.Chain.Core;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.SQLite.CommandBuilders
{
	/// <summary>
	/// Command object that represents a delete operation.
	/// </summary>
	internal sealed class SQLiteDeleteObject<TArgument> : SQLiteObjectCommand<TArgument>
		where TArgument : class
	{
		readonly DeleteOptions m_Options;

		/// <summary>
		/// Initializes a new instance of the <see cref="SQLiteDeleteObject{TArgument}"/> class.
		/// </summary>
		/// <param name="dataSource">The data source.</param>
		/// <param name="tableName">The table.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="options">The options.</param>
		public SQLiteDeleteObject(SQLiteDataSourceBase dataSource, SQLiteObjectName tableName, TArgument argumentValue, DeleteOptions options)
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

			var sqlBuilder = Table.CreateSqlBuilder(StrictMode);
			var desiredColumns = materializer.DesiredColumns();
			sqlBuilder.ApplyArgumentValue(DataSource, ArgumentValue, m_Options, desiredColumns == Materializer.NoColumns);
			sqlBuilder.ApplyDesiredColumns(desiredColumns);

			if (KeyColumns.Count > 0)
				sqlBuilder.OverrideKeys(KeyColumns);

			var sql = new StringBuilder();
			sqlBuilder.BuildSelectByKeyStatement(sql, Table.Name.ToQuotedString(), ";");
			sql.AppendLine();
			sqlBuilder.BuildDeleteStatement(sql, Table.Name.ToQuotedString(), ";");

			return new SQLiteCommandExecutionToken(DataSource, "Delete from " + Table.Name, sql.ToString(), sqlBuilder.GetParameters(), lockType: LockType.Write).CheckDeleteRowCount(m_Options);
		}
	}
}