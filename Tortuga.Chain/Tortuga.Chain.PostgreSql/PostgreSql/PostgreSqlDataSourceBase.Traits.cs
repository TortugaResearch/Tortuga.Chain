using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.PostgreSql.CommandBuilders;
using Tortuga.Shipwright;
using Traits;

namespace Tortuga.Chain.PostgreSql
{

	[UseTrait(typeof(SupportsDeleteAllTrait<AbstractObjectName>))]
	[UseTrait(typeof(SupportsTruncateTrait<AbstractObjectName>))]
	[UseTrait(typeof(SupportsSqlQueriesTrait<AbstractCommand, AbstractParameter>))]
	[UseTrait(typeof(SupportsInsertBatchTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType,
	MultipleRowDbCommandBuilder<AbstractCommand, AbstractParameter>>))]
	partial class PostgreSqlDataSourceBase
	{


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
			return new PostgreSqlSqlCall(this, sqlStatement, argumentValue);
		}

		private partial DatabaseMetadataCache<AbstractObjectName, AbstractDbType> OnGetDatabaseMetadata2() => DatabaseMetadata;

		private partial List<AbstractParameter> OnGetParameters(SqlBuilder<AbstractDbType> builder)
			=> builder.GetParameters();

		DbCommandBuilder<AbstractCommand, AbstractParameter> IInsertBatchHelper<AbstractCommand, AbstractParameter, AbstractObjectName>.OnInsertBatch<TObject>(AbstractObjectName tableName, IEnumerable<TObject> objects, InsertOptions options)
		{
			return new PostgreSqlInsertBatch<TObject>(this, tableName, objects, options); ;
		}
	}
}
