using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.CommandBuilders;
using Tortuga.Shipwright;
using Traits;

namespace Tortuga.Chain.SqlServer;

[UseTrait(typeof(SupportsDeleteAllTrait<AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsTruncateTrait<AbstractObjectName, AbstractDbType>))]
[UseTrait(typeof(SupportsSqlQueriesTrait<AbstractCommand, AbstractParameter>))]
partial class OleDbSqlServerDataSourceBase
{
	DatabaseMetadataCache<AbstractObjectName, AbstractDbType> ICommandHelper<AbstractObjectName, AbstractDbType>.DatabaseMetadata => DatabaseMetadata;

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
		return new OleDbSqlServerSqlCall(this, sqlStatement, argumentValue);
	}
}
