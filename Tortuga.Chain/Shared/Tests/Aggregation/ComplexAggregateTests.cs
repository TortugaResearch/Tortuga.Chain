using Tests.Models;
using Tortuga.Chain.Aggregates;

namespace Tests.Aggregate;

[TestClass]
public class ComplexAggregateTests : TestBase
{
	const string Filter = "EmployeeKey < 100"; //So we don't overlfow on Sum/Avg

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void MinMaxAvg(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		//WriteLine($"Table {tableName}");
		try
		{
			var result = dataSource.From<Employee>(Filter).AsAggregate(
				new AggregateColumn(AggregateType.Min, "EmployeeKey", "MinEmployeeKey"),
				new AggregateColumn(AggregateType.Max, "EmployeeKey", "MaxEmployeeKey"),
				new AggregateColumn(AggregateType.Count, "EmployeeKey", "CountEmployeeKey")
				).ToRow().Execute();

			Assert.IsTrue(result.ContainsKey("MinEmployeeKey"));
			Assert.IsTrue(result.ContainsKey("MaxEmployeeKey"));
			Assert.IsTrue(result.ContainsKey("CountEmployeeKey"));
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void MinMaxAvg_WithGroup(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		//WriteLine($"Table {tableName}");
		try
		{
			var result = dataSource.From<Employee>(Filter).AsAggregate(
				new AggregateColumn(AggregateType.Min, "EmployeeKey", "MinEmployeeKey"),
				new AggregateColumn(AggregateType.Max, "EmployeeKey", "MaxEmployeeKey"),
				new AggregateColumn(AggregateType.Count, "EmployeeKey", "CountEmployeeKey"),
				new AggregateColumn("Gender")
				).ToTable().Execute();

			Assert.IsTrue(result.ColumnNames.Contains("MinEmployeeKey"));
			Assert.IsTrue(result.ColumnNames.Contains("MaxEmployeeKey"));
			Assert.IsTrue(result.ColumnNames.Contains("CountEmployeeKey"));
			Assert.IsTrue(result.ColumnNames.Contains("Gender"));
		}
		finally
		{
			Release(dataSource);
		}
	}
}
