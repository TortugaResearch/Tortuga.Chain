using Tests.Models;
using Tortuga.Chain;

namespace Tests.CommandBuilders
{
	[TestClass]
	public class UpdateTests : TestBase
	{
		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void FailedUpdate_ViewNeedsKeys(string dataSourceName, DataSourceType mode)
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

				var inserted = dataSource.Insert(EmployeeTableName, original).ToObject<Employee>().Execute();

				inserted.FirstName = "Changed";
				inserted.Title = "Also Changed";

				try
				{
					dataSource.Update(EmployeeViewName, inserted).ToObject<ChangeTrackingEmployee>().Execute();
					Assert.Fail("Expected a mapping exception.");
				}
				catch (MappingException)
				{
					//OK
				}
			}
			finally
			{
				Release(dataSource);
			}
		}

#if SQL_SERVER_SDS || SQL_SERVER_MDS || SQL_SERVER_OLEDB

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public void UpdateViaView(string dataSourceName, DataSourceType mode)
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

                var inserted = dataSource.Insert(EmployeeTableName, original).ToObject<ChangeTrackingEmployee>().Execute();

                inserted.FirstName = "Changed";
                inserted.Title = "Also Changed";

                var updated = dataSource.Update(EmployeeViewName, inserted).WithKeys("EmployeeKey").ToObject<ChangeTrackingEmployee>().Execute();
                Assert.AreEqual(inserted.FirstName, updated.FirstName, "FirstName should have changed");
                Assert.AreEqual(inserted.Title, updated.Title, "Title should have changed");
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void ChangeTrackingTest(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				var original = new ChangeTrackingEmployee()
				{
					FirstName = "Test",
					LastName = "Employee" + DateTime.Now.Ticks,
					Title = "Mail Room",
					EmployeeId = Guid.NewGuid().ToString()
				};

				var inserted = dataSource.Insert(EmployeeTableName, original).ToObject<ChangeTrackingEmployee>().Execute();
				Assert.IsFalse(inserted.IsChanged, "Accept changes wasn't called by the materializer");

				inserted.FirstName = "Changed";
				inserted.AcceptChanges(); //only properties changed AFTER this line should actually be updated in the database
				inserted.Title = "Also Changed";

				var updated = dataSource.Update(EmployeeTableName, inserted, UpdateOptions.ChangedPropertiesOnly).ToObject<ChangeTrackingEmployee>().Execute();
				Assert.AreEqual(original.FirstName, updated.FirstName, "FirstName shouldn't have changed");
				Assert.AreEqual(original.LastName, updated.LastName, "LastName shouldn't have changed");
				Assert.AreEqual(inserted.Title, updated.Title, "Title should have changed");
			}
			finally
			{
				Release(dataSource);
			}
		}

		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void ChangeTrackingTest_NothingChanged(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				var original = new ChangeTrackingEmployee()
				{
					FirstName = "Test",
					LastName = "Employee" + DateTime.Now.Ticks,
					Title = "Mail Room",
					EmployeeId = Guid.NewGuid().ToString()
				};

				var inserted = dataSource.Insert(EmployeeTableName, original).ToObject<ChangeTrackingEmployee>().Execute();
				Assert.IsFalse(inserted.IsChanged, "Accept changes wasn't called by the materializer");

				try
				{
					var updated = dataSource.Update(EmployeeTableName, inserted, UpdateOptions.ChangedPropertiesOnly).ToObject<ChangeTrackingEmployee>().Execute();
					Assert.Fail("Exception Expected");
				}
				catch (ArgumentException)
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
		public void FailedUpdateTest(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				var original = new ChangeTrackingEmployee()
				{
					FirstName = "Test",
					LastName = "Employee" + DateTime.Now.Ticks,
					Title = "Mail Room",
					EmployeeId = Guid.NewGuid().ToString()
				};

				var inserted = dataSource.Insert(EmployeeTableName, original).ToObject<ChangeTrackingEmployee>().Execute();
				inserted.Title = "President";

				dataSource.Update(EmployeeTableName, inserted).Execute();
				dataSource.Delete(EmployeeTableName, inserted).Execute();

				try
				{
					dataSource.Update(EmployeeTableName, inserted).Execute();
					Assert.Fail("Expected a MissingDataException when trying to update a deleted row but didn't get one.");
				}
				catch (MissingDataException)
				{
					//pass
				}

				dataSource.Update(EmployeeTableName, inserted, UpdateOptions.IgnoreRowsAffected).Execute(); //no error
			}
			finally
			{
				Release(dataSource);
			}
		}

#if !SQLite

		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void ChangeTrackingTest_Compiled(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				var original = new ChangeTrackingEmployee()
				{
					FirstName = "Test",
					LastName = "Employee" + DateTime.Now.Ticks,
					Title = "Mail Room",
					EmployeeId = Guid.NewGuid().ToString()
				};

				var inserted = dataSource.Insert(EmployeeTableName, original).Compile().ToObject<ChangeTrackingEmployee>().Execute();
				inserted.FirstName = "Changed";
				inserted.AcceptChanges(); //only properties changed AFTER this line should actually be updated in the database
				inserted.Title = "Also Changed";

				var updated = dataSource.Update(EmployeeTableName, inserted, UpdateOptions.ChangedPropertiesOnly).Compile().ToObject<ChangeTrackingEmployee>().Execute();
				Assert.AreEqual(original.FirstName, updated.FirstName, "FirstName shouldn't have changed");
				Assert.AreEqual(original.LastName, updated.LastName, "LastName shouldn't have changed");
				Assert.AreEqual(inserted.Title, updated.Title, "Title should have changed");
			}
			finally
			{
				Release(dataSource);
			}
		}

		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void ChangeTrackingTest_NothingChanged_Compiled(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				var original = new ChangeTrackingEmployee()
				{
					FirstName = "Test",
					LastName = "Employee" + DateTime.Now.Ticks,
					Title = "Mail Room",
					EmployeeId = Guid.NewGuid().ToString()
				};

				var inserted = dataSource.Insert(EmployeeTableName, original).Compile().ToObject<ChangeTrackingEmployee>().Execute();

				try
				{
					var updated = dataSource.Update(EmployeeTableName, inserted, UpdateOptions.ChangedPropertiesOnly).Compile().ToObject<ChangeTrackingEmployee>().Execute();
					Assert.Fail("Exception Expected");
				}
				catch (ArgumentException)
				{
					//pass
				}
			}
			finally
			{
				Release(dataSource);
			}
		}

#endif

		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void UpdateByKey(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				var lookupKey = Guid.NewGuid().ToString();
				for (var i = 0; i < 10; i++)
					dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

				var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
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

				var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
				var keysToUpdate = allKeys.Take(5).ToList();

				var newValues = new { FirstName = "Bob" };
				var updatedRows = dataSource.UpdateByKeyList(EmployeeTableName, newValues, (System.Collections.Generic.IEnumerable<int>)keysToUpdate).ToCollection<Employee>().Execute();

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

#if SQL_SERVER_SDS || SQL_SERVER_MDS || SQL_SERVER_OLEDB //SQL Server has problems with CRUD operations that return values on tables with triggers.

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public void ChangeTrackingTest_Trigger(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var original = new ChangeTrackingEmployee()
                {
                    FirstName = "Test",
                    LastName = "Employee" + DateTime.Now.Ticks,
                    Title = "Mail Room",
                    EmployeeId = Guid.NewGuid().ToString()
                };

                var inserted = dataSource.Insert(EmployeeTableName_Trigger, original).ToObject<ChangeTrackingEmployee>().Execute();
                Assert.IsFalse(inserted.IsChanged, "Accept changes wasn't called by the materializer");

                inserted.FirstName = "Changed";
                inserted.AcceptChanges(); //only properties changed AFTER this line should actually be updated in the database
                inserted.Title = "Also Changed";

                var updated = dataSource.Update(EmployeeTableName_Trigger, inserted, UpdateOptions.ChangedPropertiesOnly).ToObject<ChangeTrackingEmployee>().Execute();
                Assert.AreEqual(original.FirstName, updated.FirstName, "FirstName shouldn't have changed");
                Assert.AreEqual(original.LastName, updated.LastName, "LastName shouldn't have changed");
                Assert.AreEqual(inserted.Title, updated.Title, "Title should have changed");
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif
	}
}
