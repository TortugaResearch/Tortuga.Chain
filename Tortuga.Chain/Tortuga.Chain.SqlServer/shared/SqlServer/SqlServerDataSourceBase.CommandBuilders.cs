using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.SqlServer.CommandBuilders;

namespace Tortuga.Chain.SqlServer;

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
	[Obsolete("Use InsertBulk(...).WithOptions(SqlBulkCopyOptions) instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public SqlServerInsertBulk InsertBulk(SqlServerObjectName tableName, DataTable dataTable, SqlBulkCopyOptions options)
	{
		return InsertBulk(tableName, dataTable).WithOptions(options);
	}

	/// <summary>
	/// Inserts the batch of records using bulk insert.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="dataReader">The data reader.</param>
	/// <param name="options">The options.</param>
	/// <returns>SqlServerInsertBulk.</returns>
	[Obsolete("Use InsertBulk(...).WithOptions(SqlBulkCopyOptions) instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public SqlServerInsertBulk InsertBulk(SqlServerObjectName tableName, IDataReader dataReader, SqlBulkCopyOptions options)
	{
		return InsertBulk(tableName, dataReader).WithOptions(options);
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
	[Obsolete("Use InsertBulk(...).WithOptions(SqlBulkCopyOptions) instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public SqlServerInsertBulk InsertBulk<TObject>(SqlServerObjectName tableName, IEnumerable<TObject> objects, SqlBulkCopyOptions options) where TObject : class
	{
		return InsertBulk<TObject>(tableName, objects).WithOptions(options);
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
	[Obsolete("Use InsertBulk(...).WithOptions(SqlBulkCopyOptions) instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public SqlServerInsertBulk InsertBulk<TObject>(DataTable dataTable, SqlBulkCopyOptions options) where TObject : class
	{
		return InsertBulk<TObject>(dataTable).WithOptions(options);
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
	[Obsolete("Use InsertBulk(...).WithOptions(SqlBulkCopyOptions) instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public SqlServerInsertBulk InsertBulk<TObject>(IDataReader dataReader, SqlBulkCopyOptions options) where TObject : class
	{
		return InsertBulk<TObject>(dataReader).WithOptions(options);
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
	[Obsolete("Use InsertBulk(...).WithOptions(SqlBulkCopyOptions) instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public SqlServerInsertBulk InsertBulk<TObject>(IEnumerable<TObject> objects, SqlBulkCopyOptions options) where TObject : class
	{
		return InsertBulk<TObject>(objects).WithOptions(options);
	}

	/// <summary>
	/// Creates a operation based on a raw SQL statement.
	/// </summary>
	/// <param name="sqlStatement">The SQL statement.</param>
	/// <param name="argumentValue">The argument value.</param>
	/// <param name="defaults">Set the default type/size for parameters to address performance issues in SQL Server.</param>>
	/// <returns>SqlServerSqlCall.</returns>
	public SqlCallCommandBuilder<AbstractCommand, AbstractParameter> Sql(string sqlStatement, object argumentValue, SqlServerParameterDefaults defaults)
	{
		return new SqlServerSqlCall(this, sqlStatement, argumentValue, defaults);
	}
}
