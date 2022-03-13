using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.MySql.CommandBuilders;
using Tortuga.Shipwright;
using Traits;

namespace Tortuga.Chain.MySql
{
	[UseTrait(typeof(SupportsDeleteAllTrait<AbstractObjectName, AbstractDbType>))]
	[UseTrait(typeof(SupportsTruncateTrait<AbstractObjectName, AbstractDbType>))]
	[UseTrait(typeof(SupportsSqlQueriesTrait<AbstractCommand, AbstractParameter>))]
	[UseTrait(typeof(SupportsInsertBatchTrait<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType,
	DbCommandBuilder<AbstractCommand, AbstractParameter>>))]
	partial class MySqlDataSourceBase
	{
		DatabaseMetadataCache<AbstractObjectName, AbstractDbType> ICommandHelper<AbstractObjectName, AbstractDbType>.DatabaseMetadata => DatabaseMetadata;

		List<AbstractParameter> ICommandHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.GetParameters(SqlBuilder<AbstractDbType> builder) => builder.GetParameters();

		AbstractObjectName ICommandHelper<AbstractObjectName, AbstractDbType>.ParseObjectName(string objectName) => new(objectName);

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

		private partial MultipleTableDbCommandBuilder<AbstractCommand, AbstractParameter> OnSql(string sqlStatement, object? argumentValue)
		{
			return new MySqlSqlCall(this, sqlStatement, argumentValue);
		}

		DbCommandBuilder<AbstractCommand, AbstractParameter> IInsertBatchHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.OnInsertBatch<TObject>(AbstractObjectName tableName, IEnumerable<TObject> objects, InsertOptions options)
		{
			return new MySqlInsertBatch<TObject>(this, tableName, objects, options); ;
		}
	}
}
