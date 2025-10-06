using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tortuga.Anchor.Modeling;

namespace Tests.Models;

#if SQLITE || ACCESS
[Table("Customer")]
#else

[Table("Sales.Customer")]
#endif
public class CustomerWithValidation : ModelBase
{
	[IgnoreOnUpdate]
	public KeyType? CreatedByKey
	{
		get { return Get<KeyType?>(); }
		set { Set(value); }
	}

	[IgnoreOnUpdate]
	public DateTime? CreatedDate
	{
		get { return Get<DateTime?>(); }
		set { Set(value); }
	}

	public int? CustomerKey { get; set; }

	[IgnoreOnInsert, IgnoreOnUpdate]
	public KeyType? DeletedByKey
	{
		get { return Get<KeyType?>(); }
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

	[IgnoreOnInsert, IgnoreOnUpdate]
	public KeyType? UpdatedByKey
	{
		get { return Get<KeyType?>(); }
		set { Set(value); }
	}

	[IgnoreOnInsert, IgnoreOnUpdate]
	public DateTime? UpdatedDate
	{
		get { return Get<DateTime?>(); }
		set { Set(value); }
	}
}
