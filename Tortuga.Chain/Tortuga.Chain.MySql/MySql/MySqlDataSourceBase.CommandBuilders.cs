using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.MySql;

partial class MySqlDataSourceBase
{

	/// <summary>
	/// Gets a table's row count.
	/// </summary>
	/// <param name="tableName">Name of the table.</param>
	[SuppressMessage("Globalization", "CA1305")]
	public ILink<long> GetTableApproximateCount(MySqlObjectName tableName)
	{
		var table = DatabaseMetadata.GetTableOrView(tableName).Name; //get the real name
		var sql = $@"SHOW TABLE STATUS IN {table.Schema} like '{table.Name}'";
		return Sql(sql).ToRow().Transform(row => Convert.ToInt64(row["Rows"]));
	}

	/// <summary>
	/// Gets a table's row count.
	/// </summary>
	public ILink<long> GetTableApproximateCount<TObject>() => GetTableApproximateCount(DatabaseMetadata.GetTableOrViewFromClass<TObject>(OperationType.Select).Name);


}
