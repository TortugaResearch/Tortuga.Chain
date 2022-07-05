using Tests.Models;

namespace Tests.CommandBuilders;

[TestClass]
public class GetByColumnTests : TestBase
{
	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void GetByColumn(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var reference = Guid.NewGuid().ToString();
			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = reference };
			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = reference };
			var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = reference };
			var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = reference };

			emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
			emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
			emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
			emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

			var find2 = dataSource.GetByColumn<Employee>("Title", reference).ToCollection().Execute();
			Assert.AreEqual(4, find2.Count, "The wrong number of employees were returned");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void GetByColumnList(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var reference1 = Guid.NewGuid().ToString();
			var reference2 = Guid.NewGuid().ToString();
			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = reference1 };
			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = reference1 };
			var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = reference1 };
			var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = reference2 };

			emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
			emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
			emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
			emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

			var find2 = dataSource.GetByColumnList<Employee>("Title", new[] { reference1, reference2 }).ToCollection().Execute();
			Assert.AreEqual(4, find2.Count, "The wrong number of employees were returned");
		}
		finally
		{
			Release(dataSource);
		}
	}
}
