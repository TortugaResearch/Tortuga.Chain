using Tortuga.Anchor;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.CommandBuilders;
using Tortuga.Shipwright;
using Traits;

namespace Tortuga.Chain.SqlServer;

[UseTrait(typeof(SupportsDeleteAllTrait<AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsTruncateTrait<AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsSqlQueriesTrait<AbstractCommand, AbstractParameter>))]
[UseTrait(typeof(SupportsInsertBatchTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType,
	MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter>>))]
[UseTrait(typeof(SupportsDeleteByKeyListTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsDeleteTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsUpdateTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsUpdateByKeyListTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsInsertTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsUpdateSet<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsDeleteSet<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsFromTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType, AbstractLimitOption>))]
[UseTrait(typeof(SupportsGetByKeyListTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsUpsertTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsInsertBulkTrait<SqlServerInsertBulk, AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsScalarFunctionTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsProcedureTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsTableFunctionTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType, AbstractLimitOption>))]
partial class SqlServerDataSourceBase : ICrudDataSource, IAdvancedCrudDataSource
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

		var parameters = new List<SqlParameter>();
		for (var i = 0; i < keyList.Count; i++)
		{
			var param = new SqlParameter("@Param" + i, keyList[i]);
			if (columnMetadata.DbType.HasValue)
				param.SqlDbType = columnMetadata.DbType.Value;
			parameters.Add(param);
		}

		var table = DatabaseMetadata.GetTableOrView(tableName);
		if (!AuditRules.UseSoftDelete(table))
			return new SqlServerDeleteSet(this, tableName, where, parameters, parameters.Count, options);

		UpdateOptions effectiveOptions = UpdateOptions.SoftDelete;

		if (!options.HasFlag(DeleteOptions.CheckRowsAffected))
			effectiveOptions |= UpdateOptions.IgnoreRowsAffected;

		if (options.HasFlag(DeleteOptions.UseKeyAttribute))
			effectiveOptions |= UpdateOptions.UseKeyAttribute;

		return new SqlServerUpdateSet(this, tableName, null, where, parameters, parameters.Count, effectiveOptions);
	}

	ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> IUpdateDeleteHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnDeleteObject<TArgument>(AbstractObjectName tableName, TArgument argumentValue, DeleteOptions options)
		where TArgument : class
	{
		return new SqlServerDeleteObject<TArgument>(this, tableName, argumentValue, options);
	}

	MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteSetHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnDeleteSet(AbstractObjectName tableName, string whereClause, object? argumentValue)
	{
		return new SqlServerDeleteSet(this, tableName, whereClause, argumentValue);
	}

	MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteSetHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnDeleteSet(AbstractObjectName tableName, object filterValue, FilterOptions filterOptions)
	{
		return new SqlServerDeleteSet(this, tableName, filterValue, filterOptions);
	}

	TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption, TObject> IFromHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType, AbstractLimitOption>.OnFromTableOrView<TObject>(AbstractObjectName tableOrViewName, string? whereClause, object? argumentValue)
	where TObject : class
	{
		return new SqlServerTableOrView<TObject>(this, tableOrViewName, whereClause, argumentValue);
	}

	TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption, TObject> IFromHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType, AbstractLimitOption>.OnFromTableOrView<TObject>(AbstractObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
		where TObject : class
	{
		return new SqlServerTableOrView<TObject>(this, tableOrViewName, filterValue, filterOptions);
	}

	SingleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IGetByKeyHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnGetByKey<TObject, TKey>(AbstractObjectName tableName, ColumnMetadata<AbstractDbType> keyColumn, TKey key)
		where TObject : class
	{
		var where = keyColumn.SqlName + " = @Param0";

		var parameters = new List<SqlParameter>();
		var param = new SqlParameter("@Param0", key);
		if (keyColumn.DbType.HasValue)
			param.SqlDbType = keyColumn.DbType.Value;
		parameters.Add(param);

		return new SqlServerTableOrView<TObject>(this, tableName, where, parameters);

	}

	MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IGetByKeyHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnGetByKeyList<TObject, TKey>(AbstractObjectName tableName, ColumnMetadata<AbstractDbType> keyColumn, IEnumerable<TKey> keys) where TObject : class
	{
		var keyList = keys.AsList();
		string where;
		if (keys.Count() > 1)
			where = keyColumn.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";
		else
			where = keyColumn.SqlName + " = @Param0";

		var parameters = new List<SqlParameter>();
		for (var i = 0; i < keyList.Count; i++)
		{
			var param = new SqlParameter("@Param" + i, keyList[i]);
			if (keyColumn.DbType.HasValue)
				param.SqlDbType = keyColumn.DbType.Value;
			parameters.Add(param);
		}

		return new MultipleRowDbCommandBuilder<SqlCommand, SqlParameter, TObject>(new SqlServerTableOrView<TObject>(this, tableName, where, parameters));

	}

	DbCommandBuilder<AbstractCommand, AbstractParameter> IInsertBatchHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertBatch<TObject>(AbstractObjectName tableName, IEnumerable<TObject> objects, InsertOptions options)
	{
		return new SqlServerInsertBatch<TObject>(this, tableName, objects, options); ;
	}

	SqlServerInsertBulk IInsertBulkHelper<SqlServerInsertBulk, AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertBulk(AbstractObjectName tableName, DataTable dataTable)
	{
		return new SqlServerInsertBulk(this, tableName, dataTable);
	}

	SqlServerInsertBulk IInsertBulkHelper<SqlServerInsertBulk, AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertBulk(AbstractObjectName tableName, IDataReader dataReader)
	{
		return new SqlServerInsertBulk(this, tableName, dataReader);
	}

	ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> IInsertHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertObject<TArgument>(AbstractObjectName tableName, TArgument argumentValue, InsertOptions options)
		where TArgument : class
	{
		return new SqlServerInsertObject<TArgument>(this, tableName, argumentValue, options);
	}

	ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> IUpsertHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertOrUpdateObject<TArgument>(AbstractObjectName tableName, TArgument argumentValue, UpsertOptions options)
	{
		return new SqlServerInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
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

		var parameters = new List<SqlParameter>();
		for (var i = 0; i < keyList.Count; i++)
		{
			var param = new SqlParameter("@Param" + i, keyList[i]);
			if (columnMetadata.DbType.HasValue)
				param.SqlDbType = columnMetadata.DbType.Value;
			parameters.Add(param);
		}

		return new SqlServerUpdateSet(this, tableName, newValues, where, parameters, parameters.Count, options);
	}

	ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> IUpdateDeleteHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnUpdateObject<TArgument>(AbstractObjectName tableName, TArgument argumentValue, UpdateOptions options)
where TArgument : class
	{
		return new SqlServerUpdateObject<TArgument>(this, tableName, argumentValue, options);
	}

	IUpdateSetDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteSetHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnUpdateSet(AbstractObjectName tableName, string updateExpression, object? updateArgumentValue, UpdateOptions options)
	{
		return new SqlServerUpdateSet(this, tableName, updateExpression, updateArgumentValue, options);
	}

	IUpdateSetDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteSetHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnUpdateSet(AbstractObjectName tableName, object? newValues, UpdateOptions options)
	{
		return new SqlServerUpdateSet(this, tableName, newValues, options);
	}

	private partial ILink<int?> OnDeleteAll(AbstractObjectName tableName)
	{
		//Verify the table name actually exists.
		var table = DatabaseMetadata.GetTableOrView(tableName);
		return Sql("DELETE FROM " + table.Name.ToQuotedString() + ";").AsNonQuery();
	}

	private partial ProcedureDbCommandBuilder<AbstractCommand, AbstractParameter> OnProcedure(AbstractObjectName procedureName, object? argumentValue)
	{
		return new AbstractProcedureCall(this, procedureName, argumentValue);
	}

	private partial ScalarDbCommandBuilder<AbstractCommand, AbstractParameter> OnScalarFunction(AbstractObjectName scalarFunctionName, object? argumentValue)
	{
		return new AbstractScalarFunction(this, scalarFunctionName, argumentValue);
	}

	private partial MultipleTableDbCommandBuilder<AbstractCommand, AbstractParameter> OnSql(string sqlStatement, object? argumentValue)
	{
		return new SqlServerSqlCall(this, sqlStatement, argumentValue);
	}

	private partial TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption> OnTableFunction(AbstractObjectName tableFunctionName, object? functionArgumentValue)
	{
		return new AbstractTableFunction(this, tableFunctionName, functionArgumentValue);
	}

	private partial ILink<int?> OnTruncate(AbstractObjectName tableName)
	{
		//Verify the table name actually exists.
		var table = DatabaseMetadata.GetTableOrView(tableName);
		return Sql("TRUNCATE TABLE " + table.Name.ToQuotedString() + ";").AsNonQuery();
	}
}
