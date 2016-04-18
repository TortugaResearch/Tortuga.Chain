using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Class2Databases
{
    [TestClass]
    public class ProcedureTests : TestBase
    {
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
        public async Task Proc1_ToCollectionSet()
        {
            var result = await DataSource.Procedure(Proc1Name, new { @State = "CA" }).ToCollectionSet<Customer, Order>().ExecuteAsync();
        }
    }
}

