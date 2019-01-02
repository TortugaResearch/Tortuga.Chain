using System;
using System.Collections.Concurrent;
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

#if SQL_SERVER || OLE_SQL_SERVER
        public static TablesAndViewLimitData<SqlServerLimitOption> TablesAndViewLimit = new TablesAndViewLimitData<SqlServerLimitOption>(s_DataSources.Values);
#elif SQLITE
        public static TablesAndViewLimitData<SQLiteLimitOption> TablesAndViewLimit = new TablesAndViewLimitData<SQLiteLimitOption>(s_DataSources.Values);
#elif POSTGRESQL
        public static TablesAndViewLimitData<PostgreSqlLimitOption> TablesAndViewLimit = new TablesAndViewLimitData<PostgreSqlLimitOption>(s_DataSources.Values);
#elif ACCESS
        public static TablesAndViewLimitData<AccessLimitOption> TablesAndViewLimit = new TablesAndViewLimitData<AccessLimitOption>(s_DataSources.Values);
#elif MYSQL
        public static TablesAndViewLimitData<MySqlLimitOption> TablesAndViewLimit = new TablesAndViewLimitData<MySqlLimitOption>(s_DataSources.Values);
#endif

        public FromTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory, MemberData(nameof(TablesAndViews))]
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

        [Theory, MemberData(nameof(TablesAndViews))]
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

        [Theory, MemberData(nameof(TablesAndViews))]
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

        [Theory, MemberData(nameof(TablesAndViews))]
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

        [Theory, MemberData(nameof(TablesAndViews))]
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

        [Theory, MemberData(nameof(TablesAndViews))]
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

        [Theory, MemberData(nameof(TablesAndViews))]
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

        [Theory, MemberData(nameof(TablesAndViews))]
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

        [Theory, MemberData(nameof(TablesAndViews))]
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

        [Theory, MemberData(nameof(TablesAndViews))]
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

        [Theory, MemberData(nameof(TablesAndViews))]
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

        [Theory, MemberData(nameof(TablesAndViews))]
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

        [Theory, MemberData(nameof(TablesAndViewLimit))]
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

        [Theory, MemberData(nameof(TablesAndViewLimit))]
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

        [Theory, MemberData(nameof(TablesAndViews))]
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

        [Theory, MemberData(nameof(TablesAndViews))]
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

        [Theory, MemberData(nameof(TablesAndViewsWithColumns))]
        public void CountByColumn(string assemblyName, string dataSourceName, DataSourceType mode, string tableName, string columnName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            WriteLine($"Table {tableName}");
            try
            {
                var columnType = dataSource.DatabaseMetadata.GetTableOrView(tableName).Columns[columnName].TypeName;
                if (columnType == "xml" || columnType == "ntext" || columnType == "text" || columnType == "image" || columnType == "geography" || columnType == "geometry")
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

        [Theory, MemberData(nameof(TablesAndViewsWithColumns))]
        public void CountByColumn(string assemblyName, string dataSourceName, DataSourceType mode, string tableName, string columnName)
        {
            var dataSource = DataSource(dataSourceName, mode);
            WriteLine($"Table {tableName}");
            try
            {
                var columnType = dataSource.DatabaseMetadata.GetTableOrView(tableName).Columns[columnName].TypeName;
                if (columnType == "xml" || columnType == "ntext" || columnType == "text" || columnType == "image" || columnType == "geography" || columnType == "geometry")
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

        [Theory, MemberData(nameof(TablesAndViewsWithColumns))]
        public async Task CountByColumn_Async(string assemblyName, string dataSourceName, DataSourceType mode, string tableName, string columnName)
        {
            var dataSource = await DataSourceAsync(dataSourceName, mode);
            WriteLine($"Table {tableName}");
            try
            {
                var columnType = dataSource.DatabaseMetadata.GetTableOrView(tableName).Columns[columnName].TypeName;
                if (columnType == "xml" || columnType == "ntext" || columnType == "text" || columnType == "image" || columnType == "geography" || columnType == "geometry")
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

        [Theory, MemberData(nameof(TablesAndViewsWithColumns))]
        public async Task CountByColumn_Async(string assemblyName, string dataSourceName, DataSourceType mode, string tableName, string columnName)
        {
            var dataSource = await DataSourceAsync(dataSourceName, mode);
            WriteLine($"Table {tableName}");
            try
            {
                var columnType = dataSource.DatabaseMetadata.GetTableOrView(tableName).Columns[columnName].TypeName;
                if (columnType == "xml" || columnType == "ntext" || columnType == "text" || columnType == "image" || columnType == "geography" || columnType == "geometry")
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

        [Theory, MemberData(nameof(Prime))]
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

#if !ACCESS
                var distinctColumnCount = dataSource.From(EmployeeTableName, new { Title = key }).AsCount("Title", true).Execute();
                var distinctColumnCount2 = dataSource.From(EmployeeTableName, new { Title = key }).AsCount("LastName", true).Execute();
#endif

                Assert.AreEqual(10, count, "All of the rows");
                Assert.AreEqual(10, columnCount, "No nulls");
                Assert.AreEqual(5, columnCount2, "Half of the rows are nul");

#if !ACCESS
                Assert.AreEqual(1, distinctColumnCount, "Only one distinct value");
                Assert.AreEqual(10, distinctColumnCount2, "Every value is distinct");
#endif
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_ToInferredObject(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);

            try
            {
                var uniqueKey = Guid.NewGuid().ToString();

                var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
                dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();

                var lookup = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToObject<EmployeeLookup>(RowOptions.InferConstructor).Execute();

                Assert.AreEqual("A", lookup.FirstName, "First Name");
                Assert.AreEqual("1", lookup.LastName, "Last Name");
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Prime))]
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

        [Theory, MemberData(nameof(Prime))]
        public void FilterByObject_Compiled(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var key = Guid.NewGuid().ToString();

                for (var i = 0; i < 10; i++)
                    dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key, MiddleName = i % 2 == 0 ? "A" + i : null }).Compile().ToObject<Employee>().Execute();

                var all = dataSource.From(EmployeeTableName, new { Title = key }).Compile().ToCollection<Employee>().Execute();
                var middleNameIsNull = dataSource.From(EmployeeTableName, new { Title = key, MiddleName = (string)null }).Compile().ToCollection<Employee>().Execute();
                var ignoreNulls = dataSource.From(EmployeeTableName, new { Title = key, MiddleName = (string)null }, FilterOptions.IgnoreNullProperties).Compile().ToCollection<Employee>().Execute();

                Assert.AreEqual(10, all.Count, "All of the rows");
                Assert.AreEqual(5, middleNameIsNull.Count, "Middle name is null");
                Assert.AreEqual(10, ignoreNulls.Count, "Ignore nulls should return all of the rows");
            }
            finally
            {
                Release(dataSource);
            }
        }

#if !NO_DISTINCT_COUNT

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_Counts(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var count = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).AsCount().Execute();
                var columnCount = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).AsCount("Title").Execute();
                var columnCount2 = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).AsCount("MiddleName").Execute();
                var distinctColumnCount = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).AsCount("Title", true).Execute();
                var distinctColumnCount2 = dataSource.From(EmployeeTableName, new { Title = EmployeeSearchKey1000 }).AsCount("LastName", true).Execute();

                Assert.AreEqual(1000, count, "All of the rows");
                Assert.AreEqual(1000, columnCount, "No nulls");
                Assert.AreEqual(500, columnCount2, "Half of the rows are nul");
                Assert.AreEqual(1, distinctColumnCount, "Only one distinct value");
                Assert.AreEqual(1000, distinctColumnCount2, "Every value is distinct");
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

#if SQLITE

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_TakeRandom(string assemblyName, string dataSourceName, DataSourceType mode)
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

#if OLE_SQL_SERVER || SQL_SERVER

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_TakePercent(string assemblyName, string dataSourceName, DataSourceType mode)
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

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_TakePercentWithTies(string assemblyName, string dataSourceName, DataSourceType mode)
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

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_TakeWithTies(string assemblyName, string dataSourceName, DataSourceType mode)
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

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_TableSampleSystemPercentage(string assemblyName, string dataSourceName, DataSourceType mode)
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

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_TableSampleSystemRows(string assemblyName, string dataSourceName, DataSourceType mode)
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

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_TableSampleSystemPercentage_Repeatable(string assemblyName, string dataSourceName, DataSourceType mode)
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

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_TableSampleSystemRows_Repeatable(string assemblyName, string dataSourceName, DataSourceType mode)
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

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_Sorting(string assemblyName, string dataSourceName, DataSourceType mode)
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
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_ImmutableArray(string assemblyName, string dataSourceName, DataSourceType mode)
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

                var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("FirstName").ToImmutableArray<Employee>().Execute();
                Assert.AreEqual(emp1.EmployeeKey, test1[0].EmployeeKey);
                Assert.AreEqual(emp2.EmployeeKey, test1[1].EmployeeKey);
                Assert.AreEqual(emp3.EmployeeKey, test1[2].EmployeeKey);
                Assert.AreEqual(emp4.EmployeeKey, test1[3].EmployeeKey);

                var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting(new SortExpression("FirstName", SortDirection.Descending)).ToImmutableArray<Employee>().Execute();
                Assert.AreEqual(emp4.EmployeeKey, test2[0].EmployeeKey);
                Assert.AreEqual(emp3.EmployeeKey, test2[1].EmployeeKey);
                Assert.AreEqual(emp2.EmployeeKey, test2[2].EmployeeKey);
                Assert.AreEqual(emp1.EmployeeKey, test2[3].EmployeeKey);

                var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("LastName", "FirstName").ToImmutableArray<Employee>().Execute();
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

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_ImmutableList(string assemblyName, string dataSourceName, DataSourceType mode)
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

                var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("FirstName").ToImmutableList<Employee>().Execute();
                Assert.AreEqual(emp1.EmployeeKey, test1[0].EmployeeKey);
                Assert.AreEqual(emp2.EmployeeKey, test1[1].EmployeeKey);
                Assert.AreEqual(emp3.EmployeeKey, test1[2].EmployeeKey);
                Assert.AreEqual(emp4.EmployeeKey, test1[3].EmployeeKey);

                var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting(new SortExpression("FirstName", SortDirection.Descending)).ToImmutableList<Employee>().Execute();
                Assert.AreEqual(emp4.EmployeeKey, test2[0].EmployeeKey);
                Assert.AreEqual(emp3.EmployeeKey, test2[1].EmployeeKey);
                Assert.AreEqual(emp2.EmployeeKey, test2[2].EmployeeKey);
                Assert.AreEqual(emp1.EmployeeKey, test2[3].EmployeeKey);

                var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("LastName", "FirstName").ToImmutableList<Employee>().Execute();
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

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_GetByKey(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var emp1 = new Employee() { FirstName = "A", LastName = "1" };
                var emp2 = new Employee() { FirstName = "B", LastName = "2" };
                var emp3 = new Employee() { FirstName = "C", LastName = "3" };
                var emp4 = new Employee() { FirstName = "D", LastName = "4" };

                emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
                emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
                emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
                emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

                var find2 = dataSource.GetByKey(EmployeeTableName, emp2.EmployeeKey.Value).ToObject<Employee>().Execute();
                Assert.AreEqual(emp2.EmployeeKey, find2.EmployeeKey, "The wrong employee was returned");

                var list = dataSource.GetByKeyList(EmployeeTableName, new[] { emp2.EmployeeKey.Value, emp3.EmployeeKey.Value, emp4.EmployeeKey.Value }).ToCollection<Employee>().Execute();
                Assert.AreEqual(3, list.Count, "GetByKey returned the wrong number of rows");
                Assert.IsTrue(list.Any(e => e.EmployeeKey == emp2.EmployeeKey));
                Assert.IsTrue(list.Any(e => e.EmployeeKey == emp3.EmployeeKey));
                Assert.IsTrue(list.Any(e => e.EmployeeKey == emp4.EmployeeKey));
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_Take_NoSort(string assemblyName, string dataSourceName, DataSourceType mode)
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

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_Take(string assemblyName, string dataSourceName, DataSourceType mode)
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

#if !NO_ROW_SKIPPING

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_SkipTake(string assemblyName, string dataSourceName, DataSourceType mode)
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

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_Sorting_ImmutableCollection(string assemblyName, string dataSourceName, DataSourceType mode)
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

#else
        [Theory, MemberData(nameof(Prime))]
        public void FromTests_Sorting_ImmutableCollection(string assemblyName, string dataSourceName, DataSourceType mode)
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

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_ToDictionary_InferredObject(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var uniqueKey = Guid.NewGuid().ToString();

                var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
                var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
                var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = uniqueKey };
                var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = uniqueKey };

                emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
                emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
                emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
                emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

                var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup>("FirstName", DictionaryOptions.InferConstructor).Execute();

                Assert.AreEqual("1", test1["A"].LastName);
                Assert.AreEqual("2", test1["B"].LastName);
                Assert.AreEqual("3", test1["C"].LastName);
                Assert.AreEqual("4", test1["D"].LastName);

                var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName), DictionaryOptions.InferConstructor).Execute();

                Assert.AreEqual("A", test2[1].FirstName);
                Assert.AreEqual("B", test2[2].FirstName);
                Assert.AreEqual("C", test2[3].FirstName);
                Assert.AreEqual("D", test2[4].FirstName);

                var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup, ConcurrentDictionary<string, EmployeeLookup>>("FirstName", DictionaryOptions.InferConstructor).Execute();
                Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
                Assert.AreEqual("1", test3["A"].LastName);
                Assert.AreEqual("2", test3["B"].LastName);
                Assert.AreEqual("3", test3["C"].LastName);
                Assert.AreEqual("4", test3["D"].LastName);

                var test4 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup, ConcurrentDictionary<int, EmployeeLookup>>(e => int.Parse(e.LastName), DictionaryOptions.InferConstructor).Execute();
                Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
                Assert.AreEqual("A", test4[1].FirstName);
                Assert.AreEqual("B", test4[2].FirstName);
                Assert.AreEqual("C", test4[3].FirstName);
                Assert.AreEqual("D", test4[4].FirstName);

                var test5 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<string, EmployeeLookup>("FirstName", DictionaryOptions.InferConstructor).Execute();
                Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
                Assert.AreEqual("1", test5["A"].LastName);
                Assert.AreEqual("2", test5["B"].LastName);
                Assert.AreEqual("3", test5["C"].LastName);
                Assert.AreEqual("4", test5["D"].LastName);

                var test6 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName), DictionaryOptions.InferConstructor).Execute();
                Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
                Assert.AreEqual("A", test6[1].FirstName);
                Assert.AreEqual("B", test6[2].FirstName);
                Assert.AreEqual("C", test6[3].FirstName);
                Assert.AreEqual("D", test6[4].FirstName);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_ToDictionary(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var uniqueKey = Guid.NewGuid().ToString();

                var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
                var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
                var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = uniqueKey };
                var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = uniqueKey };

                emp1 = dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
                emp2 = dataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
                emp3 = dataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
                emp4 = dataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

                var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, Employee>("FirstName").Execute();

                Assert.AreEqual("1", test1["A"].LastName);
                Assert.AreEqual("2", test1["B"].LastName);
                Assert.AreEqual("3", test1["C"].LastName);
                Assert.AreEqual("4", test1["D"].LastName);

                var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, Employee>(e => int.Parse(e.LastName)).Execute();

                Assert.AreEqual("A", test2[1].FirstName);
                Assert.AreEqual("B", test2[2].FirstName);
                Assert.AreEqual("C", test2[3].FirstName);
                Assert.AreEqual("D", test2[4].FirstName);

                var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, Employee, ConcurrentDictionary<string, Employee>>("FirstName").Execute();
                Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, Employee>));
                Assert.AreEqual("1", test3["A"].LastName);
                Assert.AreEqual("2", test3["B"].LastName);
                Assert.AreEqual("3", test3["C"].LastName);
                Assert.AreEqual("4", test3["D"].LastName);

                var test4 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, Employee, ConcurrentDictionary<int, Employee>>(e => int.Parse(e.LastName)).Execute();
                Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, Employee>));
                Assert.AreEqual("A", test4[1].FirstName);
                Assert.AreEqual("B", test4[2].FirstName);
                Assert.AreEqual("C", test4[3].FirstName);
                Assert.AreEqual("D", test4[4].FirstName);

                var test5 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<string, Employee>("FirstName").Execute();
                Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, Employee>));
                Assert.AreEqual("1", test5["A"].LastName);
                Assert.AreEqual("2", test5["B"].LastName);
                Assert.AreEqual("3", test5["C"].LastName);
                Assert.AreEqual("4", test5["D"].LastName);

                var test6 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<int, Employee>(e => int.Parse(e.LastName)).Execute();
                Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, Employee>));
                Assert.AreEqual("A", test6[1].FirstName);
                Assert.AreEqual("B", test6[2].FirstName);
                Assert.AreEqual("C", test6[3].FirstName);
                Assert.AreEqual("D", test6[4].FirstName);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_Sorting_InferredCollection(string assemblyName, string dataSourceName, DataSourceType mode)
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

#if SQLITE

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_ToImmutableObject(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var uniqueKey = Guid.NewGuid().ToString();

                var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
                dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();

                var lookup = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToObject<EmployeeLookup>().WithConstructor<long, string, string>().Execute();

                Assert.AreEqual("A", lookup.FirstName);
                Assert.AreEqual("1", lookup.LastName);
            }
            finally
            {
                Release(dataSource);
            }
        }

#else

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_ToImmutableObject(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var uniqueKey = Guid.NewGuid().ToString();

                var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
                dataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();

                var lookup = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToObject<EmployeeLookup>().WithConstructor<int, string, string>().Execute();

                Assert.AreEqual("A", lookup.FirstName);
                Assert.AreEqual("1", lookup.LastName);
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

#if !SQLITE

        [Theory, MemberData(nameof(Prime))]
        public void FromTests_ToDictionary_ImmutableObject(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var uniqueKey = Guid.NewGuid().ToString();

                var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
                var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
                var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = uniqueKey };
                var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = uniqueKey };

                dataSource.Insert(EmployeeTableName, emp1).WithRefresh().Execute();
                dataSource.Insert(EmployeeTableName, emp2).WithRefresh().Execute();
                dataSource.Insert(EmployeeTableName, emp3).WithRefresh().Execute();
                dataSource.Insert(EmployeeTableName, emp4).WithRefresh().Execute();

                var test1 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup>("FirstName").WithConstructor<int, string, string>().Execute();

                Assert.AreEqual("1", test1["A"].LastName);
                Assert.AreEqual("2", test1["B"].LastName);
                Assert.AreEqual("3", test1["C"].LastName);
                Assert.AreEqual("4", test1["D"].LastName);

                var test2 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName)).WithConstructor<int, string, string>().Execute();

                Assert.AreEqual("A", test2[1].FirstName);
                Assert.AreEqual("B", test2[2].FirstName);
                Assert.AreEqual("C", test2[3].FirstName);
                Assert.AreEqual("D", test2[4].FirstName);

                var test3 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup, ConcurrentDictionary<string, EmployeeLookup>>("FirstName").WithConstructor<int, string, string>().Execute();
                Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
                Assert.AreEqual("1", test3["A"].LastName);
                Assert.AreEqual("2", test3["B"].LastName);
                Assert.AreEqual("3", test3["C"].LastName);
                Assert.AreEqual("4", test3["D"].LastName);

                var test4 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup, ConcurrentDictionary<int, EmployeeLookup>>(e => int.Parse(e.LastName)).WithConstructor<int, string, string>().Execute();
                Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
                Assert.AreEqual("A", test4[1].FirstName);
                Assert.AreEqual("B", test4[2].FirstName);
                Assert.AreEqual("C", test4[3].FirstName);
                Assert.AreEqual("D", test4[4].FirstName);

                var test5 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<string, EmployeeLookup>("FirstName").WithConstructor<int, string, string>().Execute();
                Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
                Assert.AreEqual("1", test5["A"].LastName);
                Assert.AreEqual("2", test5["B"].LastName);
                Assert.AreEqual("3", test5["C"].LastName);
                Assert.AreEqual("4", test5["D"].LastName);

                var test6 = dataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName)).WithConstructor<int, string, string>().Execute();
                Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
                Assert.AreEqual("A", test6[1].FirstName);
                Assert.AreEqual("B", test6[2].FirstName);
                Assert.AreEqual("C", test6[3].FirstName);
                Assert.AreEqual("D", test6[4].FirstName);
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif
    }
}