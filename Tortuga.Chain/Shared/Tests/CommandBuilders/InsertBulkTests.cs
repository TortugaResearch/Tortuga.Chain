using System.Diagnostics;
using Tests.Models;

#if SQL_SERVER_MDS

using Microsoft.Data.SqlClient;

#endif

namespace Tests.CommandBuilders;

[TestClass]
public class InsertBulkTests : TestBase
{
#if SQL_SERVER_MDS || MYSQL || POSTGRESQL

	IEnumerable<Employee> StreamRecords(string key, int maxRecords)
	{
		var i = 0;
		while (i < maxRecords)
		{
			yield return new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key, UpdatedDate = DateTime.Now };
			i++;
		}
	}

#if SQL_SERVER_MDS || MYSQL

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertBulk_List_WithBatches(string dataSourceName, DataSourceType mode)
	{
		var key1000 = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();

		for (var i = 0; i < 1000; i++)
			employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = dataSource.InsertBulk(EmployeeTableName, employeeList).WithBatchSize(250).Execute();
			Assert.AreEqual(1000, count);

			var count2 = dataSource.From(EmployeeTableName, new { Title = key1000 }).AsCount().Execute();
			Assert.AreEqual(1000, count2);
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertBulk_Dictionary(string dataSourceName, DataSourceType mode)
	{
		var key1000 = Guid.NewGuid().ToString();
		var employeeList = new List<Dictionary<string, object>>();

		const int RowCount = 1000;
		for (var i = 0; i < RowCount; i++)
			employeeList.Add(new Dictionary<string, object>() {
				{ "FirstName", i.ToString("0000") },
				{ "LastName", "Z" + (int.MaxValue - i) },
				{ "Title",key1000 },
				{ "GendeR",' '}, //intentionally using the wrong case
				{ "EmployeeId" , Guid.NewGuid().ToString()}});

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = dataSource.InsertBulk(EmployeeTableName, employeeList).Execute();
			Assert.AreEqual(RowCount, count);

			var count2 = dataSource.From(EmployeeTableName, new { Title = key1000 }).AsCount().Execute();
			Assert.AreEqual(RowCount, count2);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertBulk_List(string dataSourceName, DataSourceType mode)
	{
		var key1000 = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();

		const int RowCount = 1000;
		for (var i = 0; i < RowCount; i++)
			employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = dataSource.InsertBulk(EmployeeTableName, employeeList).Execute();
			Assert.AreEqual(RowCount, count);

			var count2 = dataSource.From(EmployeeTableName, new { Title = key1000 }).AsCount().Execute();
			Assert.AreEqual(RowCount, count2);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertBulk_List_Empty(string dataSourceName, DataSourceType mode)
	{
		var key1000 = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();

		const int RowCount = 0;

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = dataSource.InsertBulk(EmployeeTableName, employeeList).Execute();
			Assert.AreEqual(RowCount, count);

			var count2 = dataSource.From(EmployeeTableName, new { Title = key1000 }).AsCount().Execute();
			Assert.AreEqual(RowCount, count2);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertBulk_List_ImpliedTableName(string dataSourceName, DataSourceType mode)
	{
		var key1000 = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();

		for (var i = 0; i < 1000; i++)
			employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = dataSource.InsertBulk(employeeList).Execute();
			Assert.AreEqual(1000, count);

			var count2 = dataSource.From(EmployeeTableName, new { Title = key1000 }).AsCount().Execute();
			Assert.AreEqual(1000, count2);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertBulk_Enumeration(string dataSourceName, DataSourceType mode)
	{
		var key1000 = Guid.NewGuid().ToString();

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = dataSource.InsertBulk(EmployeeTableName, StreamRecords(key1000, 1000)).Execute();

#if SQL_SERVER_MDS
			Assert.AreEqual(-1, count); //streaming prevents returning a row count;
#elif MYSQL || POSTGRESQL
			Assert.AreEqual(1000, count);
#endif

			var count2 = dataSource.From(EmployeeTableName, new { Title = key1000 }).AsCount().Execute();
			Assert.AreEqual(1000, count2);
		}
		finally
		{
			Release(dataSource);
		}
	}

#if SQL_SERVER_MDS || MYSQL

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task InsertBulkAsync_List_WithBatches(string dataSourceName, DataSourceType mode)
	{
		var key1000 = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();

		for (var i = 0; i < 1000; i++)
			employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = await dataSource.InsertBulk(EmployeeTableName, employeeList).WithBatchSize(250).ExecuteAsync().ConfigureAwait(false);
			Assert.AreEqual(1000, count);

			var count2 = await dataSource.From(EmployeeTableName, new { Title = key1000 }).AsCount().ExecuteAsync().ConfigureAwait(false);
			Assert.AreEqual(1000, count2);
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task InsertBulkAsync_List(string dataSourceName, DataSourceType mode)
	{
		var key1000 = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();

		for (var i = 0; i < 1000; i++)
			employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = await dataSource.InsertBulk(EmployeeTableName, employeeList).ExecuteAsync().ConfigureAwait(false);
			Assert.AreEqual(1000, count);

			var count2 = await dataSource.From(EmployeeTableName, new { Title = key1000 }).AsCount().ExecuteAsync().ConfigureAwait(false);
			Assert.AreEqual(1000, count2);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task InsertBulkAsync_List_Empty(string dataSourceName, DataSourceType mode)
	{
		var key1000 = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = await dataSource.InsertBulk(EmployeeTableName, employeeList).ExecuteAsync().ConfigureAwait(false);
			Assert.AreEqual(0, count);

			var count2 = await dataSource.From(EmployeeTableName, new { Title = key1000 }).AsCount().ExecuteAsync().ConfigureAwait(false);
			Assert.AreEqual(0, count2);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task InsertBulkAsync_Enumeration(string dataSourceName, DataSourceType mode)
	{
		var key1000 = Guid.NewGuid().ToString();

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = await dataSource.InsertBulk(EmployeeTableName, StreamRecords(key1000, 1000)).ExecuteAsync().ConfigureAwait(false);

#if SQL_SERVER_MDS
			Assert.AreEqual(-1, count); //streaming prevents returning a row count;
#elif MYSQL || POSTGRESQL
			Assert.AreEqual(1000, count);
#endif

			var count2 = await dataSource.From(EmployeeTableName, new { Title = key1000 }).AsCount().ExecuteAsync().ConfigureAwait(false);
			Assert.AreEqual(1000, count2);
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

#if SQL_SERVER_MDS

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	[Obsolete]
	public void InsertBulk_IdentityInsert(string dataSourceName, DataSourceType mode)
	{
		var key1000 = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var employeeTable = dataSource.DatabaseMetadata.GetTableOrView(EmployeeTableName);
			var primaryColumn = employeeTable.Columns.SingleOrDefault(c => c.IsIdentity);
			if (primaryColumn == null) //SQLite
				primaryColumn = employeeTable.Columns.SingleOrDefault(c => c.IsPrimaryKey);

			//Skipping ahead by 500
			var nextKey = 500 + dataSource.Sql($"SELECT Max({primaryColumn.QuotedSqlName}) FROM {employeeTable.Name.ToQuotedString()}").ToInt32().Execute();

			for (var i = 0; i < 1000; i++)
				employeeList.Add(new Employee() { EmployeeKey = nextKey + i, FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

			var count = dataSource.InsertBulk(EmployeeTableName, employeeList, SqlBulkCopyOptions.KeepIdentity).Execute();
			Assert.AreEqual(1000, count);

			var newMax = dataSource.Sql($"SELECT Max({primaryColumn.QuotedSqlName}) FROM {employeeTable.Name.ToQuotedString()}").ToInt32().Execute();

			Assert.AreEqual(nextKey + 1000 - 1, newMax, "Identity insert didn't use the desired primary key values");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertBulk_WithStreaming(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var key = Guid.NewGuid().ToString();
			var employeeList = new List<Employee>();

			var count = dataSource.InsertBulk(EmployeeTableName, StreamRecords(key, 1000)).WithStreaming().Execute();
			Assert.AreEqual(-1, count); //streaming prevents returning a row count;

			var count2 = dataSource.From(EmployeeTableName, new { Title = key }).AsCount().Execute();
			Assert.AreEqual(1000, count2);
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

#if SQL_SERVER_MDS

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertBulk_WithEvents(string dataSourceName, DataSourceType mode)
	{
		long runningCount = 0;
		var key = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = dataSource.InsertBulk(EmployeeTableName, StreamRecords(key, 1000)).WithBatchSize(75).WithNotifications((s, e) =>
			{
				Debug.WriteLine($"Copied {e.RowsAffected} rows");
				runningCount = e.RowsAffected;
			}, 90).Execute();

			Assert.AreEqual(-1, count); //streaming prevents returning a row count;
			Assert.AreNotEqual(0, runningCount, "record count is wrong"); //but we can get it another way

			var count2 = dataSource.From(EmployeeTableName, new { Title = key }).AsCount().Execute();
			Assert.AreEqual(1000, count2);
		}
		finally
		{
			Release(dataSource);
		}
	}

#elif POSTGRESQL || MYSQL

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertBulk_WithEvents(string dataSourceName, DataSourceType mode)
	{
		long runningCount = 0;
		var key = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = dataSource.InsertBulk(EmployeeTableName, StreamRecords(key, 1000)).WithNotifications((s, e) =>
			{
				Debug.WriteLine($"Copied {e.RowsAffected} rows");
				runningCount = e.RowsAffected;
			}, 90).Execute();

			Assert.AreEqual(1000, count); //streaming prevents returning a row count;
			Assert.AreNotEqual(0, runningCount, "record count is wrong"); //but we can get it another way

			var count2 = dataSource.From(EmployeeTableName, new { Title = key }).AsCount().Execute();
			Assert.AreEqual(1000, count2);
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif
}
