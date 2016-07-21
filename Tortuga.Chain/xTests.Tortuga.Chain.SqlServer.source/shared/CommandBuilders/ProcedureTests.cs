
#if SQL_SERVER

using System.Collections.Generic;
using System.Threading.Tasks;
using Tests.Models;
using Xunit;
using Xunit.Abstractions;


namespace Tests.CommandBuilders
{
    public class ProcedureTests : TestBase
    {
        public static BasicData Prime = new BasicData(s_PrimaryDataSource);
        const string CheckA = @"SELECT Count(*) FROM Sales.Customer c WHERE c.State = @State;";
        const string CheckB = @"SELECT Count(*) FROM Sales.[Order] o INNER JOIN Sales.Customer c ON o.CustomerKey = c.CustomerKey WHERE c.State = @State;";

        static object Parameter1 = new { @State = "CA" };

        public ProcedureTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory, MemberData("Prime")]
        public void Proc1_Object(string assemblyName, string dataSourceName, DataSourceType mode)
        {

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, Parameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, Parameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultsetProc1Name, new { @State = "CA" }).ToTableSet("cust", "order").Execute();

                Assert.Equal(2, result.Count);
                Assert.Equal(countA, result["cust"].Rows.Count);
                Assert.Equal(countB, result["order"].Rows.Count);

            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("Prime")]
        public async Task Proc1_Object_Async(string assemblyName, string dataSourceName, DataSourceType mode)
        {

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, Parameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, Parameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultsetProc1Name, new { @State = "CA" }).ToTableSet("cust", "order").ExecuteAsync();

                Assert.Equal(2, result.Count);
                Assert.Equal(countA, result["cust"].Rows.Count);
                Assert.Equal(countB, result["order"].Rows.Count);

            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory, MemberData("Prime")]
        public void Proc1_Dictionary(string assemblyName, string dataSourceName, DataSourceType mode)
        {

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, Parameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, Parameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultsetProc1Name, new Dictionary<string, object>() { { "State", "CA" } }).ToTableSet("cust", "order").Execute();

                Assert.Equal(2, result.Count);
                Assert.Equal(countA, result["cust"].Rows.Count);
                Assert.Equal(countB, result["order"].Rows.Count);

            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public async Task Proc1_Dictionary_Async(string assemblyName, string dataSourceName, DataSourceType mode)
        {

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, Parameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, Parameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultsetProc1Name, new Dictionary<string, object>() { { "State", "CA" } }).ToTableSet("cust", "order").ExecuteAsync();
                Assert.Equal(2, result.Count);
                Assert.Equal(countA, result["cust"].Rows.Count);
                Assert.Equal(countB, result["order"].Rows.Count);

            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public void Proc1_Dictionary2(string assemblyName, string dataSourceName, DataSourceType mode)
        {

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, Parameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, Parameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultsetProc1Name, new Dictionary<string, object>() { { "@State", "CA" } }).ToTableSet("cust", "order").Execute();
                Assert.Equal(2, result.Count);
                Assert.Equal(countA, result["cust"].Rows.Count);
                Assert.Equal(countB, result["order"].Rows.Count);

            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public async Task Proc1_Dictionary2_Async(string assemblyName, string dataSourceName, DataSourceType mode)
        {

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, Parameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, Parameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultsetProc1Name, new Dictionary<string, object>() { { "@State", "CA" } }).ToTableSet("cust", "order").ExecuteAsync();
                Assert.Equal(2, result.Count);
                Assert.Equal(countA, result["cust"].Rows.Count);
                Assert.Equal(countB, result["order"].Rows.Count);

            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public async Task Proc1_ToCollectionSet(string assemblyName, string dataSourceName, DataSourceType mode)
        {

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, Parameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, Parameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultsetProc1Name, new { @State = "CA" }).ToCollectionSet<Customer, Order>().ExecuteAsync();
                Assert.Equal(countA, result.Item1.Count);
                Assert.Equal(countB, result.Item2.Count);

            }
            finally
            {
                Release(dataSource);
            }
        }


        [Theory, MemberData("Prime")]
        public void Proc1_Object_DataSet(string assemblyName, string dataSourceName, DataSourceType mode)
        {

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, Parameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, Parameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultsetProc1Name, new { @State = "CA" }).ToDataSet("cust", "order").Execute();
                Assert.Equal(2, result.Tables.Count);
                Assert.Equal(countA, result.Tables["cust"].Rows.Count);
                Assert.Equal(countB, result.Tables["order"].Rows.Count);

            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public async Task Proc1_Object_Async_DataSet(string assemblyName, string dataSourceName, DataSourceType mode)
        {

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, Parameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, Parameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultsetProc1Name, new { @State = "CA" }).ToDataSet("cust", "order").ExecuteAsync();
                Assert.Equal(2, result.Tables.Count);
                Assert.Equal(countA, result.Tables["cust"].Rows.Count);
                Assert.Equal(countB, result.Tables["order"].Rows.Count);

            }
            finally
            {
                Release(dataSource);
            }
        }


        [Theory, MemberData("Prime")]
        public void Proc1_Dictionary_DataSet(string assemblyName, string dataSourceName, DataSourceType mode)
        {

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, Parameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, Parameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultsetProc1Name, new Dictionary<string, object>() { { "State", "CA" } }).ToDataSet("cust", "order").Execute();
                Assert.Equal(2, result.Tables.Count);
                Assert.Equal(countA, result.Tables["cust"].Rows.Count);
                Assert.Equal(countB, result.Tables["order"].Rows.Count);

            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public async Task Proc1_Dictionary_Async_DataSet(string assemblyName, string dataSourceName, DataSourceType mode)
        {

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, Parameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, Parameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultsetProc1Name, new Dictionary<string, object>() { { "State", "CA" } }).ToDataSet("cust", "order").ExecuteAsync();
                Assert.Equal(2, result.Tables.Count);
                Assert.Equal(countA, result.Tables["cust"].Rows.Count);
                Assert.Equal(countB, result.Tables["order"].Rows.Count);

            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public void Proc1_Dictionary2_DataSet(string assemblyName, string dataSourceName, DataSourceType mode)
        {

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, Parameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, Parameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultsetProc1Name, new Dictionary<string, object>() { { "@State", "CA" } }).ToDataSet("cust", "order").Execute();
                Assert.Equal(2, result.Tables.Count);
                Assert.Equal(countA, result.Tables["cust"].Rows.Count);
                Assert.Equal(countB, result.Tables["order"].Rows.Count);

            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public async Task Proc1_Dictionary2_Async_DataSet(string assemblyName, string dataSourceName, DataSourceType mode)
        {

            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, Parameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, Parameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultsetProc1Name, new Dictionary<string, object>() { { "@State", "CA" } }).ToDataSet("cust", "order").ExecuteAsync();
                Assert.Equal(2, result.Tables.Count);
                Assert.Equal(countA, result.Tables["cust"].Rows.Count);
                Assert.Equal(countB, result.Tables["order"].Rows.Count);

            }
            finally
            {
                Release(dataSource);
            }
        }
    }
}

#endif

