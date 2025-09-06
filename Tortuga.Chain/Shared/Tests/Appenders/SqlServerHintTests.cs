
namespace Tests.Appenders;

#if SQL_SERVER_MDS

using Microsoft.Data.SqlClient;
using Tortuga.Chain;
using Tortuga.Chain.SqlServer;

[TestClass]
public class SqlServerHintTests : TestBase
{
	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void SqlServerHintTests_IsolationLevels(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			string message = null;
			SqlInfoMessageEventHandler handler = (sender, e) => { message = e.Message; };

			var result1 = dataSource.From("Sales.Customer").WithIsolationLevel(SqlServerIsolationLevel.Serializable).AsCount().Execute();
			//Snapshot is only supported by memory-optimzed tables
			//var result2 = dataSource.From("Sales.Customer").WithIsolationLevel(SqlServerIsolationLevel.Snapshot).AsCount().Execute();
			var result3 = dataSource.From("Sales.Customer").WithIsolationLevel(SqlServerIsolationLevel.ReadUncommitted).AsCount().Execute();
			var result4 = dataSource.From("Sales.Customer").WithIsolationLevel(SqlServerIsolationLevel.ReadCommitted).AsCount().Execute();
			var result5 = dataSource.From("Sales.Customer").WithIsolationLevel(SqlServerIsolationLevel.RepeatableRead).AsCount().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}
}

#endif
