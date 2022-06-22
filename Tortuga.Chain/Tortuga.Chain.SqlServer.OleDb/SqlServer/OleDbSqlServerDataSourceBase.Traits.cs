using System.Data.OleDb;
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
[UseTrait(typeof(SupportsScalarFunctionTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsProcedureTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsTableFunctionTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType, AbstractLimitOption>))]
partial class OleDbSqlServerDataSourceBase : ICrudDataSource, IAdvancedCrudDataSource
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

		var table = DatabaseMetadata.GetTableOrView(tableName);
		if (!AuditRules.UseSoftDelete(table))
			return new OleDbSqlServerDeleteSet(this, tableName, where, parameters, parameters.Count, options);

		UpdateOptions effectiveOptions = UpdateOptions.SoftDelete | UpdateOptions.IgnoreRowsAffected;
		if (options.HasFlag(DeleteOptions.UseKeyAttribute))
			effectiveOptions = effectiveOptions | UpdateOptions.UseKeyAttribute;

		return new OleDbSqlServerUpdateSet(this, tableName, null, where, parameters, parameters.Count, effectiveOptions);
	}

	ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> IUpdateDeleteHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnDeleteObject<TArgument>(AbstractObjectName tableName, TArgument argumentValue, DeleteOptions options)
		where TArgument : class
	{
		return new OleDbSqlServerDeleteObject<TArgument>(this, tableName, argumentValue, options);
	}

	MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteSetHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnDeleteSet(AbstractObjectName tableName, string whereClause, object? argumentValue)
	{
		return new OleDbSqlServerDeleteSet(this, tableName, whereClause, argumentValue);
	}

	MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteSetHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnDeleteSet(AbstractObjectName tableName, object filterValue, FilterOptions filterOptions)
	{
		return new OleDbSqlServerDeleteSet(this, tableName, filterValue, filterOptions);
	}

	TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption, TObject> IFromHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType, AbstractLimitOption>.OnFromTableOrView<TObject>(AbstractObjectName tableOrViewName, string? whereClause, object? argumentValue)
	where TObject : class
	{
		return new OleDbSqlServerTableOrView<TObject>(this, tableOrViewName, whereClause, argumentValue);
	}

	TableDbCommandBuilder<AbstractCommand, AbstractParameter, AbstractLimitOption, TObject> IFromHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType, AbstractLimitOption>.OnFromTableOrView<TObject>(AbstractObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
		where TObject : class
	{
		return new OleDbSqlServerTableOrView<TObject>(this, tableOrViewName, filterValue, filterOptions);
	}

	MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IGetByKeyHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnGetByKey<TObject, TKey>(AbstractObjectName tableName, ColumnMetadata<AbstractDbType> keyColumn, TKey key)
		where TObject : class
	{
		string where = keyColumn.SqlName + " = ?";

		var parameters = new List<OleDbParameter>();
		var param = new OleDbParameter("@Param0", key);
		if (keyColumn.DbType.HasValue)
			param.OleDbType = keyColumn.DbType.Value;
		parameters.Add(param);

		return new OleDbSqlServerTableOrView<TObject>(this, tableName, where, parameters);
	}

	MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IGetByKeyHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnGetByKeyList<TObject, TKey>(AbstractObjectName tableName, ColumnMetadata<AbstractDbType> keyColumn, IEnumerable<TKey> keys) where TObject : class
	{
		var keyList = keys.AsList();
		string where;
		if (keys.Count() > 1)
			where = keyColumn.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "?")) + ")";
		else
			where = keyColumn.SqlName + " = ?";

		var parameters = new List<OleDbParameter>();
		for (var i = 0; i < keyList.Count; i++)
		{
			var param = new OleDbParameter("@Param" + i, keyList[i]);
			if (keyColumn.DbType.HasValue)
				param.OleDbType = keyColumn.DbType.Value;
			parameters.Add(param);
		}

		return new MultipleRowDbCommandBuilder<OleDbCommand, OleDbParameter, TObject>(new OleDbSqlServerTableOrView<TObject>(this, tableName, where, parameters));
	}

	ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> IInsertHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertObject<TArgument>(AbstractObjectName tableName, TArgument argumentValue, InsertOptions options)
where TArgument : class
	{
		return new OleDbSqlServerInsertObject<TArgument>(this, tableName, argumentValue, options);
	}

	ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> IUpsertHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertOrUpdateObject<TArgument>(AbstractObjectName tableName, TArgument argumentValue, UpsertOptions options)
	{
		return new OleDbSqlServerInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
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

		return new OleDbSqlServerUpdateSet(this, tableName, newValues, where, parameters, parameters.Count, options);
	}

	ObjectDbCommandBuilder<AbstractCommand, AbstractParameter, TArgument> IUpdateDeleteHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnUpdateObject<TArgument>(AbstractObjectName tableName, TArgument argumentValue, UpdateOptions options)
where TArgument : class
	{
		return new OleDbSqlServerUpdateObject<TArgument>(this, tableName, argumentValue, options);
	}

	IUpdateSetDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteSetHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnUpdateSet(AbstractObjectName tableName, string updateExpression, object? updateArgumentValue, UpdateOptions options)
	{
		return new OleDbSqlServerUpdateSet(this, tableName, updateExpression, updateArgumentValue, options);
	}

	IUpdateSetDbCommandBuilder<AbstractCommand, AbstractParameter> IUpdateDeleteSetHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnUpdateSet(AbstractObjectName tableName, object? newValues, UpdateOptions options)
	{
		return new OleDbSqlServerUpdateSet(this, tableName, newValues, options);
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
		return new OleDbSqlServerSqlCall(this, sqlStatement, argumentValue);
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