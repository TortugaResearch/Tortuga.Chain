using System.Data.Common;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.CommandBuilders
{
    /// <summary>
    /// This is the base class for command builders that can potentially return one row.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the t command type.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public abstract class SingleRowDbCommandBuilder<TCommandType, TParameterType> : DbCommandBuilder<TCommandType, TParameterType>
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
        /// Indicates the results should be formatted as a Boolean.
        /// </summary>
        public BooleanMaterializer<TCommandType, TParameterType> AsBoolean() { return new BooleanMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Boolean.
        /// </summary>
        public BooleanOrNullMaterializer<TCommandType, TParameterType> AsBooleanOrNull() { return new BooleanOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a byte array.
        /// </summary>
        public ByteArrayMaterializer<TCommandType, TParameterType> AsByteArray() { return new ByteArrayMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a DateTime.
        /// </summary>
        public DateTimeMaterializer<TCommandType, TParameterType> AsDateTime() { return new DateTimeMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a DateTimeOffset.
        /// </summary>
        public DateTimeOffsetMaterializer<TCommandType, TParameterType> AsDateTimeOffset() { return new DateTimeOffsetMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable DateTimeOffset.
        /// </summary>
        public DateTimeOffsetOrNullMaterializer<TCommandType, TParameterType> AsDateTimeOffsetOrNull() { return new DateTimeOffsetOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable DateTime.
        /// </summary>
        public DateTimeOrNullMaterializer<TCommandType, TParameterType> AsDateTimeOrNull() { return new DateTimeOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Decimal.
        /// </summary>
        public DecimalMaterializer<TCommandType, TParameterType> AsDecimal() { return new DecimalMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Decimal.
        /// </summary>
        public DecimalOrNullMaterializer<TCommandType, TParameterType> AsDecimalOrNull() { return new DecimalOrNullMaterializer<TCommandType, TParameterType>(this); }
        /// <summary>
        /// Indicates the results should be formatted as a Double.
        /// </summary>
        public DoubleMaterializer<TCommandType, TParameterType> AsDouble() { return new DoubleMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Double.
        /// </summary>
        public DoubleOrNullMaterializer<TCommandType, TParameterType> AsDoubleOrNull() { return new DoubleOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Guid.
        /// </summary>
        public GuidMaterializer<TCommandType, TParameterType> AsGuid() { return new GuidMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Guid.
        /// </summary>
        public GuidOrNullMaterializer<TCommandType, TParameterType> AsGuidOrNull() { return new GuidOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Int16.
        /// </summary>
        public Int16Materializer<TCommandType, TParameterType> AsInt16() { return new Int16Materializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Int16.
        /// </summary>
        public Int16OrNullMaterializer<TCommandType, TParameterType> AsInt16OrNull() { return new Int16OrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Int32.
        /// </summary>
        public Int32Materializer<TCommandType, TParameterType> AsInt32() { return new Int32Materializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Int32.
        /// </summary>
        public Int32OrNullMaterializer<TCommandType, TParameterType> AsInt32OrNull() { return new Int32OrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Int64.
        /// </summary>
        public Int64Materializer<TCommandType, TParameterType> AsInt64() { return new Int64Materializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Int64.
        /// </summary>
        public Int64OrNullMaterializer<TCommandType, TParameterType> AsInt64OrNull() { return new Int64OrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Formats the result as an instance of the indicated type
        /// </summary>
        /// <typeparam name="TObject">The type of the object returned.</typeparam>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        public ObjectMaterializer<TCommandType, TParameterType, TObject> AsObject<TObject>(RowOptions rowOptions = RowOptions.None)
            where TObject : class, new()
        {
            return new ObjectMaterializer<TCommandType, TParameterType, TObject>(this, rowOptions);
        }

        /// <summary>
        /// Indicates the results should be formatted as a Row.
        /// </summary>
        public RowMaterializer<TCommandType, TParameterType> AsRow(RowOptions rowOptions = RowOptions.None) { return new RowMaterializer<TCommandType, TParameterType>(this, rowOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a Single.
        /// </summary>
        public SingleMaterializer<TCommandType, TParameterType> AsSingle() { return new SingleMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Single.
        /// </summary>
        public SingleOrNullMaterializer<TCommandType, TParameterType> AsSingleOrNull() { return new SingleOrNullMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable string.
        /// </summary>
        /// <returns></returns>
        public StringMaterializer<TCommandType, TParameterType> AsString() { return new StringMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a TimeSpan.
        /// </summary>
        public TimeSpanMaterializer<TCommandType, TParameterType> AsTimeSpan() { return new TimeSpanMaterializer<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable TimeSpan.
        /// </summary>
        public TimeSpanOrNullMaterializer<TCommandType, TParameterType> AsTimeSpanOrNull() { return new TimeSpanOrNullMaterializer<TCommandType, TParameterType>(this); }
    }
}
