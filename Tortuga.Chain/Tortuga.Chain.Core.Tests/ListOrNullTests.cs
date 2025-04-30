using System.Data.Common;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Core.Tests;

[TestClass]
public class ListOrNullTests : MaterializerTestBase
{
	[TestMethod]
	public async Task BigIntNotNull_Int64_ListOrNullTest()
	{
		await ListOrNullTestAsync<long?>("BigIntNotNull", typeof(Int64OrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BigIntNull_Int64_ListOrNullTest()
	{
		await ListOrNullTestAsync<long?>("BigIntNull", typeof(Int64OrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BinaryNotNull_ByteArray_ListOrNullTest()
	{
		await ListOrNullTestAsync<byte[]?>("BinaryNotNull", typeof(ByteArrayOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BinaryNull_ByteArray_ListOrNullTest()
	{
		await ListOrNullTestAsync<byte[]?>("BinaryNull", typeof(ByteArrayOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BitNotNull_Boolean_ListOrNullTest()
	{
		await ListOrNullTestAsync<bool?>("BitNotNull", typeof(BooleanOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BitNull_Boolean_ListOrNullTest()
	{
		await ListOrNullTestAsync<bool?>("BitNull", typeof(BooleanOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNotNull_Char_ListOrNullTest()
	{
		await ListOrNullTestAsync<char?>("CharNotNull", typeof(CharOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNotNull_String_ListOrNullTest()
	{
		await ListOrNullTestAsync<string?>("CharNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNull_Char_ListOrNullTest()
	{
		await ListOrNullTestAsync<char?>("CharNull", typeof(CharOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNull_String_ListOrNullTest()
	{
		await ListOrNullTestAsync<string?>("CharNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharOneNotNull_Char_ListOrNullTest()
	{
		await ListOrNullTestAsync<char?>("CharOneNotNull", typeof(CharOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharOneNull_Char_ListOrNullTest()
	{
		await ListOrNullTestAsync<char?>("CharOneNull", typeof(CharOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DateNotNull_DateTime_ListOrNullTest()
	{
		await ListOrNullTestAsync<DateTime?>("DateNotNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DateNull_DateTime_ListOrNullTest()
	{
		await ListOrNullTestAsync<DateTime?>("DateNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Datetime2NotNull_DateTime_ListOrNullTest()
	{
		await ListOrNullTestAsync<DateTime?>("Datetime2NotNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Datetime2Null_DateTime_ListOrNullTest()
	{
		await ListOrNullTestAsync<DateTime?>("Datetime2Null", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeNotNull_DateTime_ListOrNullTest()
	{
		await ListOrNullTestAsync<DateTime?>("DatetimeNotNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeNull_DateTime_ListOrNullTest()
	{
		await ListOrNullTestAsync<DateTime?>("DatetimeNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeoffsetNotNull_DateTimeOffset_ListOrNullTest()
	{
		await ListOrNullTestAsync<DateTimeOffset?>("DatetimeoffsetNotNull", typeof(DateTimeOffsetOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeoffsetNull_DateTimeOffset_ListOrNullTest()
	{
		await ListOrNullTestAsync<DateTimeOffset?>("DatetimeoffsetNull", typeof(DateTimeOffsetOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DecimalNotNull_Decimal_ListOrNullTest()
	{
		await ListOrNullTestAsync<decimal?>("DecimalNotNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DecimalNull_Decimal_ListOrNullTest()
	{
		await ListOrNullTestAsync<decimal?>("DecimalNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task FloatNotNull_Double_ListOrNullTest()
	{
		await ListOrNullTestAsync<double?>("FloatNotNull", typeof(DoubleOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task FloatNull_Double_ListOrNullTest()
	{
		await ListOrNullTestAsync<double?>("FloatNull", typeof(DoubleOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task IntNotNull_Int32_ListOrNullTest()
	{
		await ListOrNullTestAsync<int?>("IntNotNull", typeof(Int32OrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task IntNull_Int32_ListOrNullTest()
	{
		await ListOrNullTestAsync<int?>("IntNull", typeof(Int32OrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task MoneyNotNull_Decimal_ListOrNullTest()
	{
		await ListOrNullTestAsync<decimal?>("MoneyNotNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task MoneyNull_Decimal_ListOrNullTest()
	{
		await ListOrNullTestAsync<decimal?>("MoneyNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NCharNotNull_String_ListOrNullTest()
	{
		await ListOrNullTestAsync<string?>("NCharNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NCharNull_String_ListOrNullTest()
	{
		await ListOrNullTestAsync<string?>("NCharNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NTextNotNull_String_ListOrNullTest()
	{
		await ListOrNullTestAsync<string?>("NTextNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NTextNull_String_ListOrNullTest()
	{
		await ListOrNullTestAsync<string?>("NTextNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NumericNotNull_Decimal_ListOrNullTest()
	{
		await ListOrNullTestAsync<decimal?>("NumericNotNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NumericNull_Decimal_ListOrNullTest()
	{
		await ListOrNullTestAsync<decimal?>("NumericNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NVarCharNotNull_String_ListOrNullTest()
	{
		await ListOrNullTestAsync<string?>("NVarCharNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NVarCharNull_String_ListOrNullTest()
	{
		await ListOrNullTestAsync<string?>("NVarCharNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task RealNotNull_Single_ListOrNullTest()
	{
		await ListOrNullTestAsync<float?>("RealNotNull", typeof(SingleOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task RealNull_Single_ListOrNullTest()
	{
		await ListOrNullTestAsync<float?>("RealNull", typeof(SingleOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmalldatetimeNotNull_DateTime_ListOrNullTest()
	{
		await ListOrNullTestAsync<DateTime?>("SmalldatetimeNotNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmalldatetimeNull_DateTime_ListOrNullTest()
	{
		await ListOrNullTestAsync<DateTime?>("SmalldatetimeNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallIntNull_Int16_ListOrNullTest()
	{
		await ListOrNullTestAsync<short?>("SmallIntNull", typeof(Int16OrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallMoneyNotNull_Decimal_ListOrNullTest()
	{
		await ListOrNullTestAsync<decimal?>("SmallMoneyNotNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallMoneyNull_Decimal_ListOrNullTest()
	{
		await ListOrNullTestAsync<decimal?>("SmallMoneyNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TextNotNull_String_ListOrNullTest()
	{
		await ListOrNullTestAsync<string?>("TextNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TextNull_String_ListOrNullTest()
	{
		await ListOrNullTestAsync<string?>("TextNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TimeNotNull_TimeSpan_ListOrNullTest()
	{
		await ListOrNullTestAsync<TimeSpan?>("TimeNotNull", typeof(TimeSpanOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TimeNull_TimeSpan_ListOrNullTest()
	{
		await ListOrNullTestAsync<TimeSpan?>("TimeNull", typeof(TimeSpanOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TinyIntNotNull_Byte_ListOrNullTest()
	{
		await ListOrNullTestAsync<byte?>("TinyIntNotNull", typeof(ByteOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TinyIntNull_Byte_ListOrNullTest()
	{
		await ListOrNullTestAsync<byte?>("TinyIntNull", typeof(ByteOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task UniqueidentifierNotNull_Guid_ListOrNullTest()
	{
		await ListOrNullTestAsync<Guid?>("UniqueidentifierNotNull", typeof(GuidOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task UniqueidentifierNull_Guid_ListOrNullTest()
	{
		await ListOrNullTestAsync<Guid?>("UniqueidentifierNull", typeof(GuidOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task VarBinaryNotNull_ByteArray_ListOrNullTest()
	{
		await ListOrNullTestAsync<byte[]?>("VarBinaryNotNull", typeof(ByteArrayOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task VarBinaryNull_ByteArray_ListOrNullTest()
	{
		await ListOrNullTestAsync<byte[]?>("VarBinaryNull", typeof(ByteArrayOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task VarCharNotNull_String_ListOrNullTest()
	{
		await ListOrNullTestAsync<string?>("VarCharNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task VarCharNull_String_ListOrNullTest()
	{
		await ListOrNullTestAsync<string?>("VarCharNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task XmlNotNull_String_ListOrNullTest()
	{
		await ListOrNullTestAsync<string?>("XmlNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task XmlNull_String_ListOrNullTest()
	{
		await ListOrNullTestAsync<string?>("XmlNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	async Task ListOrNullTestAsync<TResult>(string columnName, Type materializerType)
	{
		var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (1, 2, 3, 4)");
		var materializer1 = (ILink<List<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.None })!;
		var result1 = materializer1.Execute();
		var result1a = await materializer1.ExecuteAsync().ConfigureAwait(false);
		Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
		Assert.IsTrue(result1.Any(x => x != null));
	}
}
