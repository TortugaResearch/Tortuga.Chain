using Tests.Models;
using Tortuga.Chain;

namespace Tests.CommandBuilders;

[TestClass]
public class DeleteByKeyTests : TestBase
{
	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void DeleteByKey_Auto(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			var keyToUpdate = allKeys.First();

			dataSource.DeleteByKey<Employee>(keyToUpdate).Execute();

			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(9, allRows.Count, "The wrong number of rows remain");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void DeleteByKey(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			var keyToUpdate = allKeys.First();

			dataSource.DeleteByKey(EmployeeTableName, keyToUpdate).Execute();

			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(9, allRows.Count, "The wrong number of rows remain");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void DeleteByKey_FromObject(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			var keyToUpdate = allKeys.First();

			dataSource.DeleteByKey<Employee>(keyToUpdate).Execute();

			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(9, allRows.Count, "The wrong number of rows remain");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void DeleteByKey_Checked_Auto(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			var keyToUpdate = allKeys.First();

			dataSource.DeleteByKey<Employee>(keyToUpdate, DeleteOptions.CheckRowsAffected).Execute();

			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(9, allRows.Count, "The wrong number of rows remain");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void DeleteByKey_Checked(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			var keyToUpdate = allKeys.First();

			dataSource.DeleteByKey(EmployeeTableName, keyToUpdate, DeleteOptions.CheckRowsAffected).Execute();

			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(9, allRows.Count, "The wrong number of rows remain");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void DeleteByKey_Checked_FromObject(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			var keyToUpdate = allKeys.First();

			dataSource.DeleteByKey<Employee>(keyToUpdate, DeleteOptions.CheckRowsAffected).Execute();

			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(9, allRows.Count, "The wrong number of rows remain");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void DeleteByKey_Failed(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			try
			{
				dataSource.DeleteByKey(EmployeeTableName, -30, DeleteOptions.CheckRowsAffected).Execute();
				Assert.Fail("Expected a missing data exception");
			}
			catch (MissingDataException) { }
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void DeleteByKey_Failed_FromObject(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			try
			{
				dataSource.DeleteByKey<Employee>(-30, DeleteOptions.CheckRowsAffected).Execute();
				Assert.Fail("Expected a missing data exception");
			}
			catch (MissingDataException) { }
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void DeleteByKey_Failed_Auto(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			try
			{
				dataSource.DeleteByKey<Employee>(-30, DeleteOptions.CheckRowsAffected).Execute();
				Assert.Fail("Expected a missing data exception");
			}
			catch (MissingDataException) { }
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void DeleteByKeyList(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			var keysToUpdate = allKeys.Take(5).ToList();

			var updatedRows = dataSource.DeleteByKeyList(EmployeeTableName, keysToUpdate).ToCollection<Employee>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were deleted");

			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(5, allRows.Count, "The wrong number of rows remain");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void DeleteByKeyList_Checked(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			var keysToUpdate = allKeys.Take(5).ToList();

			var updatedRows = dataSource.DeleteByKeyList(EmployeeTableName, keysToUpdate, DeleteOptions.CheckRowsAffected).ToCollection<Employee>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were deleted");

			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(5, allRows.Count, "The wrong number of rows remain");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void DeleteByKeyList_Fail(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			var keysToUpdate = allKeys.Take(5).ToList();

			//Add two keys that don't exist
			keysToUpdate.Add(-10);
			keysToUpdate.Add(-20);

			try
			{
				var updatedRows = dataSource.DeleteByKeyList(EmployeeTableName, keysToUpdate, DeleteOptions.CheckRowsAffected).ToCollection<Employee>().Execute();
				Assert.Fail("Expected a missing data exception");
			}
			catch (MissingDataException) { }
		}
		finally
		{
			Release(dataSource);
		}
	}

#if SQL_SERVER_MDS || SQL_SERVER_OLEDB //SQL Server has problems with CRUD operations that return values on tables with triggers.

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void DeleteByKey_Trigger(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName_Trigger, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

			var allKeys = dataSource.From(EmployeeTableName_Trigger, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			var keyToUpdate = allKeys.First();

			dataSource.DeleteByKey(EmployeeTableName_Trigger, keyToUpdate).Execute();

			var allRows = dataSource.From(EmployeeTableName_Trigger, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(9, allRows.Count, "The wrong number of rows remain");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void DeleteByKeyList_Trigger(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName_Trigger, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

			var allKeys = dataSource.From(EmployeeTableName_Trigger, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			var keysToUpdate = allKeys.Take(5).ToList();

			var updatedRows = dataSource.DeleteByKeyList(EmployeeTableName_Trigger, keysToUpdate).ToCollection<Employee>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were deleted");

			var allRows = dataSource.From(EmployeeTableName_Trigger, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(5, allRows.Count, "The wrong number of rows remain");
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif
}
