using Npgsql;
using Tortuga.Anchor;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.PostgreSql.CommandBuilders;
using Tortuga.Shipwright;
using Traits;

namespace Tortuga.Chain.PostgreSql;

[UseTrait(typeof(SupportsDeleteAllTrait<AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsTruncateTrait<AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsSqlQueriesTrait<AbstractCommand, AbstractParameter>))]
[UseTrait(typeof(SupportsInsertBatchTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType,
MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter>>))]
[UseTrait(typeof(SupportsDeleteByKeyListTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsUpdateTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsDeleteTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsUpdateByKeyListTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsInsertTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsUpdateSet<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsDeleteSet<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsFromTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType, AbstractLimitOption>))]
[UseTrait(typeof(SupportsGetByKeyListTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsUpsertTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsInsertBulkTrait<PostgreSqlInsertBulk, AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
partial class PostgreSqlDataSourceBase : ICrudDataSource, IAdvancedCrudDataSource
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

		var parameters = new List<NpgsqlParameter>();
		for (var i = 0; i < keyList.Count; i++)
		{
			var param = new NpgsqlParameter("@Param" + i, keyList[i]);
			if (columnMetadata.DbType.HasValue)
				param.NpgsqlDbType = columnMetadata.DbType.Value;
			parameters.Add(param);
		}

		var table = DatabaseMetadata.GetTableOrView(tableName);
		if (!AuditRules.UseSoftDelete(table))
			return new PostgreSqlDeleteSet(this, tableName, where, parameters, parameters.Count, options);

		UpdateOptions effectiveOptions = UpdateOptions.SoftDelete;

		if (!options.HasFlag(DeleteOptions.CheckRowsAffected))
			effectiveOptions |= UpdateOptions.IgnoreRowsAffected;

		if (options.HasFlag(DeleteOptions.UseKeyAttribute))
			effectiveOptions |= UpdateOptions.UseKeyAttribute;

		return new PostgreSqlUpdateSet(this, tableName, null, where, parameters, parameters.Count, effectiveOptions);
	}

	ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> IUpdateDeleteHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnDeleteObject<TArgument>(AbstractObjectName tableName, TArgument argumentValue, DeleteOptions options)
		where TArgument : class
	{
		return new PostgreSqlDeleteObject<TArgument>(this, tableName, argumentValue, options);
	}

	MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteSetHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnDeleteSet(AbstractObjectName tableName, string whereClause, object? argumentValue)
	{
		return new PostgreSqlDeleteSet(this, tableName, whereClause, argumentValue);
	}

	MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteSetHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnDeleteSet(AbstractObjectName tableName, object filterValue, FilterOptions filterOptions)
	{
		return new PostgreSqlDeleteSet(this, tableName, filterValue, filterOptions);
	}

	TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption, TObject> IFromHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType, AbstractLimitOption>.OnFromTableOrView<TObject>(AbstractObjectName tableOrViewName, string? whereClause, object? argumentValue)
	where TObject : class
	{
		return new PostgreSqlTableOrView<TObject>(this, tableOrViewName, whereClause, argumentValue);
	}

	TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption, TObject> IFromHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType, AbstractLimitOption>.OnFromTableOrView<TObject>(AbstractObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
		where TObject : class
	{
		return new PostgreSqlTableOrView<TObject>(this, tableOrViewName, filterValue, filterOptions);
	}

	SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IGetByKeyHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnGetByKey<TObject, TKey>(AbstractObjectName tableName, ColumnMetadata<AbstractDbType> keyColumn, TKey key)
		where TObject : class
	{
		string where = keyColumn.SqlName + " = @Param0";

		var parameters = new List<NpgsqlParameter>();
		var param = new NpgsqlParameter("@Param0", key);
		if (keyColumn.DbType.HasValue)
			param.NpgsqlDbType = keyColumn.DbType.Value;
		parameters.Add(param);

		return new PostgreSqlTableOrView<TObject>(this, tableName, where, parameters);

	}

	MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IGetByKeyHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnGetByKeyList<TObject, TKey>(AbstractObjectName tableName, ColumnMetadata<AbstractDbType> keyColumn, IEnumerable<TKey> keys) where TObject : class
	{
		var keyList = keys.AsList();
		string where;
		if (keys.Count() > 1)
			where = keyColumn.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";
		else
			where = keyColumn.SqlName + " = @Param0";

		var parameters = new List<NpgsqlParameter>();
		for (var i = 0; i < keyList.Count; i++)
		{
			var param = new NpgsqlParameter("@Param" + i, keyList[i]);
			if (keyColumn.DbType.HasValue)
				param.NpgsqlDbType = keyColumn.DbType.Value;
			parameters.Add(param);
		}

		return new MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TObject>(new PostgreSqlTableOrView<TObject>(this, tableName, where, parameters));

	}

	DbCommandBuilder<AbstractCommand, AbstractParameter> IInsertBatchHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertBatch<TObject>(AbstractObjectName tableName, IEnumerable<TObject> objects, InsertOptions options)
	{
		return new PostgreSqlInsertBatch<TObject>(this, tableName, objects, options); ;
	}

	ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> IInsertHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertObject<TArgument>(AbstractObjectName tableName, TArgument argumentValue, InsertOptions options)
where TArgument : class
	{
		return new PostgreSqlInsertObject<TArgument>(this, tableName, argumentValue, options);
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

		var parameters = new List<NpgsqlParameter>();
		for (var i = 0; i < keyList.Count; i++)
		{
			var param = new NpgsqlParameter("@Param" + i, keyList[i]);
			if (columnMetadata.DbType.HasValue)
				param.NpgsqlDbType = columnMetadata.DbType.Value;
			parameters.Add(param);
		}

		return new PostgreSqlUpdateSet(this, tableName, newValues, where, parameters, parameters.Count, options);

	}

	ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> IUpdateDeleteHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnUpdateObject<TArgument>(AbstractObjectName tableName, TArgument argumentValue, UpdateOptions options)
where TArgument : class
	{
		return new PostgreSqlUpdateObject<TArgument>(this, tableName, argumentValue, options);
	}

	IUpdateSetDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteSetHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnUpdateSet(AbstractObjectName tableName, string updateExpression, object? updateArgumentValue, UpdateOptions options)
	{
		return new PostgreSqlUpdateSet(this, tableName, updateExpression, updateArgumentValue, options);
	}

	IUpdateSetDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteSetHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnUpdateSet(AbstractObjectName tableName, object? newValues, UpdateOptions options)
	{
		return new PostgreSqlUpdateSet(this, tableName, newValues, options);
	}

	private partial ILink<int?> OnDeleteAll(AbstractObjectName tableName)
	{
		//Verify the table name actually exists.
		var table = DatabaseMetadata.GetTableOrView(tableName);
		return Sql("DELETE FROM " + table.Name.ToQuotedString() + ";").AsNonQuery();
	}

	private partial MultipleTableDbCommandBuilder<AbstractCommand, AbstractParameter> OnSql(string sqlStatement, object? argumentValue)
	{
		return new PostgreSqlSqlCall(this, sqlStatement, argumentValue);
	}

	private partial ILink<int?> OnTruncate(AbstractObjectName tableName)
	{
		//Verify the table name actually exists.
		var table = DatabaseMetadata.GetTableOrView(tableName);
		return Sql("TRUNCATE TABLE " + table.Name.ToQuotedString() + ";").AsNonQuery();
	}

	ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> IUpsertHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertOrUpdateObject<TArgument>(AbstractObjectName tableName, TArgument argumentValue, UpsertOptions options)
	{
		return new PostgreSqlInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
	}

	PostgreSqlInsertBulk IInsertBulkHelper<PostgreSqlInsertBulk, AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertBulk(AbstractObjectName tableName, DataTable dataTable)
	{
		return new PostgreSqlInsertBulk(this, tableName, dataTable);
	}

	PostgreSqlInsertBulk IInsertBulkHelper<PostgreSqlInsertBulk, AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertBulk(AbstractObjectName tableName, IDataReader dataReader)
	{
		return new PostgreSqlInsertBulk(this, tableName, dataReader);
	}
}
