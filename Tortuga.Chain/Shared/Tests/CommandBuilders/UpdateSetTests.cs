using Tests.Models;
using Tortuga.Chain;

namespace Tests.CommandBuilders;

[TestClass]
public class UpdateSetTests : TestBase
{
	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void UpdateSet_Expression_Where(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" : "B" }).ToObject<Employee>().Execute();

			var updatedRows = dataSource.UpdateSet(EmployeeTableName, "FirstName = 'Redacted'").WithFilter($"Title = '{lookupKey}' AND MiddleName = 'B'").ToCollection<Employee>().Execute();
			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were returned.");
			Assert.AreEqual(5, allRows.Where(e => e.FirstName == "Redacted").Count(), "The wrong number of rows were updated.");

			var updatedRows2 = dataSource.UpdateSet(EmployeeTableName, "FirstName = 'Withheld'", UpdateOptions.ReturnOldValues).WithFilter($"Title = '{lookupKey}' AND MiddleName = 'A'").ToCollection<Employee>().Execute();
			Assert.AreEqual(5, updatedRows2.Where(e => e.FirstName != "Withheld").Count(), "The old values were not as expected.");

			var allRows2 = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(5, allRows2.Where(e => e.FirstName == "Withheld").Count(), "The wrong number of rows were updated in the second pass.");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void UpdateSet_Expression_WhereArg(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
#if SQL_SERVER_OLEDB
			var whereClause = "Title = ? AND MiddleName = ?";
#else
			var whereClause = "Title = @LookupKey AND MiddleName = @MiddleName";
#endif

			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" : "B" }).ToObject<Employee>().Execute();

			var updatedRows = dataSource.UpdateSet(EmployeeTableName, "FirstName = 'Redacted'").WithFilter(whereClause, new
			{
				LookupKey = lookupKey,
				MiddleName = "B"
			}).ToCollection<Employee>().Execute();
			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were returned.");
			Assert.AreEqual(5, allRows.Where(e => e.FirstName == "Redacted").Count(), "The wrong number of rows were updated.");

			var updatedRows2 = dataSource.UpdateSet(EmployeeTableName, "FirstName = 'Withheld'", UpdateOptions.ReturnOldValues).WithFilter(whereClause, new
			{
				LookupKey = lookupKey,
				MiddleName = "B"
			}).ToCollection<Employee>().Execute();
			Assert.AreEqual(5, updatedRows2.Where(e => e.FirstName != "Withheld").Count(), "The old values were not as expected.");

			var allRows2 = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(5, allRows2.Where(e => e.FirstName == "Withheld").Count(), "The wrong number of rows were updated in the second pass.");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void UpdateSet_ExpressionArg_WhereArg(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
#if SQL_SERVER_OLEDB
			var updateExpression = "FirstName = ?";
			var whereClause = "Title = ? AND MiddleName = ?";
#else
			var updateExpression = "FirstName = @FirstName";
			var whereClause = "Title = @LookupKey AND MiddleName = @MiddleName";
#endif

			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" : "B" }).ToObject<Employee>().Execute();

			var updatedRows = dataSource.UpdateSet(EmployeeTableName, updateExpression, new { FirstName = "Redacted" }).WithFilter(whereClause, new
			{
				LookupKey = lookupKey,
				MiddleName = "B"
			}).ToCollection<Employee>().Execute();
			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were returned.");
			Assert.AreEqual(5, allRows.Where(e => e.FirstName == "Redacted").Count(), "The wrong number of rows were updated.");

			var updatedRows2 = dataSource.UpdateSet(EmployeeTableName, updateExpression, new { FirstName = "Withheld" }, UpdateOptions.ReturnOldValues).WithFilter(whereClause, new
			{
				LookupKey = lookupKey,
				MiddleName = "B"
			}).ToCollection<Employee>().Execute();
			Assert.AreEqual(5, updatedRows2.Where(e => e.FirstName != "Withheld").Count(), "The old values were not as expected.");

			var allRows2 = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(5, allRows2.Where(e => e.FirstName == "Withheld").Count(), "The wrong number of rows were updated in the second pass.");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void UpdateSet_Expression_Filter(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" : "B" }).ToObject<Employee>().Execute();

			var updatedRows = dataSource.UpdateSet(EmployeeTableName, "FirstName = 'Redacted'").WithFilter(new
			{
				Title = lookupKey,
				MiddleName = "B"
			}).ToCollection<Employee>().Execute();
			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were returned.");
			Assert.AreEqual(5, allRows.Where(e => e.FirstName == "Redacted").Count(), "The wrong number of rows were updated.");

			var updatedRows2 = dataSource.UpdateSet(EmployeeTableName, "FirstName = 'Withheld'", UpdateOptions.ReturnOldValues).WithFilter(new
			{
				Title = lookupKey,
				MiddleName = "B"
			}, FilterOptions.None).ToCollection<Employee>().Execute();
			Assert.AreEqual(5, updatedRows2.Where(e => e.FirstName != "Withheld").Count(), "The old values were not as expected.");

			var allRows2 = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(5, allRows2.Where(e => e.FirstName == "Withheld").Count(), "The wrong number of rows were updated in the second pass.");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void UpdateSet_Value_Where(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" : "B" }).ToObject<Employee>().Execute();

			var updatedRows = dataSource.UpdateSet(EmployeeTableName, new { FirstName = "Redacted" }).WithFilter($"Title = '{lookupKey}' AND MiddleName = 'B'").ToCollection<Employee>().Execute();
			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were returned.");
			Assert.AreEqual(5, allRows.Where(e => e.FirstName == "Redacted").Count(), "The wrong number of rows were updated.");

			var updatedRows2 = dataSource.UpdateSet(EmployeeTableName, new { FirstName = "Withheld" }, UpdateOptions.ReturnOldValues).WithFilter($"Title = '{lookupKey}' AND MiddleName = 'A'").ToCollection<Employee>().Execute();
			Assert.AreEqual(5, updatedRows2.Where(e => e.FirstName != "Withheld").Count(), "The old values were not as expected.");

			var allRows2 = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(5, allRows2.Where(e => e.FirstName == "Withheld").Count(), "The wrong number of rows were updated in the second pass.");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void UpdateSet_Value_WhereArg(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
#if SQL_SERVER_OLEDB
				var whereClause = "Title = ? AND MiddleName = ?";
#else
			var whereClause = "Title = @LookupKey AND MiddleName = @MiddleName";
#endif

			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" : "B" }).ToObject<Employee>().Execute();

			var updatedRows = dataSource.UpdateSet(EmployeeTableName, new { FirstName = "Redacted" }).WithFilter(whereClause, new
			{
				LookupKey = lookupKey,
				MiddleName = "B"
			}).ToCollection<Employee>().Execute();
			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were returned.");
			Assert.AreEqual(5, allRows.Where(e => e.FirstName == "Redacted").Count(), "The wrong number of rows were updated.");

			var updatedRows2 = dataSource.UpdateSet(EmployeeTableName, new { FirstName = "Withheld" }, UpdateOptions.ReturnOldValues).WithFilter(whereClause, new
			{
				LookupKey = lookupKey,
				MiddleName = "B"
			}).ToCollection<Employee>().Execute();
			Assert.AreEqual(5, updatedRows2.Where(e => e.FirstName != "Withheld").Count(), "The old values were not as expected.");

			var allRows2 = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(5, allRows2.Where(e => e.FirstName == "Withheld").Count(), "The wrong number of rows were updated in the second pass.");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void UpdateSet_Value_Filter(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" : "B" }).ToObject<Employee>().Execute();

			var updatedRows = dataSource.UpdateSet(EmployeeTableName, new { FirstName = "Redacted" }).WithFilter(new { Title = lookupKey, MiddleName = "B" }).ToCollection<Employee>().Execute();
			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were returned.");
			Assert.AreEqual(5, allRows.Where(e => e.FirstName == "Redacted").Count(), "The wrong number of rows were updated.");

			var updatedRows2 = dataSource.UpdateSet(EmployeeTableName, new { FirstName = "Withheld" }, UpdateOptions.ReturnOldValues).WithFilter(new { Title = lookupKey, MiddleName = "B" }, FilterOptions.None).ToCollection<Employee>().Execute();
			Assert.AreEqual(5, updatedRows2.Where(e => e.FirstName != "Withheld").Count(), "The old values were not as expected.");

			var allRows2 = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();
			Assert.AreEqual(5, allRows2.Where(e => e.FirstName == "Withheld").Count(), "The wrong number of rows were updated in the second pass.");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void UpdateSet_Disallowed_1(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" : "B" }).ToObject<Employee>().Execute();

			try
			{
				dataSource.UpdateSet(EmployeeTableName, new { MiddleName = "C" }).WithFilter(new { Title = lookupKey, MiddleName = "B" }).Execute();
				Assert.Fail("Expected an exception");
			}
			catch (InvalidOperationException)
			{
				//expected;
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void UpdateSet_Disallowed_2(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" : "B" }).ToObject<Employee>().Execute();

			try
			{
				dataSource.UpdateSet(EmployeeTableName, new { MiddleName = "C" }).WithFilter($"Title = @LookupKey AND MiddleName = @MiddleName", new { LookupKey = lookupKey, MiddleName = "B" }).Execute();
				Assert.Fail("Expected an exception");
			}
			catch (InvalidOperationException)
			{
				//expected;
			}
		}
		finally
		{
			Release(dataSource);
		}
	}
}
