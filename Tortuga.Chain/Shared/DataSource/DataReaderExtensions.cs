using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

#if MYSQL

using MySqlConnector;

#endif

namespace Tortuga.Chain.Metadata
{
	/// <summary>
	/// DataReaderExtensions is used for generating metadata.
	/// </summary>
	internal static class DataReaderExtensions
	{
#if !NET5_0_OR_GREATER
		/// <summary>
		/// Gets the boolean.
		/// </summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static bool GetBoolean(this DbDataReader dataReader, string columnName)
		{
			return dataReader.GetBoolean(dataReader.GetOrdinal(columnName));
		}
#endif

		/// <summary>
		/// Gets the boolean or null.
		/// </summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static bool? GetBooleanOrNull(this DbDataReader dataReader, string columnName)
		{
			var ordinal = dataReader.GetOrdinal(columnName);
			if (dataReader.IsDBNull(ordinal))
				return null;
			else
				return dataReader.GetBoolean(ordinal);
		}

#if !NET5_0_OR_GREATER
		/// <summary>
		/// Gets the int16.
		/// </summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>System.Int16.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static short GetInt16(this DbDataReader dataReader, string columnName)
		{
			return dataReader.GetInt16(dataReader.GetOrdinal(columnName));
		}
#endif

		/// <summary>
		/// Gets the int16 or null.
		/// </summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>System.Nullable&lt;System.Int16&gt;.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static short? GetInt16OrNull(this DbDataReader dataReader, string columnName)
		{
			var ordinal = dataReader.GetOrdinal(columnName);
			if (dataReader.IsDBNull(ordinal))
				return null;
			else
				return dataReader.GetInt16(ordinal);
		}

#if !NET5_0_OR_GREATER
		/// <summary>
		/// Gets the int32.
		/// </summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>System.Int32.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static int GetInt32(this DbDataReader dataReader, string columnName)
		{
			return dataReader.GetInt32(dataReader.GetOrdinal(columnName));
		}
#endif

		/// <summary>
		/// Gets the int32 or null.
		/// </summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static int? GetInt32OrNull(this DbDataReader dataReader, string columnName)
		{
			var ordinal = dataReader.GetOrdinal(columnName);
			if (dataReader.IsDBNull(ordinal))
				return null;
			else
				return dataReader.GetInt32(ordinal);
		}

#if !NET5_0_OR_GREATER
		/// <summary>
		/// Gets the int64.
		/// </summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>System.Int64.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static long GetInt64(this DbDataReader dataReader, string columnName)
		{
			return dataReader.GetInt64(dataReader.GetOrdinal(columnName));
		}
#endif

#if !NET5_0_OR_GREATER
		/// <summary>Gets the byte.</summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>System.Int64.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static long GetByte(this DbDataReader dataReader, string columnName)
		{
			return dataReader.GetByte(dataReader.GetOrdinal(columnName));
		}
#endif

		/// <summary>
		/// Gets the int64 or null.
		/// </summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>System.Nullable&lt;System.Int64&gt;.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static long? GetInt64OrNull(this DbDataReader dataReader, string columnName)
		{
			var ordinal = dataReader.GetOrdinal(columnName);
			if (dataReader.IsDBNull(ordinal))
				return null;
			else
				return dataReader.GetInt64(ordinal);
		}

#if !NET5_0_OR_GREATER
		/// <summary>
		/// Gets the string.
		/// </summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>System.String.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static string GetString(this DbDataReader dataReader, string columnName)
		{
			return dataReader.GetString(dataReader.GetOrdinal(columnName));
		}
#endif

#if !NET5_0_OR_GREATER
		/// <summary>
		/// Gets the char.
		/// </summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static char GetChar(this DbDataReader dataReader, string columnName)
		{
			return dataReader.GetChar(dataReader.GetOrdinal(columnName));
		}
#endif

		/// <summary>
		/// Gets the string or null.
		/// </summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>System.String.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static string? GetStringOrNull(this DbDataReader dataReader, string columnName)
		{
			var ordinal = dataReader.GetOrdinal(columnName);
			if (dataReader.IsDBNull(ordinal))
				return null;
			else
				return dataReader.GetString(ordinal);
		}

		/// <summary>
		/// Gets the char or null.
		/// </summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static char? GetCharOrNull(this DbDataReader dataReader, string columnName)
		{
			var ordinal = dataReader.GetOrdinal(columnName);
			if (dataReader.IsDBNull(ordinal))
				return null;
			else
				return dataReader.GetChar(ordinal);
		}

		/// <summary>
		/// Gets the uint32.
		/// </summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>System.UInt32.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static uint GetUInt32(this DbDataReader dataReader, string columnName)
		{
			return dataReader.GetValue<uint>(columnName);
		}

		/// <summary>
		/// Gets the uint32 or null.
		/// </summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>System.Nullable&lt;System.UInt32&gt;.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static uint? GetUInt32OrNull(this DbDataReader dataReader, string columnName)
		{
			var ordinal = dataReader.GetOrdinal(columnName);
			if (dataReader.IsDBNull(ordinal))
				return null;
			else
				return (uint)dataReader.GetValue(ordinal);
		}

		/// <summary>Gets the u int64.</summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>System.UInt64.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static ulong GetUInt64(this DbDataReader dataReader, string columnName)
		{
			return dataReader.GetValue<ulong>(dataReader.GetOrdinal(columnName));
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>System.Object.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static object GetValue(this DbDataReader dataReader, string columnName)
		{
			return dataReader.GetValue(dataReader.GetOrdinal(columnName));
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>T.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static T GetValue<T>(this DbDataReader dataReader, string columnName)
		{
			return (T)dataReader.GetValue(dataReader.GetOrdinal(columnName));
		}

		/// <summary>Gets the value.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="ordinal">The ordinal.</param>
		/// <returns>T.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static T GetValue<T>(this DbDataReader dataReader, int ordinal)
		{
			return (T)dataReader.GetValue(ordinal);
		}

#if !NET5_0_OR_GREATER
		/// <summary>
		/// Determines whether [is database null] [the specified column name].
		/// </summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns><c>true</c> if [is database null] [the specified column name]; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static bool IsDBNull(this DbDataReader dataReader, string columnName)
		{
			return dataReader.IsDBNull(dataReader.GetOrdinal(columnName));
		}
#endif

#if MYSQL

		/// <summary>Gets the u int64.</summary>
		/// <param name="dataReader">The data reader.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>System.UInt64.</returns>
		public static ulong? GetUInt64OrNull(this MySqlDataReader dataReader, string columnName)
		{
			var ordinal = dataReader.GetOrdinal(columnName);
			if (dataReader.IsDBNull(ordinal))
				return null;
			else
				return dataReader.GetUInt64(ordinal);
		}

#endif
	}
}
