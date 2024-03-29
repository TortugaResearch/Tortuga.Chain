using System.ComponentModel.DataAnnotations.Schema;
using Tortuga.Anchor.Modeling;

#if SQLITE
using KeyType = System.Int64;
#else

using KeyType = System.Int32;

#endif

namespace Tests.Models;

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

	public char Gender { get; set; } = ' ';
	public char? Status { get; set; }
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

	public char Gender { get; set; } = ' ';
}

[View("EmployeeWithManager", Schema = "HR")]
public class ManagerWithEmployees
{
	public int? ManagerEmployeeKey { get; set; }
	public string ManagerFirstName { get; set; }
	public string ManagerLastName { get; set; }
	public int? ManagerManagerKey { get; set; }
	public string ManagerMiddleName { get; set; }
	public string ManagerTitle { get; set; }

	public char ManagerGender { get; set; } = ' ';

	public List<EmployeeWithName> DirectReports { get; } = new();
}
