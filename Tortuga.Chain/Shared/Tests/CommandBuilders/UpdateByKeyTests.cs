using Tests.Models;

namespace Tests.CommandBuilders;

[TestClass]
public class UpdateByKeyTests : TestBase
{
	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void UpdateByKey(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

#if SQLITE

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt64List("EmployeeKey").Execute();
#elif MYSQL
			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToUInt64List("EmployeeKey").Execute();
#else
			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
#endif
			var keyToUpdate = allKeys.First();

			var newValues = new { FirstName = "Bob" };
			dataSource.UpdateByKey(EmployeeTableName, newValues, keyToUpdate).Execute();

			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			foreach (var row in allRows)
			{
				if (keyToUpdate == row.EmployeeKey.Value)
					Assert.AreEqual("Bob", row.FirstName, "FirstName should have been changed.");
				else
					Assert.AreNotEqual("Bob", row.FirstName, "FirstName should not have been changed.");
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void UpdateByKeyList(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

#if SQLITE

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt64List("EmployeeKey").Execute();
#elif MYSQL
			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToUInt64List("EmployeeKey").Execute();
#else
			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
#endif
			var keysToUpdate = allKeys.Take(5).ToList();

			var newValues = new { FirstName = "Bob" };
			var updatedRows = dataSource.UpdateByKeyList(EmployeeTableName, newValues, keysToUpdate).ToCollection<Employee>().Execute();

			Assert.AreEqual(5, updatedRows.Count);
			foreach (var row in updatedRows)
				Assert.AreEqual("Bob", row.FirstName);

			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			foreach (var row in allRows)
			{
				if (keysToUpdate.Contains(row.EmployeeKey.Value))
					Assert.AreEqual("Bob", row.FirstName, "FirstName should have been changed.");
				else
					Assert.AreNotEqual("Bob", row.FirstName, "FirstName should not have been changed.");
			}
		}
		finally
		{
			Release(dataSource);
		}
	}
}
