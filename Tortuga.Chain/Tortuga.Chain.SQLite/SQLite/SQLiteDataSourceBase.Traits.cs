using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SQLite.CommandBuilders;
using Tortuga.Shipwright;
using Traits;

namespace Tortuga.Chain.SQLite
{
	[UseTrait(typeof(SupportsDeleteAllTrait<AbstractObjectName, AbstractDbType>))]
	[UseTrait(typeof(SupportsTruncateTrait<AbstractObjectName, AbstractDbType>))]
	[UseTrait(typeof(SupportsSqlQueriesTrait<AbstractCommand, AbstractParameter>))]
	[UseTrait(typeof(SupportsInsertBatchTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType,
	DbCommandBuilder<AbstractCommand, AbstractParameter>>))]
	partial class SQLiteDataSourceBase
	{
		DatabaseMetadataCache<AbstractObjectName, AbstractDbType> ICommandHelper<AbstractObjectName, AbstractDbType>.DatabaseMetadata => DatabaseMetadata;

		List<AbstractParameter> ICommandHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.GetParameters(SqlBuilder<AbstractDbType> builder) => builder.GetParameters();

		AbstractObjectName ICommandHelper<AbstractObjectName, AbstractDbType>.ParseObjectName(string objectName) => new(objectName);

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

		private partial MultipleTableDbCommandBuilder<AbstractCommand, AbstractParameter> OnSql(string sqlStatement, object? argumentValue)
		{
			return new SQLiteSqlCall(this, sqlStatement, argumentValue, LockType.Write);
		}

		DbCommandBuilder<AbstractCommand, AbstractParameter> IInsertBatchHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertBatch<TObject>(AbstractObjectName tableName, IEnumerable<TObject> objects, InsertOptions options)
		{
			return new SQLiteInsertBatch<TObject>(this, tableName, objects, options); ;
		}
	}
}


