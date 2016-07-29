using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tests.Models;
using Tortuga.Chain;
using Xunit;
using Xunit.Abstractions;

namespace Tests.CommandBuilders
{
    public class FromTests : TestBase
    {
        public static BasicData Prime = new BasicData(s_PrimaryDataSource);
        public static TablesAndViewData TablesAndViews = new TablesAndViewData(s_DataSources.Values);
        public static TablesAndViewColumnsData TablesAndViewsWithColumns = new TablesAndViewColumnsData(s_DataSources.Values);

#if SQL_SERVER
        public static TablesAndViewLimitData<SqlServerLimitOption> TablesAndViewLimit = new TablesAndViewLimitData<SqlServerLimitOption>(s_DataSources.Values);
#elif SQLITE
        public static TablesAndViewLimitData<SQLiteLimitOption> TablesAndViewLimit = new TablesAndViewLimitData<SQLiteLimitOption>(s_DataSources.Values);
#elif POSTGRESQL
        public static TablesAndViewLimitData<PostgreSqlLimitOption> TablesAndViewLimit = new TablesAndViewLimitData<PostgreSqlLimitOption>(s_DataSources.Values);
#elif ACCESS
        public static TablesAndViewLimitData<AccessLimitOption> TablesAndViewLimit = new TablesAndViewLimitData<AccessLimitOption>(s_DataSources.Values);
#endif

        public FromTests(ITestOutputHelper output) : base(output)
        {

        }

        [Theory, MemberData("TablesAndViews")]
        public void ToDynamicCollection(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

                var result = dataSource.From(tableName).WithLimits(10).ToDynamicCollection().Execute();
                Assert.True(result.Count <= 10);
                if (result.Count > 0)
                {
                    var first = (IDictionary<string, object>)result.First();
                    Assert.Equal(table.Columns.Count, first.Count);
                }

            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("TablesAndViews")]
        public async Task ToDynamicCollection_Async(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = await DataSourceAsync(dataSourceName, mode);
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

                var result = await dataSource.From(tableName).WithLimits(10).ToDynamicCollection().ExecuteAsync();
                Assert.True(result.Count <= 10);
                if (result.Count > 0)
                {
                    var row = (IDictionary<string, object>)result.First();
                    Assert.Equal(table.Columns.Count, row.Count);
                }

            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("TablesAndViews")]
        public void ToDynamicObject(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

                var result = dataSource.From(tableName).WithLimits(1).ToDynamicObject(RowOptions.AllowEmptyResults).Execute();
                if (result != null)
                {
                    var row = (IDictionary<string, object>)result;
                    Assert.Equal(table.Columns.Count, row.Count);
                }
            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("TablesAndViews")]
        public async Task ToDynamicObject_Async(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = await DataSourceAsync(dataSourceName, mode);
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

                var result = await dataSource.From(tableName).WithLimits(1).ToDynamicObject(RowOptions.AllowEmptyResults).ExecuteAsync();
                if (result != null)
                {
                    var row = (IDictionary<string, object>)result;
                    Assert.Equal(table.Columns.Count, row.Count);
                }
            }
            finally
            {
                Release(dataSource);
            }

        }



        [Theory, MemberData("TablesAndViews")]
        public void ToDataTable(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

                var result = dataSource.From(tableName).WithLimits(10).ToDataTable().Execute();
                Assert.True(result.Rows.Count <= 10);
                Assert.Equal(table.Columns.Count, result.Columns.Count);

            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("TablesAndViews")]
        public async Task ToDataTable_Async(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = await DataSourceAsync(dataSourceName, mode);
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

                var result = await dataSource.From(tableName).WithLimits(10).ToDataTable().ExecuteAsync();
                Assert.True(result.Rows.Count <= 10);
                Assert.Equal(table.Columns.Count, result.Columns.Count);

            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("TablesAndViews")]
        public void ToDataRow(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

                var result = dataSource.From(tableName).WithLimits(1).ToDataRow(RowOptions.AllowEmptyResults).Execute();
                if (result != null)
                {
                    Assert.Equal(table.Columns.Count, result.Table.Columns.Count);
                }
            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("TablesAndViews")]
        public async Task ToDataRow_Async(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = await DataSourceAsync(dataSourceName, mode);
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

                var result = await dataSource.From(tableName).WithLimits(1).ToDataRow(RowOptions.AllowEmptyResults).ExecuteAsync();
                if (result != null)
                {
                    Assert.Equal(table.Columns.Count, result.Table.Columns.Count);
                }

            }
            finally
            {
                Release(dataSource);
            }

        }



        [Theory, MemberData("TablesAndViews")]
        public void ToTable(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

                var result = dataSource.From(tableName).WithLimits(10).ToTable().Execute();
                Assert.True(result.Rows.Count <= 10);
                Assert.Equal(table.Columns.Count, result.ColumnNames.Count);

            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("TablesAndViews")]
        public async Task ToTable_Async(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = await DataSourceAsync(dataSourceName, mode);
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

                var result = await dataSource.From(tableName).WithLimits(10).ToTable().ExecuteAsync();
                Assert.True(result.Rows.Count <= 10);
                Assert.Equal(table.Columns.Count, result.ColumnNames.Count);

            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("TablesAndViews")]
        public void ToRow(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

                var result = dataSource.From(tableName).WithLimits(1).ToRow(RowOptions.AllowEmptyResults).Execute();
                if (result != null)
                {
                    Assert.Equal(table.Columns.Count, result.Count);
                }
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("TablesAndViews")]
        public async Task ToRow_Async(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = await DataSourceAsync(dataSourceName, mode);
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

                var result = await dataSource.From(tableName).WithLimits(1).ToRow(RowOptions.AllowEmptyResults).ExecuteAsync();
                if (result != null)
                {
                    Assert.Equal(table.Columns.Count, result.Count);
                }

            }
            finally
            {
                Release(dataSource);
            }

        }



        [Theory, MemberData("TablesAndViewLimit")]
        public void ToTable_WithLimit(string assemblyName, string dataSourceName, DataSourceType mode, string tableName, LimitOptions limitOptions)
        {
            var dataSource = DataSource(dataSourceName, mode);
            WriteLine($"Table {tableName}, Limit Option {limitOptions}");
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

                var prep = ((IClass1DataSource)dataSource).From(tableName).WithLimits(10, limitOptions);
                switch (limitOptions)
                {
                    case LimitOptions.RowsWithTies:
                    case LimitOptions.PercentageWithTies:
                        prep = prep.WithSorting(table.Columns[0].SqlName);
                        break;
                }
                var result = prep.ToTable().Execute();
                //Assert.True(result.Rows.Count <= 10, $"Row count was {result.Rows.Count}");
                Assert.Equal(table.Columns.Count, result.ColumnNames.Count);

            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("TablesAndViewLimit")]
        public async Task ToTable_WithLimit_Async(string assemblyName, string dataSourceName, DataSourceType mode, string tableName, LimitOptions limitOptions)
        {
            var dataSource = await DataSourceAsync(dataSourceName, mode);
            WriteLine($"Table {tableName}, Limit Option {limitOptions}");
            try
            {
                var table = dataSource.DatabaseMetadata.GetTableOrView(tableName);

                var prep = ((IClass1DataSource)dataSource).From(tableName).WithLimits(10, limitOptions);
                switch (limitOptions)
                {
                    case LimitOptions.RowsWithTies:
                    case LimitOptions.PercentageWithTies:
                        prep = prep.WithSorting(table.Columns[0].SqlName);
                        break;
                }
                var result = await prep.ToTable().ExecuteAsync();
                //Assert.True(result.Rows.Count <= 10, $"Row count was {result.Rows.Count}");
                Assert.Equal(table.Columns.Count, result.ColumnNames.Count);

            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("TablesAndViews")]
        public void Count(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            WriteLine($"Table {tableName}");
            try
            {
                var count = dataSource.From(tableName).AsCount().Execute();
                Assert.True(count >= 0);
            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("TablesAndViews")]
        public async Task Count_Async(string assemblyName, string dataSourceName, DataSourceType mode, string tableName)
        {
            var dataSource = await DataSourceAsync(dataSourceName, mode);
            WriteLine($"Table {tableName}");
            try
            {
                var count = await dataSource.From(tableName).AsCount().ExecuteAsync();
                Assert.True(count >= 0);
            }
            finally
            {
                Release(dataSource);
            }

        }

#if NO_DISTINCT_COUNT

        [Theory, MemberData("TablesAndViewsWithColumns")]
        public void CountByColumn(string assemblyName, string dataSourceName, DataSourceType mode, string tableName, string columnName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            WriteLine($"Table {tableName}");
            try
            {
                var columnType = dataSource.DatabaseMetadata.GetTableOrView(tableName).Columns[columnName].TypeName;
                if (columnType == "xml" || columnType == "ntext" || columnType == "text" || columnType == "image")
                    return; //SQL Server limitation

                var count = dataSource.From(tableName).AsCount(columnName).Execute();
                Assert.True(count >= 0, "Count cannot be less than zero");
            }
            finally
            {
                Release(dataSource);
            }
        }
#else
        [Theory, MemberData("TablesAndViewsWithColumns")]
        public void CountByColumn(string assemblyName, string dataSourceName, DataSourceType mode, string tableName, string columnName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            WriteLine($"Table {tableName}");
            try
            {
                var columnType = dataSource.DatabaseMetadata.GetTableOrView(tableName).Columns[columnName].TypeName;
                if (columnType == "xml" || columnType == "ntext" || columnType == "text" || columnType == "image")
                    return; //SQL Server limitation

                var count = dataSource.From(tableName).AsCount(columnName).Execute();
                var countDistinct = dataSource.From(tableName).AsCount(columnName, true).Execute();
                Assert.True(count >= 0, "Count cannot be less than zero");
                Assert.True(countDistinct <= count, "Count distinct cannot be greater than count");
            }
            finally
            {
                Release(dataSource);
            }
        }
#endif


#if NO_DISTINCT_COUNT
        [Theory, MemberData("TablesAndViewsWithColumns")]
        public async Task CountByColumn_Async(string assemblyName, string dataSourceName, DataSourceType mode, string tableName, string columnName)
        {
            var dataSource = await DataSourceAsync(dataSourceName, mode);
            WriteLine($"Table {tableName}");
            try
            {
                var columnType = dataSource.DatabaseMetadata.GetTableOrView(tableName).Columns[columnName].TypeName;
                if (columnType == "xml" || columnType == "ntext" || columnType == "text" || columnType == "image")
                    return; //SQL Server limitation

                var count = await dataSource.From(tableName).AsCount(columnName).ExecuteAsync();
                Assert.True(count >= 0, "Count cannot be less than zero");
            }
            finally
            {
                Release(dataSource);
            }
        }
#else
        [Theory, MemberData("TablesAndViewsWithColumns")]
        public async Task CountByColumn_Async(string assemblyName, string dataSourceName, DataSourceType mode, string tableName, string columnName)
        {
            var dataSource = await DataSourceAsync(dataSourceName, mode);
            WriteLine($"Table {tableName}");
            try
            {
                var columnType = dataSource.DatabaseMetadata.GetTableOrView(tableName).Columns[columnName].TypeName;
                if (columnType == "xml" || columnType == "ntext" || columnType == "text" || columnType == "image")
                    return; //SQL Server limitation

                var count = await dataSource.From(tableName).AsCount(columnName).ExecuteAsync();
                var countDistinct = await dataSource.From(tableName).AsCount(columnName, true).ExecuteAsync();
                Assert.True(count >= 0, "Count cannot be less than zero");
                Assert.True(countDistinct <= count, "Count distinct cannot be greater than count");
            }
            finally
            {
                Release(dataSource);
            }
        }
#endif

        [Theory, MemberData("Prime")]
        public void AsCount(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var key = Guid.NewGuid().ToString();

                for (var i = 0; i < 10; i++)
                    dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();


                var count = dataSource.From(EmployeeTableName, new { Title = key }).AsCount().Execute();
                var columnCount = dataSource.From(EmployeeTableName, new { Title = key }).AsCount("Title").Execute();
                var columnCount2 = dataSource.From(EmployeeTableName, new { Title = key }).AsCount("MiddleName").Execute();
                var distinctColumnCount = dataSource.From(EmployeeTableName, new { Title = key }).AsCount("Title", true).Execute();
                var distinctColumnCount2 = dataSource.From(EmployeeTableName, new { Title = key }).AsCount("LastName", true).Execute();

                Assert.AreEqual(10, count, "All of the rows");
                Assert.AreEqual(10, columnCount, "No nulls");
                Assert.AreEqual(5, columnCount2, "Half of the rows are nul");
                Assert.AreEqual(1, distinctColumnCount, "Only one distinct value");
                Assert.AreEqual(10, distinctColumnCount2, "Every value is distinct");

            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public void FilterByObject(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var key = Guid.NewGuid().ToString();

                for (var i = 0; i < 10; i++)
                    dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();


                var all = dataSource.From(EmployeeTableName, new { Title = key }).ToCollection<Employee>().Execute();
                var middleNameIsNull = dataSource.From(EmployeeTableName, new { Title = key, MiddleName = (string)null }).ToCollection<Employee>().Execute();
                var ignoreNulls = dataSource.From(EmployeeTableName, new { Title = key, MiddleName = (string)null }, FilterOptions.IgnoreNullProperties).ToCollection<Employee>().Execute();

                Assert.AreEqual(10, all.Count, "All of the rows");
                Assert.AreEqual(5, middleNameIsNull.Count, "Middle name is null");
                Assert.AreEqual(10, ignoreNulls.Count, "Ignore nulls should return all of the rows");


            }
            finally
            {
                Release(dataSource);
            }
        }
    }
}




