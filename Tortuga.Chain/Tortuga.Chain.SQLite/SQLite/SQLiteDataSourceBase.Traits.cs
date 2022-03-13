using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SQLite.CommandBuilders;
using Tortuga.Shipwright;
using Traits;

namespace Tortuga.Chain.SQLite
{
	[UseTrait(typeof(SupportsDeleteAllTrait<AbstractObjectName>))]
	[UseTrait(typeof(SupportsTruncateTrait<AbstractObjectName>))]
	[UseTrait(typeof(SupportsSqlQueriesTrait<AbstractCommand, AbstractParameter>))]
	[UseTrait(typeof(SupportsInsertBatchTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType,
	DbCommandBuilder<AbstractCommand, AbstractParameter>>))]
	partial class SQLiteDataSourceBase
	{

		private partial AbstractObjectName OnGetTableOrViewNameFromClass(Type type, OperationType operationType)
		{
			var table = (TableOrViewMetadata<AbstractObjectName, AbstractDbType>)DatabaseMetadata.GetDatabaseObjectFromClass(type, operationType)!;
			return table.Name;
		}

		private partial ILink<int?> OnDeleteAll(AbstractObjectName tableName)
		{
			//SQLite determines for itself if a delete all should be interpreted as a truncate.
			return OnTruncate(tableName);
		}

		private partial ILink<int?> OnTruncate(AbstractObjectName tableName)
		{
			//Verify the table name actually exists.
			var table = DatabaseMetadata.GetTableOrView(tableName);
			//In SQLite, a delete without a where clause is interpreted as a truncate if other conditions are met.
			return Sql("DELETE FROM " + table.Name.ToQuotedString() + ";").AsNonQuery();
		}

		private partial AbstractObjectName OnParseObjectName(string objectName) { return new(objectName); }

		private partial MultipleTableDbCommandBuilder<AbstractCommand, AbstractParameter> OnSql(string sqlStatement, object? argumentValue)
		{
			return new SQLiteSqlCall(this, sqlStatement, argumentValue, LockType.Write);
		}

		private partial DatabaseMetadataCache<AbstractObjectName, AbstractDbType> OnGetDatabaseMetadata2() => DatabaseMetadata;

		private partial List<AbstractParameter> OnGetParameters(SqlBuilder<AbstractDbType> builder)
			=> builder.GetParameters();

		DbCommandBuilder<AbstractCommand, AbstractParameter> IInsertBatchHelper<AbstractCommand, AbstractParameter, AbstractObjectName>.OnInsertBatch<TObject>(AbstractObjectName tableName, IEnumerable<TObject> objects, InsertOptions options)
		{
			return new SQLiteInsertBatch<TObject>(this, tableName, objects, options); ;
		}
	}
}


