using Tests.Models;

namespace Tests.Materializers;

[TestClass]
public class ToCollectionTests : TestBase
{
	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToCollection_NoDefaultConstructor(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);

		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
			dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();

			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();

			var lookup = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("EmployeeKey").ToCollection<EmployeeLookup>().Execute();

			Assert.AreEqual("A", lookup[0].FirstName, "First Name");
			Assert.AreEqual("1", lookup[0].LastName, "Last Name");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToMasterDetailCollection(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);

		try
		{
			var managerUniqueKey = Guid.NewGuid().ToString();
			var uniqueKey = Guid.NewGuid().ToString();

			var managerA = new Employee() { FirstName = "A", LastName = "1", Title = managerUniqueKey };

#if SQLITE
			var managerAKey = dataSource.Insert(EmployeeTableName, managerA).ToInt64().Execute();
#elif MYSQL
			var managerAKey = dataSource.Insert(EmployeeTableName, managerA).ToUInt64().Execute();
#else
			var managerAKey = dataSource.Insert(EmployeeTableName, managerA).ToInt32().Execute();
#endif

			var empA1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey, ManagerKey = managerAKey };
			dataSource.Insert(EmployeeTableName, empA1).ToObject<Employee>().Execute();

			var empA2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey, ManagerKey = managerAKey };
			dataSource.Insert(EmployeeTableName, empA2).ToObject<Employee>().Execute();

			var managerB = new Employee() { FirstName = "B", LastName = "1", Title = managerUniqueKey };

#if SQLITE
			var managerBKey = dataSource.Insert(EmployeeTableName, managerB).ToInt64().Execute();
#elif MYSQL
			var managerBKey = dataSource.Insert(EmployeeTableName, managerB).ToUInt64().Execute();
#else
			var managerBKey = dataSource.Insert(EmployeeTableName, managerB).ToInt32().Execute();
#endif

			var empB1 = new Employee() { FirstName = "B", LastName = "1", Title = uniqueKey, ManagerKey = managerBKey };
			dataSource.Insert(EmployeeTableName, empB1).ToObject<Employee>().Execute();

			var empB2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey, ManagerKey = managerBKey };
			dataSource.Insert(EmployeeTableName, empB2).ToObject<Employee>().Execute();

			var empB3 = new Employee() { FirstName = "B", LastName = "3", Title = uniqueKey, ManagerKey = managerBKey };
			dataSource.Insert(EmployeeTableName, empB3).ToObject<Employee>().Execute();

			var lookup = dataSource.From<ManagerWithEmployees>(new { ManagerTitle = managerUniqueKey }).ToMasterDetailCollection("ManagerEmployeeKey", m => m.DirectReports).Execute();

			Assert.AreEqual(2, lookup.Count);
			var lookupA = lookup.Single(m => m.ManagerEmployeeKey == managerAKey);
			var lookupB = lookup.Single(m => m.ManagerEmployeeKey == managerBKey);

			Assert.AreEqual(2, lookupA.DirectReports.Count);
			Assert.AreEqual(3, lookupB.DirectReports.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}
}
