using System.Data.OleDb;
using Tortuga.Chain.Access.CommandBuilders;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Shipwright;

namespace Tortuga.Chain.Access
{
	[UseTrait(typeof(Traits.SupportsDeleteAllTrait<AbstractObjectName>))]
	[UseTrait(typeof(Traits.SupportsSqlQueriesTrait<AbstractCommand, AbstractParameter>))]
	partial class AccessDataSourceBase
	{
		private partial AccessObjectName OnGetTableOrViewNameFromClass(Type type, OperationType operationType)
		{
			var table = (TableOrViewMetadata<AccessObjectName, OleDbType>)DatabaseMetadata.GetDatabaseObjectFromClass(type, operationType)!;
			return table.Name;
		}

		private partial ILink<int?> OnDeleteAll(AccessObjectName tableName)
		{
			//Verify the table name actually exists.
			var table = DatabaseMetadata.GetTableOrView(tableName);
			return Sql("DELETE FROM " + table.Name.ToQuotedString() + ";").AsNonQuery();
		}

		private partial AccessObjectName OnParseObjectName(string objectName) { return new(objectName); }


		private partial MultipleTableDbCommandBuilder<OleDbCommand, OleDbParameter> OnSql(string sqlStatement, object? argumentValue)
		{
			return new AccessSqlCall(this, sqlStatement, argumentValue);
		}



	}
}

