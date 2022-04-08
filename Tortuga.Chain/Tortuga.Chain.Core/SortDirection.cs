using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain;

/// <summary>
/// Used to indicate the sort direction
/// </summary>
[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
public enum SortDirection : byte
{
	/// <summary>
	/// Ascending
	/// </summary>
	Ascending = 0,

	/// <summary>
	/// Descending
	/// </summary>
	Descending = 1
}
