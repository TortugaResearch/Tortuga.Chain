using System.Data.Odbc;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain.Metadata;

/// <summary>
/// Meatadata cache base class for ODBC databases
/// </summary>
/// <typeparam name="TObjectName">The type used to represent database object names.</typeparam>
public abstract class OdbcDatabaseMetadataCache<TObjectName> : DatabaseMetadataCache<TObjectName, OdbcType>
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
	protected override Type? ToClrType(OdbcType dbType, bool isNullable, int? maxLength)
	{
		switch (dbType)
		{
			case OdbcType.BigInt:
				return isNullable ? typeof(long?) : typeof(long);

			case OdbcType.Binary:
				return typeof(byte[]);

			case OdbcType.Bit:
				return isNullable ? typeof(bool?) : typeof(bool);

			case OdbcType.NChar:
			case OdbcType.NVarChar:
			case OdbcType.Char:
			case OdbcType.VarChar:
				return (maxLength == 1) ?
					(isNullable ? typeof(char?) : typeof(char))
				: typeof(string);

			case OdbcType.Decimal:
			case OdbcType.Numeric:
				return isNullable ? typeof(decimal?) : typeof(decimal);

			case OdbcType.Double:
				return isNullable ? typeof(double?) : typeof(double);

			case OdbcType.Int:
				return isNullable ? typeof(int?) : typeof(int);

			case OdbcType.Text:
			case OdbcType.NText:
				return typeof(string);

			case OdbcType.Real:
				return isNullable ? typeof(float?) : typeof(float);

			case OdbcType.UniqueIdentifier:
				return isNullable ? typeof(Guid?) : typeof(Guid);

			case OdbcType.SmallInt:
				return isNullable ? typeof(ushort?) : typeof(ushort);

			case OdbcType.TinyInt:
				return isNullable ? typeof(byte?) : typeof(byte);

			case OdbcType.VarBinary:
			case OdbcType.Image:
			case OdbcType.Timestamp:
				return typeof(byte[]);

			case OdbcType.Date:
			case OdbcType.DateTime:
			case OdbcType.SmallDateTime:
			case OdbcType.Time:
				return typeof(DateTime);
		}
		return null;
	}
}