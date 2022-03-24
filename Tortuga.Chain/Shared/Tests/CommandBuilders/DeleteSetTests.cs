using Tests.Models;

namespace Tests.CommandBuilders;

[TestClass]
public class DeleteSetTests : TestBase
{

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void DeleteSet_Where(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			var keysToUpdate = allKeys.Take(5).ToList();

			var updatedRows = dataSource.DeleteSet(EmployeeTableName, $"Title = '{lookupKey}' AND MiddleName Is Null").ToCollection<Employee>().Execute();

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
	public void DeleteSet_WhereArg(string dataSourceName, DataSourceType mode)
	{
#if SQL_SERVER_OLEDB
			var whereClause = "Title = ? AND MiddleName Is Null";
#else
		var whereClause = "Title = @LookupKey AND MiddleName Is Null";
#endif

		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			var keysToUpdate = allKeys.Take(5).ToList();

			var updatedRows = dataSource.DeleteSet(EmployeeTableName, whereClause, new { @LookupKey = lookupKey }).ToCollection<Employee>().Execute();

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
	public void DeleteSet_Filter(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

			var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
			var keysToUpdate = allKeys.Take(5).ToList();

			var updatedRows = dataSource.DeleteSet(EmployeeTableName, new { Title = lookupKey, MiddleName = (string)null }).ToCollection<Employee>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were deleted");

			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(5, allRows.Count, "The wrong number of rows remain");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void DeleteSet_Where_SoftDelete(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			var dataSourceRules = AttachSoftDeleteRulesWithUser(dataSource);

			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSourceRules.Insert(CustomerTableName, new Customer() { FullName = lookupKey, State = i % 2 == 0 ? "AA" : "BB" }).ToObject<Customer>().Execute();

			var allKeys = dataSourceRules.From(CustomerTableName, new { FullName = lookupKey }).ToInt32List("CustomerKey").Execute();
			var keysToUpdate = allKeys.Take(5).ToList();

			var updatedRows = dataSourceRules.DeleteSet(CustomerTableName, $"FullName = '{lookupKey}' AND State = 'BB'").ToCollection<Customer>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were deleted");

			var allRows = dataSourceRules.From(CustomerTableName, new { FullName = lookupKey }).ToCollection<Customer>().Execute();
			Assert.AreEqual(5, allRows.Count, "The wrong number of rows remain");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void DeleteSet_WhereArg_SoftDelete(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			var dataSourceRules = AttachSoftDeleteRulesWithUser(dataSource);

			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSourceRules.Insert(CustomerTableName, new Customer() { FullName = lookupKey, State = i % 2 == 0 ? "AA" : "BB" }).ToObject<Customer>().Execute();

			var allKeys = dataSourceRules.From(CustomerTableName, new { FullName = lookupKey }).ToInt32List("CustomerKey").Execute();
			var keysToUpdate = allKeys.Take(5).ToList();

#if SQL_SERVER_OLEDB
			var whereClause = "FullName = ? AND State = ?";
#else
			var whereClause = "FullName = @Lookup AND State = @State";
#endif

			var updatedRows = dataSourceRules.DeleteSet(CustomerTableName, whereClause, new { @Lookup = lookupKey, State = "BB" }).ToCollection<Customer>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were deleted");

			var allRows = dataSourceRules.From(CustomerTableName, new { FullName = lookupKey }).ToCollection<Customer>().Execute();
			Assert.AreEqual(5, allRows.Count, "The wrong number of rows remain");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void DeleteSet_Filter_SoftDelete(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			var dataSourceRules = AttachSoftDeleteRulesWithUser(dataSource);

			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSourceRules.Insert(CustomerTableName, new Customer() { FullName = lookupKey, State = i % 2 == 0 ? "AA" : "BB" }).ToObject<Customer>().Execute();

			var allKeys = dataSourceRules.From(CustomerTableName, new { FullName = lookupKey }).ToInt32List("CustomerKey").Execute();
			var keysToUpdate = allKeys.Take(5).ToList();

			var updatedRows = dataSourceRules.DeleteSet(CustomerTableName, new { @FullName = lookupKey, State = "BB" }).ToCollection<Customer>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were deleted");

			var allRows = dataSourceRules.From(CustomerTableName, new { FullName = lookupKey }).ToCollection<Customer>().Execute();
			Assert.AreEqual(5, allRows.Count, "The wrong number of rows remain");
		}
		finally
		{
			Release(dataSource);
		}
	}
}
