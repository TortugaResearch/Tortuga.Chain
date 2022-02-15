using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.Models
{
	public class Customer
	{
		public int? CustomerKey { get; set; }
		public string FullName { get; set; }
		public string State { get; set; }

		[NotMapped] public string FakeProperty { get; set; }

		public List<Order> Orders { get; } = new List<Order>();
	}
}
