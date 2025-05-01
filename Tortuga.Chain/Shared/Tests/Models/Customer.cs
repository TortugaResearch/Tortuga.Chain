using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.Models;

public class Customer
{
	public int? CustomerKey { get; set; }
	[NotMapped] public string FakeProperty { get; set; }
	public string FullName { get; set; }
	public List<Order> Orders { get; } = new List<Order>();
	public string State { get; set; }
}

public class CustomerWithDate
{
	public DateOnly BirthDay { get; set; }
	public int? CustomerKey { get; set; }
	public string FullName { get; set; }
	public string State { get; set; }
}

public class CustomerWithTime
{
	public int? CustomerKey { get; set; }
	public string FullName { get; set; }
	public TimeOnly PreferredCallTime { get; set; }
	public string State { get; set; }
}

public class CustomerWithTimeSpan
{
	public int? CustomerKey { get; set; }
	public string FullName { get; set; }
	public TimeSpan PreferredCallTime { get; set; }
	public string State { get; set; }
}
