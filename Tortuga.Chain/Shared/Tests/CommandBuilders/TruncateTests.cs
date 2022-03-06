#if CLASS_2
using Tests.Models.Sales;

namespace Tests.CommandBuilders;

[TestClass]
public class TruncateTests : TestBase
{


	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Truncate(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{

			dataSource.Insert(new Location() { LocationName = "Example " + DateTime.Now.Ticks }).Execute();
			Assert.IsTrue(dataSource.From<Location>().AsCount().Execute() > 0, "Expected at least one row");
			dataSource.Truncate<Location>().Execute();
			Assert.IsTrue(dataSource.From<Location>().AsCount().Execute() == 0, "Expected zero rows");

		}
		finally
		{
			Release(dataSource);
		}
	}
}

#endif