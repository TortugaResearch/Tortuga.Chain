using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.Models;

public class Customer
{
	public int? CustomerKey { get; set; }
	public string FullName { get; set; }
	public string State { get; set; }

	[NotMapped] public string FakeProperty { get; set; }

	public List<Order> Orders { get; } = new List<Order>();
}

#if NET6_0_OR_GREATER
public class CustomerWithDate
{
	public int? CustomerKey { get; set; }
	public string FullName { get; set; }
	public string State { get; set; }
	public DateOnly BirthDay { get; set; }
}

public class CustomerWithTime
{
	public int? CustomerKey { get; set; }
	public string FullName { get; set; }
	public string State { get; set; }
	public TimeOnly PreferredCallTime { get; set; }
}

#endif

public class CustomerWithTimeSpan
{
	public int? CustomerKey { get; set; }
	public string FullName { get; set; }
	public string State { get; set; }
	public TimeSpan PreferredCallTime { get; set; }
}
