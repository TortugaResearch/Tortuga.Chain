using Tests.Models;
using Tortuga.Chain;

namespace Tests.Materializers;

[TestClass]
public class DateAndTimeOnlyTests : TestBase
{
#if MYSQL

	//Use a 1 second delta because MySQL doesn't support milliseconds.
	const long TimeSpanDelta = TimeSpan.TicksPerSecond * 60;

#else
	const long TimeSpanDelta = TimeSpan.TicksPerSecond;
#endif

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
			Assert.IsTrue(cust.PreferredCallTime.Ticks - lookup.PreferredCallTime.Ticks < TimeSpanDelta, $"Actual difference was {cust.PreferredCallTime.Ticks - lookup.PreferredCallTime.Ticks}");
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
			Assert.IsTrue(cust1.PreferredCallTime.Ticks - lookup[0].PreferredCallTime.Ticks < TimeSpanDelta, $"Actual difference was {cust1.PreferredCallTime.Ticks - lookup[0].PreferredCallTime.Ticks}");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToObject_DateOnlyColumn_Compiled(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);

		try
		{
			var cust = new CustomerWithDate() { FullName = Guid.NewGuid().ToString(), State = "XX", BirthDay = DateOnly.FromDateTime(DateTime.Now) };

			var key = dataSource.Insert(CustomerTableName, cust).ToInt32().Execute();

			var lookup = dataSource.GetByKey(CustomerTableName, key).Compile().ToObject<CustomerWithDate>().Execute();
			Assert.AreEqual(cust.BirthDay, lookup.BirthDay);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToObject_TimeOnlyColumn_Compiled(string dataSourceName, DataSourceType mode)
	{
		var dataSource = DataSource(dataSourceName, mode);

		try
		{
			var cust = new CustomerWithTime() { FullName = Guid.NewGuid().ToString(), State = "XX", PreferredCallTime = TimeOnly.FromDateTime(DateTime.Now) };
			var key = dataSource.Insert(CustomerTableName, cust).ToInt32().Execute();

			var lookup = dataSource.GetByKey(CustomerTableName, key).Compile().ToObject<CustomerWithTime>().Execute();

			//To account for rounding, allow a 1 ms delta
			Assert.IsTrue(cust.PreferredCallTime.Ticks - lookup.PreferredCallTime.Ticks < TimeSpanDelta, $"Actual difference was {cust.PreferredCallTime.Ticks - lookup.PreferredCallTime.Ticks}");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToCollection_DateOnlyColumn_Compiled(string dataSourceName, DataSourceType mode)
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

			var lookup = dataSource.From(CustomerTableName, new { FullName = uniqueKey }).WithSorting("CustomerKey").Compile().ToCollection<CustomerWithDate>().Execute();
			Assert.AreEqual(cust1.BirthDay, lookup[0].BirthDay);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, BasicData(DataSourceGroup.Primary)]
	public void ToCollection_TimeOnlyColumn_Compiled(string dataSourceName, DataSourceType mode)
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

			var lookup = dataSource.From(CustomerTableName, new { FullName = uniqueKey }).WithSorting("CustomerKey").Compile().ToCollection<CustomerWithTime>().Execute();

			//To account for rounding, allow a 1 ms delta
			Assert.IsTrue(cust1.PreferredCallTime.Ticks - lookup[0].PreferredCallTime.Ticks < TimeSpanDelta, $"Actual difference was {cust1.PreferredCallTime.Ticks - lookup[0].PreferredCallTime.Ticks}");
		}
		finally
		{
			Release(dataSource);
		}
	}
}
