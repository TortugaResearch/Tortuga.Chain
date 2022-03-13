using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SqlServer.CommandBuilders;
using Tortuga.Shipwright;

namespace Tortuga.Chain.SqlServer;

[UseTrait(typeof(Traits.SupportsDeleteAllTrait<AbstractObjectName>))]
[UseTrait(typeof(Traits.SupportsTruncateTrait<AbstractObjectName>))]
[UseTrait(typeof(Traits.SupportsSqlQueriesTrait<AbstractCommand, AbstractParameter>))]
partial class OleDbSqlServerDataSourceBase
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
		return new OleDbSqlServerSqlCall(this, sqlStatement, argumentValue);
	}
}
