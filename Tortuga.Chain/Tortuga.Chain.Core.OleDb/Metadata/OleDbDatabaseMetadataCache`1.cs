using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain.Metadata
{
	/// <summary>
	/// Meatadata cache base class for OleDB databases
	/// </summary>
	/// <typeparam name="TObjectName">The type used to represent database object names.</typeparam>
	public abstract class OleDbDatabaseMetadataCache<TObjectName> : DatabaseMetadataCache<TObjectName, OleDbType>
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
		/// <remarks>This does not take into consideration registered types.</remarks>
		[SuppressMessage("Microsoft.Maintainability", "CA1502")]
		protected override Type? ToClrType(OleDbType dbType, bool isNullable, int? maxLength)
		{
			switch (dbType)
			{
				case OleDbType.BigInt:
					return isNullable ? typeof(long?) : typeof(long);

				case OleDbType.Binary:
				case OleDbType.LongVarBinary:
				case OleDbType.VarBinary:
					return typeof(byte[]);

				case OleDbType.Boolean:
					return typeof(bool);

				case OleDbType.BSTR:
				case OleDbType.Char:
				case OleDbType.VarChar:
				case OleDbType.VarWChar:
				case OleDbType.WChar:
					return (maxLength == 1) ? (isNullable ? typeof(char?) : typeof(char)) : typeof(string);

				case OleDbType.Currency:
				case OleDbType.Decimal:
				case OleDbType.Numeric:
					return isNullable ? typeof(decimal?) : typeof(decimal);

				case OleDbType.Date:
				case OleDbType.DBDate:
				case OleDbType.DBTimeStamp:
				case OleDbType.Filetime:
					return isNullable ? typeof(DateTime?) : typeof(DateTime);

				case OleDbType.DBTime:
					return isNullable ? typeof(TimeSpan?) : typeof(TimeSpan);

				case OleDbType.Double:
					return isNullable ? typeof(double?) : typeof(double);

				case OleDbType.Error:
					return typeof(Exception);

				case OleDbType.Guid:
					return isNullable ? typeof(Guid?) : typeof(Guid);

				case OleDbType.IDispatch:
				case OleDbType.IUnknown:
				case OleDbType.PropVariant:
				case OleDbType.Variant:
					return typeof(object);

				case OleDbType.Integer:
					return isNullable ? typeof(int?) : typeof(int);

				case OleDbType.LongVarChar:
				case OleDbType.LongVarWChar:
					return typeof(string);

				case OleDbType.Single:
					return isNullable ? typeof(float?) : typeof(float);

				case OleDbType.SmallInt:
					return isNullable ? typeof(short?) : typeof(short);

				case OleDbType.TinyInt:
					return isNullable ? typeof(sbyte?) : typeof(sbyte);

				case OleDbType.UnsignedBigInt:
					return isNullable ? typeof(ulong?) : typeof(ulong);

				case OleDbType.UnsignedInt:
					return isNullable ? typeof(uint?) : typeof(uint);

				case OleDbType.UnsignedSmallInt:
					return isNullable ? typeof(ushort?) : typeof(ushort);

				case OleDbType.UnsignedTinyInt:
					return isNullable ? typeof(byte?) : typeof(byte);

				case OleDbType.VarNumeric:
					return isNullable ? typeof(decimal?) : typeof(decimal);

				case OleDbType.Empty:
					return null;
			}
			return null;
		}
	}
}
