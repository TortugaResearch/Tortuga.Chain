using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.SqlServer.CommandBuilders;

namespace Tortuga.Chain.SqlServer
{
	partial class SqlServerDataSourceBase
	{


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
















		ObjectDbCommandBuilder<SqlCommand, SqlParameter, TArgument> OnInsertOrUpdateObject<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, UpsertOptions options) where TArgument : class
		{
			return new SqlServerInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
		}

	}
}
