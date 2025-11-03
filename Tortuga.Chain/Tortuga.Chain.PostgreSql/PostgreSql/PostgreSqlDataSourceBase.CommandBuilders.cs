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

	/// <summary>
	/// Reseeds the identity column of a table using the indicated seed value.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="seed">If the seed is null, the max value of the table will be used..</param>
	/// <returns>The new seed value.</returns>
	public ILink<long?> ReseedIdentityColumn(PostgreSqlObjectName tableName, int? seed)
	{
		return ReseedIdentityColumn(tableName, (long?)seed); //This always returns an Int64, even for Int32 columns.
	}

	/// <summary>
	/// Reseeds the identity column of a table using the indicated seed value.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="seed">If the seed is null, the max value of the table will be used..</param>
	/// <returns>The new seed value.</returns>
	public ILink<long?> ReseedIdentityColumn(PostgreSqlObjectName tableName, long? seed )
	{
		var table = DatabaseMetadata.GetTableOrView(tableName);
		var column = table.Columns.SingleOrDefault(c => c.IsIdentity);
		if (column == null)
			throw new MappingException($"Could not find an identity column on table {table.Name};");

		if (seed == null)
			return ReseedIdentityColumn(tableName);


		var sql = $"SELECT setval(pg_get_serial_sequence('{table.Name.ToQuotedString()}', '{column.SqlName}'), (SELECT GREATEST({seed}, MAX({column.QuotedSqlName})) FROM {table.Name.ToQuotedString()}));";

		return Sql(sql).ToInt64OrNull();
	}

	/// <summary>
	/// Reseeds the identity column of a table using the max value of the table.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	/// <returns>The new seed value.</returns>
	public ILink<long?> ReseedIdentityColumn(PostgreSqlObjectName tableName)
	{
		var table = DatabaseMetadata.GetTableOrView(tableName);
		var column = table.Columns.SingleOrDefault(c => c.IsIdentity);
		if (column == null)
			throw new MappingException($"Could not find an identity column on table {table.Name};");

		var sql = $"SELECT setval(pg_get_serial_sequence('{table.Name.ToQuotedString()}', '{column.SqlName}'), (SELECT MAX({column.QuotedSqlName}) FROM {table.Name.ToQuotedString()}));";

		return Sql(sql).ToInt64OrNull();
	}

}
