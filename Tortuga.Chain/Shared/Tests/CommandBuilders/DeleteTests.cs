using Tests.Models;
using Tortuga.Chain;

namespace Tests.CommandBuilders;

[TestClass]
public class DeleteTests : TestBase
{
	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Delete(string dataSourceName, DataSourceType mode)
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

			var key = dataSource.Insert(EmployeeTableName, original).ToInt32().Execute();
			var inserted = dataSource.GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();

			dataSource.Delete(EmployeeTableName, inserted).Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Delete_Attribute(string dataSourceName, DataSourceType mode)
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

			var key = dataSource.Insert(EmployeeTableName, original).ToInt32().Execute();
			var inserted = dataSource.GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();

			dataSource.Delete(inserted).Execute(); //reads the table name from the C# class

			try
			{
				dataSource.GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();
				Assert.Fail("Expected a missing data exception");
			}
			catch (MissingDataException) { }
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Delete_Implied(string dataSourceName, DataSourceType mode)
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

			var key = dataSource.Insert(EmployeeTableName, original).ToInt32().Execute();

			dataSource.Delete(new Models.HR.Employee() { EmployeeKey = key }).Execute();

			try
			{
				dataSource.GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();
				Assert.Fail("Expected a missing data exception");
			}
			catch (MissingDataException) { }
		}
		finally
		{
			Release(dataSource);
		}
	}


#if SQL_SERVER_SDS || SQL_SERVER_MDS || SQL_SERVER_OLEDB //SQL Server has problems with CRUD operations that return values on tables with triggers.

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Delete_Trigger(string dataSourceName, DataSourceType mode)
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

			var key = dataSource.Insert(EmployeeTableName_Trigger, original).ToInt32().Execute();
			var inserted = dataSource.GetByKey(EmployeeTableName_Trigger, key).ToObject<Employee>().Execute();

			dataSource.Delete(EmployeeTableName_Trigger, inserted).Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}
#endif
}
