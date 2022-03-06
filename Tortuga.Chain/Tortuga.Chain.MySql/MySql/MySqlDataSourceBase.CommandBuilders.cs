using MySqlConnector;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.MySql.CommandBuilders;

namespace Tortuga.Chain.MySql
{
	partial class MySqlDataSourceBase
	{
		/// <summary>
		/// Delete multiple rows by key.
		/// </summary>
		/// <typeparam name="TKey">The type of the t key.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="keys">The keys.</param>
		/// <param name="options">Update options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;MySqlCommand, MySqlParameter&gt;.</returns>
		/// <exception cref="MappingException"></exception>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKey")]
		public MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> DeleteByKeyList<TKey>(MySqlObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
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

		/// <summary>
		/// Update multiple rows by key.
		/// </summary>
		/// <typeparam name="TArgument">The type of the t argument.</typeparam>
		/// <typeparam name="TKey">The type of the t key.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="newValues">The new values to use.</param>
		/// <param name="keys">The keys.</param>
		/// <param name="options">Update options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;MySqlCommand, MySqlParameter&gt;.</returns>
		/// <exception cref="MappingException"></exception>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKeyList")]
		public MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> UpdateByKeyList<TArgument, TKey>(MySqlObjectName tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
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

			var parameters = new List<MySqlParameter>();
			for (var i = 0; i < keyList.Count; i++)
			{
				var param = new MySqlParameter("@Param" + i, keyList[i]);
				if (columnMetadata.DbType.HasValue)
					param.MySqlDbType = columnMetadata.DbType.Value;
				parameters.Add(param);
			}

			return new MySqlUpdateMany(this, tableName, newValues, where, parameters, parameters.Count, options);
		}

		MySqlTableOrView<TObject> OnGetByKey<TObject, TKey>(MySqlObjectName tableName, ColumnMetadata<MySqlDbType> columnMetadata, TKey key)
			where TObject : class
		{
			string where = columnMetadata.SqlName + " = @Param0";

			var parameters = new List<MySqlParameter>();
			var param = new MySqlParameter("@Param0", key);
			if (columnMetadata.DbType.HasValue)
				param.MySqlDbType = columnMetadata.DbType.Value;
			parameters.Add(param);

			return new MySqlTableOrView<TObject>(this, tableName, where, parameters);
		}

		MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter, TObject> OnGetByKeyList<TObject, TKey>(MySqlObjectName tableName, ColumnMetadata<MySqlDbType> columnMetadata, IEnumerable<TKey> keys)
			where TObject : class
		{
			var keyList = keys.AsList();

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

			return new MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter, TObject>(new MySqlTableOrView<TObject>(this, tableName, where, parameters));
		}

		MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> OnDeleteMany(MySqlObjectName tableName, string whereClause, object? argumentValue)
		{
			return new MySqlDeleteMany(this, tableName, whereClause, argumentValue);
		}

		MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> OnDeleteMany(MySqlObjectName tableName, object filterValue, FilterOptions filterOptions)
		{
			return new MySqlDeleteMany(this, tableName, filterValue, filterOptions);
		}

		ObjectDbCommandBuilder<MySqlCommand, MySqlParameter, TArgument> OnDeleteObject<TArgument>(MySqlObjectName tableName, TArgument argumentValue, DeleteOptions options) where TArgument : class
		{
			return new MySqlDeleteObject<TArgument>(this, tableName, argumentValue, options);
		}

		TableDbCommandBuilder<MySqlCommand, MySqlParameter, MySqlLimitOption, TObject> OnFromTableOrView<TObject>(MySqlObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
			where TObject : class
		{
			return new MySqlTableOrView<TObject>(this, tableOrViewName, filterValue, filterOptions);
		}

		TableDbCommandBuilder<MySqlCommand, MySqlParameter, MySqlLimitOption, TObject> OnFromTableOrView<TObject>(MySqlObjectName tableOrViewName, string? whereClause, object? argumentValue)
			where TObject : class
		{
			return new MySqlTableOrView<TObject>(this, tableOrViewName, whereClause, argumentValue);
		}

		ObjectDbCommandBuilder<MySqlCommand, MySqlParameter, TArgument> OnInsertObject<TArgument>(MySqlObjectName tableName, TArgument argumentValue, InsertOptions options)
			   where TArgument : class
		{
			return new MySqlInsertObject<TArgument>(this, tableName, argumentValue, options);
		}

		ObjectDbCommandBuilder<MySqlCommand, MySqlParameter, TArgument> OnInsertOrUpdateObject<TArgument>(MySqlObjectName tableName, TArgument argumentValue, UpsertOptions options) where TArgument : class
		{
			return new MySqlInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
		}

		MultipleTableDbCommandBuilder<MySqlCommand, MySqlParameter> OnSql(string sqlStatement, object? argumentValue)
		{
			return new MySqlSqlCall(this, sqlStatement, argumentValue);
		}

		IUpdateManyDbCommandBuilder<MySqlCommand, MySqlParameter> OnUpdateMany(MySqlObjectName tableName, string updateExpression, object? updateArgumentValue, UpdateOptions options)
		{
			return new MySqlUpdateMany(this, tableName, updateExpression, updateArgumentValue, options);
		}

		IUpdateManyDbCommandBuilder<MySqlCommand, MySqlParameter> OnUpdateMany(MySqlObjectName tableName, object? newValues, UpdateOptions options)
		{
			return new MySqlUpdateMany(this, tableName, newValues, options);
		}

		ObjectDbCommandBuilder<MySqlCommand, MySqlParameter, TArgument> OnUpdateObject<TArgument>(MySqlObjectName tableName, TArgument argumentValue, UpdateOptions options) where TArgument : class
		{
			return new MySqlUpdateObject<TArgument>(this, tableName, argumentValue, options);
		}

		DbCommandBuilder<MySqlCommand, MySqlParameter> OnInsertBatch<TObject>(MySqlObjectName tableName, IEnumerable<TObject> objects, InsertOptions options)
	where TObject : class
		{
			return new MySqlInsertBatch<TObject>(this, tableName, objects, options);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="dataTable">The data table.</param>
		/// <returns>MySqlInsertBulk.</returns>
		public MySqlInsertBulk InsertBulk(MySqlObjectName tableName, DataTable dataTable)
		{
			return new MySqlInsertBulk(this, tableName, dataTable);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="dataReader">The data reader.</param>
		/// <returns>MySqlInsertBulk.</returns>
		public MySqlInsertBulk InsertBulk(MySqlObjectName tableName, IDataReader dataReader)
		{
			return new MySqlInsertBulk(this, tableName, dataReader);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="objects">The objects.</param>
		/// <returns>MySqlInsertBulk.</returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public MySqlInsertBulk InsertBulk<TObject>(MySqlObjectName tableName, IEnumerable<TObject> objects) where TObject : class
		{
			var table = DatabaseMetadata.GetTableOrView(tableName);
			return new MySqlInsertBulk(this, tableName, new ObjectDataReader<TObject>(table, objects, OperationTypes.Insert));
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="dataTable">The data table.</param>
		/// <returns>
		/// MySqlInsertBulk.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public MySqlInsertBulk InsertBulk<TObject>(DataTable dataTable) where TObject : class
		{
			return InsertBulk(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, dataTable);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="dataReader">The data reader.</param>
		/// <returns>
		/// MySqlInsertBulk.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public MySqlInsertBulk InsertBulk<TObject>(IDataReader dataReader) where TObject : class
		{
			return InsertBulk(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, dataReader);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="objects">The objects.</param>
		/// <returns>
		/// MySqlInsertBulk.
		/// </returns>
		public MySqlInsertBulk InsertBulk<TObject>(IEnumerable<TObject> objects) where TObject : class
		{
			return InsertBulk(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, objects);
		}

		/// <summary>
		/// Gets a table's row count.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		[SuppressMessage("Globalization", "CA1305")]
		public ILink<long> GetTableApproximateCount(MySqlObjectName tableName)
		{
			var table = DatabaseMetadata.GetTableOrView(tableName).Name; //get the real name
			var sql = $@"SHOW TABLE STATUS IN {table.Schema} like '{table.Name}'";
			return Sql(sql).ToRow().Transform(row => Convert.ToInt64(row["Rows"]));
		}

		/// <summary>
		/// Gets a table's row count.
		/// </summary>
		public ILink<long> GetTableApproximateCount<TObject>() => GetTableApproximateCount(DatabaseObjectAsTableOrView<TObject>(OperationType.Select).Name);

		public partial ILink<int?> Truncate(MySqlObjectName tableName)
		{
			//Verify the table name actually exists.
			var table = DatabaseMetadata.GetTableOrView(tableName);
			return Sql("TRUNCATE " + table.Name.ToQuotedString() + ";").AsNonQuery();
		}
	}
}
