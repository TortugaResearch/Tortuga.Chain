using Tests.Models;
using Tortuga.Chain;

namespace Tests.CommandBuilders;

[TestClass]
public class InsertTests : TestBase
{
	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Insert(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).Execute();

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			Assert.AreEqual(10, allKeys.Count, "Count if inserted rows is off.");
		}
		finally
		{
			Release(dataSource);
		}
	}

#if !POSTGRESQL

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Identity_Insert(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			var employeeTable = dataSource.DatabaseMetadata.GetTableOrView(EmployeeTableName);
			var primaryColumn = employeeTable.Columns.SingleOrDefault(c => c.IsIdentity);
			if (primaryColumn == null) //SQLite
				primaryColumn = employeeTable.PrimaryKeyColumns.SingleOrDefault();

			//Skipping ahead by 500
			var nextKey = 500 + dataSource.Sql($"SELECT Max({primaryColumn.QuotedSqlName}) FROM {employeeTable.Name.ToQuotedString()}").ToInt32().Execute();

			var keyForOverriddenRow = dataSource.Insert(EmployeeTableName, new Employee() { EmployeeKey = nextKey, FirstName = "0000", LastName = "Z" + (int.MaxValue), Title = lookupKey, MiddleName = "A0" }, InsertOptions.IdentityInsert).ToInt32().Execute();

			Assert.AreEqual(nextKey, keyForOverriddenRow, "Identity column was not correctly overridden");

			var keyForNewRow = dataSource.Insert(EmployeeTableName, new Employee() { FirstName = "0001", LastName = "Z" + (int.MaxValue - 1), Title = lookupKey, MiddleName = null }).ToInt32().Execute();
			Assert.AreEqual(keyForOverriddenRow + 1, keyForNewRow, "Next inserted value didn't have the correct key");
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertEchoObject(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var list = new List<Employee>();
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				list.Add(dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute());

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			Assert.AreEqual(list.Count, allKeys.Count, "Count if inserted rows is off.");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertEchoNewKey(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var list = new List<int>();
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				list.Add(dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToInt32().Execute());

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			Assert.AreEqual(list.Count, allKeys.Count, "Count if inserted rows is off.");
		}
		finally
		{
			Release(dataSource);
		}
	}

#if SQL_SERVER_SDS || SQL_SERVER_MDS || SQL_SERVER_OLEDB //SQL Server has problems with CRUD operations that return values on tables with triggers.

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Insert_Trigger(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName_Trigger, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).Execute();

			var allKeys = dataSource.From(EmployeeTableName_Trigger, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			Assert.AreEqual(10, allKeys.Count, "Count if inserted rows is off.");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertEchoObject_Trigger(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var list = new List<Employee>();
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				list.Add(dataSource.Insert(EmployeeTableName_Trigger, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute());

			var allKeys = dataSource.From(EmployeeTableName_Trigger, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			Assert.AreEqual(list.Count, allKeys.Count, "Count if inserted rows is off.");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertEchoNewKey_Trigger(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var list = new List<int>();
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				list.Add(dataSource.Insert(EmployeeTableName_Trigger, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToInt32().Execute());

			var allKeys = dataSource.From(EmployeeTableName_Trigger, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			Assert.AreEqual(list.Count, allKeys.Count, "Count if inserted rows is off.");
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif
}
