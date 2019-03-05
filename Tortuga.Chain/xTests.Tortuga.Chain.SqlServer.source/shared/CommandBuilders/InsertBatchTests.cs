using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Models;
using Tortuga.Chain;
using Xunit;
using Xunit.Abstractions;

namespace Tests.CommandBuilders
{
    public class InsertBatchTests : TestBase
    {
        const string TableType = "HR.EmployeeTable";
        public static BasicData Prime = new BasicData(s_PrimaryDataSource);
        public static RootData Root = new RootData(s_PrimaryDataSource);

        public InsertBatchTests(ITestOutputHelper output) : base(output)
        {
        }

#if SQL_SERVER

        [Theory, MemberData(nameof(Prime))]
        public void InsertBatch(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var key1000 = Guid.NewGuid().ToString();
            var employeeList = new List<Employee>();

            for (var i = 0; i < 1000; i++)
                employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var count = dataSource.InsertBatch(EmployeeTableName, employeeList, TableType).Execute();
                Assert.AreEqual(1000, count);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Prime))]
        public void InsertBatch_Identity(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            const string TableType = "HR.EmployeeTable";

            var key1000 = Guid.NewGuid().ToString();
            var employeeList = new List<Employee>();

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var employeeTable = dataSource.DatabaseMetadata.GetTableOrView(EmployeeTableName);
                var primaryColumn = employeeTable.Columns.SingleOrDefault(c => c.IsIdentity);
                if (primaryColumn == null) //SQLite
                    primaryColumn = employeeTable.Columns.SingleOrDefault(c => c.IsPrimaryKey);

                //Skipping ahead by 5
                var nextKey = 5 + dataSource.Sql($"SELECT Max({primaryColumn.QuotedSqlName}) FROM {employeeTable.Name.ToQuotedString()}").ToInt32().Execute();

                for (var i = 0; i < 1000; i++)
                    employeeList.Add(new Employee() { EmployeeKey = nextKey + i, FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

                var count = dataSource.InsertBatch(EmployeeTableName, employeeList, TableType, InsertOptions.IdentityInsert).Execute();
                Assert.AreEqual(1000, count);

                var newMax = dataSource.Sql($"SELECT Max({primaryColumn.QuotedSqlName}) FROM {employeeTable.Name.ToQuotedString()}").ToInt32().Execute();

                Assert.AreEqual(nextKey + 1000 - 1, newMax, "Identity insert didn't use the desired primary key values");
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Prime))]
        public void InsertBatch_Streaming(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var key1000 = Guid.NewGuid().ToString();

                dataSource.InsertBatch(EmployeeTableName, StreamRecords(key1000, 1000), TableType).Execute();
            }
            finally
            {
                Release(dataSource);
            }
        }

        IEnumerable<Employee> StreamRecords(string key, int maxRecords)
        {
            var i = 0;
            while (i < maxRecords)
            {
                yield return new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key };
                i++;
            }
        }

        [Theory, MemberData(nameof(Prime))]
        public void InsertBatch_SelectBack(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var key1000 = Guid.NewGuid().ToString();
            var employeeList = new List<Employee>();

            for (var i = 0; i < 1000; i++)
                employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var employeeList2 = dataSource.InsertBatch(EmployeeTableName, employeeList, TableType).ToCollection<EmployeeLookup>(CollectionOptions.InferConstructor).Execute();

                Assert.AreEqual(employeeList.Count, employeeList2.Count);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Prime))]
        public void InsertBatch_SelectKeys(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var key1000 = Guid.NewGuid().ToString();
            var employeeList = new List<Employee>();

            for (var i = 0; i < 1000; i++)
                employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var employeeList2 = dataSource.InsertBatch(EmployeeTableName, employeeList, TableType).ToInt32List().Execute();

                Assert.AreEqual(employeeList.Count, employeeList2.Count);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Root))]
        public void InsertBatch_AuditRules(string assemblyName, string dataSourceName)
        {
            var key1000 = Guid.NewGuid().ToString();
            var employeeList = new List<Employee>();

            for (var i = 0; i < 1000; i++)
                employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

            var dataSource = AttachRules(DataSource(dataSourceName));
            try
            {
                var employeeList2 = dataSource.InsertBatch(EmployeeTableName, employeeList, TableType).ToCollection<Employee>().Execute();
                Assert.AreEqual(employeeList.Count, employeeList2.Count);
                Assert.NotNull(employeeList2[0].UpdatedDate, "Updated date should have been set by the audit rules");
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif
    }
}
