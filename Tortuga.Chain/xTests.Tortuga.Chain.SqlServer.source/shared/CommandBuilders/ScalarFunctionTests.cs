using Xunit;
using Xunit.Abstractions;

#if SQL_SERVER || OLE_SQL_SERVER || POSTGRESQL

namespace Tests.CommandBuilders
{
    public class ScalarFunctionTests : TestBase
    {
        public static BasicData Prime = new BasicData(s_PrimaryDataSource);

#if SQL_SERVER || OLE_SQL_SERVER
        static object Filter_Integer_WithNullParameter = new { ManagerKey = (int?)null };
        static object Filter_Integer = new { ManagerKey = 1 };
#elif POSTGRESQL
        static object Filter_Integer_WithNullParameter = new { p_managerKey = (int?)null };
        static object Filter_Integer = new { p_managerKey = 1 };
#endif


        public ScalarFunctionTests(ITestOutputHelper output) : base(output)
        {
        }


        [Theory, MemberData("Prime")]
        public void ScalarFunction1_Integer_WithNullParameter(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var rowCount = dataSource.From(EmployeeTableName).AsCount().Execute();
                var result = dataSource.ScalarFunction(ScalarFunction1Name, Filter_Integer_WithNullParameter).ToInt32().Execute();
                Assert.Equal(rowCount, result, "Expected result was the total number of rows");
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public void ScalarFunction1_Integer(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var rowCount = dataSource.From(EmployeeTableName, new { ManagerKey = 1 }).AsCount().Execute();
                var result = dataSource.ScalarFunction(ScalarFunction1Name, Filter_Integer).ToInt32().Execute();
                Assert.Equal(rowCount, result, "Expected result was the total number of rows with a manager key of 1");
            }
            finally
            {
                Release(dataSource);
            }
        }
    }
}

#endif

