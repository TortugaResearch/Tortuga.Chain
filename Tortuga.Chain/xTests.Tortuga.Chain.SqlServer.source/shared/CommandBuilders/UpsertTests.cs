using System;
using System.Linq;
using Tests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Tests.CommandBuilders
{
    public class UpsertTests : TestBase
    {
        public static BasicData Prime = new BasicData(s_PrimaryDataSource);

        public UpsertTests(ITestOutputHelper output) : base(output)
        {
        }

#if !ACCESS

        [Theory, MemberData(nameof(Prime))]
        public void BasicUpsertTest(string assemblyName, string dataSourceName, DataSourceType mode)
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

        [Theory, MemberData(nameof(Prime))]
        public void AlternateKeyUpsertTest(string assemblyName, string dataSourceName, DataSourceType mode)
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

                var inserted = dataSource.Upsert(EmployeeTableName, original).ToObject<Employee>().Execute();
                var employeeKey = inserted.EmployeeKey;

                inserted.EmployeeKey = null; //we're not matching on this anyways.
                inserted.FirstName = "Changed";
                inserted.Title = "Also Changed";

                var updated = dataSource.Upsert(EmployeeTableName, inserted).MatchOn("LastName").ToObject<Employee>().Execute();
                Assert.AreEqual(employeeKey, updated.EmployeeKey, "EmployeeKey should have been read.");
                Assert.AreEqual(inserted.FirstName, updated.FirstName, "FirstName shouldn't have changed");
                Assert.AreEqual(original.LastName, updated.LastName, "LastName shouldn't have changed");
                Assert.AreEqual(inserted.Title, updated.Title, "Title should have changed");
            }
            finally
            {
                Release(dataSource);
            }
        }

#if !POSTGRESQL

        [Theory, MemberData(nameof(Prime))]
        public void UpsertTest_Identity_Insert(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var employeeTable = dataSource.DatabaseMetadata.GetTableOrView(EmployeeTableName);
                var primaryColumn = employeeTable.Columns.SingleOrDefault(c => c.IsIdentity);
                if (primaryColumn == null) //SQLite
                    primaryColumn = employeeTable.Columns.SingleOrDefault(c => c.IsPrimaryKey);

                //Skipping ahead by 5
                var nextKey = 5 + dataSource.Sql($"SELECT Max({primaryColumn.QuotedSqlName}) FROM {employeeTable.Name.ToQuotedString()}").ToInt32().Execute();

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

#if SQL_SERVER || OLE_SQL_SERVER //SQL Server has problems with CRUD operations that return values on tables with triggers.

        [Theory, MemberData(nameof(Prime))]
        public void BasicUpsertTest_Trigger(string assemblyName, string dataSourceName, DataSourceType mode)
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
