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
        ILink<bool> AsBoolean();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Boolean.
        /// </summary>
        ILink<bool?> AsBooleanOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a byte array.
        /// </summary>
        ILink<byte[]> AsByteArray();
        /// <summary>
        /// Indicates the results should be materialized as a DateTime.
        /// </summary>
        ILink<DateTime> AsDateTime();
        /// <summary>
        /// Indicates the results should be materialized as a DateTimeOffset.
        /// </summary>
        ILink<DateTimeOffset> AsDateTimeOffset();
        /// <summary>
        /// Indicates the results should be materialized as a nullable DateTimeOffset.
        /// </summary>
        ILink<DateTimeOffset?> AsDateTimeOffsetOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a nullable DateTime.
        /// </summary>
        ILink<DateTime?> AsDateTimeOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Decimal.
        /// </summary>
        ILink<decimal> AsDecimal();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Decimal.
        /// </summary>
        ILink<decimal?> AsDecimalOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Double.
        /// </summary>
        ILink<double> AsDouble();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Double.
        /// </summary>
        ILink<double?> AsDoubleOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Guid.
        /// </summary>
        ILink<Guid> AsGuid();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Guid.
        /// </summary>
        ILink<Guid?> AsGuidOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Int16.
        /// </summary>
        ILink<short> AsInt16();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Int16.
        /// </summary>
        ILink<short?> AsInt16OrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Int32.
        /// </summary>
        ILink<int> AsInt32();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Int32.
        /// </summary>
        ILink<int?> AsInt32OrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Int64.
        /// </summary>
        ILink<long> AsInt64();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Int64.
        /// </summary>
        ILink<long?> AsInt64OrNull();
        /// <summary>
        /// Materializes the result as an instance of the indicated type
        /// </summary>
        /// <typeparam name="TObject">The type of the object returned.</typeparam>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        ILink<TObject> AsObject<TObject>(RowOptions rowOptions = RowOptions.None)
            where TObject : class, new();
        /// <summary>
        /// Indicates the results should be materialized as a Row.
        /// </summary>
        ILink<IReadOnlyDictionary<string, object>> AsRow(RowOptions rowOptions = RowOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a Single.
        /// </summary>
        ILink<float> AsSingle();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Single.
        /// </summary>
        ILink<float?> AsSingleOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a nullable string.
        /// </summary>
        /// <returns></returns>
        ILink<string> AsString();
        /// <summary>
        /// Indicates the results should be materialized as a TimeSpan.
        /// </summary>
        ILink<TimeSpan> AsTimeSpan();
        /// <summary>
        /// Indicates the results should be materialized as a nullable TimeSpan.
        /// </summary>
        ILink<TimeSpan?> AsTimeSpanOrNull();
    }
}
