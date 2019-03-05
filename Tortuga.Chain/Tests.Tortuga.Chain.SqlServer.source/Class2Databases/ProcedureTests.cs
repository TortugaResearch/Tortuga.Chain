using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tortuga.Anchor.Modeling;

namespace Tests.Class2Databases
{
    public class Customer
    {
        public int? CustomerKey { get; set; }
        public string FullName { get; set; }
        public List<Order> Orders { get; } = new List<Order>();
        public string State { get; set; }
    }

    public class Order
    {
        [IgnoreOnUpdate]
        public int CustomerKey { get; set; }

        public int? OrderKey { get; set; }
    }

    [TestClass]
    public class ProcedureTests : TestBase
    {
        [TestMethod]
        public void Proc1_Dictionary()
        {
            var result = DataSource.Procedure(Proc1Name, new Dictionary<string, object>() { { "State", "CA" } }).ToTableSet("cust", "order").Execute();
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task Proc1_Dictionary_Async()
        {
            var result = await DataSource.Procedure(Proc1Name, new Dictionary<string, object>() { { "State", "CA" } }).ToTableSet("cust", "order").ExecuteAsync();
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task Proc1_Dictionary_Async_DataSet()
        {
            var result = await DataSource.Procedure(Proc1Name, new Dictionary<string, object>() { { "State", "CA" } }).ToDataSet("cust", "order").ExecuteAsync();
            Assert.AreEqual(2, result.Tables.Count);
        }

        [TestMethod]
        public void Proc1_Dictionary_DataSet()
        {
            var result = DataSource.Procedure(Proc1Name, new Dictionary<string, object>() { { "State", "CA" } }).ToDataSet("cust", "order").Execute();
            Assert.AreEqual(2, result.Tables.Count);
        }

        [TestMethod]
        public void Proc1_Dictionary2()
        {
            var result = DataSource.Procedure(Proc1Name, new Dictionary<string, object>() { { "@State", "CA" } }).ToTableSet("cust", "order").Execute();
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task Proc1_Dictionary2_Async()
        {
            var result = await DataSource.Procedure(Proc1Name, new Dictionary<string, object>() { { "@State", "CA" } }).ToTableSet("cust", "order").ExecuteAsync();
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task Proc1_Dictionary2_Async_DataSet()
        {
            var result = await DataSource.Procedure(Proc1Name, new Dictionary<string, object>() { { "@State", "CA" } }).ToDataSet("cust", "order").ExecuteAsync();
            Assert.AreEqual(2, result.Tables.Count);
        }

        [TestMethod]
        public void Proc1_Dictionary2_DataSet()
        {
            var result = DataSource.Procedure(Proc1Name, new Dictionary<string, object>() { { "@State", "CA" } }).ToDataSet("cust", "order").Execute();
            Assert.AreEqual(2, result.Tables.Count);
        }

        [TestMethod]
        public void Proc1_Object()
        {
            var result = DataSource.Procedure(Proc1Name, new { @State = "CA" }).ToTableSet("cust", "order").Execute();
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task Proc1_Object_Async()
        {
            var result = await DataSource.Procedure(Proc1Name, new { @State = "CA" }).ToTableSet("cust", "order").ExecuteAsync();
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task Proc1_Object_Async_DataSet()
        {
            var result = await DataSource.Procedure(Proc1Name, new { @State = "CA" }).ToDataSet("cust", "order").ExecuteAsync();
            Assert.AreEqual(2, result.Tables.Count);
        }

        [TestMethod]
        public void Proc1_Object_DataSet()
        {
            var result = DataSource.Procedure(Proc1Name, new { @State = "CA" }).ToDataSet("cust", "order").Execute();
            Assert.AreEqual(2, result.Tables.Count);
        }

        [TestMethod]
        public async Task Proc1_ToCollectionSet()
        {
            var result = await DataSource.Procedure(Proc1Name, new { @State = "CA" }).ToCollectionSet<Customer, Order>().ExecuteAsync();
        }
    }
}
