#if CLASS_3

using System.ComponentModel.DataAnnotations.Schema;
using Tests.Models;
using Tortuga.Chain;

#if SQL_SERVER_SDS

using System.Data.SqlClient;

#elif SQL_SERVER_MDS
using Microsoft.Data.SqlClient;
#endif

namespace Tests.CommandBuilders;

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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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
		var dataSource = DataSource3(dataSourceName, mode);
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

#if SQL_SERVER_SDS || SQL_SERVER_MDS

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task SystemStoredProcedure_IgnoreOutputs(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource3(dataSourceName, mode);
		try
		{
			await dataSource.Procedure("sys.sp_sequence_get_range", new { @sequence_name = "dbo.TestSequence", @range_size = 10 }).ExecuteAsync();
		}
		finally
		{
			Release(dataSource);
		}
	}

	class SequenceGetRangeResult
	{
		public long? range_first_value { get; set; }

		public long? range_last_value { get; set; }
		public long? range_cycle_count { get; set; }
		public long? sequence_increment { get; set; }
		public long? sequence_min_value { get; set; }
		public long? sequence_max_value { get; set; }
	}

	class SequenceGetRangeInputMapped
	{
		[Column("sequence_name")]
		public string SequenceName { get; set; }

		[Column("range_size")]
		public long RangeSize { get; set; }
	}

	class SequenceGetRangeResultMapped
	{
		[Column("range_first_value")]
		public long? RangeFirstValue { get; set; }

		[Column("range_last_value")]
		public long? RangeLastValue { get; set; }

		[Column("range_cycle_count")]
		public long? RangeCycleCount { get; set; }

		[Column("sequence_increment")]
		public long? SequenceIncrement { get; set; }

		[Column("sequence_min_value")]
		public long? SequenceMinValue { get; set; }

		[Column("sequence_max_value")]
		public long? SequenceMaxValue { get; set; }
	}

	/*
	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task SystemStoredProcedure_CaptureOutputsInParameter(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource2(dataSourceName, mode);
		try
		{
			var param = new SequenceGetRangeResult() { sequence_name = "dbo.TestSequence", range_size = 10 };
			await dataSource.Procedure("sys.sp_sequence_get_range", param).AsNonQuery().CaptureOutputs().ExecuteAsync();

			Assert.IsTrue(param.range_first_value > 0);
		}
		finally
		{
			Release(dataSource);
		}
	}
	*/

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task SystemStoredProcedure_CaptureOutputsAsObject(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource3(dataSourceName, mode);
		try
		{
			var result = await dataSource.Procedure("sys.sp_sequence_get_range", new { sequence_name = "dbo.TestSequence", range_size = 10 }).AsOutputs<SequenceGetRangeResult>().ExecuteAsync();

			Assert.IsTrue(result.range_first_value > 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task SystemStoredProcedure_CaptureOutputsAsObject_MappedInput(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource3(dataSourceName, mode);
		try
		{
			var result = await dataSource.Procedure("sys.sp_sequence_get_range", new SequenceGetRangeInputMapped { SequenceName = "dbo.TestSequence", RangeSize = 10 }).AsOutputs<SequenceGetRangeResultMapped>().ExecuteAsync();

			Assert.IsTrue(result.RangeFirstValue > 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task SystemStoredProcedure_CaptureOutputsAsObject_Mapped(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource3(dataSourceName, mode);
		try
		{
			var result = await dataSource.Procedure("sys.sp_sequence_get_range", new { sequence_name = "dbo.TestSequence", range_size = 10 }).AsOutputs<SequenceGetRangeResultMapped>().ExecuteAsync();

			Assert.IsTrue(result.RangeFirstValue > 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task SystemStoredProcedure_CaptureOutputsAsDictionary(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource3(dataSourceName, mode);
		try
		{
			var result = await dataSource.Procedure("sys.sp_sequence_get_range", new { sequence_name = "dbo.TestSequence", range_size = 10 }).AsOutputs().ExecuteAsync();

			Assert.IsTrue(result.ContainsKey("range_first_value"));
			Assert.IsTrue((long)result["range_first_value"] > 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

	/*
	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task SystemStoredProcedure_CaptureOutputsInObject(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource2(dataSourceName, mode);
		try
		{
			var param = new SequenceGetRangeResult() { sequence_name = "dbo.TestSequence", range_size = 10 };
			var outputs = new SequenceGetRangeResult();
			await dataSource.Procedure("sys.sp_sequence_get_range", param).AsNonQuery().CaptureOutputs(outputs).ExecuteAsync();

			Assert.AreNotSame(param, outputs);
			Assert.IsTrue(outputs.range_first_value > 0);
		}
		finally
		{
			Release(dataSource);
		}
	}
	*/

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task SystemStoredProcedure_WorkAround(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource2(dataSourceName, mode);
		try
		{
			var parameters = new List<SqlParameter>()
			{
				new SqlParameter("@sequence_name", "dbo.TestSequence"),
				new SqlParameter("@range_size", "10"),
				new SqlParameter("@range_first_value", DBNull.Value){ SqlDbType= SqlDbType.Variant, Direction=ParameterDirection.Output},
				new SqlParameter("@range_last_value", DBNull.Value){ SqlDbType= SqlDbType.Variant, Direction=ParameterDirection.Output},
				new SqlParameter("@range_cycle_count", DBNull.Value){ SqlDbType= SqlDbType.Int, Direction=ParameterDirection.Output},
				new SqlParameter("@sequence_increment", DBNull.Value){ SqlDbType= SqlDbType.Variant, Direction=ParameterDirection.Output},
				new SqlParameter("@sequence_min_value", DBNull.Value){ SqlDbType= SqlDbType.Variant, Direction=ParameterDirection.Output},
				new SqlParameter("@sequence_max_value", DBNull.Value){ SqlDbType= SqlDbType.Variant, Direction=ParameterDirection.Output},
			};

			await dataSource.Sql(@"EXEC sys.sp_sequence_get_range @sequence_name = @sequence_name,
							   @range_size = @range_size,
							   @range_first_value = @range_first_value OUTPUT,
							   @range_last_value = @range_last_value OUTPUT,
							   @range_cycle_count = @range_cycle_count OUTPUT,
							   @sequence_increment = @sequence_increment OUTPUT,
							   @sequence_min_value = @sequence_min_value OUTPUT,
							   @sequence_max_value = @sequence_max_value OUTPUT;", parameters).ExecuteAsync();

			var rangeFirstValue = parameters.Single(p => p.ParameterName == "@range_first_value").Value;
			Assert.IsTrue((long)rangeFirstValue > 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task SystemStoredProcedure_ManualOutputs(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource3(dataSourceName, mode);
		try
		{
			var parameters = new List<SqlParameter>()
			{
				new SqlParameter("@sequence_name", "dbo.TestSequence"),
				new SqlParameter("@range_size", "10"),
				new SqlParameter("@range_first_value", DBNull.Value){ SqlDbType= SqlDbType.Variant, Direction=ParameterDirection.Output},
				new SqlParameter("@range_last_value", DBNull.Value){ SqlDbType= SqlDbType.Variant, Direction=ParameterDirection.Output},
				new SqlParameter("@range_cycle_count", DBNull.Value){ SqlDbType= SqlDbType.Int, Direction=ParameterDirection.Output},
				new SqlParameter("@sequence_increment", DBNull.Value){ SqlDbType= SqlDbType.Variant, Direction=ParameterDirection.Output},
				new SqlParameter("@sequence_min_value", DBNull.Value){ SqlDbType= SqlDbType.Variant, Direction=ParameterDirection.Output},
				new SqlParameter("@sequence_max_value", DBNull.Value){ SqlDbType= SqlDbType.Variant, Direction=ParameterDirection.Output},
			};

			await dataSource.Procedure("sys.sp_sequence_get_range", parameters).ExecuteAsync();

			var rangeFirstValue = parameters.Single(p => p.ParameterName == "@range_first_value").Value;
			Assert.IsTrue((long)rangeFirstValue > 0);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public async Task MasterStoredProcedure(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource3(dataSourceName, mode);
		try
		{
			await dataSource.Procedure("sp_MScleanupmergepublisher").ExecuteAsync();
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif
}

#endif
