using Tests.Models;

namespace Tests.Materializers;

[TestClass]
public class DateAndTimeOnlyTests : TestBase
{
#if NET6_0_OR_GREATER

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToObject_DateOnlyColumn(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);

		try
		{
			var cust = new CustomerWithDate() { FullName = Guid.NewGuid().ToString(), State = "XX", BirthDay = DateOnly.FromDateTime(DateTime.Now) };

			var key = dataSource.Insert(CustomerTableName, cust).ToInt32().Execute();

			var lookup = dataSource.GetByKey(CustomerTableName, key).ToObject<CustomerWithDate>().Execute();
			Assert.AreEqual(cust.BirthDay, lookup.BirthDay);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToObject_TimeOnlyColumn(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);

		try
		{
			var cust = new CustomerWithTime() { FullName = Guid.NewGuid().ToString(), State = "XX", PreferredCallTime = TimeOnly.FromDateTime(DateTime.Now) };
			var key = dataSource.Insert(CustomerTableName, cust).ToInt32().Execute();

			var lookup = dataSource.GetByKey(CustomerTableName, key).ToObject<CustomerWithTime>().Execute();

			//To account for rounding, allow a 1 ms delta
			Assert.AreEqual(cust.PreferredCallTime.Ticks, lookup.PreferredCallTime.Ticks, TimeSpan.TicksPerMillisecond * 2, $"Actual difference was {cust.PreferredCallTime.Ticks - lookup.PreferredCallTime.Ticks}");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToCollection_DateOnlyColumn(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);

		try
		{
			var uniqueKey = Guid.NewGuid().ToString();
			var cust1 = new CustomerWithDate() { FullName = uniqueKey, State = "XX", BirthDay = DateOnly.FromDateTime(DateTime.Now) };
			var cust2 = new CustomerWithDate() { FullName = uniqueKey, State = "XX", BirthDay = DateOnly.FromDateTime(DateTime.Now) };
			var cust3 = new CustomerWithDate() { FullName = uniqueKey, State = "XX", BirthDay = DateOnly.FromDateTime(DateTime.Now) };
			dataSource.Insert(CustomerTableName, cust1).Execute();
			dataSource.Insert(CustomerTableName, cust2).Execute();
			dataSource.Insert(CustomerTableName, cust3).Execute();

			var lookup = dataSource.From(CustomerTableName, new { FullName = uniqueKey }).WithSorting("CustomerKey").ToCollection<CustomerWithDate>().Execute();
			Assert.AreEqual(cust1.BirthDay, lookup[0].BirthDay);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToCollection_TimeOnlyColumn(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);

		try
		{
			var uniqueKey = Guid.NewGuid().ToString();
			var cust1 = new CustomerWithTime() { FullName = uniqueKey, State = "XX", PreferredCallTime = TimeOnly.FromDateTime(DateTime.Now) };
			var cust2 = new CustomerWithTime() { FullName = uniqueKey, State = "XX", PreferredCallTime = TimeOnly.FromDateTime(DateTime.Now) };
			var cust3 = new CustomerWithTime() { FullName = uniqueKey, State = "XX", PreferredCallTime = TimeOnly.FromDateTime(DateTime.Now) };
			dataSource.Insert(CustomerTableName, cust1).Execute();
			dataSource.Insert(CustomerTableName, cust2).Execute();
			dataSource.Insert(CustomerTableName, cust3).Execute();

			var lookup = dataSource.From(CustomerTableName, new { FullName = uniqueKey }).WithSorting("CustomerKey").ToCollection<CustomerWithTime>().Execute();

			//To account for rounding, allow a 1 ms delta
			Assert.AreEqual(cust1.PreferredCallTime.Ticks, lookup[0].PreferredCallTime.Ticks, TimeSpan.TicksPerMillisecond * 2, $"Actual difference was {cust1.PreferredCallTime.Ticks - lookup[0].PreferredCallTime.Ticks}");
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif
}