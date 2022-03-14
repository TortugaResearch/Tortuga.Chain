using MySqlConnector;
using Tortuga.Anchor;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.MySql.CommandBuilders;
using Tortuga.Shipwright;
using Traits;

namespace Tortuga.Chain.MySql
{
	[UseTrait(typeof(SupportsDeleteAllTrait<AbstractObjectName, AbstractDbType>))]
	[UseTrait(typeof(SupportsTruncateTrait<AbstractObjectName, AbstractDbType>))]
	[UseTrait(typeof(SupportsSqlQueriesTrait<AbstractCommand, AbstractParameter>))]
	[UseTrait(typeof(SupportsInsertBatchTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType,
	DbCommandBuilder<AbstractCommand, AbstractParameter>>))]
	[UseTrait(typeof(SupportsDeleteByKeyList<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]

	partial class MySqlDataSourceBase
	{
		DatabaseMetadataCache<AbstractObjectName, AbstractDbType> ICommandHelper<AbstractObjectName, AbstractDbType>.DatabaseMetadata => DatabaseMetadata;

		List<AbstractParameter> ICommandHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.GetParameters(SqlBuilder<AbstractDbType> builder) => builder.GetParameters();

		AbstractObjectName ICommandHelper<AbstractObjectName, AbstractDbType>.ParseObjectName(string objectName) => new(objectName);

		private partial ILink<int?> OnDeleteAll(AbstractObjectName tableName)
		{
			//Verify the table name actually exists.
			var table = DatabaseMetadata.GetTableOrView(tableName);
			return Sql("DELETE FROM " + table.Name.ToQuotedString() + ";").AsNonQuery();
		}

		private partial ILink<int?> OnTruncate(AbstractObjectName tableName)
		{
			//Verify the table name actually exists.
			var table = DatabaseMetadata.GetTableOrView(tableName);
			return Sql("TRUNCATE TABLE " + table.Name.ToQuotedString() + ";").AsNonQuery();
		}

		private partial MultipleTableDbCommandBuilder<AbstractCommand, AbstractParameter> OnSql(string sqlStatement, object? argumentValue)
		{
			return new MySqlSqlCall(this, sqlStatement, argumentValue);
		}

		DbCommandBuilder<AbstractCommand, AbstractParameter> IInsertBatchHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertBatch<TObject>(AbstractObjectName tableName, IEnumerable<TObject> objects, InsertOptions options)
		{
			return new MySqlInsertBatch<TObject>(this, tableName, objects, options); ;
		}

		MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter> IDeleteByKeyHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnDeleteByKeyList<TKey>(AbstractObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options)
		{
			var primaryKeys = DatabaseMetadata.GetTableOrView(tableName).PrimaryKeyColumns;
			if (primaryKeys.Count != 1)
				throw new MappingException($"DeleteByKey/DeleteByKeyList operation isn't allowed on {tableName} because it doesn't have a single primary key.");

			var keyList = keys.AsList();
			var columnMetadata = primaryKeys.Single();
			string where;
			if (keys.Count() > 1)
				where = columnMetadata.SqlName + " IN (" + string.Join(", ", keyList.Select((s, i) => "@Param" + i)) + ")";
			else
				where = columnMetadata.SqlName + " = @Param0";

			var parameters = new List<MySqlParameter>();
			for (var i = 0; i < keyList.Count; i++)
			{
				var param = new MySqlParameter("@Param" + i, keyList[i]);
				if (columnMetadata.DbType.HasValue)
					param.MySqlDbType = columnMetadata.DbType.Value;
				parameters.Add(param);
			}

			var table = DatabaseMetadata.GetTableOrView(tableName);
			if (!AuditRules.UseSoftDelete(table))
				return new MySqlDeleteMany(this, tableName, where, parameters, parameters.Count, options);

			UpdateOptions effectiveOptions = UpdateOptions.SoftDelete;

			if (!options.HasFlag(DeleteOptions.CheckRowsAffected))
				effectiveOptions |= UpdateOptions.IgnoreRowsAffected;

			if (options.HasFlag(DeleteOptions.UseKeyAttribute))
				effectiveOptions |= UpdateOptions.UseKeyAttribute;

			return new MySqlUpdateMany(this, tableName, null, where, parameters, parameters.Count, effectiveOptions);
		}





	}
}
