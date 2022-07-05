namespace Tortuga.Chain.Metadata;

/// <summary>
/// This interface is used to allow SqlBuilder to be used with stored procs, TVFs, and other non-table objects.
/// </summary>
/// <typeparam name="TDbType">The type of the database type.</typeparam>
/// <remarks>For internal use only.</remarks>
public interface ISqlBuilderEntryDetails<TDbType> : ISqlBuilderEntryDetails where TDbType : struct
{
	/// <summary>
	/// Gets the database specific DbType
	/// </summary>
	/// <value>The type of the database.</value>
	TDbType? DbType { get; }
}
