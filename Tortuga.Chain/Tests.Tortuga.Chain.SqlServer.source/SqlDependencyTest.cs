using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
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



