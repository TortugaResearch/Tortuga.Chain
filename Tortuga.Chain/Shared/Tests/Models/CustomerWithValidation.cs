using System.ComponentModel.DataAnnotations;
using Tortuga.Anchor.Modeling;

namespace Tests.Models;

public class CustomerWithValidation : ModelBase
{
	public int? CustomerKey { get; set; }

	public string FullName
	{
		get { return Get<string>(); }
		set { Set(value); }
	}

	[Required]
	public string State
	{
		get { return Get<string>(); }
		set { Set(value); }
	}

	[IgnoreOnUpdate]
	public DateTime? CreatedDate
	{
		get { return Get<DateTime?>(); }
		set { Set(value); }
	}

	[IgnoreOnInsert, IgnoreOnUpdate]
	public DateTime? UpdatedDate
	{
		get { return Get<DateTime?>(); }
		set { Set(value); }
	}

	[IgnoreOnUpdate]
	public int? CreatedByKey
	{
		get { return Get<int?>(); }
		set { Set(value); }
	}

	[IgnoreOnInsert, IgnoreOnUpdate]
	public int? UpdatedByKey
	{
		get { return Get<int?>(); }
		set { Set(value); }
	}

	[IgnoreOnInsert, IgnoreOnUpdate]
	public int? DeletedByKey
	{
		get { return Get<int?>(); }
		set { Set(value); }
	}

	[IgnoreOnInsert, IgnoreOnUpdate]
	public DateTime? DeletedDate
	{
		get { return Get<DateTime?>(); }
		set { Set(value); }
	}

	[IgnoreOnInsert, IgnoreOnUpdate]
	public bool DeletedFlag
	{
		get { return Get<bool>(); }
		set { Set(value); }
	}
}



