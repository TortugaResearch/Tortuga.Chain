using Tortuga.Chain.SQLite;

namespace Tests.Core
{
	[TestClass]
	public class SQLiteDataSourceTest : TestBase
	{
		[DataTestMethod, RootData(DataSourceGroup.Primary)]
		public void FK_Disabled(string dataSourceName)
		{
			var dataSource = DataSource(dataSourceName).WithSettings(new SQLiteDataSourceSettings() { EnforceForeignKeys = false });
			try
			{
				dataSource.TestConnection();
			}
			finally
			{
				Release(dataSource);
			}
		}

		[DataTestMethod, RootData(DataSourceGroup.Primary)]
		public void FK_Enabled(string dataSourceName)
		{
			var dataSource = DataSource(dataSourceName).WithSettings(new SQLiteDataSourceSettings() { EnforceForeignKeys = true });
			try
			{
				dataSource.TestConnection();
			}
			finally
			{
				Release(dataSource);
			}
		}
	}
}
