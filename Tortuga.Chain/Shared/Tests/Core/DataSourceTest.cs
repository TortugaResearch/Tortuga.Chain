
namespace Tests.Core
{
	[TestClass]
	public class DataSourceTest : TestBase
	{
		[DataTestMethod, BasicData(DataSourceGroup.All)]
		public void TestConnection(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				dataSource.TestConnection();
			}
			finally
			{
				Release(dataSource);
			}
		}

		[DataTestMethod, BasicData(DataSourceGroup.All)]
		public async Task TestConnectionAsync(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				await dataSource.TestConnectionAsync();
			}
			finally
			{
				Release(dataSource);
			}
		}
	}
}
