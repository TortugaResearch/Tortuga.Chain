using System.Data.OleDb;
using Tortuga.Chain.Access.CommandBuilders;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Shipwright;
using Traits;

namespace Tortuga.Chain.Access
{
	[UseTrait(typeof(SupportsDeleteAllTrait<AbstractObjectName, AbstractDbType>))]
	//[UseTrait(typeof(SupportsDeleteByKeyList<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>))]
	[UseTrait(typeof(SupportsSqlQueriesTrait<AbstractCommand, AbstractParameter>))]
	partial class AccessDataSourceBase //: ICommandHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>
	{
		DatabaseMetadataCache<AbstractObjectName, AbstractDbType> ICommandHelper<AbstractObjectName, AbstractDbType>.DatabaseMetadata => DatabaseMetadata;

		//List<AbstractParameter> ICommandHelper<AbstractCommand, AbstractParameter, AbstractObjectName, AbstractDbType>.GetParameters(SqlBuilder<AbstractDbType> builder) => builder.GetParameters();

		AbstractObjectName ICommandHelper<AbstractObjectName, AbstractDbType>.ParseObjectName(string objectName) => new(objectName);

		private partial ILink<int?> OnDeleteAll(AccessObjectName tableName)
		{
			//Verify the table name actually exists.
			var table = DatabaseMetadata.GetTableOrView(tableName);
			return Sql("DELETE FROM " + table.Name.ToQuotedString() + ";").AsNonQuery();
		}



		private partial MultipleTableDbCommandBuilder<OleDbCommand, OleDbParameter> OnSql(string sqlStatement, object? argumentValue)
		{
			return new AccessSqlCall(this, sqlStatement, argumentValue);
		}
	}
}

