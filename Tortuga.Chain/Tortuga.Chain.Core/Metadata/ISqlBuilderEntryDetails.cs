namespace Tortuga.Chain.Metadata;

/// <summary>
/// This interface is used to allow SqlBuilder to be used with stored procs, TVFs, and other non-table objects.
/// </summary>
/// <seealso cref="ISqlBuilderEntryDetails" />
/// <remarks>For internal use only.</remarks>
public interface ISqlBuilderEntryDetails
{
	/// <summary>
	/// Gets the name of the color.
	/// </summary>
	/// <value>The name of the color.</value>
	string ClrName { get; }

	/// <summary>
	/// Gets the used by CLR objects using standardized naming conventions..
	/// </summary>
	/// <remarks>The name is PascalCased and underscores are removed.</remarks>
	public string ClrNameStandardized { get; }


	/// <summary>
	/// Indicates the direction of the parameter.
	/// </summary>
	ParameterDirection Direction { get; }

	/// <summary>
	/// Gets or sets the full name of the type including max length, precision, and/or scale.
	/// </summary>
	/// <value>
	/// The full name of the type.
	/// </value>
	string FullTypeName { get; }

	/// <summary>
	/// Gets a value indicating whether this instance is identity.
	/// </summary>
	/// <value><c>true</c> if this instance is identity; otherwise, <c>false</c>.</value>
	bool IsIdentity { get; }

	/// <summary>
	/// Gets the maximum length.
	/// </summary>
	/// <value>
	/// The maximum length.
	/// </value>
	int? MaxLength { get; }

	/// <summary>
	/// Gets the name of the quoted SQL.
	/// </summary>
	/// <value>The name of the quoted SQL.</value>
	string? QuotedSqlName { get; }

	/// <summary>
	/// Gets or sets the scale.
	/// </summary>
	/// <value>
	/// The scale.
	/// </value>
	int? Scale { get; }

	/// <summary>
	/// Gets the name of the SQL.
	/// </summary>
	/// <value>The name of the SQL.</value>
	string? SqlName { get; }

	/// <summary>
	/// Gets the name of the SQL variable.
	/// </summary>
	/// <value>The name of the SQL variable.</value>
	string SqlVariableName { get; }

	/// <summary>
	/// Gets the name of the type.
	/// </summary>
	/// <value>The name of the type.</value>
	string TypeName { get; }
}
