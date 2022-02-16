using Tests.Models;

namespace Tests.CommandBuilders
{
	[TestClass]
	public class UpsertTests : TestBase
	{
#if !ACCESS

		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void BasicUpsertTest(string dataSourceName, DataSourceType mode)
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

				var inserted = dataSource.Upsert(EmployeeTableName, original).ToObject<Employee>().Execute();

				inserted.FirstName = "Changed";
				inserted.Title = "Also Changed";

				var updated = dataSource.Upsert(EmployeeTableName, inserted).ToObject<Employee>().Execute();
				Assert.AreEqual(inserted.FirstName, updated.FirstName, "FirstName shouldn't have changed");
				Assert.AreEqual(original.LastName, updated.LastName, "LastName shouldn't have changed");
				Assert.AreEqual(inserted.Title, updated.Title, "Title should have changed");
			}
			finally
			{
				Release(dataSource);
			}
		}

		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void AlternateKeyUpsertTest(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				var original = new Employee()
				{
					FirstName = "Test",
					LastName = "Employee" + Guid.NewGuid().ToString(),
					Title = "Mail Room"
				};

				var employeeKey = dataSource.Upsert(EmployeeTableName, original).ToObject<Employee>().Execute().EmployeeKey;

				var updater = new EmployeeWithoutKey()
				{
					EmployeeId = original.EmployeeId,
					FirstName = "Changed",
					Title = "Also Changed",
					LastName = original.LastName
				};

				var updated = dataSource.Upsert(EmployeeTableName, updater).WithKeys("EmployeeId").ToObject<Employee>().Execute();
				Assert.AreEqual(employeeKey, updated.EmployeeKey, "EmployeeKey should have been read.");
				Assert.AreEqual(updater.FirstName, updated.FirstName, "FirstName should have changed");
				Assert.AreEqual(original.LastName, updated.LastName, "LastName shouldn't have changed");
				Assert.AreEqual(updater.Title, updated.Title, "Title should have changed");
			}
			finally
			{
				Release(dataSource);
			}
		}

#if !POSTGRESQL

		[DataTestMethod, BasicData(DataSourceGroup.Primary)]
		public void UpsertTest_Identity_Insert(string dataSourceName, DataSourceType mode)
		{
			var dataSource = DataSource(dataSourceName, mode);
			try
			{
				var employeeTable = dataSource.DatabaseMetadata.GetTableOrView(EmployeeTableName);
				var primaryColumn = employeeTable.Columns.SingleOrDefault(c => c.IsIdentity);
				if (primaryColumn == null) //SQLite
					primaryColumn = employeeTable.PrimaryKeyColumns.SingleOrDefault();

				//Skipping ahead by 500
				var nextKey = 500 + dataSource.Sql($"SELECT Max({primaryColumn.QuotedSqlName}) FROM {employeeTable.Name.ToQuotedString()}").ToInt32().Execute();

				var original = new Employee()
				{
					FirstName = "Test",
					LastName = "Employee" + DateTime.Now.Ticks,
					Title = "Mail Room",
					EmployeeKey = nextKey
				};

				var inserted = dataSource.Upsert(EmployeeTableName, original, Tortuga.Chain.UpsertOptions.IdentityInsert).ToObject<Employee>().Execute();

				inserted.FirstName = "Changed";
				inserted.Title = "Also Changed";

				var updated = dataSource.Upsert(EmployeeTableName, inserted, Tortuga.Chain.UpsertOptions.IdentityInsert).ToObject<Employee>().Execute();
				Assert.AreEqual(inserted.FirstName, updated.FirstName, "FirstName shouldn't have changed");
				Assert.AreEqual(original.LastName, updated.LastName, "LastName shouldn't have changed");
				Assert.AreEqual(inserted.Title, updated.Title, "Title should have changed");
			}
			finally
			{
				Release(dataSource);
			}
		}

#endif

#if SQL_SERVER_SDS || SQL_SERVER_MDS || SQL_SERVER_OLEDB //SQL Server has problems with CRUD operations that return values on tables with triggers.

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public void BasicUpsertTest_Trigger(string dataSourceName, DataSourceType mode)
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

                var inserted = dataSource.Upsert(EmployeeTableName_Trigger, original).ToObject<Employee>().Execute();

                inserted.FirstName = "Changed";
                inserted.Title = "Also Changed";

                var updated = dataSource.Upsert(EmployeeTableName_Trigger, inserted).ToObject<Employee>().Execute();
                Assert.AreEqual(inserted.FirstName, updated.FirstName, "FirstName shouldn't have changed");
                Assert.AreEqual(original.LastName, updated.LastName, "LastName shouldn't have changed");
                Assert.AreEqual(inserted.Title, updated.Title, "Title should have changed");
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

#endif
	}
}
