using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.CommandBuilders;

#if SQL_SERVER_SDS

using System.Data.SqlClient;

#elif SQL_SERVER_MDS

using Microsoft.Data.SqlClient;

#endif

namespace Tortuga.Chain.SqlServer
{
	partial class SqlServerDataSourceBase
	{
		/// <summary>
		/// Delete multiple rows by key.
		/// </summary>
		/// <typeparam name="TKey">The type of the t key.</typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="keys">The keys.</param>
		/// <param name="options">Update options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
		/// <exception cref="MappingException"></exception>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DeleteByKeyList")]
		public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> DeleteByKeyList<TKey>(SqlServerObjectName tableName, IEnumerable<TKey> keys, DeleteOptions options = DeleteOptions.None)
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
				return new SqlServerDeleteMany(this, tableName, where, parameters, parameters.Count, options);

			UpdateOptions effectiveOptions = UpdateOptions.SoftDelete;

			if (!options.HasFlag(DeleteOptions.CheckRowsAffected))
				effectiveOptions |= UpdateOptions.IgnoreRowsAffected;

			if (options.HasFlag(DeleteOptions.UseKeyAttribute))
				effectiveOptions |= UpdateOptions.UseKeyAttribute;

			return new SqlServerUpdateMany(this, tableName, null, where, parameters, parameters.Count, effectiveOptions);
		}

		/// <summary>
		/// Inserts the batch of records as one operation.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="tableTypeName">Name of the table type.</param>
		/// <param name="dataTable">The data table.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
		public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch(SqlServerObjectName tableName, DataTable dataTable, SqlServerObjectName tableTypeName, InsertOptions options = InsertOptions.None)
		{
			return new SqlServerInsertBatchTable(this, tableName, dataTable, tableTypeName, options);
		}

		/// <summary>
		/// Inserts the batch of records as one operation.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="tableTypeName">Name of the table type.</param>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
		public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch(SqlServerObjectName tableName, DbDataReader dataReader, SqlServerObjectName tableTypeName, InsertOptions options = InsertOptions.None)
		{
			return new SqlServerInsertBatchTable(this, tableName, dataReader, tableTypeName, options);
		}

		/// <summary>
		/// Inserts the batch of records as one operation.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="tableTypeName">Name of the table type.</param>
		/// <param name="objects">The objects.</param>
		/// <param name="options">The options.</param>
		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch<TObject>(SqlServerObjectName tableName, IEnumerable<TObject> objects, SqlServerObjectName tableTypeName, InsertOptions options = InsertOptions.None)
		{
			var tableType = DatabaseMetadata.GetUserDefinedTableType(tableTypeName);
			return new SqlServerInsertBatchTable(this, tableName, new ObjectDataReader<TObject>(tableType, objects), tableTypeName, options);
		}

		/// <summary>
		/// Inserts the batch of records as one operation..
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="tableTypeName">Name of the table type.</param>
		/// <param name="objects">The objects.</param>
		/// <param name="options">The options.</param>
		/// <returns>
		/// MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.
		/// </returns>
		public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> InsertBatch<TObject>(IEnumerable<TObject> objects, SqlServerObjectName tableTypeName, InsertOptions options = InsertOptions.None) where TObject : class
		{
			return InsertBatch(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, objects, tableTypeName, options);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="dataTable">The data table.</param>
		/// <param name="options">The options.</param>
		/// <returns>SqlServerInsertBulk.</returns>
		public SqlServerInsertBulk InsertBulk(SqlServerObjectName tableName, DataTable dataTable, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
		{
			return new SqlServerInsertBulk(this, tableName, dataTable, options);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="options">The options.</param>
		/// <returns>SqlServerInsertBulk.</returns>
		public SqlServerInsertBulk InsertBulk(SqlServerObjectName tableName, IDataReader dataReader, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
		{
			return new SqlServerInsertBulk(this, tableName, dataReader, options);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="objects">The objects.</param>
		/// <param name="options">The options.</param>
		/// <returns>SqlServerInsertBulk.</returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public SqlServerInsertBulk InsertBulk<TObject>(SqlServerObjectName tableName, IEnumerable<TObject> objects, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default) where TObject : class
		{
			var table = DatabaseMetadata.GetTableOrView(tableName);
			return new SqlServerInsertBulk(this, tableName, new ObjectDataReader<TObject>(table, objects, OperationTypes.Insert), options);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="dataTable">The data table.</param>
		/// <param name="options">The options.</param>
		/// <returns>
		/// SqlServerInsertBulk.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public SqlServerInsertBulk InsertBulk<TObject>(DataTable dataTable, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default) where TObject : class
		{
			return InsertBulk(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, dataTable, options);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="options">The options.</param>
		/// <returns>
		/// SqlServerInsertBulk.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public SqlServerInsertBulk InsertBulk<TObject>(IDataReader dataReader, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default) where TObject : class
		{
			return InsertBulk(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, dataReader, options);
		}

		/// <summary>
		/// Inserts the batch of records using bulk insert.
		/// </summary>
		/// <typeparam name="TObject">The type of the object.</typeparam>
		/// <param name="objects">The objects.</param>
		/// <param name="options">The options.</param>
		/// <returns>
		/// SqlServerInsertBulk.
		/// </returns>
		public SqlServerInsertBulk InsertBulk<TObject>(IEnumerable<TObject> objects, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default) where TObject : class
		{
			return InsertBulk(DatabaseMetadata.GetTableOrViewFromClass<TObject>().Name, objects, options);
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
		/// <returns>MultipleRowDbCommandBuilder&lt;SqlCommand, SqlParameter&gt;.</returns>
		/// <exception cref="MappingException"></exception>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UpdateByKeyList")]
		public MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> UpdateByKeyList<TArgument, TKey>(SqlServerObjectName tableName, TArgument newValues, IEnumerable<TKey> keys, UpdateOptions options = UpdateOptions.None)
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

			return new SqlServerUpdateMany(this, tableName, newValues, where, parameters, parameters.Count, options);
		}

		SqlServerTableOrView<TObject> OnGetByKey<TObject, TKey>(SqlServerObjectName tableName, ColumnMetadata<SqlDbType> columnMetadata, TKey key)
			where TObject : class
		{
			var where = columnMetadata.SqlName + " = @Param0";

			var parameters = new List<SqlParameter>();
			var param = new SqlParameter("@Param0", key);
			if (columnMetadata.DbType.HasValue)
				param.SqlDbType = columnMetadata.DbType.Value;
			parameters.Add(param);

			return new SqlServerTableOrView<TObject>(this, tableName, where, parameters);
		}

		MultipleRowDbCommandBuilder<SqlCommand, SqlParameter, TObject> OnGetByKeyList<TObject, TKey>(SqlServerObjectName tableName, ColumnMetadata<SqlDbType> columnMetadata, IEnumerable<TKey> keys)
			where TObject : class
		{
			var keyList = keys.AsList();
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

			return new MultipleRowDbCommandBuilder<SqlCommand, SqlParameter, TObject>(new SqlServerTableOrView<TObject>(this, tableName, where, parameters));
		}

		MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> OnDeleteMany(SqlServerObjectName tableName, string whereClause, object? argumentValue)
		{
			return new SqlServerDeleteMany(this, tableName, whereClause, argumentValue);
		}

		MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> OnDeleteMany(SqlServerObjectName tableName, object filterValue, FilterOptions filterOptions)
		{
			return new SqlServerDeleteMany(this, tableName, filterValue, filterOptions);
		}

		ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> OnDeleteObject<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, DeleteOptions options) where TArgument : class
		{
			return new SqlServerDeleteObject<TArgument>(this, tableName, argumentValue, options);
		}

		TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption, TObject> OnFromTableOrView<TObject>(SqlServerObjectName tableOrViewName, object filterValue, FilterOptions filterOptions)
			where TObject : class
		{
			return new SqlServerTableOrView<TObject>(this, tableOrViewName, filterValue, filterOptions);
		}

		TableDbCommandBuilder<SqlCommand, SqlParameter, SqlServerLimitOption, TObject> OnFromTableOrView<TObject>(SqlServerObjectName tableOrViewName, string? whereClause, object? argumentValue)
			where TObject : class
		{
			return new SqlServerTableOrView<TObject>(this, tableOrViewName, whereClause, argumentValue);
		}

		ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> OnInsertObject<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, InsertOptions options)
			   where TArgument : class
		{
			return new SqlServerInsertObject<TArgument>(this, tableName, argumentValue, options);
		}

		ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> OnInsertOrUpdateObject<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, UpsertOptions options) where TArgument : class
		{
			return new SqlServerInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
		}

		IUpdateManyDbCommandBuilder<SqlCommand, SqlParameter> OnUpdateMany(SqlServerObjectName tableName, string updateExpression, object? updateArgumentValue, UpdateOptions options)
		{
			return new SqlServerUpdateMany(this, tableName, updateExpression, updateArgumentValue, options);
		}

		IUpdateManyDbCommandBuilder<SqlCommand, SqlParameter> OnUpdateMany(SqlServerObjectName tableName, object? newValues, UpdateOptions options)
		{
			return new SqlServerUpdateMany(this, tableName, newValues, options);
		}

		ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> OnUpdateObject<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, UpdateOptions options) where TArgument : class
		{
			return new SqlServerUpdateObject<TArgument>(this, tableName, argumentValue, options);
		}

		MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> OnInsertBatch<TObject>(SqlServerObjectName tableName, IEnumerable<TObject> objects, InsertOptions options)
			where TObject : class
		{
			return new SqlServerInsertBatch<TObject>(this, tableName, objects, options);
		}

	}
}
