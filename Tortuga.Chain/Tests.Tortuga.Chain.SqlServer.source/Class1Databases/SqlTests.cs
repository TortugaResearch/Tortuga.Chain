using System.Threading.Tasks;
using Tortuga.Chain;

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Tests.Class1Databases
{
    [TestClass]
    public class SqlTests : TestBase
    {
        [TestMethod]
        public void Sql_1()
        {
            var sql = "SELECT 5";
            var result = DataSource.Sql(sql).ToInt32().Execute();
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void Sql_2()
        {
            var sql = "SELECT @Input";
            var result = DataSource.Sql(sql, new { @Input = 5 }).ToInt32().WithTracingToDebug().Execute();
            Assert.AreEqual(5, result);

            var outSql = DataSource.Sql(sql, new { @Input = 5 }).ToInt32().WithTracingToDebug().CommandText();
            Assert.AreEqual(sql, outSql);
        }


        [TestMethod]
        public void Sql_2nq()
        {
            var sql = "SELECT @Input";
            DataSource.Sql(sql, new { @Input = 5 }).AsNonQuery().WithTracingToDebug().Execute();

            var outSql = DataSource.Sql(sql, new { @Input = 5 }).ToInt32().WithTracingToDebug().CommandText();
            Assert.AreEqual(sql, outSql);
        }

        [TestMethod]
        public async Task Sql_2_Async()
        {
            var sql = "SELECT @Input";
            var result = await DataSource.Sql(sql, new { @Input = 5 }).ToInt32().WithTracingToDebug().ExecuteAsync();
            Assert.AreEqual(5, result);

        }


        [TestMethod]
        public async Task Sql_2nq_Async()
        {
            var sql = "SELECT @Input";
            await DataSource.Sql(sql, new { @Input = 5 }).AsNonQuery().WithTracingToDebug().ExecuteAsync();

        }
    }
}
