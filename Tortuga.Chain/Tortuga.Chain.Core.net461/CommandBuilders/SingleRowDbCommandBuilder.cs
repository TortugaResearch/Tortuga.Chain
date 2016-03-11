using System;
using System.Collections.Generic;
using System.Data.Common;
using Tortuga.Chain.Core;
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
        public ILink<bool> AsBoolean() { return new BooleanMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Boolean.
        /// </summary>
        public ILink<bool?> AsBooleanOrNull() { return new BooleanOrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a byte array.
        /// </summary>
        public ILink<byte[]> AsByteArray() { return new ByteArrayMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a DateTime.
        /// </summary>
        public ILink<DateTime> AsDateTime() { return new DateTimeMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a DateTimeOffset.
        /// </summary>
        public ILink<DateTimeOffset> AsDateTimeOffset() { return new DateTimeOffsetMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable DateTimeOffset.
        /// </summary>
        public ILink<DateTimeOffset?> AsDateTimeOffsetOrNull() { return new DateTimeOffsetOrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable DateTime.
        /// </summary>
        public ILink<DateTime?> AsDateTimeOrNull() { return new DateTimeOrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Decimal.
        /// </summary>
        public ILink<decimal> AsDecimal() { return new DecimalMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Decimal.
        /// </summary>
        public ILink<decimal?> AsDecimalOrNull() { return new DecimalOrNullMaterializer<TCommand, TParameter>(this); }
        /// <summary>
        /// Indicates the results should be materialized as a Double.
        /// </summary>
        public ILink<double> AsDouble() { return new DoubleMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Double.
        /// </summary>
        public ILink<double?> AsDoubleOrNull() { return new DoubleOrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Guid.
        /// </summary>
        public ILink<Guid> AsGuid() { return new GuidMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Guid.
        /// </summary>
        public ILink<Guid?> AsGuidOrNull() { return new GuidOrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Int16.
        /// </summary>
        public ILink<short> AsInt16() { return new Int16Materializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Int16.
        /// </summary>
        public ILink<short?> AsInt16OrNull() { return new Int16OrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Int32.
        /// </summary>
        public ILink<int> AsInt32() { return new Int32Materializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Int32.
        /// </summary>
        public ILink<int?> AsInt32OrNull() { return new Int32OrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Int64.
        /// </summary>
        public ILink<long> AsInt64() { return new Int64Materializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Int64.
        /// </summary>
        public ILink<long?> AsInt64OrNull() { return new Int64OrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Materializes the result as an instance of the indicated type
        /// </summary>
        /// <typeparam name="TObject">The type of the object returned.</typeparam>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        public ILink<TObject> AsObject<TObject>(RowOptions rowOptions = RowOptions.None)
            where TObject : class, new()
        {
            return new ObjectMaterializer<TCommand, TParameter, TObject>(this, rowOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a Row.
        /// </summary>
        public ILink<IReadOnlyDictionary<string, object>> AsRow(RowOptions rowOptions = RowOptions.None) { return new RowMaterializer<TCommand, TParameter>(this, rowOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a Single.
        /// </summary>
        public ILink<float> AsSingle() { return new SingleMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Single.
        /// </summary>
        public ILink<float?> AsSingleOrNull() { return new SingleOrNullMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable string.
        /// </summary>
        /// <returns></returns>
        public ILink<string> AsString() { return new StringMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a TimeSpan.
        /// </summary>
        public ILink<TimeSpan> AsTimeSpan() { return new TimeSpanMaterializer<TCommand, TParameter>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable TimeSpan.
        /// </summary>
        public ILink<TimeSpan?> AsTimeSpanOrNull() { return new TimeSpanOrNullMaterializer<TCommand, TParameter>(this); }
    }
}
