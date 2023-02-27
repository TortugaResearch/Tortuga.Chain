namespace Tortuga.Chain;

/// <summary>
/// The SqlServerParameterDefaults struct is used to override the Data Source level defaults for parameters.
/// </summary>
/// <remarks>This is primarily used by raw SQL calls when the parameter's database type/size cannot be inferred.</remarks>
public struct SqlServerParameterDefaults
{
	/// <summary>
	/// Gets the default type of string parameters. This is used when the query builder cannot determine the best parameter type.
	/// </summary>
	/// <remarks>Set this if encountering performance issues from type conversions in the execution plan.</remarks>
	public AbstractDbType? StringType { get; init; }

	/// <summary>
	/// Gets the default length of string parameters. This is used when the query builder cannot determine the best parameter type and the parameter's actual length is smaller than the default length.
	/// </summary>
	/// <remarks>Set this is encountering an excessive number of execution plans that only differ by the length of a string .</remarks>
	public int? StringLength { get; init; }

	/// <summary>
	/// Used for the length of a varChar(max) or nVarChar(max).
	/// </summary>
	public const int Max = -1;

}
