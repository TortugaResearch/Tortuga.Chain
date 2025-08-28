using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.Models;

#if SQLITE
using KeyType = System.Int64;
#elif MYSQL

using KeyType = System.UInt64;

#else

using KeyType = System.Int32;

#endif

/// <summary>
/// This is used to test immutable object constructors
/// </summary>
[Table("Employee", Schema = "HR")]
public class EmployeeLookup
{
	public EmployeeLookup(KeyType employeeKey, string firstName, string lastName)
	{
		EmployeeKey = employeeKey;
		FirstName = firstName;
		LastName = lastName;
	}

	public KeyType EmployeeKey { get; }
	public string FirstName { get; }
	public string LastName { get; }
}

/// <summary>
/// This is used to test immutable object constructors
/// </summary>
[Table("Employee", Schema = "HR")]
public class EmployeeLookup_InferConstructor
{
	public EmployeeLookup_InferConstructor()
	{
		//if RowOptions.InferConstructor is working correctly, this will never be called.
	}

	public EmployeeLookup_InferConstructor(KeyType employeeKey, string firstName, string lastName)
	{
		EmployeeKey = (int)employeeKey;
		FirstName = firstName;
		LastName = lastName;
	}

	public int EmployeeKey { get; }
	public string FirstName { get; }
	public string LastName { get; }
}
