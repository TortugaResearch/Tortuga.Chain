using System;
using System.Data.Common;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;
using System.Linq;

#if !WINDOWS_UWP
using System.Data;
#endif

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
            where TObject : class
        {
            if (rowOptions.HasFlag(RowOptions.InferConstructor))
            {
                var constructors = typeof(TObject).GetConstructors().Select(c => c.GetParameters()).Where(c => c.Length > 0).ToList();
                if (constructors.Count == 0)
                    throw new MappingException($"Type {typeof(TObject).Name} has does not have any non-default constructors.");
                if (constructors.Count > 1)
                    throw new MappingException($"Type {typeof(TObject).Name} has more than one non-default constructor. Please specify which one to use.");
                return new InitializedObjectMaterializer<TCommand, TParameter, TObject>(this, constructors[0].Select(p => p.ParameterType).ToArray(), rowOptions);

            }
            else
            {
                var constructor = typeof(TObject).GetConstructor(new Type[] { });
                if (constructor == null)
                    throw new MappingException($"Type {typeof(TObject).Name} has does not have a default constructor.");

                var rawType = typeof(ObjectMaterializer<,,>);
                var typeArgs = new Type[] { typeof(TCommand), typeof(TParameter), typeof(TObject) };
                var constructedType = rawType.MakeGenericType(typeArgs);
                var result = Activator.CreateInstance(constructedType, new object[] { this, rowOptions });
                return (ILink<TObject>)result;
            }
        }



        /// <summary>
        /// Materializes the result as an instance of the indicated type
        /// </summary>
        /// <typeparam name="TObject">The type of the object returned.</typeparam>
        /// <param name="rowOptions">The row options.</param>
        /// <param name="constructorSignature">The constructor signature to use.</param>
        /// <returns></returns>
        /// <remarks>This version will not set properties.</remarks>
        public ILink<TObject> ToObject<TObject>(Type[] constructorSignature, RowOptions rowOptions = RowOptions.None)
            where TObject : class
        {
            return new InitializedObjectMaterializer<TCommand, TParameter, TObject>(this, constructorSignature, rowOptions);
        }

        /// <summary>
        /// Materializes the result as a dynamic object
        /// </summary>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        public ILink<dynamic> ToDynamicObject(RowOptions rowOptions = RowOptions.None)
        {
            return new DynamicObjectMaterializer<TCommand, TParameter>(this, rowOptions);
        }

#if !WINDOWS_UWP
        /// <summary>
        /// Indicates the results should be materialized as a Row.
        /// </summary>
        public ILink<DataRow> ToDataRow(RowOptions rowOptions = RowOptions.None) { return new DataRowMaterializer<TCommand, TParameter>(this, rowOptions); }
#endif

        /// <summary>
        /// Indicates the results should be materialized as a Row.
        /// </summary>
        public ILink<Row> ToRow(RowOptions rowOptions = RowOptions.None) { return new RowMaterializer<TCommand, TParameter>(this, rowOptions); }

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

        /// <summary>
        /// Indicates the results should be materialized as a nullable TimeSpan.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<TimeSpan?> ToTimeSpanOrNull(string columnName) { return new TimeSpanOrNullMaterializer<TCommand, TParameter>(this, columnName); }




        /// <summary>
        /// Indicates the results should be materialized as a Boolean.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<bool> ToBoolean(string columnName) { return new BooleanMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Boolean.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<bool?> ToBooleanOrNull(string columnName) { return new BooleanOrNullMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a byte.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<byte> ToByte(string columnName)
        {
            return new ByteMaterializer<TCommand, TParameter>(this, columnName);
        }

        /// <summary>
        /// Indicates the results should be materialized as a byte array.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<byte[]> ToByteArray(string columnName) { return new ByteArrayMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable byte.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<byte?> ToByteOrNull(string columnName)
        {
            return new ByteOrNullMaterializer<TCommand, TParameter>(this, columnName);
        }

        /// <summary>
        /// Indicates the results should be materialized as a DateTime.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<DateTime> ToDateTime(string columnName) { return new DateTimeMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a DateTimeOffset.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<DateTimeOffset> ToDateTimeOffset(string columnName) { return new DateTimeOffsetMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable DateTimeOffset.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<DateTimeOffset?> ToDateTimeOffsetOrNull(string columnName) { return new DateTimeOffsetOrNullMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable DateTime.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<DateTime?> ToDateTimeOrNull(string columnName) { return new DateTimeOrNullMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a Decimal.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<decimal> ToDecimal(string columnName) { return new DecimalMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Decimal.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<decimal?> ToDecimalOrNull(string columnName) { return new DecimalOrNullMaterializer<TCommand, TParameter>(this, columnName); }
        /// <summary>
        /// Indicates the results should be materialized as a Double.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<double> ToDouble(string columnName) { return new DoubleMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Double.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<double?> ToDoubleOrNull(string columnName) { return new DoubleOrNullMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a Guid.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<Guid> ToGuid(string columnName) { return new GuidMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Guid.
        /// </summary>
        public ILink<Guid?> ToGuidOrNull(string columnName) { return new GuidOrNullMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a Int16.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<short> ToInt16(string columnName) { return new Int16Materializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Int16.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<short?> ToInt16OrNull(string columnName) { return new Int16OrNullMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a Int32.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<int> ToInt32(string columnName) { return new Int32Materializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Int32.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<int?> ToInt32OrNull(string columnName) { return new Int32OrNullMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a Int64.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<long> ToInt64(string columnName) { return new Int64Materializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Int64.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<long?> ToInt64OrNull(string columnName) { return new Int64OrNullMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a Single.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<float> ToSingle(string columnName) { return new SingleMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable Single.
        /// </summary>
        public ILink<float?> ToSingleOrNull(string columnName) { return new SingleOrNullMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a nullable string.
        /// </summary>
        /// <returns></returns>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<string> ToString(string columnName) { return new StringMaterializer<TCommand, TParameter>(this, columnName); }

        /// <summary>
        /// Indicates the results should be materialized as a TimeSpan.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        public ILink<TimeSpan> ToTimeSpan(string columnName) { return new TimeSpanMaterializer<TCommand, TParameter>(this, columnName); }
    }
}
