using Npgsql;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.PostgreSql.CommandBuilders;

namespace Tortuga.Chain.PostgreSql
{
	partial class PostgreSqlDataSourceBase
	{


		MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> OnDeleteSet(PostgreSqlObjectName tableName, string whereClause, object? argumentValue)
		{
			return new PostgreSqlDeleteSet(this, tableName, whereClause, argumentValue);
		}

		MultipleRowDbCommandBuilder<NpgsqlCommand, NpgsqlParameter> OnDeleteSet(PostgreSqlObjectName tableName, object? filterValue, FilterOptions filterOptions)
		{
			return new PostgreSqlDeleteSet(this, tableName, filterValue, filterOptions);
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
		/// Gets a table or view's row count.
		/// </summary>
		///<typeparam name="TObject">This is used to determine which view to count. If the class isn't associated with a view, then it looks for a matching table.</typeparam>
		public ILink<long> GetTableApproximateCount<TObject>() => GetTableApproximateCount(DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name);

	}
}
