using System.ComponentModel;

namespace Tortuga.Chain;

/// <summary>
/// Controls what happens when performing a model-based update
/// </summary>
[Flags]
public enum UpdateOptions
{
	/// <summary>
	/// Update all non-primary key columns using the primary key columns for the where clause.
	/// </summary>
	None = 0,

	/// <summary>
	/// Uses the IPropertyChangeTracking interface to only update changed properties.
	/// </summary>
	/// <remarks>If this flag is set and IPropertyChangeTracking.IsChanged is false, an error will occur.</remarks>
	ChangedPropertiesOnly = 1,

	/// <summary>
	/// Ignore the primary keys on the table and perform the update using the Key attribute on properties to construct the where clause.
	/// </summary>
	/// <remarks>This is generally used for heap-style tables, though technically heap tables may have primary, non-clustered keys.</remarks>
	UseKeyAttribute = 2,

	/// <summary>
	/// The return original values instead of the updated values.
	/// </summary>
	/// <remarks>This has no effect when using the NonQuery materializer.</remarks>
	ReturnOldValues = 4,

	/// <summary>
	/// Perform a soft delete as part of this update operation.
	/// </summary>
	/// <remarks>This is meant for internal use only.</remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	SoftDelete = 8,

	/// <summary>
	/// The ignore rows affected count. Without this flag, an error will be thrown if the rows affected by the update operation is not one.
	/// </summary>
	IgnoreRowsAffected = 16
}
