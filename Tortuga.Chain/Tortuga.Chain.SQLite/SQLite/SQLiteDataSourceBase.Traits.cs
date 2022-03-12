using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Metadata;
using Tortuga.Chain.SQLite.CommandBuilders;
using Tortuga.Shipwright;

namespace Tortuga.Chain.SQLite
{
	[UseTrait(typeof(Traits.SupportsDeleteAllTrait<AbstractObjectName>))]
	[UseTrait(typeof(Traits.SupportsTruncateTrait<AbstractObjectName>))]
	[UseTrait(typeof(Traits.SupportsSqlQueriesTrait<AbstractCommand, AbstractParameter>))]
	partial class SQLiteDataSourceBase
	{

		/// <summary>
		/// Called when Database.DatabaseMetadata is invoked.
		/// </summary>
		/// <returns></returns>
		protected override IDatabaseMetadataCache OnGetDatabaseMetadata()
		{
			return DatabaseMetadata;
		}

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


	}
}


