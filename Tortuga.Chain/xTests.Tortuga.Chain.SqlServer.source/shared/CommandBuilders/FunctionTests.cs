using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

#if SQL_SERVER || POSTGRESQL

namespace Tests.CommandBuilders
{
    public class FunctionTests : TestBase
    {
        public static BasicData Prime = new BasicData(s_PrimaryDataSource);


        public FunctionTests(ITestOutputHelper output) : base(output)
        {
        }


#if SQL_SERVER
        //Only SQL Server has inline functions.

        [Theory, MemberData("Prime")]
        public void TableFunction2_Object(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var result = dataSource.TableFunction(TableFunction2Name, new { @State = "CA" }).ToTable().Execute();
            }
            finally
            {
                Release(dataSource);
            }
        }
#endif

        [Theory, MemberData("Prime")]
        public void TableFunction1_Object_Limit(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var result = dataSource.TableFunction(TableFunction1Name, new { @State = "CA" }).WithLimits(1).ToTable().Execute();
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public void TableFunction1_Object_Filter(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var result = dataSource.TableFunction(TableFunction1Name, new { @State = "CA" }).WithFilter(new { @FullName = "Tom Jones" }).ToTable().Execute();
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public void TableFunction1_Object_Sort(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var result = dataSource.TableFunction(TableFunction1Name, new { @State = "CA" }).WithSorting("FullName").ToTable().Execute();
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public void TableFunction1_Object(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var result = dataSource.TableFunction(TableFunction1Name, new { @State = "CA" }).ToTable().Execute();
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public async Task TableFunction1_Object_Async(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var result = await dataSource.TableFunction(TableFunction1Name, new { @State = "CA" }).ToTable().ExecuteAsync();
            }
            finally
            {
                Release(dataSource);
            }
        }


        [Theory, MemberData("Prime")]
        public void TableFunction1_Dictionary(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var result = dataSource.TableFunction(TableFunction1Name, new Dictionary<string, object>() { { "State", "CA" } }).ToTable().Execute();
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public async Task TableFunction1_Dictionary_Async(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var result = await dataSource.TableFunction(TableFunction1Name, new Dictionary<string, object>() { { "State", "CA" } }).ToTable().ExecuteAsync();
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public void TableFunction1_Dictionary2(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var result = dataSource.TableFunction(TableFunction1Name, new Dictionary<string, object>() { { "@State", "CA" } }).ToTable().Execute();
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public async Task TableFunction1_Dictionary2_Async(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var result = await dataSource.TableFunction(TableFunction1Name, new Dictionary<string, object>() { { "@State", "CA" } }).ToTable().ExecuteAsync();
            }
            finally
            {
                Release(dataSource);
            }
        }

    }
}

#endif

