using System.Data.OleDb;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.SqlServer.CommandBuilders;

namespace Tortuga.Chain.SqlServer
{
	partial class OleDbSqlServerDataSourceBase
	{

		ObjectDbCommandBuilder<OleDbCommand, OleDbParameter, TArgument> OnInsertOrUpdateObject<TArgument>(SqlServerObjectName tableName, TArgument argumentValue, UpsertOptions options) where TArgument : class
		{
			return new OleDbSqlServerInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
		}

	}
}
