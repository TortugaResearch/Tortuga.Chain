using Tests.Models;
using Tortuga.Chain;
using Tortuga.Chain.SqlServer;

namespace Tests.Appenders;

#if SQL_SERVER_SDS
using System.Data.SqlClient;
#elif SQL_SERVER_MDS

using Microsoft.Data.SqlClient;

#endif

#if (SQL_SERVER_SDS || SQL_SERVER_MDS)

[TestClass]
public class SqlDependencyTests : TestBase
{
	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void SqlServerDataSourceTests_SqlDependency(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			dataSource.StartSqlDependency();
			dataSource.TestConnection();
			dataSource.StopSqlDependency();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void SqlServerDataSourceTests_WithChangeNotification_Fired(string dataSourceName)
	{
		int eventCount = 0;
		var dataSource = DataSource(dataSourceName);
		try
		{
			dataSource.StartSqlDependency();

			//Insert a matching row
			var customerCount1 = dataSource.From("Sales.Customer", new { State = "CA" }).ToInt32List("CustomerKey").WithChangeNotification((s, e) => eventCount++).Execute();
			dataSource.Insert("Sales.Customer", new Customer() { FullName = "Tod Test" + DateTime.Now.Ticks, State = "CA" }).Execute();
			Thread.Sleep(500); //give the event time to fire
			Assert.AreEqual(1, eventCount);

			//Insert another matching row
			var customerCount2 = dataSource.From("Sales.Customer", new { State = "CA" }).ToInt32List("CustomerKey").WithChangeNotification((s, e) => eventCount++).Execute();
			dataSource.Insert("Sales.Customer", new Customer() { FullName = "Frank Test" + DateTime.Now.Ticks, State = "CA" }).Execute();
			Thread.Sleep(500); //give the event time to fire
			Assert.AreEqual(2, eventCount);

			//Insert a non-matching row
			var customerCount3 = dataSource.From("Sales.Customer", new { State = "CA" }).ToInt32List("CustomerKey").WithChangeNotification((s, e) => eventCount++).Execute();
			dataSource.Insert("Sales.Customer", new Customer() { FullName = "Wrong State Test" + DateTime.Now.Ticks, State = "TX" }).Execute();
			Thread.Sleep(500); //give the event time to fire
			Assert.AreEqual(2, eventCount);
		}
		finally
		{
			dataSource.StopSqlDependency();
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public async Task SqlServerDataSourceTests_WithChangeNotification_Fired_Async(string dataSourceName)
	{
		int eventCount = 0;
		var dataSource = DataSource(dataSourceName);
		try
		{
			dataSource.StartSqlDependency();

			//Insert a matching row
			var customerCount1 = await dataSource.From("Sales.Customer", new { State = "CA" }).ToInt32List("CustomerKey").WithChangeNotification((s, e) => eventCount++).ExecuteAsync().ConfigureAwait(false);
			await dataSource.Insert("Sales.Customer", new Customer() { FullName = "Tod Test" + DateTime.Now.Ticks, State = "CA" }).ExecuteAsync().ConfigureAwait(false);

			await Task.Delay(500).ConfigureAwait(false); //give the event time to fire
			Assert.AreEqual(1, eventCount);

			//Insert another matching row
			var customerCount2 = await dataSource.From("Sales.Customer", new { State = "CA" }).ToInt32List("CustomerKey").WithChangeNotification((s, e) => eventCount++).ExecuteAsync().ConfigureAwait(false);
			await dataSource.Insert("Sales.Customer", new Customer() { FullName = "Frank Test" + DateTime.Now.Ticks, State = "CA" }).ExecuteAsync().ConfigureAwait(false);
			await Task.Delay(500).ConfigureAwait(false); //give the event time to fire
			Assert.AreEqual(2, eventCount);

			//Insert a non-matching row
			var customerCount3 = await dataSource.From("Sales.Customer", new { State = "CA" }).ToInt32List("CustomerKey").WithChangeNotification((s, e) => eventCount++).ExecuteAsync().ConfigureAwait(false);
			await dataSource.Insert("Sales.Customer", new Customer() { FullName = "Wrong State Test" + DateTime.Now.Ticks, State = "TX" }).ExecuteAsync().ConfigureAwait(false);
			await Task.Delay(500).ConfigureAwait(false); //give the event time to fire
			Assert.AreEqual(2, eventCount);
		}
		finally
		{
			dataSource.StopSqlDependency();
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void SqlServerDataSourceTests_WithCaching(string dataSourceName)
	{
		const string CacheKey = "All_Customer";

		var dataSource = DataSource(dataSourceName);
		try
		{
			dataSource.StartSqlDependency();

			//Insert a matching row
			var customerCount1 = dataSource.From("Sales.Customer", new { State = "CA" }).ToInt32List("CustomerKey").Cache(CacheKey).AutoInvalidate().Execute();

			Assert.IsTrue(dataSource.Cache.TryRead<object>(CacheKey, out var _));

			//Insert another matching row
			dataSource.Insert("Sales.Customer", new Customer() { FullName = "Frank Test" + DateTime.Now.Ticks, State = "CA" }).Execute();

			Thread.Sleep(500); //give the event time to fire

			Assert.IsFalse(dataSource.Cache.TryRead<object>(CacheKey, out var _), "Cache was not cleared");
		}
		finally
		{
			dataSource.StopSqlDependency();
			Release(dataSource);
		}
	}
}

#endif
