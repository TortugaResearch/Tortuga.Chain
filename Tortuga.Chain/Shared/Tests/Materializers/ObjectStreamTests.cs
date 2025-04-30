using Tests.Models;

namespace Tests.Materializers;

[TestClass]
public class ToObjectStreamTests : TestBase
{
	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToObjectStream(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);

		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var empA1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
			dataSource.Insert(EmployeeTableName, empA1).ToObject<Employee>().Execute();

			var empA2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			dataSource.Insert(EmployeeTableName, empA2).ToObject<Employee>().Execute();

			var empB1 = new Employee() { FirstName = "B", LastName = "1", Title = uniqueKey };
			dataSource.Insert(EmployeeTableName, empB1).ToObject<Employee>().Execute();

			var empB2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			dataSource.Insert(EmployeeTableName, empB2).ToObject<Employee>().Execute();

			var empB3 = new Employee() { FirstName = "B", LastName = "3", Title = uniqueKey };
			dataSource.Insert(EmployeeTableName, empB3).ToObject<Employee>().Execute();

			using var objectStream = dataSource.From<Employee>(new { Title = uniqueKey }).ToObjectStream<Employee>().Execute();
			foreach (var item in objectStream)
			{
				Assert.AreEqual(uniqueKey, item.Title);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task ToObjectStreamAsync(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);

		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var empA1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
			dataSource.Insert(EmployeeTableName, empA1).ToObject<Employee>().Execute();

			var empA2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			dataSource.Insert(EmployeeTableName, empA2).ToObject<Employee>().Execute();

			var empB1 = new Employee() { FirstName = "B", LastName = "1", Title = uniqueKey };
			dataSource.Insert(EmployeeTableName, empB1).ToObject<Employee>().Execute();

			var empB2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			dataSource.Insert(EmployeeTableName, empB2).ToObject<Employee>().Execute();

			var empB3 = new Employee() { FirstName = "B", LastName = "3", Title = uniqueKey };
			dataSource.Insert(EmployeeTableName, empB3).ToObject<Employee>().Execute();

			await using var objectStream = await dataSource.From<Employee>(new { Title = uniqueKey }).ToObjectStream<Employee>().ExecuteAsync().ConfigureAwait(false);
			await foreach (var item in objectStream)
			{
				Assert.AreEqual(uniqueKey, item.Title);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}
}
