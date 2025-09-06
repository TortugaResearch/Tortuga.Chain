namespace Tests.Appenders;

#if SQL_SERVER_MDS

using Microsoft.Data.SqlClient;
using Tortuga.Chain.SqlServer;


[TestClass]
public class SqlInfoMessageEventTests : TestBase
{
	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void SqlInfoMessageEventTests_InfoMessageEvents(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			string message = null;
			SqlInfoMessageEventHandler handler = (object sender, SqlInfoMessageEventArgs e) => { message = e.Message; };

			var result = dataSource.Procedure("HR.EmployeeList").ToTable().WithInfoMessageNotification(handler).Execute();

			Assert.AreEqual("Listing employees", message);
		}
		finally
		{
			Release(dataSource);
		}
	}
}

#endif
