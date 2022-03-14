using Npgsql;
using NpgsqlTypes;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.PostgreSql.CommandBuilders;

namespace Tortuga.Chain.PostgreSql
{
	partial class PostgreSqlDataSourceBase
	{


		/// <summary>
		/// Update multiple rows by key.
		/// </summary>
		/// <typeparam name="TArgument">The type of the t argument.</typeparam>
		/// <typeparam name="TKey">The type of the t key.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="newValues">The new values to use.</param>
		/// <param name="keys">The keys.</param>
		/// <param name="options">Update options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;NpgsqlCommand, NpgsqlParameter&gt;.</returns>
		/// <exception cref="MappingException"></exception>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKeyList")]
		public MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> UpdateByKeyList<TArgument, TKey>(PostgreSqlObjectName tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
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

			return new PostgreSqlUpdateMany(this, tableName, newValues, where, parameters, parameters.Count, options);
		}

		PostgreSqlTableOrView<TObject> OnGetByKey<TObject, TKey>(PostgreSqlObjectName tableName, ColumnMetadata<NpgsqlDbType> columnMetadata, TKey key)
			where TObject : class
		{
			string where = columnMetadata.SqlName + " = @Param0";

			var parameters = new List<NpgsqlParameter>();
			var param = new NpgsqlParameter("@Param0", key);
			if (columnMetadata.DbType.HasValue)
				param.NpgsqlDbType = columnMetadata.DbType.Value;
			parameters.Add(param);

			return new PostgreSqlTableOrView<TObject>(this, tableName, where, parameters);
		}

		MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TObject> OnGetByKeyList<TObject, TKey>(PostgreSqlObjectName tableName, ColumnMetadata<NpgsqlDbType> columnMetadata, IEnumerable<TKey> keys)
			where TObject : class
		{
			var keyList = keys.AsList();
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

			return new MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TObject>(new PostgreSqlTableOrView<TObject>(this, tableName, where, parameters));
		}

		MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> OnDeleteMany(PostgreSqlObjectName tableName, string whereClause, object? argumentValue)
		{
			return new PostgreSqlDeleteMany(this, tableName, whereClause, argumentValue);
		}

		MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> OnDeleteMany(PostgreSqlObjectName tableName, object filterValue, FilterOptions filterOptions)
		{
			return new PostgreSqlDeleteMany(this, tableName, filterValue, filterOptions);
		}

		ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> OnDeleteObject<TArgument>(PostgreSqlObjectName tableName, TArgument argumentValue, DeleteOptions options) where TArgument : class
		{
			return new PostgreSqlDeleteObject<TArgument>(this, tableName, argumentValue, options);
		}

		TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption, TObject> OnFromTableOrView<TObject>(PostgreSqlObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
			where TObject : class
		{
			return new PostgreSqlTableOrView<TObject>(this, tableOrViewName, filterValue, filterOptions);
		}

		TableDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, PostgreSqlLimitOption, TObject> OnFromTableOrView<TObject>(PostgreSqlObjectName tableOrViewName, string? whereClause, object? argumentValue)
			where TObject : class
		{
			return new PostgreSqlTableOrView<TObject>(this, tableOrViewName, whereClause, argumentValue);
		}

		ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> OnInsertObject<TArgument>(PostgreSqlObjectName tableName, TArgument argumentValue, InsertOptions options)
			   where TArgument : class
		{
			return new PostgreSqlInsertObject<TArgument>(this, tableName, argumentValue, options);
		}

		ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> OnInsertOrUpdateObject<TArgument>(PostgreSqlObjectName tableName, TArgument argumentValue, UpsertOptions options) where TArgument : class
		{
			return new PostgreSqlInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
		}

		IUpdateManyDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> OnUpdateMany(PostgreSqlObjectName tableName, string updateExpression, object? updateArgumentValue, UpdateOptions options)
		{
			return new PostgreSqlUpdateMany(this, tableName, updateExpression, updateArgumentValue, options);
		}

		IUpdateManyDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> OnUpdateMany(PostgreSqlObjectName tableName, object? newValues, UpdateOptions options)
		{
			return new PostgreSqlUpdateMany(this, tableName, newValues, options);
		}

		ObjectDbCommandBuilder<NpgsqlCommand, NpgsqlParameter, TArgument> OnUpdateObject<TArgument>(PostgreSqlObjectName tableName, TArgument argumentValue, UpdateOptions options) where TArgument : class
		{
			return new PostgreSqlUpdateObject<TArgument>(this, tableName, argumentValue, options);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="dataTable">The data table.</param>
		/// <returns>PostgreSqlInsertBulk.</returns>
		public PostgreSqlInsertBulk InsertBulk(PostgreSqlObjectName tableName, DataTable dataTable)
		{
			return new PostgreSqlInsertBulk(this, tableName, dataTable);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="dataReader">The data reader.</param>
		/// <returns>PostgreSqlInsertBulk.</returns>
		public PostgreSqlInsertBulk InsertBulk(PostgreSqlObjectName tableName, IDataReader dataReader)
		{
			return new PostgreSqlInsertBulk(this, tableName, dataReader);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="objects">The objects.</param>
		/// <returns>PostgreSqlInsertBulk.</returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public PostgreSqlInsertBulk InsertBulk<TObject>(PostgreSqlObjectName tableName, IEnumerable<TObject> objects) where TObject : class
		{
			var table = DatabaseMetadata.GetTableOrView(tableName);
			return new PostgreSqlInsertBulk(this, tableName, new ObjectDataReader<TObject>(table, objects, OperationTypes.Insert));
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="dataTable">The data table.</param>
		/// <returns>
		/// PostgreSqlInsertBulk.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public PostgreSqlInsertBulk InsertBulk<TObject>(DataTable dataTable) where TObject : class
		{
			return InsertBulk(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, dataTable);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="dataReader">The data reader.</param>
		/// <returns>
		/// PostgreSqlInsertBulk.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public PostgreSqlInsertBulk InsertBulk<TObject>(IDataReader dataReader) where TObject : class
		{
			return InsertBulk(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, dataReader);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="objects">The objects.</param>
		/// <returns>
		/// PostgreSqlInsertBulk.
		/// </returns>
		public PostgreSqlInsertBulk InsertBulk<TObject>(IEnumerable<TObject> objects) where TObject : class
		{
			return InsertBulk(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, objects);
		}

		/// <summary>
		/// Gets a table's row count.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		public ILink<long> GetTableApproximateCount(PostgreSqlObjectName tableName)
		{
			var table = DatabaseMetadata.GetTableOrView(tableName); //get the real name
			var sql = @"SELECT tab.reltuples::BIGINT AS estimate FROM pg_class tab
INNER JOIN pg_namespace ns on ns.oid=tab.relnamespace
WHERE ns.nspname = @Schema AND tab.relname = @Name;";

			//If there are zero rows, this can return -1 instead.
			return Sql(sql, new { table.Name.Schema, table.Name.Name }).ToInt64().Transform(x => x == -1 ? 0 : x);
		}

		/// <summary>
		/// Gets a table's row count.
		/// </summary>
		public ILink<long> GetTableApproximateCount<TObject>() => GetTableApproximateCount(DatabaseObjectAsTableOrView<TObject>(OperationType.Select).Name);

	}
}
