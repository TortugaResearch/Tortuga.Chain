namespace Tortuga.Chain.AuditRules;

/// <summary>
/// Indicates the type of operation being performed.
/// </summary>
/// <remarks>Keep numbers in sync with Metadata.OperationType.</remarks>
[Flags]
public enum OperationTypes
{
	/// <summary>
	/// Undefined operation.
	/// </summary>
	None = 0,

	/// <summary>
	/// Applies the rule when performing an insert operation (including the insert portion of an Upsert)
	/// </summary>
	Insert = 1,

	/// <summary>
	/// Applies the rule when performing an update operation (including the update portion of an Upsert)
	/// </summary>
	Update = 2,

	/// <summary>
	/// Applies the rule when performing an insert or update operation (including the update portion of an Upsert)
	/// </summary>
	InsertOrUpdate = Insert | Update,

	/// <summary>
	/// Applies the rule when performing a delete operation.
	/// </summary>
	/// <remarks>Usually used for soft delete support</remarks>
	Delete = 4,

	/// <summary>
	/// Applies the rule when performing a select operation.
	/// </summary>
	/// <remarks>Usually used for soft delete support</remarks>
	Select = 8,

	/// <summary>
	/// Applies the rule when performing a select or delete operation.
	/// </summary>
	/// <remarks>Usually used for soft delete support</remarks>
	SelectOrDelete = Delete | Select
}
