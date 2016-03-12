using System;
using System.Collections.Generic;
using Tortuga.Chain.Core;
namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This allows the use of scalar and single row materializers against a command builder.
    /// </summary>
    public interface ISingleRowDbCommandBuilder : IDbCommandBuilder
    {
        /// <summary>
        /// Indicates the results should be materialized as a Boolean.
        /// </summary>
        ILink<bool> ToBoolean();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Boolean.
        /// </summary>
        ILink<bool?> ToBooleanOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a byte array.
        /// </summary>
        ILink<byte[]> ToByteArray();
        /// <summary>
        /// Indicates the results should be materialized as a DateTime.
        /// </summary>
        ILink<DateTime> ToDateTime();
        /// <summary>
        /// Indicates the results should be materialized as a DateTimeOffset.
        /// </summary>
        ILink<DateTimeOffset> ToDateTimeOffset();
        /// <summary>
        /// Indicates the results should be materialized as a nullable DateTimeOffset.
        /// </summary>
        ILink<DateTimeOffset?> ToDateTimeOffsetOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a nullable DateTime.
        /// </summary>
        ILink<DateTime?> ToDateTimeOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Decimal.
        /// </summary>
        ILink<decimal> ToDecimal();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Decimal.
        /// </summary>
        ILink<decimal?> ToDecimalOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Double.
        /// </summary>
        ILink<double> ToDouble();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Double.
        /// </summary>
        ILink<double?> ToDoubleOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Guid.
        /// </summary>
        ILink<Guid> ToGuid();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Guid.
        /// </summary>
        ILink<Guid?> ToGuidOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Int16.
        /// </summary>
        ILink<short> ToInt16();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Int16.
        /// </summary>
        ILink<short?> ToInt16OrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Int32.
        /// </summary>
        ILink<int> ToInt32();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Int32.
        /// </summary>
        ILink<int?> ToInt32OrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Int64.
        /// </summary>
        ILink<long> ToInt64();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Int64.
        /// </summary>
        ILink<long?> ToInt64OrNull();
        /// <summary>
        /// Materializes the result as an instance of the indicated type
        /// </summary>
        /// <typeparam name="TObject">The type of the object returned.</typeparam>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        ILink<TObject> ToObject<TObject>(RowOptions rowOptions = RowOptions.None)
            where TObject : class, new();
        /// <summary>
        /// Indicates the results should be materialized as a Row.
        /// </summary>
        ILink<IReadOnlyDictionary<string, object>> ToRow(RowOptions rowOptions = RowOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a Single.
        /// </summary>
        ILink<float> ToSingle();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Single.
        /// </summary>
        ILink<float?> ToSingleOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a nullable string.
        /// </summary>
        /// <returns></returns>
        ILink<string> ToString();
        /// <summary>
        /// Indicates the results should be materialized as a TimeSpan.
        /// </summary>
        ILink<TimeSpan> ToTimeSpan();
        /// <summary>
        /// Indicates the results should be materialized as a nullable TimeSpan.
        /// </summary>
        ILink<TimeSpan?> ToTimeSpanOrNull();
    }
}
