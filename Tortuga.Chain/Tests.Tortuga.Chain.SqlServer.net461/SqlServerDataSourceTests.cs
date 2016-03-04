using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tortuga.Chain;

namespace Tests
{
    [TestClass]
    public class SqlServerDataSourceTests
    {
        [TestMethod]
        public void SqlServerDataSourceTests_Ctr()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerTestDatabase"].ConnectionString;
            var dataSource = new SqlServerDataSource(connectionString);
            dataSource.TestConnection();
        }
    }
}
