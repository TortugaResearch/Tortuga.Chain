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

		MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> OnDeleteSet(MySqlObjectName tableName, string whereClause, object? argumentValue)
		{
			return new MySqlDeleteSet(this, tableName, whereClause, argumentValue);
		}

		MultipleRowDbCommandBuilder<MySqlCommand, MySqlParameter> OnDeleteSet(MySqlObjectName tableName, object filterValue, FilterOptions filterOptions)
		{
			return new MySqlDeleteSet(this, tableName, filterValue, filterOptions);
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
		public ILink<long> GetTableApproximateCount<TObject>() => GetTableApproximateCount(DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name);


	}
}
