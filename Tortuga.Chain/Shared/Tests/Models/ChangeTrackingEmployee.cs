using Tortuga.Anchor.Modeling;

namespace Tests.Models;

public class ChangeTrackingEmployee : ChangeTrackingModelBase
{
	[IgnoreOnInsert, IgnoreOnUpdate]
	public DateTime? CreatedDate
	{
		get { return Get<DateTime?>(); }
		set { Set(value); }
	}

	public string EmployeeId
	{
		get { return Get<string>(); }
		set { Set(value); }
	}

	public long? EmployeeKey
	{
		get { return Get<long?>(); }
		set { Set(value); }
	}

	public string FirstName
	{
		get { return Get<string>(); }
		set { Set(value); }
	}

	public string LastName
	{
		get { return Get<string>(); }
		set { Set(value); }
	}

	public int? ManagerKey
	{
		get { return Get<int?>(); }
		set { Set(value); }
	}

	public string MiddleName
	{
		get { return Get<string>(); }
		set { Set(value); }
	}

	public string Title
	{
		get { return Get<string>(); }
		set { Set(value); }
	}

	[IgnoreOnUpdate]
	public DateTime? UpdatedDate
	{
		get { return Get<DateTime?>(); }
		set { Set(value); }
	}
}
