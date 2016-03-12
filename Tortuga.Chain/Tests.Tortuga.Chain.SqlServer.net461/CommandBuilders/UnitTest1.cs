using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var result = DataSource.Sql(sql).AsInt32().Execute();
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void SqlServerSqlCallTests_Sql_2()
        {
            var sql = "SELECT @Input";
            var result = DataSource.Sql(sql, new { @Input = 5 }).AsInt32().WithTracingToDebug().Execute();
            Assert.AreEqual(5, result);
        }


        [TestMethod]
        public void SqlServerSqlCallTests_Sql_2nq()
        {
            var sql = "SELECT @Input";
            DataSource.Sql(sql, new { @Input = 5 }).AsNonQuery().WithTracingToDebug().Execute();
        }
    }
}
