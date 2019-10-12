using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Tests.Models;

namespace Tests.CommandBuilders
{
    [TestClass]
    public class InsertBulkTests : TestBase
    {
#if SQL_SERVER_SDS || SQL_SERVER_MDS

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public void InsertBulk(string dataSourceName, DataSourceType mode)
        {
            var key1000 = Guid.NewGuid().ToString();
            var employeeList = new List<Employee>();

            for (var i = 0; i < 1000; i++)
                employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var count = dataSource.InsertBulk(EmployeeTableName, employeeList).Execute();
                Assert.AreEqual(1000, count);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public void InsertBulk_IdentityInsert(string dataSourceName, DataSourceType mode)
        {
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

                var count = dataSource.InsertBulk(EmployeeTableName, employeeList, SqlBulkCopyOptions.KeepIdentity).Execute();
                Assert.AreEqual(1000, count);

                var newMax = dataSource.Sql($"SELECT Max({primaryColumn.QuotedSqlName}) FROM {employeeTable.Name.ToQuotedString()}").ToInt32().Execute();

                Assert.AreEqual(nextKey + 1000 - 1, newMax, "Identity insert didn't use the desired primary key values");
            }
            finally
            {
                Release(dataSource);
            }
        }

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public void InsertBulk_WithStreaming(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var key = Guid.NewGuid().ToString();
                var employeeList = new List<Employee>();

                var count = dataSource.InsertBulk(EmployeeTableName, StreamRecords(key, 1000)).WithStreaming().Execute();
                Assert.AreEqual(-1, count); //streaming prevents returning a row count;
            }
            finally
            {
                Release(dataSource);
            }
        }

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public void InsertBulk_WithEvents(string dataSourceName, DataSourceType mode)
        {
            long runningCount = 0;
            var key = Guid.NewGuid().ToString();
            var employeeList = new List<Employee>();

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var count = dataSource.InsertBulk(EmployeeTableName, StreamRecords(key, 1000)).WithBatchSize(75).WithNotifications((s, e) =>
                {
                    Debug.WriteLine($"Copied {e.RowsCopied} rows");
                    runningCount = e.RowsCopied;
                }, 90).Execute();

                Assert.AreEqual(-1, count); //streaming prevents returning a row count;
                Assert.AreNotEqual(0, runningCount, "record count is wrong"); //but we can get it another way
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

#endif
    }
}
