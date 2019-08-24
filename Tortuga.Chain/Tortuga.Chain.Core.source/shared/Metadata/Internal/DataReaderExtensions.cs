﻿using System.Data.Common;

namespace Tortuga.Chain.Metadata.Internal
{
    /// <summary>
    /// DataReaderExtensions is used for generating metadata.
    /// </summary>
    public static class DataReaderExtensions
    {
        /// <summary>
        /// Gets the boolean.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetBoolean(this DbDataReader dataReader, string columnName)
        {
            return dataReader.GetBoolean(dataReader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets the boolean or null.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool? GetBooleanOrNull(this DbDataReader dataReader, string columnName)
        {
            var ordinal = dataReader.GetOrdinal(columnName);
            if (dataReader.IsDBNull(ordinal))
                return null;
            else
                return dataReader.GetBoolean(ordinal);
        }

        /// <summary>
        /// Gets the int16.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.Int16.</returns>
        public static short GetInt16(this DbDataReader dataReader, string columnName)
        {
            return dataReader.GetInt16(dataReader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets the int16 or null.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.Nullable&lt;System.Int16&gt;.</returns>
        public static short? GetInt16OrNull(this DbDataReader dataReader, string columnName)
        {
            var ordinal = dataReader.GetOrdinal(columnName);
            if (dataReader.IsDBNull(ordinal))
                return null;
            else
                return dataReader.GetInt16(ordinal);
        }

        /// <summary>
        /// Gets the int32.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.Int32.</returns>
        public static int GetInt32(this DbDataReader dataReader, string columnName)
        {
            return dataReader.GetInt32(dataReader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets the int32 or null.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
        public static int? GetInt32OrNull(this DbDataReader dataReader, string columnName)
        {
            var ordinal = dataReader.GetOrdinal(columnName);
            if (dataReader.IsDBNull(ordinal))
                return null;
            else
                return dataReader.GetInt32(ordinal);
        }

        /// <summary>
        /// Gets the int64.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.Int64.</returns>
        public static long GetInt64(this DbDataReader dataReader, string columnName)
        {
            return dataReader.GetInt64(dataReader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets the int64 or null.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.Nullable&lt;System.Int64&gt;.</returns>
        public static long? GetInt64OrNull(this DbDataReader dataReader, string columnName)
        {
            var ordinal = dataReader.GetOrdinal(columnName);
            if (dataReader.IsDBNull(ordinal))
                return null;
            else
                return dataReader.GetInt64(ordinal);
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        public static string GetString(this DbDataReader dataReader, string columnName)
        {
            return dataReader.GetString(dataReader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Gets the string or null.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        public static string GetStringOrNull(this DbDataReader dataReader, string columnName)
        {
            var ordinal = dataReader.GetOrdinal(columnName);
            if (dataReader.IsDBNull(ordinal))
                return null;
            else
                return dataReader.GetString(ordinal);
        }

        /// <summary>
        /// Gets the uint32.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.UInt32.</returns>
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
        public static uint? GetUInt32OrNull(this DbDataReader dataReader, string columnName)
        {
            var ordinal = dataReader.GetOrdinal(columnName);
            if (dataReader.IsDBNull(ordinal))
                return null;
            else
                return (uint)dataReader.GetValue(ordinal);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.Object.</returns>
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
        public static T GetValue<T>(this DbDataReader dataReader, string columnName)
        {
            return (T)dataReader.GetValue(dataReader.GetOrdinal(columnName));
        }

        /// <summary>
        /// Determines whether [is database null] [the specified column name].
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns><c>true</c> if [is database null] [the specified column name]; otherwise, <c>false</c>.</returns>
        public static bool IsDBNull(this DbDataReader dataReader, string columnName)
        {
            return dataReader.IsDBNull(dataReader.GetOrdinal(columnName));
        }
    }
}
