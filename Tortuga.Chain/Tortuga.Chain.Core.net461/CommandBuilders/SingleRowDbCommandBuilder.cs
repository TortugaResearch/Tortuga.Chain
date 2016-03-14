using System;
using System.Collections.Generic;
using System.Data.Common;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This is the base class for command builders that can potentially return one row.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    public abstract class SingleRowDbCommandBuilder<TCommand, TParameter> : DbCommandBuilder<TCommand, TParameter>, ISingleRowDbCommandBuilder
        where TCommand : DbCommand
        where TParameter : DbParameter
    {
        /// <summary>
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected SingleRowDbCommandBuilder(DataSource<TCommand, TParameter> dataSource)
            : base(dataSource)
        {

        }

        /// <summary>
        /// Indicates the results should be materialized as a Boolean.
        /// </summary>
        public ILink<bool> ToBoolean() { return new BooleanMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Boolean.
        /// </summary>
        public ILink<bool?> ToBooleanOrNull() { return new BooleanOrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a byte.
        /// </summary>
        public ILink<byte> ToByte()
        {
            return new ByteMaterializer<TCommand, TParameter>(this);
        }

        /// <summary>
        /// Indicates the results should be materialized as a byte array.
        /// </summary>
        public ILink<byte[]> ToByteArray() { return new ByteArrayMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable byte.
        /// </summary>
        public ILink<byte?> ToByteOrNull()
        {
            return new ByteOrNullMaterializer<TCommand, TParameter>(this);
        }

        /// <summary>
        /// Indicates the results should be materialized as a DateTime.
        /// </summary>
        public ILink<DateTime> ToDateTime() { return new DateTimeMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a DateTimeOffset.
        /// </summary>
        public ILink<DateTimeOffset> ToDateTimeOffset() { return new DateTimeOffsetMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable DateTimeOffset.
        /// </summary>
        public ILink<DateTimeOffset?> ToDateTimeOffsetOrNull() { return new DateTimeOffsetOrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable DateTime.
        /// </summary>
        public ILink<DateTime?> ToDateTimeOrNull() { return new DateTimeOrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Decimal.
        /// </summary>
        public ILink<decimal> ToDecimal() { return new DecimalMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Decimal.
        /// </summary>
        public ILink<decimal?> ToDecimalOrNull() { return new DecimalOrNullMaterializer<TCommand, TParameter>(this); }
        /// <summary>
        /// Indicates the results should be materialized as a Double.
        /// </summary>
        public ILink<double> ToDouble() { return new DoubleMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Double.
        /// </summary>
        public ILink<double?> ToDoubleOrNull() { return new DoubleOrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Guid.
        /// </summary>
        public ILink<Guid> ToGuid() { return new GuidMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Guid.
        /// </summary>
        public ILink<Guid?> ToGuidOrNull() { return new GuidOrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Int16.
        /// </summary>
        public ILink<short> ToInt16() { return new Int16Materializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Int16.
        /// </summary>
        public ILink<short?> ToInt16OrNull() { return new Int16OrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Int32.
        /// </summary>
        public ILink<int> ToInt32() { return new Int32Materializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Int32.
        /// </summary>
        public ILink<int?> ToInt32OrNull() { return new Int32OrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Int64.
        /// </summary>
        public ILink<long> ToInt64() { return new Int64Materializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Int64.
        /// </summary>
        public ILink<long?> ToInt64OrNull() { return new Int64OrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Materializes the result as an instance of the indicated type
        /// </summary>
        /// <typeparam name="TObject">The type of the object returned.</typeparam>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        public ILink<TObject> ToObject<TObject>(RowOptions rowOptions = RowOptions.None)
            where TObject : class, new()
        {
            return new ObjectMaterializer<TCommand, TParameter, TObject>(this, rowOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a Row.
        /// </summary>
        public ILink<IReadOnlyDictionary<string, object>> ToRow(RowOptions rowOptions = RowOptions.None) { return new RowMaterializer<TCommand, TParameter>(this, rowOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a Single.
        /// </summary>
        public ILink<float> ToSingle() { return new SingleMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Single.
        /// </summary>
        public ILink<float?> ToSingleOrNull() { return new SingleOrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable string.
        /// </summary>
        /// <returns></returns>
        public new ILink<string> ToString() { return new StringMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a TimeSpan.
        /// </summary>
        public ILink<TimeSpan> ToTimeSpan() { return new TimeSpanMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable TimeSpan.
        /// </summary>
        public ILink<TimeSpan?> ToTimeSpanOrNull() { return new TimeSpanOrNullMaterializer<TCommand, TParameter>(this); }
    }
}
