using System.Data.SQLite;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.SQLite.CommandBuilders;

namespace Tortuga.Chain.SQLite
{
	partial class SQLiteDataSourceBase
	{
		ObjectDbCommandBuilder<SQLiteCommand, SQLiteParameter, TArgument> OnInsertOrUpdateObject<TArgument>(SQLiteObjectName tableName, TArgument argumentValue, UpsertOptions options) where TArgument : class
		{
			return new SQLiteInsertOrUpdateObject<TArgument>(this, tableName, argumentValue, options);
		}

	}
}
