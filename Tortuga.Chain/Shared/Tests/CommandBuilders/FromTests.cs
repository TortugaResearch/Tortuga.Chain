using System.Diagnostics;
using Tests.Models;
using Tortuga.Chain;

#if SQL_SERVER_MDS

using Tortuga.Chain.SqlServer;

#endif

namespace Tests.CommandBuilders;

[TestClass]
public class FromTests : TestBase
{
	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void WriteToTableReadFromView(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			//Get a random manager key
			var manager = dataSource.From<Employee>().WithLimits(1).ToObject<Employee>().Execute();

			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(new EmployeeWithManager() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null, ManagerKey = manager.EmployeeKey, EmployeeId = Guid.NewGuid().ToString() })
					.AsNonQuery().SetStrictMode(false).Execute();

			var values = dataSource.From<EmployeeWithManager>(new { Title = lookupKey }).ToCollection<EmployeeWithManager>().Execute();
			Assert.AreEqual(10, values.Count);

			foreach (var echo in values)
			{
				Assert.AreEqual(manager.EmployeeKey, echo.ManagerKey);
				Assert.AreEqual(manager.EmployeeKey, echo.Manager.EmployeeKey);
				Assert.AreEqual(manager.FirstName, echo.Manager.FirstName);
				Assert.AreEqual(manager.LastName, echo.Manager.LastName);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void FilterByObject(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var key = Guid.NewGuid().ToString();

			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

			var all = dataSource.From(EmployeeTableName, new { Title = key }).ToCollection<Employee>().Execute();
			var middleNameIsNull = dataSource.From(EmployeeTableName, new { Title = key, MiddleName = (string)null }).ToCollection<Employee>().Execute();
			var ignoreNulls = dataSource.From(EmployeeTableName, new { Title = key, MiddleName = (string)null }, FilterOptions.IgnoreNullProperties).ToCollection<Employee>().Execute();

			Assert.AreEqual(10, all.Count, "All of the rows");
			Assert.AreEqual(5, middleNameIsNull.Count, "Middle name is null");
			Assert.AreEqual(10, ignoreNulls.Count, "Ignore nulls should return all of the rows");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void FilterByObject_Compiled(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var key = Guid.NewGuid().ToString();

			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key, MiddleName = i % 2 == 0 ? "A" + i : null }).Compile().ToObject<Employee>().Execute();

			var all = dataSource.From(EmployeeTableName, new { Title = key }).Compile().ToCollection<Employee>().Execute();
			var middleNameIsNull = dataSource.From(EmployeeTableName, new { Title = key, MiddleName = (string)null }).Compile().ToCollection<Employee>().Execute();
			var ignoreNulls = dataSource.From(EmployeeTableName, new { Title = key, MiddleName = (string)null }, FilterOptions.IgnoreNullProperties).Compile().ToCollection<Employee>().Execute();

			Assert.AreEqual(10, all.Count, "All of the rows");
			Assert.AreEqual(5, middleNameIsNull.Count, "Middle name is null");
			Assert.AreEqual(10, ignoreNulls.Count, "Ignore nulls should return all of the rows");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void FilterByObject_Compiled_AutoTableSelection(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var key = Guid.NewGuid().ToString();

			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key, MiddleName = i % 2 == 0 ? "A" + i : null }).Compile().ToObject<Employee>().Execute();

			var all = dataSource.From<Employee>(new { Title = key }).Compile().ToCollection().Execute();
			var middleNameIsNull = dataSource.From<Employee>(new { Title = key, MiddleName = (string)null }).Compile().ToCollection().Execute();
			var ignoreNulls = dataSource.From<Employee>(new { Title = key, MiddleName = (string)null }, FilterOptions.IgnoreNullProperties).Compile().ToCollection().Execute();

			Assert.AreEqual(10, all.Count, "All of the rows");
			Assert.AreEqual(5, middleNameIsNull.Count, "Middle name is null");
			Assert.AreEqual(10, ignoreNulls.Count, "Ignore nulls should return all of the rows");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToImmutableArray(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "2", Title = uniqueKey };
			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			var emp3 = new Employee() { FirstName = "C", LastName = "1", Title = uniqueKey };
			var emp4 = new Employee() { FirstName = "D", LastName = "1", Title = uniqueKey };

			emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
			emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
			emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
			emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

			var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("FirstName").ToImmutableArray<Employee>().Execute();
			Assert.AreEqual(emp1.EmployeeKey, test1[0].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test1[1].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test1[2].EmployeeKey);
			Assert.AreEqual(emp4.EmployeeKey, test1[3].EmployeeKey);

			var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting(new SortExpression("FirstName", SortDirection.Descending)).ToImmutableArray<Employee>().Execute();
			Assert.AreEqual(emp4.EmployeeKey, test2[0].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test2[1].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test2[2].EmployeeKey);
			Assert.AreEqual(emp1.EmployeeKey, test2[3].EmployeeKey);

			var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("LastName", "FirstName").ToImmutableArray<Employee>().Execute();
			Assert.AreEqual(emp3.EmployeeKey, test3[0].EmployeeKey);
			Assert.AreEqual(emp4.EmployeeKey, test3[1].EmployeeKey);
			Assert.AreEqual(emp1.EmployeeKey, test3[2].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test3[3].EmployeeKey);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToImmutableList(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "2", Title = uniqueKey };
			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			var emp3 = new Employee() { FirstName = "C", LastName = "1", Title = uniqueKey };
			var emp4 = new Employee() { FirstName = "D", LastName = "1", Title = uniqueKey };

			emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
			emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
			emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
			emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

			var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("FirstName").ToImmutableList<Employee>().Execute();
			Assert.AreEqual(emp1.EmployeeKey, test1[0].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test1[1].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test1[2].EmployeeKey);
			Assert.AreEqual(emp4.EmployeeKey, test1[3].EmployeeKey);

			var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting(new SortExpression("FirstName", SortDirection.Descending)).ToImmutableList<Employee>().Execute();
			Assert.AreEqual(emp4.EmployeeKey, test2[0].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test2[1].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test2[2].EmployeeKey);
			Assert.AreEqual(emp1.EmployeeKey, test2[3].EmployeeKey);

			var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("LastName", "FirstName").ToImmutableList<Employee>().Execute();
			Assert.AreEqual(emp3.EmployeeKey, test3[0].EmployeeKey);
			Assert.AreEqual(emp4.EmployeeKey, test3[1].EmployeeKey);
			Assert.AreEqual(emp1.EmployeeKey, test3[2].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test3[3].EmployeeKey);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToImmutableList_NoDefaultConstructor(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);

		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
			dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();

			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();

			var lookup = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("EmployeeKey").ToImmutableList<EmployeeLookup>().Execute();

			Assert.AreEqual("A", lookup[0].FirstName, "First Name");
			Assert.AreEqual("1", lookup[0].LastName, "Last Name");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToImmutableArray_NoDefaultConstructor(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);

		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
			dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();

			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();

			var lookup = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("EmployeeKey").ToImmutableArray<EmployeeLookup>().Execute();

			Assert.AreEqual("A", lookup[0].FirstName, "First Name");
			Assert.AreEqual("1", lookup[0].LastName, "Last Name");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void FilterWithRecord(string dataSourceName, DataSourceType mode)
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

			var find2 = dataSource.From<EmployeeRecord>(new EmployeeRecordFilter { EmployeeId = emp2.EmployeeId }).ToObject().Execute();
			Assert.AreEqual(emp2.EmployeeKey, find2.EmployeeKey, "The wrong employee was returned");
		}
		finally
		{
			Release(dataSource);
		}
	}

#if SQL_SERVER_MDS

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

			Debug.WriteLine("Address Key " + record.AddressKey);

			var all = dataSource.From<Address>(new { record.AddressKey }).WithHistory().ToCollection().Execute();
			Assert.AreEqual(3, all.Count);

			var current = dataSource.From<Address>(new { record.AddressKey }).ToObject().Execute();
			Assert.AreEqual("CCC", current.AddressLine1);

			var old = dataSource.From<Address>(new { record.AddressKey }).WithHistory(time1).ToObject().Execute();
			Assert.AreEqual("AAA", old.AddressLine1);

			var fromTo = dataSource.From<Address>(new { record.AddressKey }).WithHistory(time1, time2, HistoryQueryMode.FromTo).ToCollection().Execute();
			Assert.AreEqual(2, fromTo.Count);

			var between = dataSource.From<Address>(new { record.AddressKey }).WithHistory(time1, time2, HistoryQueryMode.Between).ToCollection().Execute();
			Assert.AreEqual(2, between.Count);

			var contains = dataSource.From<Address>(new { record.AddressKey }).WithHistory(time1, time3, HistoryQueryMode.Contains).ToCollection().Execute();
			Assert.AreEqual(1, contains.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

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

			Debug.WriteLine("Address Key " + record.AddressKey);

			var all = dataSource.From(tableName, new { record.AddressKey }).WithHistory().ToCollection<Address>().Execute();
			Assert.AreEqual(3, all.Count);

			var current = dataSource.From(tableName, new { record.AddressKey }).ToObject<Address>().Execute();
			Assert.AreEqual("CCC", current.AddressLine1);

			var old = dataSource.From(tableName, new { record.AddressKey }).WithHistory(time1).ToObject<Address>().Execute();
			Assert.AreEqual("AAA", old.AddressLine1);

			var fromTo = dataSource.From(tableName, new { record.AddressKey }).WithHistory(time1, time2, HistoryQueryMode.FromTo).ToCollection<Address>().Execute();
			Assert.AreEqual(2, fromTo.Count);

			var between = dataSource.From(tableName, new { record.AddressKey }).WithHistory(time1, time2, HistoryQueryMode.Between).ToCollection<Address>().Execute();
			Assert.AreEqual(2, between.Count);

			var contains = dataSource.From(tableName, new { record.AddressKey }).WithHistory(time1, time3, HistoryQueryMode.Contains).ToCollection<Address>().Execute();
			Assert.AreEqual(1, contains.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif
}
