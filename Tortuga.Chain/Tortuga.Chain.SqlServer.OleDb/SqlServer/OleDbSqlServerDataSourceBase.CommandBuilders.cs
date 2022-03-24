using System.Data.OleDb;
using Tortuga.Anchor;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.CommandBuilders;

namespace Tortuga.Chain.SqlServer
{
	partial class OleDbSqlServerDataSourceBase
	{

		OleDbSqlServerTableOrView<TObject> OnGetByKey<TObject, TKey>(SqlServerObjectName tableName, ColumnMetadata<OleDbType> columnMetadata, TKey key)
			where TObject : class
		{
			string where = columnMetadata.SqlName + " = ?";

			var parameters = new List<OleDbParameter>();
			var param = new OleDbParameter("@Param0", key);
			if (columnMetadata.DbType.HasValue)
				param.OleDbType = columnMetadata.DbType.Value;
			parameters.Add(param);

			return new OleDbSqlServerTableOrView<TObject>(this, tableName, where, parameters);
		}

		MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter, TObject> OnGetByKeyList<TObject, TKey>(SqlServerObjectName tableName, ColumnMetadata<OleDbType> columnMetadata, IEnumerable<TKey> keys)
			where TObject : class
		{
			var keyList = keys.AsList();
			string where;
			if (keys.Count() > 1)
				where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "?")) + ")";
			else
				where = columnMetadata.SqlName + " = ?";

			var parameters = new List<OleDbParameter>();
			for (var i = 0; i < keyList.Count; i++)
			{
				var param = new OleDbParameter("@Param" + i, keyList[i]);
				if (columnMetadata.DbType.HasValue)
					param.OleDbType = columnMetadata.DbType.Value;
				parameters.Add(param);
			}

			return new MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter, TObject>(new OleDbSqlServerTableOrView<TObject>(this, tableName, where, parameters));
		}

		MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> OnDeleteSet(SqlServerObjectName tableName, string? whereClause, object? argumentValue)
		{
			return new OleDbSqlServerDeleteSet(this, tableName, whereClause, argumentValue);
		}

		MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> OnDeleteSet(SqlServerObjectName tableName, object filterValue, FilterOptions filterOptions)
		{
			return new OleDbSqlServerDeleteSet(this, tableName, filterValue, filterOptions);
		}



		TableDbCommandBuilder<OleDbCommand, OleDbParameter, SqlServerLimitOption, TObject> OnFromTableOrView<TObject>(SqlServerObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
			where TObject : class
		{
			return new OleDbSqlServerTableOrView<TObject>(this, tableOrViewName, filterValue, filterOptions);
		}

		TableDbCommandBuilder<OleDbCommand, OleDbParameter, SqlServerLimitOption, TObject> OnFromTableOrView<TObject>(SqlServerObjectName tableOrViewName, string? whereClause, object? argumentValue)
			where TObject : class
		{
			return new OleDbSqlServerTableOrView<TObject>(this, tableOrViewName, whereClause, argumentValue);
		}

		ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument> OnInsertObject<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, InsertOptions options)
			   where TArgument : class
		{
			return new OleDbSqlServerInsertObject<TArgument>(this, tableName, argumentValue, options);
		}

		ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument> OnInsertOrUpdateObject<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, UpsertOptions options) where TArgument : class
		{
			return new OleDbSqlServerInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
		}

	}
}
