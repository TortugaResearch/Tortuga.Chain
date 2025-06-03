using Tests.Models;
using Tortuga.Chain;

namespace Tests.CommandBuilders;

[TestClass]
public class InsertBatchTests : TestBase
{
	const string TableType = "HR.EmployeeTable";

#if SQL_SERVER_MDS || SQLITE || POSTGRESQL || MYSQL

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertMultipleBatch(string dataSourceName, DataSourceType mode)
	{
		var key = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var maxRows = 1000;

			for (var i = 0; i < maxRows; i++)
				employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key });

			var count = dataSource.InsertMultipleBatch(EmployeeTableName, employeeList).SetStrictMode(true).Execute();
			Assert.AreEqual(maxRows, count);

			var rows = dataSource.From(EmployeeTableName, new { Title = key }).ToCollection<Employee>().Execute();
			Assert.AreEqual(maxRows, rows.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertMultipleBatch_Empty(string dataSourceName, DataSourceType mode)
	{
		var key = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			try
			{
				var count = dataSource.InsertMultipleBatch(EmployeeTableName, employeeList).SetStrictMode(true).Execute();
				Assert.Fail($"{nameof(ArgumentException)} was expected");
			}
			catch (ArgumentException ex) when (ex.ParamName == "objects")
			{
				//PASS
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

#if SQL_SERVER_MDS || SQLITE || MYSQL

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertBatch_IdentityInsert(string dataSourceName, DataSourceType mode)
	{
		var key = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			//Get the max key, then skip a few rows so we can prove identity insert occurred
			var maxKey = dataSource.Sql("SELECT Max(EmployeeKey) FROM " + EmployeeTableName).ToInt32().Execute() + 10;

			var maxRows = 10;

			for (var i = 0; i < maxRows; i++)
				employeeList.Add(new Employee() { EmployeeKey = maxKey + i, FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key });

			var count = dataSource.InsertBatch(EmployeeTableName, employeeList, InsertOptions.IdentityInsert).AsNonQuery().SetStrictMode(true).Execute();

			Assert.AreEqual(maxRows, count);

			var rows = dataSource.From(EmployeeTableName, new { Title = key }).ToCollection<Employee>().Execute();
			Assert.AreEqual(maxRows, rows.Count);

			foreach (var item in rows)
			{
				Assert.IsTrue(item.EmployeeKey.Value >= maxKey);
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

#if SQL_SERVER_MDS || POSTGRESQL

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertBatch_ReturnKeys(string dataSourceName, DataSourceType mode)
	{
		var key = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var maxRows = 10;

			for (var i = 0; i < maxRows; i++)
				employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key });

			var keys = dataSource.InsertBatch(EmployeeTableName, employeeList).ToInt32List().SetStrictMode(true).Execute();
			Assert.AreEqual(maxRows, keys.Count);

			var rows = dataSource.From(EmployeeTableName, new { Title = key }).ToCollection<Employee>().Execute();
			Assert.AreEqual(maxRows, rows.Count);

			foreach (var item in rows)
			{
				Assert.IsTrue(keys.Contains(item.EmployeeKey.Value));
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertBatch_ReturnKeys_Empty(string dataSourceName, DataSourceType mode)
	{
		var key = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			try
			{
				dataSource.InsertBatch(EmployeeTableName, employeeList).ToInt32List().SetStrictMode(true).Execute();
				Assert.Fail($"{nameof(ArgumentException)} was expected");
			}
			catch (ArgumentException ex) when (ex.ParamName == "objects")
			{
				//PASS
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

#if SQL_SERVER_MDS

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertBatchTable(string dataSourceName, DataSourceType mode)
	{
		var key1000 = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();

		for (var i = 0; i < 1000; i++)
			employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var count = dataSource.InsertBatch(EmployeeTableName, employeeList, TableType).Execute();
			Assert.AreEqual(1000, count);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertBatchTable_Identity(string dataSourceName, DataSourceType mode)
	{
		const string TableType = "HR.EmployeeTable";

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

			var count = dataSource.InsertBatch(EmployeeTableName, employeeList, TableType, InsertOptions.IdentityInsert).Execute();
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
	public void InsertBatchTable_Streaming(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var key1000 = Guid.NewGuid().ToString();

			dataSource.InsertBatch(EmployeeTableName, StreamRecords(key1000, 1000), TableType).Execute();
		}
		finally
		{
			Release(dataSource);
		}
	}

	IEnumerable<Employee> StreamRecords(string key, int maxRecords)
	{
		var i = 0;
		while (i < maxRecords)
		{
			yield return new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key };
			i++;
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertBatchTable_SelectBack(string dataSourceName, DataSourceType mode)
	{
		var key1000 = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();

		for (var i = 0; i < 1000; i++)
			employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var employeeList2 = dataSource.InsertBatch(EmployeeTableName, employeeList, TableType).ToCollection<EmployeeLookup>(CollectionOptions.InferConstructor).Execute();

			Assert.AreEqual(employeeList.Count, employeeList2.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void InsertBatchTable_SelectKeys(string dataSourceName, DataSourceType mode)
	{
		var key1000 = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();

		for (var i = 0; i < 1000; i++)
			employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var employeeList2 = dataSource.InsertBatch(EmployeeTableName, employeeList, TableType).ToInt32List().Execute();

			Assert.AreEqual(employeeList.Count, employeeList2.Count);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void InsertBatchTable_AuditRules(string dataSourceName)
	{
		var key1000 = Guid.NewGuid().ToString();
		var employeeList = new List<Employee>();

		for (var i = 0; i < 1000; i++)
			employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

		var dataSource = AttachRules(DataSource(dataSourceName));
		try
		{
			var employeeList2 = dataSource.InsertBatch(EmployeeTableName, employeeList, TableType).ToCollection<Employee>().Execute();
			Assert.AreEqual(employeeList.Count, employeeList2.Count);
			Assert.IsNotNull(employeeList2[0].UpdatedDate, "Updated date should have been set by the audit rules");
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif
}
