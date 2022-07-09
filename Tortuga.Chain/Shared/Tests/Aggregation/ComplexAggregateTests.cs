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

		try
		{
			//PostgreSQL is case sensitive, so we need to ensure we're using the correct name.
			var table = dataSource.DatabaseMetadata.GetTableOrViewFromClass<Employee>();
			var columnName = table.Columns["EmployeeKey"].SqlName;

			var result = dataSource.From<Employee>(Filter).AsAggregate(
				new AggregateColumn(AggregateType.Min, columnName, "MinEmployeeKey"),
				new AggregateColumn(AggregateType.Max, columnName, "MaxEmployeeKey"),
				new AggregateColumn(AggregateType.Count, columnName, "CountEmployeeKey")
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
		try
		{
			//PostgreSQL is case sensitive, so we need to ensure we're using the correct name.
			var table = dataSource.DatabaseMetadata.GetTableOrViewFromClass<Employee>();
			var ekColumnName = table.Columns["EmployeeKey"].SqlName;
			var gColumnName = table.Columns["Gender"].SqlName;

			var result = dataSource.From<Employee>(Filter).AsAggregate(
				new AggregateColumn(AggregateType.Min, ekColumnName, "MinEmployeeKey"),
				new AggregateColumn(AggregateType.Max, ekColumnName, "MaxEmployeeKey"),
				new AggregateColumn(AggregateType.Count, ekColumnName, "CountEmployeeKey"),
				new GroupByColumn(gColumnName, "Gender"),
				new CustomAggregateColumn($"Max({ekColumnName}) - Min({ekColumnName})", "Range")
				).ToTable().Execute();

			Assert.IsTrue(result.ColumnNames.Contains("MinEmployeeKey"));
			Assert.IsTrue(result.ColumnNames.Contains("MaxEmployeeKey"));
			Assert.IsTrue(result.ColumnNames.Contains("CountEmployeeKey"));
			Assert.IsTrue(result.ColumnNames.Contains("Gender"));
			Assert.IsTrue(result.ColumnNames.Contains("Range"));
		}
		finally
		{
			Release(dataSource);
		}
	}
}
