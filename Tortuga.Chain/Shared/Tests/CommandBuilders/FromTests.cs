using Tests.Models;
using Tortuga.Chain;
using Tortuga.Chain.DataSources;

#if SQL_SERVER_SDS || SQL_SERVER_MDS

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









	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void ToDataTable(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = dataSource.From(tableName).WithLimits(10).ToDataTable().Execute();
			Assert.IsTrue(result.Rows.Count <= 10);
			Assert.AreEqual(table.Columns.Count, result.Columns.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public async Task ToDataTable_Async(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = await dataSource.From(tableName).WithLimits(10).ToDataTable().ExecuteAsync();
			Assert.IsTrue(result.Rows.Count <= 10);
			Assert.AreEqual(table.Columns.Count, result.Columns.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void ToDataRow(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = dataSource.From(tableName).WithLimits(1).ToDataRowOrNull().Execute();
			if (result != null)
			{
				Assert.AreEqual(table.Columns.Count, result.Table.Columns.Count);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public async Task ToDataRow_Async(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = await dataSource.From(tableName).WithLimits(1).ToDataRowOrNull().ExecuteAsync();
			if (result != null)
			{
				Assert.AreEqual(table.Columns.Count, result.Table.Columns.Count);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void ToTable(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = dataSource.From(tableName).WithLimits(10).ToTable().Execute();
			Assert.IsTrue(result.Rows.Count <= 10);
			Assert.AreEqual(table.Columns.Count, result.ColumnNames.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public async Task ToTable_Async(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = await dataSource.From(tableName).WithLimits(10).ToTable().ExecuteAsync();
			Assert.IsTrue(result.Rows.Count <= 10);
			Assert.AreEqual(table.Columns.Count, result.ColumnNames.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public void ToRow(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = dataSource.From(tableName).WithLimits(1).ToRowOrNull().Execute();
			if (result != null)
			{
				Assert.AreEqual(table.Columns.Count, result.Count);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewData(DataSourceGroup.All)]
	public async Task ToRow_Async(string dataSourceName, DataSourceType mode, string tableName)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var result = await dataSource.From(tableName).WithLimits(1).ToRowOrNull().ExecuteAsync();
			if (result != null)
			{
				Assert.AreEqual(table.Columns.Count, result.Count);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewLimitData(DataSourceGroup.AllNormalOnly)]
	public void ToTable_WithLimit(string dataSourceName, DataSourceType mode, string tableName, LimitOptions limitOptions)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var prep = ((ICrudDataSource)dataSource).From(tableName).WithLimits(10, limitOptions);
			switch (limitOptions)
			{
				case LimitOptions.RowsWithTies:
				case LimitOptions.PercentageWithTies:
					prep = prep.WithSorting(table.Columns[0].SqlName);
					break;
			}
			var result = prep.ToTable().Execute();
			//Assert.IsTrue(result.Rows.Count <= 10, $"Row count was {result.Rows.Count}");
			Assert.AreEqual(table.Columns.Count, result.ColumnNames.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, TablesAndViewLimitData(DataSourceGroup.AllNormalOnly)]
	public async Task ToTable_WithLimit_Async(string dataSourceName, DataSourceType mode, string tableName, LimitOptions limitOptions)
	{
		var dataSource = await DataSourceAsync(dataSourceName, mode);
		try
		{
			var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

			var prep = ((ICrudDataSource)dataSource).From(tableName).WithLimits(10, limitOptions);
			switch (limitOptions)
			{
				case LimitOptions.RowsWithTies:
				case LimitOptions.PercentageWithTies:
					prep = prep.WithSorting(table.Columns[0].SqlName);
					break;
			}
			var result = await prep.ToTable().ExecuteAsync();
			//Assert.IsTrue(result.Rows.Count <= 10, $"Row count was {result.Rows.Count}");
			Assert.AreEqual(table.Columns.Count, result.ColumnNames.Count);
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
	public void ImmutableArray(string dataSourceName, DataSourceType mode)
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
	public void ImmutableList(string dataSourceName, DataSourceType mode)
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




}
