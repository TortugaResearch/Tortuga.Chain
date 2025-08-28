using System.ComponentModel.DataAnnotations.Schema;
using Tortuga.Anchor.Modeling;

namespace Tests.Models;

[Table("Employee", Schema = "HR")]
public class Employee
{
	[IgnoreOnInsert, IgnoreOnUpdate]
	public DateTime? CreatedDate { get; set; }

	public string EmployeeId { get; set; } = Guid.NewGuid().ToString();
	public KeyType? EmployeeKey { get; set; }
	public string FirstName { get; set; }
	public char Gender { get; set; } = ' ';
	public string LastName { get; set; }
	public KeyType? ManagerKey { get; set; }
	public string MiddleName { get; set; }
	public char? Status { get; set; }
	public string Title { get; set; }

	[IgnoreOnUpdate]
	public DateTime? UpdatedDate { get; set; }
}

[Table("Employee", Schema = "HR")]
public record EmployeeRecordFilter
{
	public string EmployeeId { get; set; }
}

[Table("Employee", Schema = "HR")]
public record EmployeeRecord
{
	[IgnoreOnInsert, IgnoreOnUpdate]
	public DateTime? CreatedDate { get; set; }

	public string EmployeeId { get; set; } = Guid.NewGuid().ToString();
	public KeyType? EmployeeKey { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public KeyType? ManagerKey { get; set; }
	public string MiddleName { get; set; }
	public string Title { get; set; }

	[IgnoreOnUpdate]
	public DateTime? UpdatedDate { get; set; }
	public char Gender { get; set; } = ' ';
}

public class EmployeeWithName
{
	public string EmployeeId { get; set; } = Guid.NewGuid().ToString();
	public KeyType? EmployeeKey { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
}

public class EmployeeWithoutKey
{
	[IgnoreOnInsert, IgnoreOnUpdate]
	public DateTime? CreatedDate { get; set; }

	public string EmployeeId { get; set; }
	public string FirstName { get; set; }
	public char Gender { get; set; } = ' ';
	public string LastName { get; set; }
	public int? ManagerKey { get; set; }
	public string MiddleName { get; set; }
	public string Title { get; set; }

	[IgnoreOnUpdate]
	public DateTime? UpdatedDate { get; set; }
}

[View("EmployeeWithManager", Schema = "HR")]
public class ManagerWithEmployees
{
	public List<EmployeeWithName> DirectReports { get; } = new();
	public KeyType? ManagerEmployeeKey { get; set; }
	public string ManagerFirstName { get; set; }
	public char ManagerGender { get; set; } = ' ';
	public string ManagerLastName { get; set; }
	public KeyType? ManagerManagerKey { get; set; }
	public string ManagerMiddleName { get; set; }
	public string ManagerTitle { get; set; }
}
