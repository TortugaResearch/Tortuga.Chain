using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Tests.CommandBuilders
{
    [TestClass]
    public class SqlServerProcedureCallTests : TestBase
    {
        [TestMethod]
        public void SqlServerProcedureCallTests_Sql_1()
        {
            var proc = "Sales.CustomerWithOrdersByState";
            var result = DataSource.Procedure(proc, new { @State = "CA" }).ToTableSet("cust", "order").Execute();
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task SqlServerProcedureCallTests_Sql_1_Async()
        {
            var proc = "Sales.CustomerWithOrdersByState";
            var result = await DataSource.Procedure(proc, new { @State = "CA" }).ToTableSet("cust", "order").ExecuteAsync();
            Assert.AreEqual(2, result.Count);
        }

    }
}

