using System.ComponentModel.DataAnnotations.Schema;
using Tortuga.Anchor.Modeling;

namespace Tests.Models;

[Table(TestBase.EmployeeTableName)]
[View(TestBase.EmployeeViewName)]
public class EmployeeWithManager
{
	[Decompose]
	public AuditInfo AuditInfo { get; set; }

	public string EmployeeId { get; set; }
	public int? EmployeeKey { get; set; }
	public string FirstName { get; set; }
	public char Gender { get; set; } = ' ';
	public string LastName { get; set; }

	[Decompose("Manager")]
	public Manager Manager { get; set; }

	public int? ManagerKey { get; set; }
	public string MiddleName { get; set; }
	public string Title { get; set; }
}

public class Manager
{
	[Decompose]
	public AuditInfo AuditInfo { get; set; }

	public int? EmployeeKey { get; set; }
	public string FirstName { get; set; }
	public char Gender { get; set; } = ' ';
	public string LastName { get; set; }
	public string MiddleName { get; set; }
	public string Title { get; set; }
}
