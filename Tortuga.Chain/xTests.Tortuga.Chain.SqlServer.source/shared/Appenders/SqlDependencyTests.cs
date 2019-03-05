using System;
using System.Threading;
using System.Threading.Tasks;
using Tests.Models;
using Tortuga.Chain;
using Tortuga.Chain.SqlServer;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Appenders
{
#if SQL_SERVER

    public class SqlDependencyTests : TestBase
    {
        public static RootData Root = new RootData(s_PrimaryDataSource);

        public SqlDependencyTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory, MemberData(nameof(Root))]
        public void SqlServerDataSourceTests_SqlDependency(string assemblyName, string dataSourceName)
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

        [Theory, MemberData(nameof(Root))]
        public void SqlServerDataSourceTests_WithChangeNotification_Fired(string assemblyName, string dataSourceName)
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

                dataSource.StopSqlDependency();
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Root))]
        public async Task SqlServerDataSourceTests_WithChangeNotification_Fired_Async(string assemblyName, string dataSourceName)
        {
            int eventCount = 0;
            var dataSource = DataSource(dataSourceName);
            try
            {
                dataSource.StartSqlDependency();

                //Insert a matching row
                var customerCount1 = await dataSource.From("Sales.Customer", new { State = "CA" }).ToInt32List("CustomerKey").WithChangeNotification((s, e) => eventCount++).ExecuteAsync();
                await dataSource.Insert("Sales.Customer", new Customer() { FullName = "Tod Test" + DateTime.Now.Ticks, State = "CA" }).ExecuteAsync();

                await Task.Delay(500); //give the event time to fire
                Assert.AreEqual(1, eventCount);

                //Insert another matching row
                var customerCount2 = await dataSource.From("Sales.Customer", new { State = "CA" }).ToInt32List("CustomerKey").WithChangeNotification((s, e) => eventCount++).ExecuteAsync();
                await dataSource.Insert("Sales.Customer", new Customer() { FullName = "Frank Test" + DateTime.Now.Ticks, State = "CA" }).ExecuteAsync();
                await Task.Delay(500); //give the event time to fire
                Assert.AreEqual(2, eventCount);

                //Insert a non-matching row
                var customerCount3 = await dataSource.From("Sales.Customer", new { State = "CA" }).ToInt32List("CustomerKey").WithChangeNotification((s, e) => eventCount++).ExecuteAsync();
                await dataSource.Insert("Sales.Customer", new Customer() { FullName = "Wrong State Test" + DateTime.Now.Ticks, State = "TX" }).ExecuteAsync();
                await Task.Delay(500); //give the event time to fire
                Assert.AreEqual(2, eventCount);

                dataSource.StopSqlDependency();
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Root))]
        public void SqlServerDataSourceTests_WithCaching(string assemblyName, string dataSourceName)
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

                dataSource.StopSqlDependency();
            }
            finally
            {
                Release(dataSource);
            }
        }
    }

#endif
}
