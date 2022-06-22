namespace Tortuga.Chain.Metadata;

/// <summary>
/// Metadata for a table or view column
/// </summary>
/// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
public sealed class ColumnMetadata<TDbType> : ColumnMetadata, ISqlBuilderEntryDetails<TDbType>
where TDbType : struct
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
	public ColumnMetadata(string name, bool isComputed, bool isPrimaryKey, bool isIdentity, string typeName, TDbType? dbType, string quotedSqlName, bool? isNullable, int? maxLength, int? precision, int? scale, string fullTypeName, Type? clrType) : base(name, isComputed, isPrimaryKey, isIdentity, typeName, dbType, quotedSqlName, isNullable, maxLength, precision, scale, fullTypeName, clrType)
	{
		DbType = dbType;
	}

	/// <summary>
	/// Gets the type used by the database.
	/// </summary>
	public new TDbType? DbType { get; }

	/// <summary>
	/// Indicates the direction of the parameter.
	/// </summary>
	/// <remarks>Only used for input parameters (e.g. filters)</remarks>
	ParameterDirection ISqlBuilderEntryDetails.Direction => ParameterDirection.Input;

	/// <summary>
	/// Returns a <see cref="string" /> that represents this instance.
	/// </summary>
	/// <returns>
	/// A <see cref="string" /> that represents this instance.
	/// </returns>
	public override string ToString() => SqlName + " (" + TypeName + ")";
}
