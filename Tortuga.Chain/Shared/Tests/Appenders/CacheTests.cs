using Tests.Models;
using Tortuga.Chain;

namespace Tests.Appenders;

[TestClass]
public class CacheTests : TestBase
{
	[DataTestMethod, RootData(DataSourceGroup.All)]
	public void CacheTest(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).Execute();

			var employees = dataSource.From<Employee>(new { Title = lookupKey }).ToCollection().CacheAllItems(x => "CacheTest" + x.EmployeeKey).Execute();

			var employee1 = employees[0];
			var employee1a = dataSource.GetByKey<Employee>(employee1.EmployeeKey.Value).ToObject().ReadOrCache("CacheTest" + employee1.EmployeeKey).Execute();
			Assert.AreSame(employee1, employee1a);
		}
		finally
		{
			Release(dataSource);
		}
	}
}
