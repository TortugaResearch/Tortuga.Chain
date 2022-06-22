namespace Tortuga.Chain.Metadata;

/// <summary>
/// Metadata for a stored procedure parameter
/// </summary>
public abstract class ParameterMetadata : ISqlBuilderEntryDetails
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ParameterMetadata{TDbType}"/> class.
	/// </summary>
	/// <param name="sqlParameterName">Name of the SQL parameter.</param>
	/// <param name="sqlVariableName">Name of the SQL variable.</param>
	/// <param name="typeName">Name of the type as known to the database.</param>
	/// <param name="dbType">Type of the database column as an enum.</param>
	/// <param name="isNullable">if set to <c>true</c> is nullable.</param>
	/// <param name="maxLength">The maximum length.</param>
	/// <param name="precision">The precision.</param>
	/// <param name="scale">The scale.</param>
	/// <param name="fullTypeName">Full name of the type.</param>
	/// <param name="direction">Indicates the direction of the parameter.</param>
	protected ParameterMetadata(string sqlParameterName, string sqlVariableName, string typeName, object? dbType, bool? isNullable, int? maxLength, int? precision, int? scale, string fullTypeName, ParameterDirection direction)
	{
		SqlParameterName = sqlParameterName;
		SqlVariableName = sqlVariableName;
		TypeName = typeName;
		ClrName = Utilities.ToClrName(sqlParameterName);
		DbType = dbType;
		IsNullable = isNullable;
		MaxLength = maxLength;
		Precision = precision;
		Scale = scale;
		FullTypeName = fullTypeName;
		Direction = direction;
	}

	/// <summary>
	/// Gets the name used by CLR objects.
	/// </summary>
	public string ClrName { get; }

	/// <summary>
	/// Gets the type of the database column as an enum.
	/// </summary>
	public object? DbType { get; }

	/// <summary>
	/// Indicates the direction of the parameter.
	/// </summary>
	public ParameterDirection Direction { get; }

	/// <summary>
	/// Gets or sets the full name of the type including max length, precision, and/or scale.
	/// </summary>
	/// <value>
	/// The full name of the type.
	/// </value>
	/// <remarks>This will be null if the data source doesn't support detailed parameter metadata.</remarks>
	public string FullTypeName { get; }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "<Pending>")]
	bool ISqlBuilderEntryDetails.IsIdentity => false;

	/// <summary>
	/// Gets a value indicating whether this instance is nullable.
	/// </summary>
	/// <remarks>This will be null if the data source doesn't support detailed parameter metadata.</remarks>
	public bool? IsNullable { get; }

	/// <summary>
	/// Gets the maximum length.
	/// </summary>
	/// <value>The maximum length.</value>
	/// <remarks>This will be null if the data source doesn't support detailed parameter metadata or if this value isn't applicable to the data type.</remarks>
	public int? MaxLength { get; }

	/// <summary>
	/// Gets the precision.
	/// </summary>
	/// <value>The precision.</value>
	/// <remarks>This will be null if the data source doesn't support detailed parameter metadata or if this value isn't applicable to the data type.</remarks>
	public int? Precision { get; }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "<Pending>")]
	string? ISqlBuilderEntryDetails.QuotedSqlName => null;

	int? ISqlBuilderEntryDetails.Scale => null;

	/// <summary>
	/// Gets or sets the scale.
	/// </summary>
	/// <value>The scale.</value>
	/// <remarks>This will be null if the data source doesn't support detailed parameter metadata or if this value isn't applicable to the data type.</remarks>
	public int? Scale { get; }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "<Pending>")]
	string? ISqlBuilderEntryDetails.SqlName => null;

	/// <summary>
	/// Gets the name used by the database.
	/// </summary>
	public string SqlParameterName { get; }

	/// <summary>
	/// Gets the name of the SQL variable.
	/// </summary>
	/// <value>The name of the SQL variable.</value>
	public string SqlVariableName { get; }

	/// <summary>
	/// Gets the name of the type as known to the database.
	/// </summary>
	public string TypeName { get; }
}
