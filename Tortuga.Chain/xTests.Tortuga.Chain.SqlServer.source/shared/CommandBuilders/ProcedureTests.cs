
#if SQL_SERVER || POSTGRESQL

using System.Collections.Generic;
using System.Threading.Tasks;
using Tests.Models;
using Tortuga.Chain;
using Xunit;
using Xunit.Abstractions;


namespace Tests.CommandBuilders
{
    public class ProcedureTests : TestBase
    {
        public static BasicData Prime = new BasicData(s_PrimaryDataSource);

#if SQL_SERVER || OLE_SQL_SERVER
        const string CheckA = @"SELECT Count(*) FROM Sales.Customer c WHERE c.State = @State;";
        const string CheckB = @"SELECT Count(*) FROM Sales.[Order] o INNER JOIN Sales.Customer c ON o.CustomerKey = c.CustomerKey WHERE c.State = @State;";
        static object Parameter1 = new { @State = "CA" };
        static object DictParameter1a = new Dictionary<string, object>() { { "State", "CA" } };
        static object DictParameter1b = new Dictionary<string, object>() { { "@State", "CA" } };
#elif POSTGRESQL
        const string CheckA = @"SELECT Count(*) FROM Sales.Customer c WHERE c.State = @param_state;";
        const string CheckB = @"SELECT Count(*) FROM Sales.Order o INNER JOIN Sales.Customer c ON o.CustomerKey = c.CustomerKey WHERE c.State = @param_state;";
        static object Parameter1 = new { @param_state = "CA" };
        static object DictParameter1a = new Dictionary<string, object>() { { "param_state", "CA" } };
        static object DictParameter1b = new Dictionary<string, object>() { { "@param_state", "CA" } };
#endif

        public ProcedureTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory, MemberData("Prime")]
        public void Proc1_Object(string assemblyName, string dataSourceName, DataSourceType mode)
        {

            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, Parameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, Parameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultSetProc1Name, Parameter1).ToTableSet("cust", "order").Execute();

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

            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, Parameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, Parameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, Parameter1).ToTableSet("cust", "order").ExecuteAsync();

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

            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, Parameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, Parameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultSetProc1Name, DictParameter1a).ToTableSet("cust", "order").Execute();

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

            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, Parameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, Parameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, DictParameter1a).ToTableSet("cust", "order").ExecuteAsync();
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

            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, Parameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, Parameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultSetProc1Name, DictParameter1b).ToTableSet("cust", "order").Execute();
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

            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, Parameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, Parameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, DictParameter1b).ToTableSet("cust", "order").ExecuteAsync();
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

            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, Parameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, Parameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, Parameter1).ToCollectionSet<Customer, Order>().ExecuteAsync();
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

            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, Parameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, Parameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultSetProc1Name, Parameter1).ToDataSet("cust", "order").Execute();
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

            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, Parameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, Parameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, Parameter1).ToDataSet("cust", "order").ExecuteAsync();
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

            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, Parameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, Parameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultSetProc1Name, DictParameter1a).ToDataSet("cust", "order").Execute();
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

            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, Parameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, Parameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, DictParameter1a).ToDataSet("cust", "order").ExecuteAsync();
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

            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, Parameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, Parameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultSetProc1Name, DictParameter1b).ToDataSet("cust", "order").Execute();
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

            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, Parameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, Parameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, DictParameter1b).ToDataSet("cust", "order").ExecuteAsync();
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

