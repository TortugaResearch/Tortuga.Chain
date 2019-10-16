#if SQL_SERVER_SDS || SQL_SERVER_MDS || POSTGRESQL || SQL_SERVER_OLEDB || MYSQL

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tests.Models;
using Tortuga.Chain;

namespace Tests.CommandBuilders
{
    [TestClass]
    public class ProcedureTests : TestBase
    {
#if SQL_SERVER_SDS || SQL_SERVER_MDS
        const string CheckA = @"SELECT Count(*) FROM Sales.Customer c WHERE c.State = @State;";
        const string CheckB = @"SELECT Count(*) FROM Sales.[Order] o INNER JOIN Sales.Customer c ON o.CustomerKey = c.CustomerKey WHERE c.State = @State;";
        static readonly object CheckParameter1 = new { @State = "CA" };
        static readonly object ProcParameter1 = new { @State = "CA" };
        static readonly object DictParameter1a = new Dictionary<string, object>() { { "State", "CA" } };
        static readonly object DictParameter1b = new Dictionary<string, object>() { { "@State", "CA" } };
#elif SQL_SERVER_OLEDB
        const string CheckA = @"SELECT Count(*) FROM Sales.Customer c WHERE c.State = ?;";
        const string CheckB = @"SELECT Count(*) FROM Sales.[Order] o INNER JOIN Sales.Customer c ON o.CustomerKey = c.CustomerKey WHERE c.State = ?;";
        static readonly object CheckParameter1 = new { @State = "CA" };
        static readonly object ProcParameter1 = new { @State = "CA" };
        static readonly object DictParameter1a = new Dictionary<string, object>() { { "State", "CA" } };
        static readonly object DictParameter1b = new Dictionary<string, object>() { { "@State", "CA" } };
#elif MYSQL
        const string CheckA = @"SELECT Count(*) FROM Sales.Customer c WHERE c.State = @State;";
        const string CheckB = @"SELECT Count(*) FROM Sales.`Order` o INNER JOIN Sales.Customer c ON o.CustomerKey = c.CustomerKey WHERE c.State = @State;";
        static readonly object CheckParameter1 = new { @State = "CA" };
        static readonly object ProcParameter1 = new { p_State = "CA" };
        static readonly object DictParameter1a = new Dictionary<string, object>() { { "p_State", "CA" } };
        //static readonly object  DictParameter1b = new Dictionary<string, object>() { { "@p_State", "CA" } };
#elif POSTGRESQL
        const string CheckA = @"SELECT Count(*) FROM Sales.Customer c WHERE c.State = @param_state;";
        const string CheckB = @"SELECT Count(*) FROM Sales.Order o INNER JOIN Sales.Customer c ON o.CustomerKey = c.CustomerKey WHERE c.State = @param_state;";
        static readonly object CheckParameter1 = new { @param_state = "CA" };
        static readonly object ProcParameter1 = new { @param_state = "CA" };
        static readonly object DictParameter1a = new Dictionary<string, object>() { { "param_state", "CA" } };
        static readonly object DictParameter1b = new Dictionary<string, object>() { { "@param_state", "CA" } };
#endif

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public void Proc1_Object(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, CheckParameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, CheckParameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultSetProc1Name, ProcParameter1).ToTableSet("cust", "order").Execute();

                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(countA, result["cust"].Rows.Count);
                Assert.AreEqual(countB, result["order"].Rows.Count);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public async Task Proc1_Object_Async(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, CheckParameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, CheckParameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, ProcParameter1).ToTableSet("cust", "order").ExecuteAsync();

                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(countA, result["cust"].Rows.Count);
                Assert.AreEqual(countB, result["order"].Rows.Count);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public void Proc1_Dictionary(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, CheckParameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, CheckParameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultSetProc1Name, DictParameter1a).ToTableSet("cust", "order").Execute();

                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(countA, result["cust"].Rows.Count);
                Assert.AreEqual(countB, result["order"].Rows.Count);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public async Task Proc1_Dictionary_Async(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, CheckParameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, CheckParameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, DictParameter1a).ToTableSet("cust", "order").ExecuteAsync();
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(countA, result["cust"].Rows.Count);
                Assert.AreEqual(countB, result["order"].Rows.Count);
            }
            finally
            {
                Release(dataSource);
            }
        }

#if !MYSQL

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public void Proc1_Dictionary2(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, CheckParameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, CheckParameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultSetProc1Name, DictParameter1b).ToTableSet("cust", "order").Execute();
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(countA, result["cust"].Rows.Count);
                Assert.AreEqual(countB, result["order"].Rows.Count);
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

#if !MYSQL

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public async Task Proc1_Dictionary2_Async(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, CheckParameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, CheckParameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, DictParameter1b).ToTableSet("cust", "order").ExecuteAsync();
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(countA, result["cust"].Rows.Count);
                Assert.AreEqual(countB, result["order"].Rows.Count);
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public async Task Proc1_ToCollectionSet(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, CheckParameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, CheckParameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, ProcParameter1).ToCollectionSet<Customer, Order>().ExecuteAsync();
                Assert.AreEqual(countA, result.Item1.Count);
                Assert.AreEqual(countB, result.Item2.Count);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [DataTestMethod, BasicDataWithJoinOptions(DataSourceGroup.Primary)]
        public async Task Proc1_ToCollectionSet_Join_Expression_1(string dataSourceName, DataSourceType mode, JoinOptions options)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, CheckParameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, CheckParameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, ProcParameter1).ToCollectionSet<Customer, Order>().Join((c, o) => c.CustomerKey == o.CustomerKey, c => c.Orders, options).ExecuteAsync();
                Assert.AreEqual(countA, result.Count);
                Assert.AreEqual(countB, result.Sum(c => c.Orders.Count));
            }
            finally
            {
                Release(dataSource);
            }
        }

        [DataTestMethod, BasicDataWithJoinOptions(DataSourceGroup.Primary)]
        public async Task Proc1_ToCollectionSet_Join_Expression_2(string dataSourceName, DataSourceType mode, JoinOptions options)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, CheckParameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, CheckParameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, ProcParameter1).ToCollectionSet<Customer, Order>().Join((c, o) => c.CustomerKey == o.CustomerKey, nameof(Customer.Orders), options).ExecuteAsync();
                Assert.AreEqual(countA, result.Count);
                Assert.AreEqual(countB, result.Sum(c => c.Orders.Count));
            }
            finally
            {
                Release(dataSource);
            }
        }

        [DataTestMethod, BasicDataWithJoinOptions(DataSourceGroup.Primary)]
        public async Task Proc1_ToCollectionSet_Join_Keys_1(string dataSourceName, DataSourceType mode, JoinOptions options)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, CheckParameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, CheckParameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, ProcParameter1).ToCollectionSet<Customer, Order>().Join(c => c.CustomerKey, o => o.CustomerKey, c => c.Orders, options).ExecuteAsync();
                Assert.AreEqual(countA, result.Count);
                Assert.AreEqual(countB, result.Sum(c => c.Orders.Count));
            }
            finally
            {
                Release(dataSource);
            }
        }

        [DataTestMethod, BasicDataWithJoinOptions(DataSourceGroup.Primary)]
        public async Task Proc1_ToCollectionSet_Join_Keys_2(string dataSourceName, DataSourceType mode, JoinOptions options)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, CheckParameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, CheckParameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, ProcParameter1).ToCollectionSet<Customer, Order>().Join(c => c.CustomerKey, o => o.CustomerKey, nameof(Customer.Orders), options).ExecuteAsync();
                Assert.AreEqual(countA, result.Count);
                Assert.AreEqual(countB, result.Sum(c => c.Orders.Count));
            }
            finally
            {
                Release(dataSource);
            }
        }

        [DataTestMethod, BasicDataWithJoinOptions(DataSourceGroup.Primary)]
        public async Task Proc1_ToCollectionSet_Join_Keys_3(string dataSourceName, DataSourceType mode, JoinOptions options)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, CheckParameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, CheckParameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, ProcParameter1).ToCollectionSet<Customer, Order>().Join(nameof(Customer.CustomerKey), nameof(Order.CustomerKey), nameof(Customer.Orders), options).ExecuteAsync();
                Assert.AreEqual(countA, result.Count);
                Assert.AreEqual(countB, result.Sum(c => c.Orders.Count));
            }
            finally
            {
                Release(dataSource);
            }
        }

        [DataTestMethod, BasicDataWithJoinOptions(DataSourceGroup.Primary)]
        public async Task Proc1_ToCollectionSet_Join_Keys_4(string dataSourceName, DataSourceType mode, JoinOptions options)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, CheckParameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, CheckParameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, ProcParameter1).ToCollectionSet<Customer, Order>().Join(nameof(Customer.CustomerKey), nameof(Customer.Orders), options).ExecuteAsync();
                Assert.AreEqual(countA, result.Count);
                Assert.AreEqual(countB, result.Sum(c => c.Orders.Count));
            }
            finally
            {
                Release(dataSource);
            }
        }

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public void Proc1_Object_DataSet(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, CheckParameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, CheckParameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultSetProc1Name, ProcParameter1).ToDataSet("cust", "order").Execute();
                Assert.AreEqual(2, result.Tables.Count);
                Assert.AreEqual(countA, result.Tables["cust"].Rows.Count);
                Assert.AreEqual(countB, result.Tables["order"].Rows.Count);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public async Task Proc1_Object_Async_DataSet(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, CheckParameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, CheckParameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, ProcParameter1).ToDataSet("cust", "order").ExecuteAsync();
                Assert.AreEqual(2, result.Tables.Count);
                Assert.AreEqual(countA, result.Tables["cust"].Rows.Count);
                Assert.AreEqual(countB, result.Tables["order"].Rows.Count);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public void Proc1_Dictionary_DataSet(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, CheckParameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, CheckParameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultSetProc1Name, DictParameter1a).ToDataSet("cust", "order").Execute();
                Assert.AreEqual(2, result.Tables.Count);
                Assert.AreEqual(countA, result.Tables["cust"].Rows.Count);
                Assert.AreEqual(countB, result.Tables["order"].Rows.Count);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public async Task Proc1_Dictionary_Async_DataSet(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, CheckParameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, CheckParameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, DictParameter1a).ToDataSet("cust", "order").ExecuteAsync();
                Assert.AreEqual(2, result.Tables.Count);
                Assert.AreEqual(countA, result.Tables["cust"].Rows.Count);
                Assert.AreEqual(countB, result.Tables["order"].Rows.Count);
            }
            finally
            {
                Release(dataSource);
            }
        }

#if !MYSQL

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public void Proc1_Dictionary2_DataSet(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = dataSource.Sql(CheckA, CheckParameter1).ToInt32().Execute();
                var countB = dataSource.Sql(CheckB, CheckParameter1).ToInt32().Execute();

                var result = dataSource.Procedure(MultiResultSetProc1Name, DictParameter1b).ToDataSet("cust", "order").Execute();
                Assert.AreEqual(2, result.Tables.Count);
                Assert.AreEqual(countA, result.Tables["cust"].Rows.Count);
                Assert.AreEqual(countB, result.Tables["order"].Rows.Count);
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif

#if !MYSQL

        [DataTestMethod, BasicData(DataSourceGroup.Primary)]
        public async Task Proc1_Dictionary2_Async_DataSet(string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource2(dataSourceName, mode);
            try
            {
                var countA = await dataSource.Sql(CheckA, CheckParameter1).ToInt32().ExecuteAsync();
                var countB = await dataSource.Sql(CheckB, CheckParameter1).ToInt32().ExecuteAsync();

                var result = await dataSource.Procedure(MultiResultSetProc1Name, DictParameter1b).ToDataSet("cust", "order").ExecuteAsync();
                Assert.AreEqual(2, result.Tables.Count);
                Assert.AreEqual(countA, result.Tables["cust"].Rows.Count);
                Assert.AreEqual(countB, result.Tables["order"].Rows.Count);
            }
            finally
            {
                Release(dataSource);
            }
        }

#endif
    }
}

#endif
