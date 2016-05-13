using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tortuga.Chain;
using Xunit;
using Xunit.Abstractions;

namespace Tests.CommandBuilders
{
    public class FromTests : TestBase
    {
        public static TablesAndViewData TablesAndViews = new TablesAndViewData(s_DataSources.Values);

#if SQL_SERVER
        public static TablesAndViewLimitData<SqlServerLimitOption> TablesAndViewLimit = new TablesAndViewLimitData<SqlServerLimitOption>(s_DataSources.Values);
#elif SQLITE
        public static TablesAndViewLimitData<SQLiteLimitOption> TablesAndViewLimit = new TablesAndViewLimitData<SQLiteLimitOption>(s_DataSources.Values);
#elif POSTGRESQL
        public static TablesAndViewLimitData<PostgreSqlLimitOption> TablesAndViewLimit = new TablesAndViewLimitData<PostgreSqlLimitOption>(s_DataSources.Values);
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
            m_Output.WriteLine($"Table {tableName}, Limit Option {limitOptions}");
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
            m_Output.WriteLine($"Table {tableName}, Limit Option {limitOptions}");
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


    }
}



