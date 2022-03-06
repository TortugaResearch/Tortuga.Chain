using Tests.Models;

namespace Tests.CommandBuilders;

[TestClass]
public class GetByKeyTests : TestBase
{

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void GetByKey(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var emp1 = new Employee() { FirstName = "A", LastName = "1" };
			var emp2 = new Employee() { FirstName = "B", LastName = "2" };
			var emp3 = new Employee() { FirstName = "C", LastName = "3" };
			var emp4 = new Employee() { FirstName = "D", LastName = "4" };

			emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
			emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
			emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
			emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

			var find2 = dataSource.GetByKey<Employee>(emp2.EmployeeKey.Value).ToObject().Execute();
			Assert.AreEqual(emp2.EmployeeKey, find2.EmployeeKey, "The wrong employee was returned");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void GetByKeyWithRecord(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var emp1 = new EmployeeRecord() { FirstName = "A", LastName = "1" };
			var emp2 = new EmployeeRecord() { FirstName = "B", LastName = "2" };
			var emp3 = new EmployeeRecord() { FirstName = "C", LastName = "3" };
			var emp4 = new EmployeeRecord() { FirstName = "D", LastName = "4" };

			emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<EmployeeRecord>().Execute();
			emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<EmployeeRecord>().Execute();
			emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<EmployeeRecord>().Execute();
			emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<EmployeeRecord>().Execute();

			var find2 = dataSource.GetByKey<EmployeeRecord>(emp2.EmployeeKey.Value).ToObject().Execute();
			Assert.AreEqual(emp2.EmployeeKey, find2.EmployeeKey, "The wrong employee was returned");
		}
		finally
		{
			Release(dataSource);
		}
	}
	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void GetByKeyWithTableAndView(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var emp1 = new Employee() { FirstName = "A", LastName = "1" };
			var emp2 = new Employee() { FirstName = "B", LastName = "2" };
			var emp3 = new Employee() { FirstName = "C", LastName = "3" };
			var emp4 = new Employee() { FirstName = "D", LastName = "4" };

			emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
			emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
			emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
			emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

			var find2 = dataSource.GetByKey<EmployeeWithView>(emp2.EmployeeKey.Value).ToObject().Execute();
			Assert.AreEqual(emp2.EmployeeKey, find2.EmployeeKey, "The wrong employee was returned");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void GetByKeyList(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var emp1 = new Employee() { FirstName = "A", LastName = "1" };
			var emp2 = new Employee() { FirstName = "B", LastName = "2" };
			var emp3 = new Employee() { FirstName = "C", LastName = "3" };
			var emp4 = new Employee() { FirstName = "D", LastName = "4" };

			emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
			emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
			emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
			emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

			var find2 = dataSource.GetByKey(EmployeeTableName, emp2.EmployeeKey.Value).ToObject<Employee>().Execute();
			Assert.AreEqual(emp2.EmployeeKey, find2.EmployeeKey, "The wrong employee was returned");

			var list = dataSource.GetByKeyList(EmployeeTableName, new[] { emp2.EmployeeKey.Value, emp3.EmployeeKey.Value, emp4.EmployeeKey.Value }).ToCollection<Employee>().Execute();
			Assert.AreEqual(3, list.Count, "GetByKeyList returned the wrong number of rows");
			Assert.IsTrue(list.Any(e => e.EmployeeKey == emp2.EmployeeKey));
			Assert.IsTrue(list.Any(e => e.EmployeeKey == emp3.EmployeeKey));
			Assert.IsTrue(list.Any(e => e.EmployeeKey == emp4.EmployeeKey));
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void GetByKeyListWithTableAndView(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var emp1 = new Employee() { FirstName = "A", LastName = "1" };
			var emp2 = new Employee() { FirstName = "B", LastName = "2" };
			var emp3 = new Employee() { FirstName = "C", LastName = "3" };
			var emp4 = new Employee() { FirstName = "D", LastName = "4" };

			emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
			emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
			emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
			emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

			var find2 = dataSource.GetByKey(EmployeeTableName, emp2.EmployeeKey.Value).ToObject<Employee>().Execute();
			Assert.AreEqual(emp2.EmployeeKey, find2.EmployeeKey, "The wrong employee was returned");

			var list = dataSource.GetByKeyList(EmployeeTableName, new[] { emp2.EmployeeKey.Value, emp3.EmployeeKey.Value, emp4.EmployeeKey.Value }).ToCollection<EmployeeWithView>().Execute();
			Assert.AreEqual(3, list.Count, "GetByKeyList returned the wrong number of rows");
			Assert.IsTrue(list.Any(e => e.EmployeeKey == emp2.EmployeeKey));
			Assert.IsTrue(list.Any(e => e.EmployeeKey == emp3.EmployeeKey));
			Assert.IsTrue(list.Any(e => e.EmployeeKey == emp4.EmployeeKey));
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void GetByKeyListWithRecord(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var emp1 = new Employee() { FirstName = "A", LastName = "1" };
			var emp2 = new Employee() { FirstName = "B", LastName = "2" };
			var emp3 = new Employee() { FirstName = "C", LastName = "3" };
			var emp4 = new Employee() { FirstName = "D", LastName = "4" };

			emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
			emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
			emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
			emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

			var find2 = dataSource.GetByKey(EmployeeTableName, emp2.EmployeeKey.Value).ToObject<Employee>().Execute();
			Assert.AreEqual(emp2.EmployeeKey, find2.EmployeeKey, "The wrong employee was returned");

			var list = dataSource.GetByKeyList(EmployeeTableName, new[] { emp2.EmployeeKey.Value, emp3.EmployeeKey.Value, emp4.EmployeeKey.Value }).ToCollection<EmployeeRecord>().Execute();
			Assert.AreEqual(3, list.Count, "GetByKey returned the wrong number of rows");
			Assert.IsTrue(list.Any(e => e.EmployeeKey == emp2.EmployeeKey));
			Assert.IsTrue(list.Any(e => e.EmployeeKey == emp3.EmployeeKey));
			Assert.IsTrue(list.Any(e => e.EmployeeKey == emp4.EmployeeKey));
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void GetByKeyList_InferredTableName(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var emp1 = new Employee() { FirstName = "A", LastName = "1" };
			var emp2 = new Employee() { FirstName = "B", LastName = "2" };
			var emp3 = new Employee() { FirstName = "C", LastName = "3" };
			var emp4 = new Employee() { FirstName = "D", LastName = "4" };

			emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
			emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
			emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
			emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

			var find2 = dataSource.GetByKey<Employee>(emp2.EmployeeKey.Value).ToObject().Execute();
			Assert.AreEqual(emp2.EmployeeKey, find2.EmployeeKey, "The wrong employee was returned");

			var list = dataSource.GetByKeyList<Employee>(new[] { emp2.EmployeeKey.Value, emp3.EmployeeKey.Value, emp4.EmployeeKey.Value }).ToCollection().Execute();
			Assert.AreEqual(3, list.Count, "GetByKey returned the wrong number of rows");
			Assert.IsTrue(list.Any(e => e.EmployeeKey == emp2.EmployeeKey));
			Assert.IsTrue(list.Any(e => e.EmployeeKey == emp3.EmployeeKey));
			Assert.IsTrue(list.Any(e => e.EmployeeKey == emp4.EmployeeKey));
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void GetByKeyList_2(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = Guid.NewGuid().ToString() };
			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = Guid.NewGuid().ToString() };
			var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = Guid.NewGuid().ToString() };
			var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = Guid.NewGuid().ToString() };

			emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
			emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
			emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
			emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

			//Pretend that Title is a unique column.

			var list = dataSource.GetByKeyList(EmployeeTableName, "Title", new[] { emp2.Title, emp3.Title, emp4.Title }).ToCollection<Employee>().Execute();
			Assert.AreEqual(3, list.Count, "GetByKey returned the wrong number of rows");
			Assert.IsTrue(list.Any(e => e.EmployeeKey == emp2.EmployeeKey));
			Assert.IsTrue(list.Any(e => e.EmployeeKey == emp3.EmployeeKey));
			Assert.IsTrue(list.Any(e => e.EmployeeKey == emp4.EmployeeKey));
		}
		finally
		{
			Release(dataSource);
		}
	}
}
