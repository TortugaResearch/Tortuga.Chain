using System.ComponentModel.DataAnnotations.Schema;
using Tortuga.Anchor.Modeling;

namespace Tests.Models
{
	[Table("Employee", Schema = "HR")]
	public class Employee
	{
		[IgnoreOnInsert, IgnoreOnUpdate]
		public DateTime? CreatedDate { get; set; }

		public string EmployeeId { get; set; } = Guid.NewGuid().ToString();
		public int? EmployeeKey { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int? ManagerKey { get; set; }
		public string MiddleName { get; set; }
		public string Title { get; set; }

		[IgnoreOnUpdate]
		public DateTime? UpdatedDate { get; set; }
	}

	public class EmployeeWithName
	{
		public string EmployeeId { get; set; } = Guid.NewGuid().ToString();
		public int? EmployeeKey { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}

	public class EmployeeWithoutKey
	{
		[IgnoreOnInsert, IgnoreOnUpdate]
		public DateTime? CreatedDate { get; set; }

		public string EmployeeId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int? ManagerKey { get; set; }
		public string MiddleName { get; set; }
		public string Title { get; set; }

		[IgnoreOnUpdate]
		public DateTime? UpdatedDate { get; set; }
	}
}
