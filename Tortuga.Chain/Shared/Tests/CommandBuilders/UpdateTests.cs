using Tests.Models;
using Tortuga.Chain;

namespace Tests.CommandBuilders;

[TestClass]
public class UpdateTests : TestBase
{

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void FailedUpdateTest(string dataSourceName, DataSourceType mode)
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
			inserted.Title = "President";

			dataSource.Update(EmployeeTableName, inserted).Execute();
			dataSource.Delete(EmployeeTableName, inserted).Execute();

			try
			{
				dataSource.Update(EmployeeTableName, inserted).Execute();
				Assert.Fail("Expected a MissingDataException when trying to update a deleted row but didn't get one.");
			}
			catch (MissingDataException)
			{
				//pass
			}

			dataSource.Update(EmployeeTableName, inserted, UpdateOptions.IgnoreRowsAffected).Execute(); //no error
		}
		finally
		{
			Release(dataSource);
		}
	}






}
