using System.Data.SQLite;
using Tortuga.Anchor;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SQLite.CommandBuilders;

namespace Tortuga.Chain.SQLite
{
	partial class SQLiteDataSourceBase
	{


		/// <summary>
		/// Creates a operation based on a raw SQL statement.
		/// </summary>
		/// <param name="sqlStatement">The SQL statement.</param>
		/// <param name="lockType">Type of the lock.</param>
		/// <returns>SQLiteSqlCall.</returns>
		public MultipleTableDbCommandBuilder<SQLiteCommand, SQLiteParameter> Sql(string sqlStatement, LockType lockType)
		{
			return new SQLiteSqlCall(this, sqlStatement, null, lockType);
		}

		/// <summary>
		/// Creates a operation based on a raw SQL statement.
		/// </summary>
		/// <param name="sqlStatement">The SQL statement.</param>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="lockType">Type of the lock.</param>
		/// <returns>SQLiteSqlCall.</returns>
		public MultipleTableDbCommandBuilder<SQLiteCommand, SQLiteParameter> Sql(string sqlStatement, object argumentValue, LockType lockType)
		{
			return new SQLiteSqlCall(this, sqlStatement, argumentValue, lockType);
		}



		SQLiteTableOrView<TObject> OnGetByKey<TObject, TKey>(SQLiteObjectName tableName, ColumnMetadata<DbType> columnMetadata, TKey key)
			where TObject : class
		{
			string where = columnMetadata.SqlName + " = @Param0";

			var parameters = new List<SQLiteParameter>();
			var param = new SQLiteParameter("@Param0", key);
			if (columnMetadata.DbType.HasValue)
				param.DbType = columnMetadata.DbType.Value;
			parameters.Add(param);

			return new SQLiteTableOrView<TObject>(this, tableName, where, parameters);
		}

		MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter, TObject> OnGetByKeyList<TObject, TKey>(SQLiteObjectName tableName, ColumnMetadata<DbType> columnMetadata, IEnumerable<TKey> keys)
			where TObject : class
		{
			var keyList = keys.AsList();

			string where;
			if (keys.Count() > 1)
				where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";
			else
				where = columnMetadata.SqlName + " = @Param0";

			var parameters = new List<SQLiteParameter>();
			for (var i = 0; i < keyList.Count; i++)
			{
				var param = new SQLiteParameter("@Param" + i, keyList[i]);
				if (columnMetadata.DbType.HasValue)
					param.DbType = columnMetadata.DbType.Value;
				parameters.Add(param);
			}

			return new MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter, TObject>(new SQLiteTableOrView<TObject>(this, tableName, where, parameters));
		}

		MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> OnDeleteSet(SQLiteObjectName tableName, string whereClause, object? argumentValue)
		{
			return new SQLiteDeleteSet(this, tableName, whereClause, argumentValue);
		}

		MultipleRowDbCommandBuilder<SQLiteCommand, SQLiteParameter> OnDeleteSet(SQLiteObjectName tableName, object filterValue, FilterOptions filterOptions)
		{
			return new SQLiteDeleteSet(this, tableName, filterValue, filterOptions);
		}



		TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption, TObject> OnFromTableOrView<TObject>(SQLiteObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
			where TObject : class
		{
			return new SQLiteTableOrView<TObject>(this, tableOrViewName, filterValue, filterOptions);
		}

		TableDbCommandBuilder<SQLiteCommand, SQLiteParameter, SQLiteLimitOption, TObject> OnFromTableOrView<TObject>(SQLiteObjectName tableOrViewName, string? whereClause, object? argumentValue)
			where TObject : class
		{
			return new SQLiteTableOrView<TObject>(this, tableOrViewName, whereClause, argumentValue);
		}

		ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> OnInsertObject<TArgument>(SQLiteObjectName tableName, TArgument argumentValue, InsertOptions options)
			   where TArgument : class
		{
			return new SQLiteInsertObject<TArgument>(this, tableName, argumentValue, options);
		}

		ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> OnInsertOrUpdateObject<TArgument>(SQLiteObjectName tableName, TArgument argumentValue, UpsertOptions options) where TArgument : class
		{
			return new SQLiteInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
		}









	}
}
