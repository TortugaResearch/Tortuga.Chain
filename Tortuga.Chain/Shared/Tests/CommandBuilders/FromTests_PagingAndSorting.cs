using Tests.Models;
using Tortuga.Chain;

namespace Tests.CommandBuilders;

[TestClass]
public class FromTests_PagingAndSorting : TestBase
{
	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Take_NoSort(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).WithLimits(10).ToCollection<Employee>().Execute();
			Assert.AreEqual(10, result.Count, "Count");
			foreach (var item in result)
			{
				Assert.AreEqual(EmployeeSearchKey1000, item.Title, "Filter");
				Assert.IsTrue(int.Parse(item.FirstName) >= 0, "Range");
				Assert.IsTrue(int.Parse(item.FirstName) < 10, "Range");
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Take(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).WithSorting("FirstName").WithLimits(10).ToCollection<Employee>().Execute();
			Assert.AreEqual(10, result.Count, "Count");
			foreach (var item in result)
			{
				Assert.AreEqual(EmployeeSearchKey1000, item.Title, "Filter");
				Assert.IsTrue(int.Parse(item.FirstName) >= 0, "Range");
				Assert.IsTrue(int.Parse(item.FirstName) < 10, "Range");
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

#if SQLITE

		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void TakeRandom(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				var result = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).WithLimits(100, SQLiteLimitOption.RandomSampleRows).ToCollection<Employee>().Execute();
				Assert.AreEqual(100, result.Count, "Count");
				foreach (var item in result)
				{
					Assert.AreEqual(EmployeeSearchKey1000, item.Title, "Filter");
				}
			}
			finally
			{
				Release(dataSource);
			}
		}

#endif

#if SQL_SERVER_OLEDB || SQL_SERVER_MDS

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void TakePercent(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).WithSorting("FirstName").WithLimits(10, SqlServerLimitOption.Percentage).ToCollection<Employee>().Execute();
			Assert.AreEqual(100, result.Count, "Count");
			foreach (var item in result)
			{
				Assert.AreEqual(EmployeeSearchKey1000, item.Title, "Filter");
				Assert.IsTrue(int.Parse(item.FirstName) >= 0, "Range");
				Assert.IsTrue(int.Parse(item.FirstName) < 100, "Range");
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void TakePercentWithTies(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).WithSorting("FirstName").WithLimits(10, SqlServerLimitOption.PercentageWithTies).ToCollection<Employee>().Execute();
			Assert.AreEqual(100, result.Count, "Count");
			foreach (var item in result)
			{
				Assert.AreEqual(EmployeeSearchKey1000, item.Title, "Filter");
				Assert.IsTrue(int.Parse(item.FirstName) >= 0, "Range");
				Assert.IsTrue(int.Parse(item.FirstName) < 100, "Range");
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void TakeWithTies(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).WithSorting("FirstName").WithLimits(10, SqlServerLimitOption.RowsWithTies).ToCollection<Employee>().Execute();
			Assert.AreEqual(10, result.Count, "Count");
			foreach (var item in result)
			{
				Assert.AreEqual(EmployeeSearchKey1000, item.Title, "Filter");
				Assert.IsTrue(int.Parse(item.FirstName) >= 0, "Range");
				Assert.IsTrue(int.Parse(item.FirstName) < 10, "Range");
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void TableSampleSystemPercentage(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).WithLimits(100, SqlServerLimitOption.TableSampleSystemPercentage).ToCollection<Employee>().Execute();

			//SQL Server is really inaccurate here for low row counts. We could get 0 rows, 55 rows, or 175 rows depending on where the data lands on the page.
			foreach (var item in result)
			{
				Assert.AreEqual(EmployeeSearchKey1000, item.Title, "Filter");
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void TableSampleSystemRows(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).WithLimits(100, SqlServerLimitOption.TableSampleSystemRows).ToCollection<Employee>().Execute();

			//SQL Server is really inaccurate here for low row counts. We could get 0 rows, 55 rows, or 175 rows depending on where the data lands on the page.
			foreach (var item in result)
			{
				Assert.AreEqual(EmployeeSearchKey1000, item.Title, "Filter");
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void TableSampleSystemPercentage_Repeatable(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var seed = 1;
			var result1 = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).WithLimits(100, SqlServerLimitOption.TableSampleSystemPercentage, seed).ToCollection<Employee>().Execute();
			var result2 = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).WithLimits(100, SqlServerLimitOption.TableSampleSystemPercentage, seed).ToCollection<Employee>().Execute();

			Assert.AreEqual(result1.Count, result2.Count, "Row count");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void TableSampleSystemRows_Repeatable(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var seed = 1;
			var result1 = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).WithLimits(100, SqlServerLimitOption.TableSampleSystemRows, seed).ToCollection<Employee>().Execute();
			var result2 = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).WithLimits(100, SqlServerLimitOption.TableSampleSystemRows, seed).ToCollection<Employee>().Execute();

			Assert.AreEqual(result1.Count, result2.Count, "Row count");
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Sorting_BadColumn(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			try
			{
				var test1 = dataSource.From(EmployeeTableName, new { Title = "Test" }).WithSorting("Frank").ToCollection<Employee>().Execute();
				Assert.Fail("This should have thrown an exception for the unknown sort column.");
			}
			catch (MappingException)
			{
				//pass
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Sorting(string dataSourceName, DataSourceType mode)
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

			var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("FirstName").ToCollection<Employee>().Execute();
			Assert.AreEqual(emp1.EmployeeKey, test1[0].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test1[1].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test1[2].EmployeeKey);
			Assert.AreEqual(emp4.EmployeeKey, test1[3].EmployeeKey);

			var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting(new SortExpression("FirstName", SortDirection.Descending)).ToCollection<Employee>().Execute();
			Assert.AreEqual(emp4.EmployeeKey, test2[0].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test2[1].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test2[2].EmployeeKey);
			Assert.AreEqual(emp1.EmployeeKey, test2[3].EmployeeKey);

			var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("LastName", "FirstName").ToCollection<Employee>().Execute();
			Assert.AreEqual(emp3.EmployeeKey, test3[0].EmployeeKey);
			Assert.AreEqual(emp4.EmployeeKey, test3[1].EmployeeKey);
			Assert.AreEqual(emp1.EmployeeKey, test3[2].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test3[3].EmployeeKey);

#if POSTGRESQL
			var sortColumnName = "First_Name";
#else
			var sortColumnName = "FirstName";
#endif

			var test5 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting($"{sortColumnName} DESC").ToCollection<Employee>().Execute();
			Assert.AreEqual(emp4.EmployeeKey, test5[0].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test5[1].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test5[2].EmployeeKey);
			Assert.AreEqual(emp1.EmployeeKey, test5[3].EmployeeKey);

			var test6 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting($"{sortColumnName} ACS").ToCollection<Employee>().Execute();
			Assert.AreEqual(emp1.EmployeeKey, test6[0].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test6[1].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test6[2].EmployeeKey);
			Assert.AreEqual(emp4.EmployeeKey, test6[3].EmployeeKey);


#if MYSQL || SQLITE
			const string expression = "CONCAT(FirstName, LastName)";
#elif POSTGRESQL

			const string expression = "First_Name || Last_Name";
#else
			const string expression = "FirstName + LastName";
#endif

			var test7 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting(new SortExpression(expression, SortDirection.Expression)).ToCollection<Employee>().Execute();
			Assert.AreEqual(emp1.EmployeeKey, test7[0].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test7[1].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test7[2].EmployeeKey);
			Assert.AreEqual(emp4.EmployeeKey, test7[3].EmployeeKey);

			var test8 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSortExpression(expression).ToCollection<Employee>().Execute();
			Assert.AreEqual(emp1.EmployeeKey, test8[0].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test8[1].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test8[2].EmployeeKey);
			Assert.AreEqual(emp4.EmployeeKey, test8[3].EmployeeKey);
		}
		finally
		{
			Release(dataSource);
		}
	}

#if !NO_ROW_SKIPPING

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void SkipTake(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var result = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).WithSorting("FirstName").WithLimits(10, 15).ToCollection<Employee>().Execute();
			Assert.AreEqual(15, result.Count, "Count");
			foreach (var item in result)
			{
				Assert.AreEqual(EmployeeSearchKey1000, item.Title, "Filter");
				Assert.IsTrue(int.Parse(item.FirstName) >= 10, "Range");
				Assert.IsTrue(int.Parse(item.FirstName) < 25, "Range");
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

#if SQLITE

		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void Sorting_ImmutableCollection(string dataSourceName, DataSourceType mode)
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

				var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("FirstName").ToCollection<EmployeeLookup>().WithConstructor<long, string, string>().Execute();
				Assert.AreEqual(emp1.EmployeeKey, test1[0].EmployeeKey);
				Assert.AreEqual(emp2.EmployeeKey, test1[1].EmployeeKey);
				Assert.AreEqual(emp3.EmployeeKey, test1[2].EmployeeKey);
				Assert.AreEqual(emp4.EmployeeKey, test1[3].EmployeeKey);

				var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting(new SortExpression("FirstName", SortDirection.Descending)).ToCollection<EmployeeLookup>().WithConstructor<long, string, string>().Execute();
				Assert.AreEqual(emp4.EmployeeKey, test2[0].EmployeeKey);
				Assert.AreEqual(emp3.EmployeeKey, test2[1].EmployeeKey);
				Assert.AreEqual(emp2.EmployeeKey, test2[2].EmployeeKey);
				Assert.AreEqual(emp1.EmployeeKey, test2[3].EmployeeKey);

				var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("LastName", "FirstName").ToCollection<EmployeeLookup>().WithConstructor<long, string, string>().Execute();
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

#elif MYSQL

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Sorting_ImmutableCollection(string dataSourceName, DataSourceType mode)
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

			var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("FirstName").ToCollection<EmployeeLookup>().WithConstructor<ulong, string, string>().Execute();
			Assert.AreEqual(emp1.EmployeeKey, test1[0].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test1[1].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test1[2].EmployeeKey);
			Assert.AreEqual(emp4.EmployeeKey, test1[3].EmployeeKey);

			var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting(new SortExpression("FirstName", SortDirection.Descending)).ToCollection<EmployeeLookup>().WithConstructor<ulong, string, string>().Execute();
			Assert.AreEqual(emp4.EmployeeKey, test2[0].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test2[1].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test2[2].EmployeeKey);
			Assert.AreEqual(emp1.EmployeeKey, test2[3].EmployeeKey);

			var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("LastName", "FirstName").ToCollection<EmployeeLookup>().WithConstructor<ulong, string, string>().Execute();
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

#else

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Sorting_ImmutableCollection(string dataSourceName, DataSourceType mode)
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

			var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("FirstName").ToCollection<EmployeeLookup>().WithConstructor<int, string, string>().Execute();
			Assert.AreEqual(emp1.EmployeeKey, test1[0].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test1[1].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test1[2].EmployeeKey);
			Assert.AreEqual(emp4.EmployeeKey, test1[3].EmployeeKey);

			var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting(new SortExpression("FirstName", SortDirection.Descending)).ToCollection<EmployeeLookup>().WithConstructor<int, string, string>().Execute();
			Assert.AreEqual(emp4.EmployeeKey, test2[0].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test2[1].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test2[2].EmployeeKey);
			Assert.AreEqual(emp1.EmployeeKey, test2[3].EmployeeKey);

			var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("LastName", "FirstName").ToCollection<EmployeeLookup>().WithConstructor<int, string, string>().Execute();
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

#endif

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void Sorting_InferredCollection_AutoTableSelection(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);
		try
		{
			var uniqueKey = Guid.NewGuid().ToString();

			var emp1 = new Employee() { FirstName = "A", LastName = "2", Title = uniqueKey };
			var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
			var emp3 = new Employee() { FirstName = "C", LastName = "1", Title = uniqueKey };
			var emp4 = new Employee() { FirstName = "D", LastName = "1", Title = uniqueKey };

			emp1 = dataSource.Insert(emp1).ToObject().Execute();
			emp2 = dataSource.Insert(emp2).ToObject().Execute();
			emp3 = dataSource.Insert(emp3).ToObject().Execute();
			emp4 = dataSource.Insert(emp4).ToObject().Execute();

			var test1 = dataSource.From<EmployeeLookup>(new { Title = uniqueKey }).WithSorting("FirstName").ToCollection(CollectionOptions.InferConstructor).Execute();
			Assert.AreEqual(emp1.EmployeeKey, test1[0].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test1[1].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test1[2].EmployeeKey);
			Assert.AreEqual(emp4.EmployeeKey, test1[3].EmployeeKey);

			var test2 = dataSource.From<EmployeeLookup>(new { Title = uniqueKey }).WithSorting(new SortExpression("FirstName", SortDirection.Descending)).ToCollection(CollectionOptions.InferConstructor).Execute();
			Assert.AreEqual(emp4.EmployeeKey, test2[0].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test2[1].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test2[2].EmployeeKey);
			Assert.AreEqual(emp1.EmployeeKey, test2[3].EmployeeKey);

			var test3 = dataSource.From<EmployeeLookup>(new { Title = uniqueKey }).WithSorting("LastName", "FirstName").ToCollection(CollectionOptions.InferConstructor).Execute();
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
	public void Sorting_InferredCollection(string dataSourceName, DataSourceType mode)
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

			var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("FirstName").ToCollection<EmployeeLookup>(CollectionOptions.InferConstructor).Execute();
			Assert.AreEqual(emp1.EmployeeKey, test1[0].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test1[1].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test1[2].EmployeeKey);
			Assert.AreEqual(emp4.EmployeeKey, test1[3].EmployeeKey);

			var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting(new SortExpression("FirstName", SortDirection.Descending)).ToCollection<EmployeeLookup>(CollectionOptions.InferConstructor).Execute();
			Assert.AreEqual(emp4.EmployeeKey, test2[0].EmployeeKey);
			Assert.AreEqual(emp3.EmployeeKey, test2[1].EmployeeKey);
			Assert.AreEqual(emp2.EmployeeKey, test2[2].EmployeeKey);
			Assert.AreEqual(emp1.EmployeeKey, test2[3].EmployeeKey);

			var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("LastName", "FirstName").ToCollection<EmployeeLookup>(CollectionOptions.InferConstructor).Execute();
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
