using Tests.Models;
using Tortuga.Chain;

namespace Tests.CommandBuilders;

[TestClass]
public class UpdateTests_Views : TestBase
{


	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void FailedUpdate_ViewNeedsKeys(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var original = new Employee()
			{
				FirstName = "Test",
				LastName = "Employee" + DateTime.Now.Ticks,
				Title = "Mail Room"
			};

			var inserted = dataSource.Insert(EmployeeTableName, original).ToObject<Employee>().Execute();

			inserted.FirstName = "Changed";
			inserted.Title = "Also Changed";

			try
			{
				dataSource.Update(EmployeeViewName, inserted).ToObject<ChangeTrackingEmployee>().Execute();
				Assert.Fail("Expected a mapping exception.");
			}
			catch (MappingException)
			{
				//OK
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

#if SQL_SERVER_MDS || SQL_SERVER_OLEDB

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void UpdateViaView(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var original = new Employee()
			{
				FirstName = "Test",
				LastName = "Employee" + DateTime.Now.Ticks,
				Title = "Mail Room"
			};

			var inserted = dataSource.Insert(EmployeeTableName, original).ToObject<ChangeTrackingEmployee>().Execute();

			inserted.FirstName = "Changed";
			inserted.Title = "Also Changed";

			var updated = dataSource.Update(EmployeeViewName, inserted).WithKeys("EmployeeKey").ToObject<ChangeTrackingEmployee>().Execute();
			Assert.AreEqual(inserted.FirstName, updated.FirstName, "FirstName should have changed");
			Assert.AreEqual(inserted.Title, updated.Title, "Title should have changed");
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif
}