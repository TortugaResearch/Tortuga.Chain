using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.CommandBuilders;
using Tortuga.Shipwright;
using Traits;

namespace Tortuga.Chain.SqlServer;

[UseTrait(typeof(SupportsDeleteAllTrait<AbstractObjectName>))]
[UseTrait(typeof(SupportsTruncateTrait<AbstractObjectName>))]
[UseTrait(typeof(SupportsSqlQueriesTrait<AbstractCommand, AbstractParameter>))]
[UseTrait(typeof(SupportsInsertBatchTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
partial class SqlServerDataSourceBase : IInsertBatchHelper<SqlCommand, SqlParameter, SqlServerObjectName>
{

	/// <summary>
	/// Called when Database.DatabaseMetadata is invoked.
	/// </summary>
	/// <returns></returns>
	protected override IDatabaseMetadataCache OnGetDatabaseMetadata()
	{
		return DatabaseMetadata;
	}

	private partial AbstractObjectName OnGetTableOrViewNameFromClass(Type type, OperationType operationType)
	{
		var table = (TableOrViewMetadata<AbstractObjectName, AbstractDbType>)DatabaseMetadata.GetDatabaseObjectFromClass(type, operationType)!;
		return table.Name;
	}

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

	private partial AbstractObjectName OnParseObjectName(string objectName) { return new(objectName); }

	private partial MultipleTableDbCommandBuilder<AbstractCommand, AbstractParameter> OnSql(string sqlStatement, object? argumentValue)
	{
		return new SqlServerSqlCall(this, sqlStatement, argumentValue);
	}

	private partial DatabaseMetadataCache<SqlServerObjectName, SqlDbType> OnGetDatabaseMetadata2() => DatabaseMetadata;

	private partial IDataSource OnGetDataSource() => this;

	private partial List<SqlParameter> OnGetParameters(SqlBuilder<SqlDbType> builder)
		=> builder.GetParameters();

	private partial IInsertBatchHelper<SqlCommand, SqlParameter, SqlServerObjectName> OnGetInsertBatchHelper() => this;

	MultipleRowDbCommandBuilder<SqlCommand, SqlParameter> IInsertBatchHelper<SqlCommand, SqlParameter, SqlServerObjectName>.OnInsertBatch<TObject>(SqlServerObjectName tableName, IEnumerable<TObject> objects, InsertOptions options)
	{
		return new SqlServerInsertBatch<TObject>(this, tableName, objects, options); ;
	}
}
