using Tests.Models;
using Tortuga.Chain;

namespace Tests.CommandBuilders;

[TestClass]
public class UpdateTests_Compiled : TestBase
{
#if !SQLite

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ChangeTrackingTest_Compiled(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var original = new ChangeTrackingEmployee()
			{
				FirstName = "Test",
				LastName = "Employee" + DateTime.Now.Ticks,
				Title = "Mail Room",
				EmployeeId = Guid.NewGuid().ToString()
			};

			var inserted = dataSource.Insert(EmployeeTableName, original).Compile().ToObject<ChangeTrackingEmployee>().Execute();
			inserted.FirstName = "Changed";
			inserted.AcceptChanges(); //only properties changed AFTER this line should actually be updated in the database
			inserted.Title = "Also Changed";

			var updated = dataSource.Update(EmployeeTableName, inserted, UpdateOptions.ChangedPropertiesOnly).Compile().ToObject<ChangeTrackingEmployee>().Execute();
			Assert.AreEqual(original.FirstName, updated.FirstName, "FirstName shouldn't have changed");
			Assert.AreEqual(original.LastName, updated.LastName, "LastName shouldn't have changed");
			Assert.AreEqual(inserted.Title, updated.Title, "Title should have changed");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ChangeTrackingTest_NothingChanged_Compiled(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var original = new ChangeTrackingEmployee()
			{
				FirstName = "Test",
				LastName = "Employee" + DateTime.Now.Ticks,
				Title = "Mail Room",
				EmployeeId = Guid.NewGuid().ToString()
			};

			var inserted = dataSource.Insert(EmployeeTableName, original).Compile().ToObject<ChangeTrackingEmployee>().Execute();

			try
			{
				var updated = dataSource.Update(EmployeeTableName, inserted, UpdateOptions.ChangedPropertiesOnly).Compile().ToObject<ChangeTrackingEmployee>().Execute();
				Assert.Fail("Exception Expected");
			}
			catch (ArgumentException)
			{
				//pass
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

}