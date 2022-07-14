using Tests.Models;

namespace Tests.Aggregate;

[TestClass]
public class SimpleAggregateTests : TestBase
{
	const string Filter = "EmployeeKey < 100"; //So we don't overlfow on Sum/Avg

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void AsMax(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		//WriteLine($"Table {tableName}");
		try
		{
			var minValue = dataSource.From<Employee>().AsMax("EmployeeKey").ToInt32().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void AsMin(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		//WriteLine($"Table {tableName}");
		try
		{
			var minValue = dataSource.From<Employee>().AsMin("EmployeeKey").ToInt32().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void AsAverage(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		//WriteLine($"Table {tableName}");
		try
		{
			var minValue = dataSource.From<Employee>(Filter).AsAverage("EmployeeKey").ToInt32().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void AsSum(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		//WriteLine($"Table {tableName}");
		try
		{
			var minValue = dataSource.From<Employee>(Filter).AsSum("EmployeeKey").ToInt32().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

#if !ACCESS

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void AsSumTest_Distinct(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		//WriteLine($"Table {tableName}");
		try
		{
			var minValue = dataSource.From<Employee>(Filter).AsSum("EmployeeKey", true).ToInt32().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif
}
