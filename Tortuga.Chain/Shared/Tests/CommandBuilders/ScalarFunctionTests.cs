#if SQL_SERVER_SDS || SQL_SERVER_MDS || SQL_SERVER_OLEDB || POSTGRESQL || MYSQL

namespace Tests.CommandBuilders;

[TestClass]
public class ScalarFunctionTests : TestBase
{
#if SQL_SERVER_SDS || SQL_SERVER_MDS || SQL_SERVER_OLEDB
	static object Filter_Integer_WithNullParameter = new { ManagerKey = (int?)null };
	static object Filter_Integer = new { ManagerKey = 1 };
#elif MYSQL
	static object Filter_Integer_WithNullParameter = new { p_managerKey = (int?)null };
	static object Filter_Integer = new { p_managerKey = 1 };
#elif POSTGRESQL
	static object Filter_Integer_WithNullParameter = new { p_managerKey = (int?)null };
	static object Filter_Integer = new { p_managerKey = 1 };
#endif

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ScalarFunction1_Integer_WithNullParameter(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var rowCount = dataSource.From(EmployeeTableName).AsCount().Execute();
			var result = dataSource.ScalarFunction(ScalarFunction1Name, Filter_Integer_WithNullParameter).ToInt32().Execute();
			Assert.AreEqual(rowCount, result, "Expected result was the total number of rows");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ScalarFunction1_Integer(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var rowCount = dataSource.From(EmployeeTableName, new { ManagerKey = 1 }).AsCount().Execute();
			var result = dataSource.ScalarFunction(ScalarFunction1Name, Filter_Integer).ToInt32().Execute();
			Assert.AreEqual(rowCount, result, "Expected result was the total number of rows with a manager key of 1");
		}
		finally
		{
			Release(dataSource);
		}
	}
}

#endif
