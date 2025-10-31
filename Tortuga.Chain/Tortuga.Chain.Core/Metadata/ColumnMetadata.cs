using System.Collections.Frozen;
using System.Data.Common;
using System.Text.RegularExpressions;
using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.Metadata;

/// <summary>
/// Abstract version of ColumnMetadata.
/// </summary>
public abstract class ColumnMetadata
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ColumnMetadata{TDbType}" /> class.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <param name="isComputed">if set to <c>true</c> is a computed column.</param>
	/// <param name="isPrimaryKey">if set to <c>true</c> is a primary key.</param>
	/// <param name="isIdentity">if set to <c>true</c> [is identity].</param>
	/// <param name="typeName">Name of the type.</param>
	/// <param name="dbType">Type used by the database.</param>
	/// <param name="quotedSqlName">Name of the quoted SQL.</param>
	/// <param name="isNullable">Indicates if the column is nullable.</param>
	/// <param name="maxLength">The maximum length.</param>
	/// <param name="precision">The precision.</param>
	/// <param name="scale">The scale.</param>
	/// <param name="fullTypeName">Full name of the type.</param>
	/// <param name="clrType">The CLR type that matches this column's database type.</param>
	protected ColumnMetadata(string name, bool isComputed, bool isPrimaryKey, bool isIdentity, string typeName, object? dbType, string quotedSqlName, bool? isNullable, int? maxLength, int? precision, int? scale, string fullTypeName, Type? clrType)
	{
		TypeName = typeName;
		SqlName = name;
		IsComputed = isComputed;
		IsPrimaryKey = isPrimaryKey;
		IsIdentity = isIdentity;
		DbType = dbType;
		QuotedSqlName = quotedSqlName;

		ClrName = Utilities.ToClrName(name);
		ClrNameStandardized = Utilities.ToClrNameStandardized(name);
		SqlVariableName = "@" + ClrName;

		ClrType = clrType ?? typeof(object);
		ClrBaseType = ClrType.IsGenericType ? ClrType.GetGenericArguments()[0] : ClrType;

		IsNullable = isNullable;
		Precision = precision;
		MaxLength = maxLength;
		Scale = scale;
		FullTypeName = fullTypeName;
	}

	/// <summary>
	/// Gets the name used by CLR objects.
	/// </summary>
	public string ClrName { get; }


	/// <summary>
	/// Gets the used by CLR objects using standardized naming conventions..
	/// </summary>
	/// <remarks>The name is PascalCased and underscores are removed.</remarks>
	public string ClrNameStandardized { get; }

	/// <summary>
	/// The CLR type of the column or System.Object if the type is unknown.
	/// </summary>
	public Type ClrType { get; }

	/// <summary>
	/// The CLR type of the column or NULL if the type is unknown. Nullable types will return the underlying type (e.g. Nullable&lt;int&gt; becomes int)
	/// </summary>
	public Type ClrBaseType { get; }

	/// <summary>
	/// Gets the type used by the database.
	/// </summary>
	public object? DbType { get; }

	/// <summary>
	/// Gets the description of the column.
	/// </summary>
	public string? Description { get; init; }

	/// <summary>
	/// Gets the extended properties.
	/// </summary>
	public IReadOnlyDictionary<string, object?> ExtendedProperties { get; init; } = FrozenDictionary<string, object?>.Empty;

	/// <summary>
	/// Gets or sets the full name of the type including max length, precision, and/or scale.
	/// </summary>
	/// <value>
	/// The full name of the type.
	/// </value>
	public string FullTypeName { get; }

	/// <summary>
	/// Gets a value indicating whether this <see cref="ColumnMetadata{TDbType}"/> is computed.
	/// </summary>
	/// <value>
	/// <c>true</c> if computed; otherwise, <c>false</c>.
	/// </value>
	public bool IsComputed { get; }

	/// <summary>
	/// Gets a value indicating whether this column is an identity column.
	/// </summary>
	/// <value><c>true</c> if this instance is identity; otherwise, <c>false</c>.</value>
	public bool IsIdentity { get; }

	/// <summary>
	/// Gets or sets a value indicating whether this column is nullable.
	/// </summary>
	/// <value>
	/// <c>true</c> if this column is nullable; otherwise, <c>false</c>.
	/// </value>
	public bool? IsNullable { get; }

	/// <summary>
	/// Gets a value indicating whether this column is a primary key.
	/// </summary>
	/// <value><c>true</c> if this instance is primary key; otherwise, <c>false</c>.</value>
	public bool IsPrimaryKey { get; }

	/// <summary>
	/// Gets or sets the maximum length.
	/// </summary>
	/// <value>
	/// The maximum length.
	/// </value>
	public int? MaxLength { get; }

	/// <summary>
	/// Gets or sets the precision.
	/// </summary>
	/// <value>
	/// The precision.
	/// </value>
	public int? Precision { get; }

	/// <summary>
	/// Gets the name used by SQL Server, quoted.
	/// </summary>
	public string QuotedSqlName { get; }

	/// <summary>
	/// Gets or sets the scale.
	/// </summary>
	/// <value>
	/// The scale.
	/// </value>
	public int? Scale { get; }

	/// <summary>
	/// Gets the name used by the database.
	/// </summary>
	public string SqlName { get; }

	/// <summary>
	/// Gets the column, formatted as a SQL variable.
	/// </summary>
	public string SqlVariableName { get; }

	/// <summary>
	/// Gets the name of the type as known to the database.
	/// </summary>
	/// <value>The name of the type as known to the database.</value>
	public string TypeName { get; }

	/// <summary>
	/// Returns the CLR Type name suitable for code generation scenarios.
	/// </summary>
	/// <param name="options">Code generation options.</param>
	/// <returns></returns>
	public string ClrTypeName(NameGenerationOptions options)
	{
		if (options.HasFlag(NameGenerationOptions.ForceNullable) && options.HasFlag(NameGenerationOptions.ForceNonNullable))
			throw new ArgumentException("Cannot specify both ForceNullable and ForceNonNullable", nameof(options));

		var langCount = 0;
		if (options.HasFlag(NameGenerationOptions.CSharp))
			langCount += 1;
		if (options.HasFlag(NameGenerationOptions.VisualBasic))
			langCount += 1;
		if (options.HasFlag(NameGenerationOptions.FSharp))
			langCount += 1;
		if (langCount != 1)
			throw new ArgumentException("Must specify one of CSharp, FSharp, or VisualBasic");

		var metadata = MetadataCache.GetMetadata(ClrType);

		bool effectivelyNullable;

		if (options.HasFlag(NameGenerationOptions.ForceNullable))
			effectivelyNullable = true;
		else if (options.HasFlag(NameGenerationOptions.ForceNonNullable))
			effectivelyNullable = false;
		else if (IsNullable.HasValue)
			effectivelyNullable = IsNullable.Value;
		else
			effectivelyNullable = options.HasFlag(NameGenerationOptions.AssumeNullable);

		string fullTypeName;

		if (options.HasFlag(NameGenerationOptions.CSharp))
			fullTypeName = metadata.CSharpFullName;
		else if (options.HasFlag(NameGenerationOptions.VisualBasic))
			fullTypeName = metadata.VisualBasicFullName;
		else if (options.HasFlag(NameGenerationOptions.FSharp))
			fullTypeName = metadata.FSharpFullName;
		else
			fullTypeName = ClrType.Name;


		//Make the data type nullable if necessary
		if (options.HasFlag(NameGenerationOptions.CSharp)
			&& (ClrType.IsValueType || options.HasFlag(NameGenerationOptions.NullableReferenceTypes))
			&& effectivelyNullable)
		{
			//value types are normally handled upstream unless we're using ForceNullable on a non-nullable column
			if (ClrType.IsValueType && !(IsNullable == true))
				fullTypeName += "?";
			else if (!ClrType.IsValueType)
				fullTypeName += "?";
		}

		if (options.HasFlag(NameGenerationOptions.CSharp))
		{
			//fullTypeName = Regex.Replace(fullTypeName, @"\bSystem\.String\b", "string"); //This is a needless complication
			fullTypeName = Regex.Replace(fullTypeName, @"System\.Nullable<([^>]+)>", "$1?");
			fullTypeName = fullTypeName.Replace("System.String", "string", StringComparison.Ordinal);
			fullTypeName = fullTypeName.Replace("System.UInt32", "uint", StringComparison.Ordinal);
			fullTypeName = fullTypeName.Replace("System.UInt64", "ulong", StringComparison.Ordinal);
			fullTypeName = fullTypeName.Replace("System.UInt16", "ushort", StringComparison.Ordinal);
			fullTypeName = fullTypeName.Replace("System.Int32", "int", StringComparison.Ordinal);
			fullTypeName = fullTypeName.Replace("System.Int64", "long", StringComparison.Ordinal);
			fullTypeName = fullTypeName.Replace("System.Int16", "short", StringComparison.Ordinal);
			fullTypeName = fullTypeName.Replace("System.Byte", "byte", StringComparison.Ordinal);
			fullTypeName = fullTypeName.Replace("System.Boolean", "bool", StringComparison.Ordinal);
			fullTypeName = fullTypeName.Replace("System.Char", "char", StringComparison.Ordinal);
			if (fullTypeName.Count(c => c == '.') == 1)
				fullTypeName = fullTypeName.Replace("System.", "", StringComparison.Ordinal);
		}

		return fullTypeName;
	}
}
