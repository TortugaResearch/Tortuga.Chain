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

#if POSTGRESQL
			var sql1 = "First_Name = 'Redacted'";
			var sql2 = $"Title = '{lookupKey}' AND Middle_Name = 'B'";
#else
			var sql1 = "FirstName = 'Redacted'";
			var sql2 = $"Title = '{lookupKey}' AND MiddleName = 'B'";
#endif
			var updatedRows = dataSource.UpdateSet(EmployeeTableName, sql1).WithFilter(sql2).ToCollection<Employee>().Execute();
			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were returned.");
			Assert.AreEqual(5, allRows.Where(e => e.FirstName == "Redacted").Count(), "The wrong number of rows were updated.");

#if POSTGRESQL
			var sql3 = "First_Name = 'Withheld'";
			var sql4 = $"Title = '{lookupKey}' AND Middle_Name = 'A'";
#else
			var sql3 = "FirstName = 'Withheld'";
			var sql4 = $"Title = '{lookupKey}' AND MiddleName = 'A'";
#endif
			var updatedRows2 = dataSource.UpdateSet(EmployeeTableName, sql3, UpdateOptions.ReturnOldValues).WithFilter(sql4).ToCollection<Employee>().Execute();
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
#elif POSTGRESQL
			var whereClause = "Title = @LookupKey AND Middle_Name = @MiddleName";
#else
			var whereClause = "Title = @LookupKey AND MiddleName = @MiddleName";
#endif

			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" : "B" }).ToObject<Employee>().Execute();

#if POSTGRESQL
			var sql1 = "First_Name = 'Redacted'";
#else
			var sql1 = "FirstName = 'Redacted'";
#endif

			var updatedRows = dataSource.UpdateSet(EmployeeTableName, sql1).WithFilter(whereClause, new
			{
				LookupKey = lookupKey,
				MiddleName = "B"
			}).ToCollection<Employee>().Execute();
			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were returned.");
			Assert.AreEqual(5, allRows.Where(e => e.FirstName == "Redacted").Count(), "The wrong number of rows were updated.");

#if POSTGRESQL
			var sql2 = "First_Name = 'Withheld'";
#else
			var sql2 = "FirstName = 'Withheld'";
#endif

			var updatedRows2 = dataSource.UpdateSet(EmployeeTableName, sql2, UpdateOptions.ReturnOldValues).WithFilter(whereClause, new
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
#elif POSTGRESQL
			var updateExpression = "First_Name = @FirstName";
			var whereClause = "Title = @LookupKey AND Middle_Name = @MiddleName";
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

#if POSTGRESQL
			var sql1 = "First_Name = 'Redacted'";
#else
			var sql1 = "FirstName = 'Redacted'";
#endif

			var updatedRows = dataSource.UpdateSet(EmployeeTableName, sql1).WithFilter(new
			{
				Title = lookupKey,
				MiddleName = "B"
			}).ToCollection<Employee>().Execute();
			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were returned.");
			Assert.AreEqual(5, allRows.Where(e => e.FirstName == "Redacted").Count(), "The wrong number of rows were updated.");

#if POSTGRESQL
			var sql2 = "First_Name = 'Withheld'";
#else
			var sql2 = "FirstName = 'Withheld'";
#endif

			var updatedRows2 = dataSource.UpdateSet(EmployeeTableName, sql2, UpdateOptions.ReturnOldValues).WithFilter(new
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

#if POSTGRESQL
			var sql1 = $"Title = '{lookupKey}' AND Middle_Name = 'B'";
#else
			var sql1 = $"Title = '{lookupKey}' AND MiddleName = 'B'";
#endif
			var updatedRows = dataSource.UpdateSet(EmployeeTableName, new { FirstName = "Redacted" }).WithFilter(sql1).ToCollection<Employee>().Execute();
			var allRows = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToCollection<Employee>().Execute();

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were returned.");
			Assert.AreEqual(5, allRows.Where(e => e.FirstName == "Redacted").Count(), "The wrong number of rows were updated.");

#if POSTGRESQL
			var sql2 = $"Title = '{lookupKey}' AND Middle_Name = 'A'";
#else
			var sql2 = $"Title = '{lookupKey}' AND MiddleName = 'A'";
#endif

			var updatedRows2 = dataSource.UpdateSet(EmployeeTableName, new { FirstName = "Withheld" }, UpdateOptions.ReturnOldValues).WithFilter(sql2).ToCollection<Employee>().Execute();
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
#elif POSTGRESQL
			var whereClause = "Title = @LookupKey AND Middle_Name = @MiddleName";
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
