using Tests.Models;
using Tortuga.Chain;

namespace Tests.CommandBuilders
{
	[TestClass]
	public class DeleteTests : TestBase
	{
		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void DeleteTests_Delete(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				var original = new Employee()
				{
					FirstName = "Test",
					LastName = "Employee" + DateTime.Now.Ticks,
					Title = "Mail Room"
				};

				var key = dataSource.Insert(EmployeeTableName, original).ToInt32().Execute();
				var inserted = dataSource.GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();

				dataSource.Delete(EmployeeTableName, inserted).Execute();
			}
			finally
			{
				Release(dataSource);
			}
		}

		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void DeleteTests_Delete_Attribute(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				var original = new Employee()
				{
					FirstName = "Test",
					LastName = "Employee" + DateTime.Now.Ticks,
					Title = "Mail Room"
				};

				var key = dataSource.Insert(EmployeeTableName, original).ToInt32().Execute();
				var inserted = dataSource.GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();

				dataSource.Delete(inserted).Execute(); //reads the table name from the C# class

				try
				{
					dataSource.GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();
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
		public void DeleteTests_Delete_Implied(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				var original = new Employee()
				{
					FirstName = "Test",
					LastName = "Employee" + DateTime.Now.Ticks,
					Title = "Mail Room"
				};

				var key = dataSource.Insert(EmployeeTableName, original).ToInt32().Execute();

				dataSource.Delete(new HR.Employee() { EmployeeKey = key }).Execute();

				try
				{
					dataSource.GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();
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

#if SQL_SERVER_SDS || SQL_SERVER_MDS || SQL_SERVER_OLEDB //SQL Server has problems with CRUD operations that return values on tables with triggers.

		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void DeleteTests_Delete_Trigger(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				var original = new Employee()
				{
					FirstName = "Test",
					LastName = "Employee" + DateTime.Now.Ticks,
					Title = "Mail Room"
				};

				var key = dataSource.Insert(EmployeeTableName_Trigger, original).ToInt32().Execute();
				var inserted = dataSource.GetByKey(EmployeeTableName_Trigger, key).ToObject<Employee>().Execute();

				dataSource.Delete(EmployeeTableName_Trigger, inserted).Execute();
			}
			finally
			{
				Release(dataSource);
			}
		}

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

		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void DeleteWithFilter_Where(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				var lookupKey = Guid.NewGuid().ToString();
				for (var i = 0; i < 10; i++)
					dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

				var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
				var keysToUpdate = allKeys.Take(5).ToList();

				var updatedRows = dataSource.DeleteWithFilter(EmployeeTableName, $"Title = '{lookupKey}' AND MiddleName Is Null").ToCollection<Employee>().Execute();

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
		public void DeleteWithFilter_WhereArg(string dataSourceName, DataSourceType mode)
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

				var updatedRows = dataSource.DeleteWithFilter(EmployeeTableName, whereClause, new { @LookupKey = lookupKey }).ToCollection<Employee>().Execute();

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
		public void DeleteWithFilter_Filter(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				var lookupKey = Guid.NewGuid().ToString();
				for (var i = 0; i < 10; i++)
					dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

				var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
				var keysToUpdate = allKeys.Take(5).ToList();

				var updatedRows = dataSource.DeleteWithFilter(EmployeeTableName, new { Title = lookupKey, MiddleName = (string)null }).ToCollection<Employee>().Execute();

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
		public void DeleteWithFilter_Where_SoftDelete(string dataSourceName)
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

				var updatedRows = dataSourceRules.DeleteWithFilter(CustomerTableName, $"FullName = '{lookupKey}' AND State = 'BB'").ToCollection<Customer>().Execute();

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
		public void DeleteWithFilter_WhereArg_SoftDelete(string dataSourceName)
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

				var updatedRows = dataSourceRules.DeleteWithFilter(CustomerTableName, whereClause, new { @Lookup = lookupKey, State = "BB" }).ToCollection<Customer>().Execute();

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
		public void DeleteWithFilter_Filter_SoftDelete(string dataSourceName)
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

				var updatedRows = dataSourceRules.DeleteWithFilter(CustomerTableName, new { @FullName = lookupKey, State = "BB" }).ToCollection<Customer>().Execute();

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
		public void Delete_TableAndView_Strict(string dataSourceName)
		{
			var dataSource = DataSource(dataSourceName, DataSourceType.Strict);
			try
			{
				var original = new Employee()
				{
					FirstName = "Test",
					LastName = "Employee" + DateTime.Now.Ticks,
					Title = "Mail Room"
				};

				var key = dataSource.Insert(original).ToInt32().Execute();
				var inserted = dataSource.GetByKey<EmployeeWithView>(key).ToObject().Execute();

				dataSource.Delete(inserted).Execute();
			}
			finally
			{
				Release(dataSource);
			}
		}

		[DataTestMethod, RootData(DataSourceGroup.Primary)]
		public void Delete_TableAndView_Strict_ReadDeleted(string dataSourceName)
		{
			var dataSource = DataSource(dataSourceName, DataSourceType.Strict);
			try
			{
				var manager = new Employee()
				{
					FirstName = "Test",
					LastName = "Employee" + DateTime.Now.Ticks,
					Title = "Mail Room"
				};

				var managerKey = dataSource.Insert(manager).ToInt32().Execute();


				var original = new Employee()
				{
					FirstName = "Test",
					LastName = "Employee" + DateTime.Now.Ticks,
					Title = "Mail Room",
					ManagerKey = managerKey
				};

				var key = dataSource.Insert(original).ToInt32().Execute();
				var inserted = dataSource.GetByKey<EmployeeWithView>(key).ToObject().Execute();

				try
				{
					var deletedRecord = dataSource.Delete(inserted).ToObject().Execute();
					Assert.Fail($"Expected a {nameof(MappingException)}");
				}
				catch (MappingException) { }

			}
			finally
			{
				Release(dataSource);
			}
		}



#if CLASS_2
		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void Truncate(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName);
			try
			{

				dataSource.Insert(new Sales.Location() { LocationName = "Example " + DateTime.Now.Ticks }).Execute();
				Assert.IsTrue(dataSource.From<Sales.Location>().AsCount().Execute() > 0, "Expected at least one row");
				dataSource.Truncate<Sales.Location>().Execute();
				Assert.IsTrue(dataSource.From<Sales.Location>().AsCount().Execute() == 0, "Expected zero rows");

			}
			finally
			{
				Release(dataSource);
			}
		}
#endif
	}

	namespace HR
	{
		public class Employee
		{
			public int EmployeeKey { get; set; }
		}
	}

	namespace Sales
	{
		public class Location
		{
			public int? LocationKey { get; set; }
			public string LocationName { get; set; }

		}
	}
}
