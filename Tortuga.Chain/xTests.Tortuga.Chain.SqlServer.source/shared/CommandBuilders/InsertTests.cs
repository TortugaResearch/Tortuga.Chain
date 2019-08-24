using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Models;
using Tortuga.Chain;
using Xunit;
using Xunit.Abstractions;

namespace Tests.CommandBuilders
{
    public class InsertTests : TestBase
    {
        public static BasicData Prime = new BasicData(s_PrimaryDataSource);

        public InsertTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory, MemberData(nameof(Prime))]
        public void Insert(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var lookupKey = Guid.NewGuid().ToString();
                for (var i = 0; i < 10; i++)
                    dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).Execute();

                var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
                Assert.Equal(10, allKeys.Count, "Count if inserted rows is off.");
            }
            finally
            {
                Release(dataSource);
            }
        }

#if !POSTGRESQL

        [Theory, MemberData(nameof(Prime))]
        public void Identity_Insert(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var lookupKey = Guid.NewGuid().ToString();
                var employeeTable = dataSource.DatabaseMetadata.GetTableOrView(EmployeeTableName);
                var primaryColumn = employeeTable.Columns.SingleOrDefault(c => c.IsIdentity);
                if (primaryColumn == null) //SQLite
                    primaryColumn = employeeTable.PrimaryKeyColumns.SingleOrDefault();

                //Skipping ahead by 5
                var nextKey = 5 + dataSource.Sql($"SELECT Max({primaryColumn.QuotedSqlName}) FROM {employeeTable.Name.ToQuotedString()}").ToInt32().Execute();

                var keyForOverriddenRow = dataSource.Insert(EmployeeTableName, new Employee() { EmployeeKey = nextKey, FirstName = "0000", LastName = "Z" + (int.MaxValue), Title = lookupKey, MiddleName = "A0" }, InsertOptions.IdentityInsert).ToInt32().Execute();

                Assert.Equal(nextKey, keyForOverriddenRow, "Identity column was not correctly overridden");

                var keyForNewRow = dataSource.Insert(EmployeeTableName, new Employee() { FirstName = "0001", LastName = "Z" + (int.MaxValue - 1), Title = lookupKey, MiddleName = null }).ToInt32().Execute();
                Assert.Equal(keyForOverriddenRow + 1, keyForNewRow, "Next inserted value didn't have the correct key");
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

        [Theory, MemberData(nameof(Prime))]
        public void InsertEchoObject(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var list = new List<Employee>();
                var lookupKey = Guid.NewGuid().ToString();
                for (var i = 0; i < 10; i++)
                    list.Add(dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute());

                var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
                Assert.Equal(list.Count, allKeys.Count, "Count if inserted rows is off.");
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Prime))]
        public void InsertEchoNewKey(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var list = new List<int>();
                var lookupKey = Guid.NewGuid().ToString();
                for (var i = 0; i < 10; i++)
                    list.Add(dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToInt32().Execute());

                var allKeys = dataSource.From(EmployeeTableName, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
                Assert.Equal(list.Count, allKeys.Count, "Count if inserted rows is off.");
            }
            finally
            {
                Release(dataSource);
            }
        }

#if SQL_SERVER || OLE_SQL_SERVER //SQL Server has problems with CRUD operations that return values on tables with triggers.

        [Theory, MemberData(nameof(Prime))]
        public void Insert_Trigger(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var lookupKey = Guid.NewGuid().ToString();
                for (var i = 0; i < 10; i++)
                    dataSource.Insert(EmployeeTableName_Trigger, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).Execute();

                var allKeys = dataSource.From(EmployeeTableName_Trigger, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
                Assert.Equal(10, allKeys.Count, "Count if inserted rows is off.");
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Prime))]
        public void InsertEchoObject_Trigger(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var list = new List<Employee>();
                var lookupKey = Guid.NewGuid().ToString();
                for (var i = 0; i < 10; i++)
                    list.Add(dataSource.Insert(EmployeeTableName_Trigger, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute());

                var allKeys = dataSource.From(EmployeeTableName_Trigger, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
                Assert.Equal(list.Count, allKeys.Count, "Count if inserted rows is off.");
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Prime))]
        public void InsertEchoNewKey_Trigger(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var list = new List<int>();
                var lookupKey = Guid.NewGuid().ToString();
                for (var i = 0; i < 10; i++)
                    list.Add(dataSource.Insert(EmployeeTableName_Trigger, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = lookupKey, MiddleName = i % 2 == 0 ? "A" + i : null }).ToInt32().Execute());

                var allKeys = dataSource.From(EmployeeTableName_Trigger, new { Title = lookupKey }).ToInt32List("EmployeeKey").Execute();
                Assert.Equal(list.Count, allKeys.Count, "Count if inserted rows is off.");
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif
    }
}
