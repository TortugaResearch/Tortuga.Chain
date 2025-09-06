
namespace Tests.Appenders;

#if SQL_SERVER_MDS

using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;
using Tortuga.Chain;
using Tortuga.Chain.SqlServer;

[TestClass]
public class SqlServerHintTests : TestBase
{
	const string IndexName = "IX_Customer_State";
	const string IndexedTableName = "Sales.Customer";

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void SqlServerHintTests_IsolationLevels(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{

			var result1 = dataSource.From(IndexedTableName).WithIsolationLevel(SqlServerIsolationLevel.Serializable).AsCount().Execute();
			//Snapshot is only supported by memory-optimzed tables
			//var result2 = dataSource.From("Sales.Customer").WithIsolationLevel(SqlServerIsolationLevel.Snapshot).AsCount().Execute();
			var result3 = dataSource.From(IndexedTableName).WithIsolationLevel(SqlServerIsolationLevel.ReadUncommitted).AsCount().Execute();
			var result4 = dataSource.From(IndexedTableName).WithIsolationLevel(SqlServerIsolationLevel.ReadCommitted).AsCount().Execute();
			var result5 = dataSource.From(IndexedTableName).WithIsolationLevel(SqlServerIsolationLevel.RepeatableRead).AsCount().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void SqlServerHintTests_IndexHint(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			var result = dataSource.From(IndexedTableName).WithIndex(IndexName).AsCount().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void SqlServerHintTests_IndexHint2(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			var index = dataSource.DatabaseMetadata.GetTableOrView(IndexedTableName).GetIndexes().Single(i => i.Name == IndexName);
			var result = dataSource.From(IndexedTableName).WithIndex(index).AsCount().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}
}

#endif
