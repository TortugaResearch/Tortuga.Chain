using System.ComponentModel.DataAnnotations.Schema;
using Tortuga.Anchor.Modeling;

namespace Tests.Models
{
	[Table("Employee", Schema = "HR")]
	[View("EmployeeWithManager", Schema = "HR")]
	public class EmployeeWithView
	{
		[IgnoreOnInsert, IgnoreOnUpdate]
		public DateTime? CreatedDate { get; set; }

		public string EmployeeId { get; set; } = Guid.NewGuid().ToString();
		public KeyType? EmployeeKey { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int? ManagerKey { get; set; }
		public string ManagerLastName { get; set; }
		public string MiddleName { get; set; }
		public string Title { get; set; }

		[IgnoreOnUpdate]
		public DateTime? UpdatedDate { get; set; }
	}
}
