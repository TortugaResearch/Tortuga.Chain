using Tortuga.Chain;

namespace Tests.Appenders;

#if SQL_SERVER_SDS
using System.Data.SqlClient;
#elif SQL_SERVER_MDS

using Microsoft.Data.SqlClient;

#endif

#if (SQL_SERVER_SDS || SQL_SERVER_MDS)

[TestClass]
public class CommandTimeoutTests : TestBase
{
	const string TimeoutSql = "WAITFOR DELAY '00:00:03'";

	[DataTestMethod, RootData(DataSourceGroup.All)]
	public void TimeoutTriggeredTest(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			dataSource.Sql(TimeoutSql).AsNonQuery().SetTimeout(TimeSpan.FromSeconds(2)).Execute();
			Assert.Fail("Timeout exception expected");
		}
		catch (SqlException) {/* expected */}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.All)]
	public async Task TimeoutTriggeredAsyncTest(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			await dataSource.Sql(TimeoutSql).AsNonQuery().SetTimeout(TimeSpan.FromSeconds(2)).ExecuteAsync();
			Assert.Fail("Timeout exception expected");
		}
		catch (SqlException) {/* expected */}
		finally
		{
			Release(dataSource);
		}
	}
}

#endif