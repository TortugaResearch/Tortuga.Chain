using System.Data.OleDb;
using Tortuga.Anchor;
using Tortuga.Chain.Access.CommandBuilders;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.Access
{
	partial class AccessDataSourceBase
	{




		AccessTableOrView<TObject> OnGetByKey<TObject, TKey>(AccessObjectName tableName, ColumnMetadata<OleDbType> columnMetadata, TKey key)
			where TObject : class
		{
			string where = columnMetadata.SqlName + " = @Param0";

			var parameters = new List<OleDbParameter>();
			var param = new OleDbParameter("@Param0", key);
			if (columnMetadata.DbType.HasValue)
				param.OleDbType = columnMetadata.DbType.Value;
			parameters.Add(param);

			return new AccessTableOrView<TObject>(this, tableName, where, parameters);
		}

		MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter, TObject> OnGetByKeyList<TObject, TKey>(AccessObjectName tableName, ColumnMetadata<OleDbType> columnMetadata, IEnumerable<TKey> keys)
			where TObject : class
		{
			var keyList = keys.AsList();
			string where;
			if (keys.Count() > 1)
				where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";
			else
				where = columnMetadata.SqlName + " = @Param0";

			var parameters = new List<OleDbParameter>();
			for (var i = 0; i < keyList.Count; i++)
			{
				var param = new OleDbParameter("@Param" + i, keyList[i]);
				if (columnMetadata.DbType.HasValue)
					param.OleDbType = columnMetadata.DbType.Value;
				parameters.Add(param);
			}

			return new MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter, TObject>(new AccessTableOrView<TObject>(this, tableName, where, parameters));
		}

		MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> OnDeleteMany(AccessObjectName tableName, string whereClause, object? argumentValue)
		{
			return new AccessDeleteMany(this, tableName, whereClause, argumentValue);
		}

		MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> OnDeleteMany(AccessObjectName tableName, object filterValue, FilterOptions filterOptions)
		{
			return new AccessDeleteMany(this, tableName, filterValue, filterOptions);
		}



		TableDbCommandBuilder<OleDbCommand, OleDbParameter, AccessLimitOption, TObject> OnFromTableOrView<TObject>(AccessObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
			where TObject : class
		{
			return new AccessTableOrView<TObject>(this, tableOrViewName, filterValue, filterOptions);
		}

		TableDbCommandBuilder<OleDbCommand, OleDbParameter, AccessLimitOption, TObject> OnFromTableOrView<TObject>(AccessObjectName tableOrViewName, string? whereClause, object? argumentValue)
			where TObject : class
		{
			return new AccessTableOrView<TObject>(this, tableOrViewName, whereClause, argumentValue);
		}

		ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument> OnInsertObject<TArgument>(AccessObjectName tableName, TArgument argumentValue, InsertOptions options)
			   where TArgument : class
		{
			return new AccessInsertObject<TArgument>(this, tableName, argumentValue, options);
		}


		IUpdateManyDbCommandBuilder<OleDbCommand, OleDbParameter> OnUpdateMany(AccessObjectName tableName, string updateExpression, object? updateArgumentValue, UpdateOptions options)
		{
			return new AccessUpdateMany(this, tableName, updateExpression, updateArgumentValue, options);
		}

		IUpdateManyDbCommandBuilder<OleDbCommand, OleDbParameter> OnUpdateMany(AccessObjectName tableName, object? newValues, UpdateOptions options)
		{
			return new AccessUpdateMany(this, tableName, newValues, options);
		}



	}
}
