using Tests.Models;

namespace Tests.CommandBuilders;

[TestClass]
public class FromTests_ToDynamic : TestBase
{
	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void ToDynamicCollection(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = dataSource.From(tableName).WithLimits(10).WithSorting(table.GetDefaultSortOrder()).ToDynamicCollection().Execute();
			Assert.IsTrue(result.Count <= 10);
			if (result.Count > 0)
			{
				var first = (IDictionary<string, object>)result.First();
				Assert.AreEqual(table.Columns.Count, first.Count);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public async Task ToDynamicCollection_Async(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode).ConfigureAwait(false);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = await dataSource.From(tableName).WithLimits(10).WithSorting(table.GetDefaultSortOrder()).ToDynamicCollection().ExecuteAsync().ConfigureAwait(false);
			Assert.IsTrue(result.Count <= 10);
			if (result.Count > 0)
			{
				var row = (IDictionary<string, object>)result.First();
				Assert.AreEqual(table.Columns.Count, row.Count);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

#if POSTGRESQL

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToDynamicCollection_IncludeColumns(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = uniqueKey };
			var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = uniqueKey };

			emp1 = dataSource.Insert(emp1).ToObject().Execute();
			emp2 = dataSource.Insert(emp2).ToObject().Execute();
			emp3 = dataSource.Insert(emp3).ToObject().Execute();
			emp4 = dataSource.Insert(emp4).ToObject().Execute();

			var list = dataSource.From<Employee>(new { Title = uniqueKey }).ToDynamicCollection().WithProperties("First_Name", "Last_Name", "Title").Execute();

			Assert.AreEqual(4, list.Count);
			var test1 = list.Single(x => x.first_name == "A");

			Assert.AreEqual("1", test1.last_name);
			Assert.AreEqual(uniqueKey, test1.title);

			var test1Internal = (IDictionary<string, object>)test1;
			Assert.IsTrue(!test1Internal.ContainsKey("employee_id"));
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToDynamicObject_ExcludeColumns(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };

			emp1 = dataSource.Insert(emp1).ToObject().Execute();

			var test1 = dataSource.From<Employee>(new { Title = uniqueKey }).ToDynamicObject().ExceptProperties("employee_id").Execute();

			Assert.AreEqual("A", test1.first_name);
			Assert.AreEqual("1", test1.last_name);
			Assert.AreEqual(uniqueKey, test1.title);

			var test1Internal = (IDictionary<string, object>)test1;
			Assert.IsTrue(!test1Internal.ContainsKey("employee_id"));
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToDynamicObject_IncludeColumns(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };

			emp1 = dataSource.Insert(emp1).ToObject().Execute();

			var test1 = dataSource.From<Employee>(new { Title = uniqueKey }).ToDynamicObject().WithProperties("FirstName", "LastName", "Title").Execute();

			Assert.AreEqual("A", test1.firstname);
			Assert.AreEqual("1", test1.lastname);
			Assert.AreEqual(uniqueKey, test1.title);

			var test1Internal = (IDictionary<string, object>)test1;
			Assert.IsTrue(!test1Internal.ContainsKey("employeeid"));
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToDynamicObjectOrNull_IncludeColumns(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };

			emp1 = dataSource.Insert(emp1).ToObject().Execute();

			var test1 = dataSource.From<Employee>(new { Title = uniqueKey }).ToDynamicObjectOrNull().WithProperties("FirstName", "LastName", "Title").Execute();

			Assert.IsNotNull(test1);

			Assert.AreEqual("A", test1.firstname);
			Assert.AreEqual("1", test1.lastname);
			Assert.AreEqual(uniqueKey, test1.title);

			var test1Internal = (IDictionary<string, object>)test1;
			Assert.IsTrue(!test1Internal.ContainsKey("employeeid"));
		}
		finally
		{
			Release(dataSource);
		}
	}

#else

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToDynamicCollection_IncludeColumns(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = uniqueKey };
			var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = uniqueKey };

			emp1 = dataSource.Insert(emp1).ToObject().Execute();
			emp2 = dataSource.Insert(emp2).ToObject().Execute();
			emp3 = dataSource.Insert(emp3).ToObject().Execute();
			emp4 = dataSource.Insert(emp4).ToObject().Execute();

			var list = dataSource.From<Employee>(new { Title = uniqueKey }).ToDynamicCollection().WithProperties("FirstName", "LastName", "Title").Execute();

			Assert.AreEqual(4, list.Count);
			var test1 = list.Single(x => x.FirstName == "A");

			Assert.AreEqual("1", test1.LastName);
			Assert.AreEqual(uniqueKey, test1.Title);

			var test1Internal = (IDictionary<string, object>)test1;
			Assert.IsTrue(!test1Internal.ContainsKey("EmployeeId"));
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToDynamicObject_ExcludeColumns(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };

			emp1 = dataSource.Insert(emp1).ToObject().Execute();

			var test1 = dataSource.From<Employee>(new { Title = uniqueKey }).ToDynamicObject().ExceptProperties("EmployeeId").Execute();

			Assert.AreEqual("A", test1.FirstName);
			Assert.AreEqual("1", test1.LastName);
			Assert.AreEqual(uniqueKey, test1.Title);

			var test1Internal = (IDictionary<string, object>)test1;
			Assert.IsTrue(!test1Internal.ContainsKey("EmployeeId"));
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToDynamicObject_IncludeColumns(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };

			emp1 = dataSource.Insert(emp1).ToObject().Execute();

			var test1 = dataSource.From<Employee>(new { Title = uniqueKey }).ToDynamicObject().WithProperties("FirstName", "LastName", "Title").Execute();

			Assert.AreEqual("A", test1.FirstName);
			Assert.AreEqual("1", test1.LastName);
			Assert.AreEqual(uniqueKey, test1.Title);

			var test1Internal = (IDictionary<string, object>)test1;
			Assert.IsTrue(!test1Internal.ContainsKey("EmployeeId"));
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToDynamicObjectOrNull_IncludeColumns(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };

			emp1 = dataSource.Insert(emp1).ToObject().Execute();

			var test1 = dataSource.From<Employee>(new { Title = uniqueKey }).ToDynamicObjectOrNull().WithProperties("FirstName", "LastName", "Title").Execute();

			Assert.IsNotNull(test1);

			Assert.AreEqual("A", test1.FirstName);
			Assert.AreEqual("1", test1.LastName);
			Assert.AreEqual(uniqueKey, test1.Title);

			var test1Internal = (IDictionary<string, object>)test1;
			Assert.IsTrue(!test1Internal.ContainsKey("EmployeeId"));
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void ToDynamicObject(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = dataSource.From(tableName).WithLimits(1).WithSorting(table.GetDefaultSortOrder()).ToDynamicObjectOrNull().Execute();
			if (result != null)
			{
				var row = (IDictionary<string, object>)result;
				Assert.AreEqual(table.Columns.Count, row.Count);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public async Task ToDynamicObject_Async(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = await dataSource.From(tableName).WithLimits(1).WithSorting(table.GetDefaultSortOrder()).ToDynamicObjectOrNull().ExecuteAsync().ConfigureAwait(false);
			if (result != null)
			{
				var row = (IDictionary<string, object>)result;
				Assert.AreEqual(table.Columns.Count, row.Count);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}
}
