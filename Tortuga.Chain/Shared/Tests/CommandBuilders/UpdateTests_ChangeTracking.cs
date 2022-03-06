using Tests.Models;
using Tortuga.Chain;

namespace Tests.CommandBuilders;

[TestClass]
public class UpdateTests_ChangeTracking : TestBase
{


	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ChangeTrackingTest(string dataSourceName, DataSourceType mode)
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

			var inserted = dataSource.Insert(EmployeeTableName, original).ToObject<ChangeTrackingEmployee>().Execute();
			Assert.IsFalse(inserted.IsChanged, "Accept changes wasn't called by the materializer");

			inserted.FirstName = "Changed";
			inserted.AcceptChanges(); //only properties changed AFTER this line should actually be updated in the database
			inserted.Title = "Also Changed";

			var updated = dataSource.Update(EmployeeTableName, inserted, UpdateOptions.ChangedPropertiesOnly).ToObject<ChangeTrackingEmployee>().Execute();
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
	public void ChangeTrackingTest_NothingChanged(string dataSourceName, DataSourceType mode)
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

			var inserted = dataSource.Insert(EmployeeTableName, original).ToObject<ChangeTrackingEmployee>().Execute();
			Assert.IsFalse(inserted.IsChanged, "Accept changes wasn't called by the materializer");

			try
			{
				var updated = dataSource.Update(EmployeeTableName, inserted, UpdateOptions.ChangedPropertiesOnly).ToObject<ChangeTrackingEmployee>().Execute();
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

#if SQL_SERVER_SDS || SQL_SERVER_MDS || SQL_SERVER_OLEDB //SQL Server has problems with CRUD operations that return values on tables with triggers.

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ChangeTrackingTest_Trigger(string dataSourceName, DataSourceType mode)
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

			var inserted = dataSource.Insert(EmployeeTableName_Trigger, original).ToObject<ChangeTrackingEmployee>().Execute();
			Assert.IsFalse(inserted.IsChanged, "Accept changes wasn't called by the materializer");

			inserted.FirstName = "Changed";
			inserted.AcceptChanges(); //only properties changed AFTER this line should actually be updated in the database
			inserted.Title = "Also Changed";

			var updated = dataSource.Update(EmployeeTableName_Trigger, inserted, UpdateOptions.ChangedPropertiesOnly).ToObject<ChangeTrackingEmployee>().Execute();
			Assert.AreEqual(original.FirstName, updated.FirstName, "FirstName shouldn't have changed");
			Assert.AreEqual(original.LastName, updated.LastName, "LastName shouldn't have changed");
			Assert.AreEqual(inserted.Title, updated.Title, "Title should have changed");
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif
}