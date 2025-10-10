using Tests.Models;
using Tortuga.Chain;

namespace Tests.Appenders;

[TestClass]
public class TagTests : TestBase
{
	[DataTestMethod, RootData(DataSourceGroup.All)]
	public void TagTest(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			dataSource.From<Employee>().AsCount().Tag("This is my message").Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.All)]
	public void TagSourceTest(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			dataSource.From<Employee>().AsCount().Tag().Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}
}
