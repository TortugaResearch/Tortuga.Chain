using System;
using System.Collections.Generic;
using System.Data.Common;
using Tortuga.Chain.Materializers;

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
        protected SingleRowDbCommandBuilder(DataSources.DataSource<TCommandType, TParameterType> dataSource)
            : base(dataSource)
        {

        }

        /// <summary>
        /// Indicates the results should be materialized as a Boolean.
        /// </summary>
        public IMaterializer<bool> AsBoolean() { return new BooleanMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Boolean.
        /// </summary>
        public IMaterializer<bool?> AsBooleanOrNull() { return new BooleanOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a byte array.
        /// </summary>
        public IMaterializer<byte[]> AsByteArray() { return new ByteArrayMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a DateTime.
        /// </summary>
        public IMaterializer<DateTime> AsDateTime() { return new DateTimeMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a DateTimeOffset.
        /// </summary>
        public IMaterializer<DateTimeOffset> AsDateTimeOffset() { return new DateTimeOffsetMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable DateTimeOffset.
        /// </summary>
        public IMaterializer<DateTimeOffset?> AsDateTimeOffsetOrNull() { return new DateTimeOffsetOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable DateTime.
        /// </summary>
        public IMaterializer<DateTime?> AsDateTimeOrNull() { return new DateTimeOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Decimal.
        /// </summary>
        public IMaterializer<decimal> AsDecimal() { return new DecimalMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Decimal.
        /// </summary>
        public IMaterializer<decimal?> AsDecimalOrNull() { return new DecimalOrNullMaterializer<TCommandType, TParameterType>(this); }
        /// <summary>
        /// Indicates the results should be materialized as a Double.
        /// </summary>
        public IMaterializer<double> AsDouble() { return new DoubleMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Double.
        /// </summary>
        public IMaterializer<double?> AsDoubleOrNull() { return new DoubleOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Guid.
        /// </summary>
        public IMaterializer<Guid> AsGuid() { return new GuidMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Guid.
        /// </summary>
        public IMaterializer<Guid?> AsGuidOrNull() { return new GuidOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Int16.
        /// </summary>
        public IMaterializer<short> AsInt16() { return new Int16Materializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Int16.
        /// </summary>
        public IMaterializer<short?> AsInt16OrNull() { return new Int16OrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Int32.
        /// </summary>
        public IMaterializer<int> AsInt32() { return new Int32Materializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Int32.
        /// </summary>
        public IMaterializer<int?> AsInt32OrNull() { return new Int32OrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a Int64.
        /// </summary>
        public IMaterializer<long> AsInt64() { return new Int64Materializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Int64.
        /// </summary>
        public IMaterializer<long?> AsInt64OrNull() { return new Int64OrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Materializes the result as an instance of the indicated type
        /// </summary>
        /// <typeparam name="TObject">The type of the object returned.</typeparam>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        public IMaterializer<TObject> AsObject<TObject>(RowOptions rowOptions = RowOptions.None)
            where TObject : class, new()
        {
            return new ObjectMaterializer<TCommandType, TParameterType, TObject>(this, rowOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a Row.
        /// </summary>
        public IMaterializer<IReadOnlyDictionary<string, object>> AsRow(RowOptions rowOptions = RowOptions.None) { return new RowMaterializer<TCommandType, TParameterType>(this, rowOptions); }

        /// <summary>
        /// Indicates the results should be materialized as a Single.
        /// </summary>
        public IMaterializer<float> AsSingle() { return new SingleMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Single.
        /// </summary>
        public IMaterializer<float?> AsSingleOrNull() { return new SingleOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable string.
        /// </summary>
        /// <returns></returns>
        public IMaterializer<string> AsString() { return new StringMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a TimeSpan.
        /// </summary>
        public IMaterializer<TimeSpan> AsTimeSpan() { return new TimeSpanMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable TimeSpan.
        /// </summary>
        public IMaterializer<TimeSpan?> AsTimeSpanOrNull() { return new TimeSpanOrNullMaterializer<TCommandType, TParameterType>(this); }
    }
}
