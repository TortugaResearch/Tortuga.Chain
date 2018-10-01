using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Anchor.Modeling;
using Tortuga.Chain;
using Tortuga.Chain.SqlServer;

namespace Tests
{
    [TestClass]
    public class SqlDependencyTest
    {
        [TestMethod]
        public void SqlServerDataSourceTests_SqlDependency()
        {
            var dataSource = SqlServerDataSource.CreateFromConfig("SqlServerTestDatabase");
            dataSource.StartSqlDependency();
            dataSource.TestConnection();
            dataSource.StopSqlDependency();
        }

        [TestMethod]
        public void SqlServerDataSourceTests_WithChangeNotification_Fired()
        {
            int eventCount = 0;
            var dataSource = SqlServerDataSource.CreateFromConfig("SqlServerTestDatabase");
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

        [TestMethod]
        public async Task SqlServerDataSourceTests_WithChangeNotification_Fired_Async()
        {
            int eventCount = 0;
            var dataSource = SqlServerDataSource.CreateFromConfig("SqlServerTestDatabase");
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

        [TestMethod]
        public void SqlServerDependencyTest()
        {
            int eventCount = 0;

            var connectionString = ConfigurationManager.ConnectionStrings["SqlServerTestDatabase"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM Sales.Customer WHERE State = 'CA'", connection))
                {
                    var dependency = new SqlDependency(command);
                    SqlDependency.Start(connectionString);
                    dependency.OnChange += new OnChangeEventHandler((s, e) => eventCount++);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read()) { } //NoOP
                    }

                    using (var command2 = new SqlCommand("INSERT INTO Sales.Customer(FullName, State ) VALUES ( N'Test 123', 'CA');", connection))
                    {
                        command2.ExecuteNonQuery();
                    }

                    Thread.Sleep(500);

                    GC.KeepAlive(dependency);

                    Assert.AreEqual(1, eventCount);
                }
            }
        }
    }

    public class Customer
    {
        public int? CustomerKey { get; set; }
        public string FullName { get; set; }
        public string State { get; set; }

        public List<Order> Orders { get; } = new List<Order>();
    }

    public class Order
    {
        public int? OrderKey { get; set; }

        [IgnoreOnUpdate]
        public int CustomerKey { get; set; }
    }
}