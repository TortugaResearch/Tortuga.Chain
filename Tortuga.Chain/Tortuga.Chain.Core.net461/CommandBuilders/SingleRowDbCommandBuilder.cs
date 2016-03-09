using System;
using System.Collections.Generic;
using System.Data.Common;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Core;
namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This is the base class for command builders that can potentially return one row.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the t command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public abstract class SingleRowDbCommandBuilder<TCommandType, TParameterType> : DbCommandBuilder<TCommandType, TParameterType>, ISingleRowDbCommandBuilder
        where TCommandType : DbCommand
        where TParameterType : DbParameter
    {
        /// <summary>
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected SingleRowDbCommandBuilder(DataSource<TCommandType, TParameterType> dataSource)
            : base(dataSource)
        {

        }

        /// <summary>
        /// Indicates the results should be materialized as a Boolean.
        /// </summary>
        public ILink<bool> AsBoolean() { return new BooleanMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Boolean.
        /// </summary>
        public ILink<bool?> AsBooleanOrNull() { return new BooleanOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a byte array.
        /// </summary>
        public ILink<byte[]> AsByteArray() { return new ByteArrayMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a DateTime.
        /// </summary>
        public ILink<DateTime> AsDateTime() { return new DateTimeMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a DateTimeOffset.
        /// </summary>
        public ILink<DateTimeOffset> AsDateTimeOffset() { return new DateTimeOffsetMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable DateTimeOffset.
        /// </summary>
        public ILink<DateTimeOffset?> AsDateTimeOffsetOrNull() { return new DateTimeOffsetOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable DateTime.
        /// </summary>
        public ILink<DateTime?> AsDateTimeOrNull() { return new DateTimeOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Decimal.
        /// </summary>
        public ILink<decimal> AsDecimal() { return new DecimalMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Decimal.
        /// </summary>
        public ILink<decimal?> AsDecimalOrNull() { return new DecimalOrNullMaterializer<TCommandType, TParameterType>(this); }
        /// <summary>
        /// Indicates the results should be materialized as a Double.
        /// </summary>
        public ILink<double> AsDouble() { return new DoubleMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Double.
        /// </summary>
        public ILink<double?> AsDoubleOrNull() { return new DoubleOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Guid.
        /// </summary>
        public ILink<Guid> AsGuid() { return new GuidMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Guid.
        /// </summary>
        public ILink<Guid?> AsGuidOrNull() { return new GuidOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Int16.
        /// </summary>
        public ILink<short> AsInt16() { return new Int16Materializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Int16.
        /// </summary>
        public ILink<short?> AsInt16OrNull() { return new Int16OrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Int32.
        /// </summary>
        public ILink<int> AsInt32() { return new Int32Materializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Int32.
        /// </summary>
        public ILink<int?> AsInt32OrNull() { return new Int32OrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Int64.
        /// </summary>
        public ILink<long> AsInt64() { return new Int64Materializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Int64.
        /// </summary>
        public ILink<long?> AsInt64OrNull() { return new Int64OrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Materializes the result as an instance of the indicated type
        /// </summary>
        /// <typeparam name="TObject">The type of the object returned.</typeparam>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        public ILink<TObject> AsObject<TObject>(RowOptions rowOptions = RowOptions.None)
            where TObject : class, new()
        {
            return new ObjectMaterializer<TCommandType, TParameterType, TObject>(this, rowOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a Row.
        /// </summary>
        public ILink<IReadOnlyDictionary<string, object>> AsRow(RowOptions rowOptions = RowOptions.None) { return new RowMaterializer<TCommandType, TParameterType>(this, rowOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a Single.
        /// </summary>
        public ILink<float> AsSingle() { return new SingleMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Single.
        /// </summary>
        public ILink<float?> AsSingleOrNull() { return new SingleOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable string.
        /// </summary>
        /// <returns></returns>
        public ILink<string> AsString() { return new StringMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a TimeSpan.
        /// </summary>
        public ILink<TimeSpan> AsTimeSpan() { return new TimeSpanMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable TimeSpan.
        /// </summary>
        public ILink<TimeSpan?> AsTimeSpanOrNull() { return new TimeSpanOrNullMaterializer<TCommandType, TParameterType>(this); }
    }
}
