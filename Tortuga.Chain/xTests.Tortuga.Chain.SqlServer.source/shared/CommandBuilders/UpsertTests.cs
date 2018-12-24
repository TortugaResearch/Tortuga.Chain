using System;
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