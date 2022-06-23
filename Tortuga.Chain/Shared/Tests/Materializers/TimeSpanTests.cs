using Tests.Models;
using Tortuga.Chain;

namespace Tests.Materializers;

[TestClass]
public class TimeSpanTests : TestBase
{
#if MYSQL

	//Use a 1 second delta because MySQL doesn't support milliseconds.
	const long TimeSpanDelta = TimeSpan.TicksPerSecond * 60;

#else
	const long TimeSpanDelta = TimeSpan.TicksPerSecond ;
#endif

#if NET6_0_OR_GREATER

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToObject_TimeSpanColumn(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);

		try
		{
			var cust = new CustomerWithTimeSpan() { FullName = Guid.NewGuid().ToString(), State = "XX", PreferredCallTime = DateTime.Now.TimeOfDay };
			var key = dataSource.Insert(CustomerTableName, cust).ToInt32().Execute();

			var lookup = dataSource.GetByKey(CustomerTableName, key).ToObject<CustomerWithTimeSpan>().Execute();

			//To account for rounding, allow a 1 ms delta
			Assert.IsTrue(cust.PreferredCallTime.Ticks - lookup.PreferredCallTime.Ticks < TimeSpanDelta, $"Actual difference was {cust.PreferredCallTime.Ticks - lookup.PreferredCallTime.Ticks}");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToCollection_TimeSpanColumn(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);

		try
		{
			var uniqueKey = Guid.NewGuid().ToString();
			var cust1 = new CustomerWithTimeSpan() { FullName = uniqueKey, State = "XX", PreferredCallTime = DateTime.Now.TimeOfDay };
			var cust2 = new CustomerWithTimeSpan() { FullName = uniqueKey, State = "XX", PreferredCallTime = DateTime.Now.TimeOfDay };
			var cust3 = new CustomerWithTimeSpan() { FullName = uniqueKey, State = "XX", PreferredCallTime = DateTime.Now.TimeOfDay };
			dataSource.Insert(CustomerTableName, cust1).Execute();
			dataSource.Insert(CustomerTableName, cust2).Execute();
			dataSource.Insert(CustomerTableName, cust3).Execute();

			var lookup = dataSource.From(CustomerTableName, new { FullName = uniqueKey }).WithSorting("CustomerKey").ToCollection<CustomerWithTimeSpan>().Execute();

			//To account for rounding, allow a 1 ms delta
			Assert.IsTrue(cust1.PreferredCallTime.Ticks - lookup[0].PreferredCallTime.Ticks < TimeSpanDelta, $"Actual difference was {cust1.PreferredCallTime.Ticks - lookup[0].PreferredCallTime.Ticks}");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToObject_TimeSpanColumn_Compiled(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);

		try
		{
			var cust = new CustomerWithTimeSpan() { FullName = Guid.NewGuid().ToString(), State = "XX", PreferredCallTime = DateTime.Now.TimeOfDay };
			var key = dataSource.Insert(CustomerTableName, cust).ToInt32().Execute();

			var lookup = dataSource.GetByKey(CustomerTableName, key).Compile().ToObject<CustomerWithTimeSpan>().Execute();

			//To account for rounding, allow a 1 ms delta
			Assert.IsTrue(cust.PreferredCallTime.Ticks - lookup.PreferredCallTime.Ticks < TimeSpanDelta, $"Actual difference was {cust.PreferredCallTime.Ticks - lookup.PreferredCallTime.Ticks}");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToCollection_TimeSpanColumn_Compiled(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);

		try
		{
			var uniqueKey = Guid.NewGuid().ToString();
			var cust1 = new CustomerWithTimeSpan() { FullName = uniqueKey, State = "XX", PreferredCallTime = DateTime.Now.TimeOfDay };
			var cust2 = new CustomerWithTimeSpan() { FullName = uniqueKey, State = "XX", PreferredCallTime = DateTime.Now.TimeOfDay };
			var cust3 = new CustomerWithTimeSpan() { FullName = uniqueKey, State = "XX", PreferredCallTime = DateTime.Now.TimeOfDay };
			dataSource.Insert(CustomerTableName, cust1).Execute();
			dataSource.Insert(CustomerTableName, cust2).Execute();
			dataSource.Insert(CustomerTableName, cust3).Execute();

			var lookup = dataSource.From(CustomerTableName, new { FullName = uniqueKey }).WithSorting("CustomerKey").Compile().ToCollection<CustomerWithTimeSpan>().Execute();

			//To account for rounding, allow a 1 ms delta
			Assert.IsTrue(cust1.PreferredCallTime.Ticks - lookup[0].PreferredCallTime.Ticks < TimeSpanDelta, $"Actual difference was {cust1.PreferredCallTime.Ticks - lookup[0].PreferredCallTime.Ticks}");
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif
}
