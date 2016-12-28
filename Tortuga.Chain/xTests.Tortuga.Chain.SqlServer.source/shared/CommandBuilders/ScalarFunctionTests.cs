using Xunit;
using Xunit.Abstractions;

#if SQL_SERVER || OLE_SQL_SERVER

namespace Tests.CommandBuilders
{
    public class ScalarFunctionTests : TestBase
    {
        public static BasicData Prime = new BasicData(s_PrimaryDataSource);



        public ScalarFunctionTests(ITestOutputHelper output) : base(output)
        {
        }


#if SQL_SERVER || OLE_SQL_SERVER
        //Only SQL Server has scalar functions.

        [Theory, MemberData("Prime")]
        public void ScalarFunction1_Integer_WithNullParameter(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var parameter = new { ManagerKey = (int?)null };
                var rowCount = dataSource.From(EmployeeTableName).AsCount().Execute();
                var result = dataSource.ScalarFunction(ScalarFunction1Name, parameter).ToInt32().Execute();
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
                var parameter = new { ManagerKey = 1 };
                var rowCount = dataSource.From(EmployeeTableName, parameter).AsCount().Execute();
                var result = dataSource.ScalarFunction(ScalarFunction1Name, parameter).ToInt32().Execute();
                Assert.Equal(rowCount, result, "Expected result was the total number of rows with a manager key of 1");
            }
            finally
            {
                Release(dataSource);
            }
        }
#endif
    }
}

#endif

