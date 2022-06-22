using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain.Metadata;

/// <summary>
/// Meatadata cache base class for DbType databases
/// </summary>
/// <typeparam name="TObjectName">The type used to represent database object names.</typeparam>
public abstract class DbDatabaseMetadataCache<TObjectName> : DatabaseMetadataCache<TObjectName, DbType>
	where TObjectName : struct
{
	/// <summary>
	/// Returns the CLR type that matches the indicated database column type.
	/// </summary>
	/// <param name="dbType">Type of the database column.</param>
	/// <param name="isNullable">If nullable, Nullable versions of primitive types are returned.</param>
	/// <param name="maxLength">Optional length. Used to distinguish between a char and string. Defaults to string.</param>
	/// <returns>
	/// A CLR type or NULL if the type is unknown.
	/// </returns>
	/// <remarks>
	/// This does not take into consideration registered types.
	/// </remarks>
	[SuppressMessage("Microsoft.Maintainability", "CA1502")]
	protected override Type? ToClrType(DbType dbType, bool isNullable, int? maxLength)
	{
		switch (dbType)
		{
			case DbType.AnsiString:
			case DbType.AnsiStringFixedLength:
			case DbType.StringFixedLength:
			case DbType.String:
				return (maxLength == 1) ? (isNullable ? typeof(char?) : typeof(char)) : typeof(string);

			case DbType.Binary:
				return typeof(byte[]);

			case DbType.Byte:
				return isNullable ? typeof(byte?) : typeof(byte);

			case DbType.Boolean:
				return isNullable ? typeof(bool?) : typeof(bool);

			case DbType.Currency:
			case DbType.Decimal:
			case DbType.VarNumeric:
				return isNullable ? typeof(decimal?) : typeof(decimal);

			case DbType.Date:
			case DbType.DateTime:
			case DbType.DateTime2:
			case DbType.Time:
				return isNullable ? typeof(DateTime?) : typeof(DateTime);

			case DbType.Double:
				return isNullable ? typeof(double?) : typeof(double);

			case DbType.Guid:
				return isNullable ? typeof(Guid?) : typeof(Guid);

			case DbType.Int16:
				return isNullable ? typeof(short?) : typeof(short);

			case DbType.Int32:
				return isNullable ? typeof(int?) : typeof(int);

			case DbType.Int64:
				return isNullable ? typeof(long?) : typeof(long);

			case DbType.Object:
				return typeof(object);

			case DbType.SByte:
				return isNullable ? typeof(sbyte?) : typeof(sbyte);

			case DbType.Single:
				return isNullable ? typeof(float?) : typeof(float);

			case DbType.UInt16:
				return isNullable ? typeof(ushort?) : typeof(ushort);

			case DbType.UInt32:
				return isNullable ? typeof(uint?) : typeof(uint);

			case DbType.UInt64:
				return isNullable ? typeof(ulong?) : typeof(ulong);

			case DbType.Xml:
				return typeof(string);

			case DbType.DateTimeOffset:
				return isNullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset);
		}
		return null;
	}
}
