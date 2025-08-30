using System.Data.Common;
using System.Xml.Linq;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// This is the base class for command builders that return one value.
/// </summary>
/// <typeparam name="TCommand">The type of the t command type.</typeparam>
/// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
public abstract class ScalarDbCommandBuilder<TCommand, TParameter> : DbCommandBuilder<TCommand, TParameter>, IScalarDbCommandBuilder
	where TCommand : DbCommand
	where TParameter : DbParameter
{
	/// <summary>Initializes a new instance of the <see cref="Tortuga.Chain.CommandBuilders.ScalarDbCommandBuilder{TCommand, TParameter}"/> class.</summary>
	/// <param name="dataSource">The data source.</param>
	protected ScalarDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource)
		: base(dataSource)
	{
	}

	/// <summary>
	/// Indicates the results should be materialized as a Boolean.
	/// </summary>
	public ILink<bool> ToBoolean() => new BooleanMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a Boolean.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<bool> ToBoolean(string columnName) => new BooleanMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable Boolean.
	/// </summary>
	public ILink<bool?> ToBooleanOrNull() => new BooleanOrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a nullable Boolean.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<bool?> ToBooleanOrNull(string columnName) => new BooleanOrNullMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a byte.
	/// </summary>
	public ILink<byte> ToByte() => new ByteMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a byte.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<byte> ToByte(string columnName) => new ByteMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a byte array.
	/// </summary>
	public ILink<byte[]> ToByteArray() => new ByteArrayMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a byte array.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<byte[]> ToByteArray(string columnName) => new ByteArrayMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a byte array or null.
	/// </summary>
	public ILink<byte[]?> ToByteArrayOrNull() => new ByteArrayOrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a byte array or null.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<byte[]?> ToByteArrayOrNull(string columnName) => new ByteArrayOrNullMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable byte.
	/// </summary>
	public ILink<byte?> ToByteOrNull() => new ByteOrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a nullable byte.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<byte?> ToByteOrNull(string columnName) => new ByteOrNullMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a non-nullable char.
	/// </summary>
	/// <returns></returns>
	public ILink<char> ToChar() => new CharMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a non-nullable char.
	/// </summary>
	/// <returns></returns>
	public ILink<char> ToChar(string columnName) => new CharMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable char.
	/// </summary>
	/// <returns></returns>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<char?> ToCharOrNull(string columnName) => new CharOrNullMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable char.
	/// </summary>
	/// <returns></returns>
	public ILink<char?> ToCharOrNull() => new CharOrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a DateOnly.
	/// </summary>
	public ILink<DateOnly> ToDateOnly() => new DateOnlyMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a DateOnly.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<DateOnly> ToDateOnly(string columnName) => new DateOnlyMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable DateOnly.
	/// </summary>
	public ILink<DateOnly?> ToDateOnlyOrNull() => new DateOnlyOrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a nullable DateOnly.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<DateOnly?> ToDateOnlyOrNull(string columnName) => new DateOnlyOrNullMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a DateTime.
	/// </summary>
	public ILink<DateTime> ToDateTime() => new DateTimeMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a DateTime.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<DateTime> ToDateTime(string columnName) => new DateTimeMaterializer<TCommand, TParameter>(this, columnName);



	/// <summary>
	/// Indicates the results should be materialized as a DateTimeOffset.
	/// </summary>
	public ILink<DateTimeOffset> ToDateTimeOffset() => new DateTimeOffsetMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a DateTimeOffset.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<DateTimeOffset> ToDateTimeOffset(string columnName) => new DateTimeOffsetMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable DateTimeOffset.
	/// </summary>
	public ILink<DateTimeOffset?> ToDateTimeOffsetOrNull() => new DateTimeOffsetOrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a nullable DateTimeOffset.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<DateTimeOffset?> ToDateTimeOffsetOrNull(string columnName) => new DateTimeOffsetOrNullMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable DateTime.
	/// </summary>
	public ILink<DateTime?> ToDateTimeOrNull() => new DateTimeOrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a nullable DateTime.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<DateTime?> ToDateTimeOrNull(string columnName) => new DateTimeOrNullMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a Decimal.
	/// </summary>
	public ILink<decimal> ToDecimal() => new DecimalMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a Decimal.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<decimal> ToDecimal(string columnName) => new DecimalMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable Decimal.
	/// </summary>
	public ILink<decimal?> ToDecimalOrNull() => new DecimalOrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a nullable Decimal.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<decimal?> ToDecimalOrNull(string columnName) => new DecimalOrNullMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a Double.
	/// </summary>
	public ILink<double> ToDouble() => new DoubleMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a Double.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<double> ToDouble(string columnName) => new DoubleMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable Double.
	/// </summary>
	public ILink<double?> ToDoubleOrNull() => new DoubleOrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a nullable Double.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<double?> ToDoubleOrNull(string columnName) => new DoubleOrNullMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a Guid.
	/// </summary>
	public ILink<Guid> ToGuid() => new GuidMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a Guid.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<Guid> ToGuid(string columnName) => new GuidMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable Guid.
	/// </summary>
	public ILink<Guid?> ToGuidOrNull() => new GuidOrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a nullable Guid.
	/// </summary>
	public ILink<Guid?> ToGuidOrNull(string columnName) => new GuidOrNullMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a Int16.
	/// </summary>
	public ILink<short> ToInt16() => new Int16Materializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a Int16.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<short> ToInt16(string columnName) => new Int16Materializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable Int16.
	/// </summary>
	public ILink<short?> ToInt16OrNull() => new Int16OrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a nullable Int16.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<short?> ToInt16OrNull(string columnName) => new Int16OrNullMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a Int32.
	/// </summary>
	public ILink<int> ToInt32() => new Int32Materializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a Int32.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<int> ToInt32(string columnName) => new Int32Materializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable Int32.
	/// </summary>
	public ILink<int?> ToInt32OrNull() => new Int32OrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a nullable Int32.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<int?> ToInt32OrNull(string columnName) => new Int32OrNullMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a Int64.
	/// </summary>
	public ILink<long> ToInt64() => new Int64Materializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a Int64.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<long> ToInt64(string columnName) => new Int64Materializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable Int64.
	/// </summary>
	public ILink<long?> ToInt64OrNull() => new Int64OrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a nullable Int64.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<long?> ToInt64OrNull(string columnName) => new Int64OrNullMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a Single.
	/// </summary>
	public ILink<float> ToSingle() => new SingleMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a Single.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<float> ToSingle(string columnName) => new SingleMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable Single.
	/// </summary>
	public ILink<float?> ToSingleOrNull() => new SingleOrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a nullable Single.
	/// </summary>
	public ILink<float?> ToSingleOrNull(string columnName) => new SingleOrNullMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a non-nullable string.
	/// </summary>
	/// <returns></returns>
	public new ILink<string> ToString() => new StringMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a non-nullable string.
	/// </summary>
	/// <returns></returns>
	public ILink<string> ToString(string columnName) => new StringMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable string.
	/// </summary>
	/// <returns></returns>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<string?> ToStringOrNull(string columnName) => new StringOrNullMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable string.
	/// </summary>
	/// <returns></returns>
	public ILink<string?> ToStringOrNull() => new StringOrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a TimeOnly.
	/// </summary>
	public ILink<TimeOnly> ToTimeOnly() => new TimeOnlyMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a TimeOnly.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<TimeOnly> ToTimeOnly(string columnName) => new TimeOnlyMaterializer<TCommand, TParameter>(this, columnName);
	/// <summary>
	/// Indicates the results should be materialized as a nullable TimeOnly.
	/// </summary>
	public ILink<TimeOnly?> ToTimeOnlyOrNull() => new TimeOnlyOrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a nullable TimeOnly.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<TimeOnly?> ToTimeOnlyOrNull(string columnName) => new TimeOnlyOrNullMaterializer<TCommand, TParameter>(this, columnName);
	/// <summary>
	/// Indicates the results should be materialized as a TimeSpan.
	/// </summary>
	public ILink<TimeSpan> ToTimeSpan() => new TimeSpanMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a TimeSpan.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<TimeSpan> ToTimeSpan(string columnName) => new TimeSpanMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a nullable TimeSpan.
	/// </summary>
	public ILink<TimeSpan?> ToTimeSpanOrNull() => new TimeSpanOrNullMaterializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a nullable TimeSpan.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<TimeSpan?> ToTimeSpanOrNull(string columnName) => new TimeSpanOrNullMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Indicates the results should be materialized as a UInt64.
	/// </summary>
	public ILink<ulong> ToUInt64() => new UInt64Materializer<TCommand, TParameter>(this);

	/// <summary>
	/// Indicates the results should be materialized as a UInt64.
	/// </summary>
	/// <param name="columnName">Name of the desired column.</param>
	public ILink<ulong> ToUInt64(string columnName) => new UInt64Materializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Materializes the result as an XElement.
	/// </summary>
	/// <returns>ILink&lt;XElement&gt;.</returns>
	public ILink<XElement> ToXml() => new XElementMaterializer<TCommand, TParameter>(this, null);

	/// <summary>
	/// Materializes the result as an XElement.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns>ILink&lt;XElement&gt;.</returns>
	public ILink<XElement> ToXml(string columnName) => new XElementMaterializer<TCommand, TParameter>(this, columnName);

	/// <summary>
	/// Materializes the result as an XElement or null.
	/// </summary>
	/// <returns>ILink&lt;XElement&gt;.</returns>
	public ILink<XElement?> ToXmlOrNull() => new XElementOrNullMaterializer<TCommand, TParameter>(this, null);

	/// <summary>
	/// Materializes the result as an XElement or null.
	/// </summary>
	/// <param name="columnName">Name of the column.</param>
	/// <returns>ILink&lt;XElement&gt;.</returns>
	public ILink<XElement?> ToXmlOrNull(string columnName) => new XElementOrNullMaterializer<TCommand, TParameter>(this, columnName);
}
