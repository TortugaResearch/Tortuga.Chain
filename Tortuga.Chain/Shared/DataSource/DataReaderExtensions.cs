using System.Data.Common;

#if MYSQL

using MySqlConnector;

#endif

namespace Tortuga.Chain.Metadata;

/// <summary>
/// DataReaderExtensions is used for generating metadata.
/// </summary>
internal static class DataReaderExtensions
{


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
	/// Gets the value or null.
	/// </summary>
	/// <param name="dataReader">The data reader.</param>
	/// <param name="columnName">Name of the column.</param>
	/// <returns>System.Object.</returns>
	[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
	public static object? GetValueOrNull(this DbDataReader dataReader, string columnName)
	{
		var ordinal = dataReader.GetOrdinal(columnName);
		if (dataReader.IsDBNull(ordinal))
			return null;
		else
			return dataReader.GetValue(ordinal);
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
