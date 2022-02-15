using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor;
using Tortuga.Chain.Access.CommandBuilders;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.Access
{
	partial class AccessDataSourceBase
	{
		/// <summary>
		/// Delete multiple rows by key.
		/// </summary>
		/// <typeparam name="TKey">The type of the t key.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="keys">The keys.</param>
		/// <param name="options">Update options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;OleDbCommand, OleDbParameter&gt;.</returns>
		/// <exception cref="MappingException"></exception>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKeyList")]
		public MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> DeleteByKeyList<TKey>(AccessObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
		{
			var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;
			if (primaryKeys.Count != 1)
				throw new MappingException($"{nameof(DeleteByKeyList)} operation isn't allowed on {tableName} because it doesn't have a single primary key.");

			var keyList = keys.AsList();
			var columnMetadata = primaryKeys.Single();
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

			var table = DatabaseMetadata.GetTableOrView(tableName);
			if (!AuditRules.UseSoftDelete(table))
				return new AccessDeleteMany(this, tableName, where, parameters, parameters.Count, options);

			UpdateOptions effectiveOptions = UpdateOptions.SoftDelete;

			if (!options.HasFlag(DeleteOptions.CheckRowsAffected))
				effectiveOptions |= UpdateOptions.IgnoreRowsAffected;

			if (options.HasFlag(DeleteOptions.UseKeyAttribute))
				effectiveOptions |= UpdateOptions.UseKeyAttribute;

			return new AccessUpdateMany(this, tableName, null, where, parameters, parameters.Count, effectiveOptions);
		}

		/// <summary>
		/// Update multiple rows by key.
		/// </summary>
		/// <typeparam name="TArgument">The type of the t argument.</typeparam>
		/// <typeparam name="TKey">The type of the t key.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="newValues">The new values to use.</param>
		/// <param name="keys">The keys.</param>
		/// <param name="options">Update options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;OleDbCommand, OleDbParameter&gt;.</returns>
		/// <exception cref="MappingException"></exception>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKeyList")]
		public MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter> UpdateByKeyList<TArgument, TKey>(AccessObjectName tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
		{
			var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;
			if (primaryKeys.Count != 1)
				throw new MappingException($"{nameof(UpdateByKeyList)} operation isn't allowed on {tableName} because it doesn't have a single primary key.");

			var keyList = keys.AsList();
			var columnMetadata = primaryKeys.Single();
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

			return new AccessUpdateMany(this, tableName, newValues, where, parameters, parameters.Count, options);
		}

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

		ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument> OnDeleteObject<TArgument>(AccessObjectName tableName, TArgument argumentValue, DeleteOptions options) where TArgument : class
		{
			return new AccessDeleteObject<TArgument>(this, tableName, argumentValue, options);
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

		MultipleTableDbCommandBuilder<OleDbCommand, OleDbParameter> OnSql(string sqlStatement, object? argumentValue)
		{
			return new AccessSqlCall(this, sqlStatement, argumentValue);
		}

		IUpdateManyDbCommandBuilder<OleDbCommand, OleDbParameter> OnUpdateMany(AccessObjectName tableName, string updateExpression, object? updateArgumentValue, UpdateOptions options)
		{
			return new AccessUpdateMany(this, tableName, updateExpression, updateArgumentValue, options);
		}

		IUpdateManyDbCommandBuilder<OleDbCommand, OleDbParameter> OnUpdateMany(AccessObjectName tableName, object? newValues, UpdateOptions options)
		{
			return new AccessUpdateMany(this, tableName, newValues, options);
		}

		ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument> OnUpdateObject<TArgument>(AccessObjectName tableName, TArgument argumentValue, UpdateOptions options) where TArgument : class
		{
			return new AccessUpdateObject<TArgument>(this, tableName, argumentValue, options);
		}
	}
}
