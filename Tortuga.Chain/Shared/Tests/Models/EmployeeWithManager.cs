using Tortuga.Anchor.Modeling;

namespace Tests.Models;

[TableAndView(TestBase.EmployeeTableName, ViewName = TestBase.EmployeeViewName)]
public class EmployeeWithManager
{
	public int? EmployeeKey { get; set; }
	public string FirstName { get; set; }
	public string MiddleName { get; set; }
	public string LastName { get; set; }
	public string Title { get; set; }

	public int? ManagerKey { get; set; }

	[Decompose("Manager")]
	public Manager Manager { get; set; }

	[Decompose]
	public AuditInfo AuditInfo { get; set; }

	public string EmployeeId { get; set; }
}

public class Manager
{
	public int? EmployeeKey { get; set; }
	public string FirstName { get; set; }
	public string MiddleName { get; set; }
	public string LastName { get; set; }
	public string Title { get; set; }

	[Decompose]
	public AuditInfo AuditInfo { get; set; }
}
