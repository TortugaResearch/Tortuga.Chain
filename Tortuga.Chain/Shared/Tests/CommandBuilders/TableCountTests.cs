using Tests.Models;

namespace Tests.CommandBuilders;

[TestClass]
public class TableCountTests : TestBase
{
#if POSTGRESQL || MYSQL

	[DataTestMethod, TableData(DataSourceGroup.Primary)]
	public void GetTableApproximateCount(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = dataSource.GetTableApproximateCount(tableName).Execute();
			Assert.IsTrue(count >= 0, $"The actual value was {count}");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void GetTableApproximateCount_Auto(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = dataSource.GetTableApproximateCount<Employee>().Execute();
			Assert.IsTrue(count >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TableData(DataSourceGroup.Primary)]
	public async Task GetTableApproximateCount_Async(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var count = await dataSource.GetTableApproximateCount(tableName).ExecuteAsync();
			Assert.IsTrue(count >= 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif
}
