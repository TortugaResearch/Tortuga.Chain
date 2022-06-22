using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.PostgreSql;

partial class PostgreSqlDataSourceBase
{


	/// <summary>
	/// Gets a table's row count.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	public ILink<long> GetTableApproximateCount(PostgreSqlObjectName tableName)
	{
		var table = DatabaseMetadata.GetTableOrView(tableName); //get the real name
		var sql = @"SELECT tab.reltuples::BIGINT AS estimate FROM pg_class tab
INNER JOIN pg_namespace ns on ns.oid=tab.relnamespace
WHERE ns.nspname = @Schema AND tab.relname = @Name;";

		//If there are zero rows, this can return -1 instead.
		return Sql(sql, new { table.Name.Schema, table.Name.Name }).ToInt64().Transform(x => x == -1 ? 0 : x);
	}

	/// <summary>
	/// Gets a table or view's row count.
	/// </summary>
	///<typeparam name="TObject">This is used to determine which view to count. If the class isn't associated with a view, then it looks for a matching table.</typeparam>
	public ILink<long> GetTableApproximateCount<TObject>() => GetTableApproximateCount(DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name);

}
