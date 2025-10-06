using Tests.Models;
using Tortuga.Chain;
using Tortuga.Chain.SqlServer;

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
	[Obsolete]
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

			var list = dataSource.GetByKeyList<EmployeeWithView>(new[] { emp2.EmployeeKey.Value, emp3.EmployeeKey.Value, emp4.EmployeeKey.Value }).ToCollection().Execute();
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

#if SQL_SERVER_MDS

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void HistoricalTests(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			var tableName = "dbo.Address";

			var record = dataSource.Insert(new Address() { AddressLine1 = "AAA" }).ToObject<Address>().Execute();
			Assert.AreNotEqual(0, record.AddressKey);
			Thread.Sleep(TimeSpan.FromSeconds(1));
			var time1 = DateTime.UtcNow;

			Thread.Sleep(TimeSpan.FromSeconds(1));
			record.AddressLine1 = "BBB";
			dataSource.Update(record).Execute();
			Thread.Sleep(TimeSpan.FromSeconds(1));
			var time2 = DateTime.UtcNow;

			Thread.Sleep(TimeSpan.FromSeconds(1));
			record.AddressLine1 = "CCC";
			dataSource.Update(record).Execute();
			Thread.Sleep(TimeSpan.FromSeconds(1));
			var time3 = DateTime.UtcNow;

			var all_deleted = dataSource.GetByKey(tableName, record.AddressKey).WithDeletedRecords().WithHistory().ToCollection<Address>().Execute();
			Assert.AreEqual(3, all_deleted.Count);

			var all = dataSource.GetByKey(tableName, record.AddressKey).WithHistory().ToCollection<Address>().Execute();
			Assert.AreEqual(3, all.Count);

			var current = dataSource.GetByKey(tableName, record.AddressKey).ToObject<Address>().Execute();
			Assert.AreEqual("CCC", current.AddressLine1);

			var old = dataSource.GetByKey(tableName, record.AddressKey).WithHistory(time1).ToObject<Address>().Execute();
			Assert.AreEqual("AAA", old.AddressLine1);

			var fromTo = dataSource.GetByKey(tableName, record.AddressKey).WithHistory(time1, time2, HistoryQueryMode.FromTo).ToCollection<Address>().Execute();
			Assert.AreEqual(2, fromTo.Count);

			var between = dataSource.GetByKey(tableName, record.AddressKey).WithHistory(time1, time2, HistoryQueryMode.Between).ToCollection<Address>().Execute();
			Assert.AreEqual(2, between.Count);

			var contains = dataSource.GetByKey(tableName, record.AddressKey).WithHistory(time1, time3, HistoryQueryMode.Contains).ToCollection<Address>().Execute();
			Assert.AreEqual(1, contains.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void HistoricalTests_ByObject(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			var record = dataSource.Insert(new Address() { AddressLine1 = "AAA" }).ToObject<Address>().Execute();
			Assert.AreNotEqual(0, record.AddressKey);
			Thread.Sleep(TimeSpan.FromSeconds(1));
			var time1 = DateTime.UtcNow;

			Thread.Sleep(TimeSpan.FromSeconds(1));
			record.AddressLine1 = "BBB";
			dataSource.Update(record).Execute();
			Thread.Sleep(TimeSpan.FromSeconds(1));
			var time2 = DateTime.UtcNow;

			Thread.Sleep(TimeSpan.FromSeconds(1));
			record.AddressLine1 = "CCC";
			dataSource.Update(record).Execute();
			Thread.Sleep(TimeSpan.FromSeconds(1));
			var time3 = DateTime.UtcNow;

			var all = dataSource.GetByKey<Address>(record.AddressKey).WithHistory().ToCollection().Execute();
			Assert.AreEqual(3, all.Count);

			var current = dataSource.GetByKey<Address>(record.AddressKey).ToObject().Execute();
			Assert.AreEqual("CCC", current.AddressLine1);

			var old = dataSource.GetByKey<Address>(record.AddressKey).WithHistory(time1).ToObject().Execute();
			Assert.AreEqual("AAA", old.AddressLine1);

			var fromTo = dataSource.GetByKey<Address>(record.AddressKey).WithHistory(time1, time2, HistoryQueryMode.FromTo).ToCollection().Execute();
			Assert.AreEqual(2, fromTo.Count);

			var between = dataSource.GetByKey<Address>(record.AddressKey).WithHistory(time1, time2, HistoryQueryMode.Between).ToCollection().Execute();
			Assert.AreEqual(2, between.Count);

			var contains = dataSource.GetByKey<Address>(record.AddressKey).WithHistory(time1, time3, HistoryQueryMode.Contains).ToCollection().Execute();
			Assert.AreEqual(1, contains.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif
}
