using System.Data.SQLite;
using Tortuga.Anchor;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SQLite.CommandBuilders;
using Tortuga.Shipwright;
using Traits;

namespace Tortuga.Chain.SQLite;

[UseTrait(typeof(SupportsDeleteAllTrait<AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsTruncateTrait<AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsSqlQueriesTrait<AbstractCommand, AbstractParameter>))]
[UseTrait(typeof(SupportsInsertBatchTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType,
DbCommandBuilder<AbstractCommand, AbstractParameter>>))]
[UseTrait(typeof(SupportsDeleteByKeyListTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsUpdateTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsDeleteTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsUpdateByKeyListTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsInsertTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsUpdateSet<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsDeleteSet<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsFromTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType, AbstractLimitOption>))]
partial class SQLiteDataSourceBase
{
	DatabaseMetadataCache<AbstractObjectName, AbstractDbType> ICommandHelper<AbstractObjectName, AbstractDbType>.DatabaseMetadata => DatabaseMetadata;

	List<AbstractParameter> ICommandHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.GetParameters(SqlBuilder<AbstractDbType> builder) => builder.GetParameters();

	MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteByKeyHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnDeleteByKeyList<TKey>(AbstractObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options)
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

		var parameters = new List<SQLiteParameter>();
		for (var i = 0; i < keyList.Count; i++)
		{
			var param = new SQLiteParameter("@Param" + i, keyList[i]);
			if (columnMetadata.DbType.HasValue)
				param.DbType = columnMetadata.DbType.Value;
			parameters.Add(param);
		}

		var table = DatabaseMetadata.GetTableOrView(tableName);
		if (!AuditRules.UseSoftDelete(table))
			return new SQLiteDeleteSet(this, tableName, where, parameters, parameters.Count, options);

		UpdateOptions effectiveOptions = UpdateOptions.SoftDelete;

		if (!options.HasFlag(DeleteOptions.CheckRowsAffected))
			effectiveOptions |= UpdateOptions.IgnoreRowsAffected;

		if (options.HasFlag(DeleteOptions.UseKeyAttribute))
			effectiveOptions |= UpdateOptions.UseKeyAttribute;

		return new SQLiteUpdateSet(this, tableName, null, where, parameters, parameters.Count, effectiveOptions);
	}

	DbCommandBuilder<AbstractCommand, AbstractParameter> IInsertBatchHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertBatch<TObject>(AbstractObjectName tableName, IEnumerable<TObject> objects, InsertOptions options)
	{
		return new SQLiteInsertBatch<TObject>(this, tableName, objects, options); ;
	}

	private partial ILink<int?> OnDeleteAll(AbstractObjectName tableName)
	{
		//SQLite determines for itself if a delete all should be interpreted as a truncate.
		return OnTruncate(tableName);
	}

	private partial MultipleTableDbCommandBuilder<AbstractCommand, AbstractParameter> OnSql(string sqlStatement, object? argumentValue)
	{
		return new SQLiteSqlCall(this, sqlStatement, argumentValue, LockType.Write);
	}

	private partial ILink<int?> OnTruncate(AbstractObjectName tableName)
	{
		//Verify the table name actually exists.
		var table = DatabaseMetadata.GetTableOrView(tableName);
		//In SQLite, a delete without a where clause is interpreted as a truncate if other conditions are met.
		return Sql("DELETE FROM " + table.Name.ToQuotedString() + ";").AsNonQuery();
	}

	ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> IUpdateDeleteHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnUpdateObject<TArgument>(AbstractObjectName tableName, TArgument argumentValue, UpdateOptions options)
where TArgument : class
	{
		return new SQLiteUpdateObject<TArgument>(this, tableName, argumentValue, options);
	}

	ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> IUpdateDeleteHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnDeleteObject<TArgument>(AbstractObjectName tableName, TArgument argumentValue, DeleteOptions options)
		where TArgument : class
	{
		return new SQLiteDeleteObject<TArgument>(this, tableName, argumentValue, options);
	}

	MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteByKeyHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnUpdateByKeyList<TArgument, TKey>(AbstractObjectName tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options)
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

		var parameters = new List<SQLiteParameter>();
		for (var i = 0; i < keyList.Count; i++)
		{
			var param = new SQLiteParameter("@Param" + i, keyList[i]);
			if (columnMetadata.DbType.HasValue)
				param.DbType = columnMetadata.DbType.Value;
			parameters.Add(param);
		}

		return new SQLiteUpdateSet(this, tableName, newValues, where, parameters, parameters.Count, options);
	}

	ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> IInsertHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertObject<TArgument>(AbstractObjectName tableName, TArgument argumentValue, InsertOptions options)
where TArgument : class
	{
		return new SQLiteInsertObject<TArgument>(this, tableName, argumentValue, options);
	}

	IUpdateSetDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteSetHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnUpdateSet(AbstractObjectName tableName, string updateExpression, object? updateArgumentValue, UpdateOptions options)
	{
		return new SQLiteUpdateSet(this, tableName, updateExpression, updateArgumentValue, options);
	}

	IUpdateSetDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteSetHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnUpdateSet(AbstractObjectName tableName, object? newValues, UpdateOptions options)
	{
		return new SQLiteUpdateSet(this, tableName, newValues, options);
	}

	MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteSetHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnDeleteSet(AbstractObjectName tableName, string whereClause, object? argumentValue)
	{
		return new SQLiteDeleteSet(this, tableName, whereClause, argumentValue);
	}

	MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteSetHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnDeleteSet(AbstractObjectName tableName, object filterValue, FilterOptions filterOptions)
	{
		return new SQLiteDeleteSet(this, tableName, filterValue, filterOptions);
	}

	TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption, TObject> IFromHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType, AbstractLimitOption>.OnFromTableOrView<TObject>(AbstractObjectName tableOrViewName, string? whereClause, object? argumentValue)
	where TObject : class
	{
		return new SQLiteTableOrView<TObject>(this, tableOrViewName, whereClause, argumentValue);
	}

	TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption, TObject> IFromHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType, AbstractLimitOption>.OnFromTableOrView<TObject>(AbstractObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
		where TObject : class
	{
		return new SQLiteTableOrView<TObject>(this, tableOrViewName, filterValue, filterOptions);
	}
}


