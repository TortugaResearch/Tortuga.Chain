using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Tortuga.Chain;

namespace Tests.CommandBuilders
{
    [TestClass]
    public class SqlServerSqlCallTests : TestBase
    {
        [TestMethod]
        public void SqlServerSqlCallTests_Sql_1()
        {
            var sql = "SELECT 5";
            var result = DataSource.Sql(sql).ToInt32().Execute();
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void SqlServerSqlCallTests_Sql_2()
        {
            var sql = "SELECT @Input";
            var result = DataSource.Sql(sql, new { @Input = 5 }).ToInt32().WithTracingToDebug().Execute();
            Assert.AreEqual(5, result);

            var outSql = DataSource.Sql(sql, new { @Input = 5 }).ToInt32().WithTracingToDebug().Sql();
            Assert.AreEqual(sql, outSql);
        }


        [TestMethod]
        public void SqlServerSqlCallTests_Sql_2nq()
        {
            var sql = "SELECT @Input";
            DataSource.Sql(sql, new { @Input = 5 }).AsNonQuery().WithTracingToDebug().Execute();

            var outSql = DataSource.Sql(sql, new { @Input = 5 }).ToInt32().WithTracingToDebug().Sql();
            Assert.AreEqual(sql, outSql);
        }

        [TestMethod]
        public async Task SqlServerSqlCallTests_Sql_2_Async()
        {
            var sql = "SELECT @Input";
            var result = await DataSource.Sql(sql, new { @Input = 5 }).ToInt32().WithTracingToDebug().ExecuteAsync();
            Assert.AreEqual(5, result);

        }


        [TestMethod]
        public async Task SqlServerSqlCallTests_Sql_2nq_Async()
        {
            var sql = "SELECT @Input";
            await DataSource.Sql(sql, new { @Input = 5 }).AsNonQuery().WithTracingToDebug().ExecuteAsync();

        }

    }
}
