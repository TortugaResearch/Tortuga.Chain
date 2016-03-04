using System;
using System.Collections.Generic;
using Tortuga.Chain.Materializers;

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
        IMaterializer<bool> AsBoolean();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Boolean.
        /// </summary>
        IMaterializer<bool?> AsBooleanOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a byte array.
        /// </summary>
        IMaterializer<byte[]> AsByteArray();
        /// <summary>
        /// Indicates the results should be materialized as a DateTime.
        /// </summary>
        IMaterializer<DateTime> AsDateTime();
        /// <summary>
        /// Indicates the results should be materialized as a DateTimeOffset.
        /// </summary>
        IMaterializer<DateTimeOffset> AsDateTimeOffset();
        /// <summary>
        /// Indicates the results should be materialized as a nullable DateTimeOffset.
        /// </summary>
        IMaterializer<DateTimeOffset?> AsDateTimeOffsetOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a nullable DateTime.
        /// </summary>
        IMaterializer<DateTime?> AsDateTimeOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Decimal.
        /// </summary>
        IMaterializer<decimal> AsDecimal();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Decimal.
        /// </summary>
        IMaterializer<decimal?> AsDecimalOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Double.
        /// </summary>
        IMaterializer<double> AsDouble();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Double.
        /// </summary>
        IMaterializer<double?> AsDoubleOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Guid.
        /// </summary>
        IMaterializer<Guid> AsGuid();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Guid.
        /// </summary>
        IMaterializer<Guid?> AsGuidOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Int16.
        /// </summary>
        IMaterializer<short> AsInt16();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Int16.
        /// </summary>
        IMaterializer<short?> AsInt16OrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Int32.
        /// </summary>
        IMaterializer<int> AsInt32();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Int32.
        /// </summary>
        IMaterializer<int?> AsInt32OrNull();
        /// <summary>
        /// Indicates the results should be materialized as a Int64.
        /// </summary>
        IMaterializer<long> AsInt64();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Int64.
        /// </summary>
        IMaterializer<long?> AsInt64OrNull();
        /// <summary>
        /// Materializes the result as an instance of the indicated type
        /// </summary>
        /// <typeparam name="TObject">The type of the object returned.</typeparam>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        IMaterializer<TObject> AsObject<TObject>(RowOptions rowOptions = RowOptions.None)
            where TObject : class, new();
        /// <summary>
        /// Indicates the results should be materialized as a Row.
        /// </summary>
        IMaterializer<IReadOnlyDictionary<string, object>> AsRow(RowOptions rowOptions = RowOptions.None);
        /// <summary>
        /// Indicates the results should be materialized as a Single.
        /// </summary>
        IMaterializer<float> AsSingle();
        /// <summary>
        /// Indicates the results should be materialized as a nullable Single.
        /// </summary>
        IMaterializer<float?> AsSingleOrNull();
        /// <summary>
        /// Indicates the results should be materialized as a nullable string.
        /// </summary>
        /// <returns></returns>
        IMaterializer<string> AsString();
        /// <summary>
        /// Indicates the results should be materialized as a TimeSpan.
        /// </summary>
        IMaterializer<TimeSpan> AsTimeSpan();
        /// <summary>
        /// Indicates the results should be materialized as a nullable TimeSpan.
        /// </summary>
        IMaterializer<TimeSpan?> AsTimeSpanOrNull();
    }
}
