using System.Data.Common;
using Tortuga.Chain.Formatters;

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
        public BooleanResult<TCommandType, TParameterType> AsBoolean() { return new BooleanResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Boolean.
        /// </summary>
        public BooleanOrNullResult<TCommandType, TParameterType> AsBooleanOrNull() { return new BooleanOrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a byte array.
        /// </summary>
        public ByteArrayResult<TCommandType, TParameterType> AsByteArray() { return new ByteArrayResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a DateTime.
        /// </summary>
        public DateTimeResult<TCommandType, TParameterType> AsDateTime() { return new DateTimeResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a DateTimeOffset.
        /// </summary>
        public DateTimeOffsetResult<TCommandType, TParameterType> AsDateTimeOffset() { return new DateTimeOffsetResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable DateTimeOffset.
        /// </summary>
        public DateTimeOffsetOrNullResult<TCommandType, TParameterType> AsDateTimeOffsetOrNull() { return new DateTimeOffsetOrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable DateTime.
        /// </summary>
        public DateTimeOrNullResult<TCommandType, TParameterType> AsDateTimeOrNull() { return new DateTimeOrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Decimal.
        /// </summary>
        public DecimalResult<TCommandType, TParameterType> AsDecimal() { return new DecimalResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Decimal.
        /// </summary>
        public DecimalOrNullResult<TCommandType, TParameterType> AsDecimalOrNull() { return new DecimalOrNullResult<TCommandType, TParameterType>(this); }
        /// <summary>
        /// Indicates the results should be formatted as a Double.
        /// </summary>
        public DoubleResult<TCommandType, TParameterType> AsDouble() { return new DoubleResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Double.
        /// </summary>
        public DoubleOrNullResult<TCommandType, TParameterType> AsDoubleOrNull() { return new DoubleOrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Guid.
        /// </summary>
        public GuidResult<TCommandType, TParameterType> AsGuid() { return new GuidResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Guid.
        /// </summary>
        public GuidOrNullResult<TCommandType, TParameterType> AsGuidOrNull() { return new GuidOrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Int16.
        /// </summary>
        public Int16Result<TCommandType, TParameterType> AsInt16() { return new Int16Result<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Int16.
        /// </summary>
        public Int16OrNullResult<TCommandType, TParameterType> AsInt16OrNull() { return new Int16OrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Int32.
        /// </summary>
        public Int32Result<TCommandType, TParameterType> AsInt32() { return new Int32Result<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Int32.
        /// </summary>
        public Int32OrNullResult<TCommandType, TParameterType> AsInt32OrNull() { return new Int32OrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Int64.
        /// </summary>
        public Int64Result<TCommandType, TParameterType> AsInt64() { return new Int64Result<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Int64.
        /// </summary>
        public Int64OrNullResult<TCommandType, TParameterType> AsInt64OrNull() { return new Int64OrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Formats the result as an instance of the indicated type
        /// </summary>
        /// <typeparam name="TObject">The type of the object returned.</typeparam>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        public ObjectResult<TCommandType, TParameterType, TObject> AsObject<TObject>(RowOptions rowOptions = RowOptions.None)
            where TObject : class, new()
        {
            return new ObjectResult<TCommandType, TParameterType, TObject>(this, rowOptions);
        }

        /// <summary>
        /// Indicates the results should be formatted as a Row.
        /// </summary>
        public RowResult<TCommandType, TParameterType> AsRow(RowOptions rowOptions = RowOptions.None) { return new RowResult<TCommandType, TParameterType>(this, rowOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a Single.
        /// </summary>
        public SingleResult<TCommandType, TParameterType> AsSingle() { return new SingleResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Single.
        /// </summary>
        public SingleOrNullResult<TCommandType, TParameterType> AsSingleOrNull() { return new SingleOrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable string.
        /// </summary>
        /// <returns></returns>
        public StringResult<TCommandType, TParameterType> AsString() { return new StringResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a TimeSpan.
        /// </summary>
        public TimeSpanResult<TCommandType, TParameterType> AsTimeSpan() { return new TimeSpanResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable TimeSpan.
        /// </summary>
        public TimeSpanOrNullResult<TCommandType, TParameterType> AsTimeSpanOrNull() { return new TimeSpanOrNullResult<TCommandType, TParameterType>(this); }
    }
}
