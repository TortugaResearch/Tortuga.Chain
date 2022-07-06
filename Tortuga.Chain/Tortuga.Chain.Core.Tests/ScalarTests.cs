#nullable disable

using System.Data.Common;
using System.Xml.Linq;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Core.Tests;

[TestClass]
public class ScalarTests : MaterializerTestBase
{
	[TestMethod]
	public async Task BigIntNotNull_Int64_ScalarTest()
	{
		await ScalarTest<long>("BigIntNotNull", typeof(Int64Materializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BigIntNull_Int64_ScalarTest()
	{
		await ScalarTest<long>("BigIntNull", typeof(Int64Materializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BinaryNotNull_ByteArray_ScalarTest()
	{
		await ScalarTest<byte[]>("BinaryNotNull", typeof(ByteArrayMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BinaryNull_ByteArray_ScalarTest()
	{
		await ScalarTest<byte[]>("BinaryNull", typeof(ByteArrayMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BitNotNull_Boolean_ScalarTest()
	{
		await ScalarTest<bool>("BitNotNull", typeof(BooleanMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BitNull_Boolean_ScalarTest()
	{
		await ScalarTest<bool>("BitNull", typeof(BooleanMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNotNull_Char_ScalarTest()
	{
		await ScalarTest<char>("CharNotNull", typeof(CharMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNotNull_String_ScalarTest()
	{
		await ScalarTest<string>("CharNotNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNull_Char_ScalarTest()
	{
		await ScalarTest<char>("CharNull", typeof(CharMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNull_String_ScalarTest()
	{
		await ScalarTest<string>("CharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DateNotNull_DateTime_ScalarTest()
	{
		await ScalarTest<DateTime>("DateNotNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DateNull_DateTime_ScalarTest()
	{
		await ScalarTest<DateTime>("DateNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task Datetime2NotNull_DateTime_ScalarTest()
	{
		await ScalarTest<DateTime>("Datetime2NotNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task Datetime2Null_DateTime_ScalarTest()
	{
		await ScalarTest<DateTime>("Datetime2Null", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeNotNull_DateTime_ScalarTest()
	{
		await ScalarTest<DateTime>("DatetimeNotNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeNull_DateTime_ScalarTest()
	{
		await ScalarTest<DateTime>("DatetimeNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeoffsetNotNull_DateTimeOffset_ScalarTest()
	{
		await ScalarTest<DateTimeOffset>("DatetimeoffsetNotNull", typeof(DateTimeOffsetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeoffsetNull_DateTimeOffset_ScalarTest()
	{
		await ScalarTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DecimalNotNull_Decimal_ScalarTest()
	{
		await ScalarTest<decimal>("DecimalNotNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DecimalNull_Decimal_ScalarTest()
	{
		await ScalarTest<decimal>("DecimalNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task FloatNotNull_Double_ScalarTest()
	{
		await ScalarTest<double>("FloatNotNull", typeof(DoubleMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task FloatNull_Double_ScalarTest()
	{
		await ScalarTest<double>("FloatNull", typeof(DoubleMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task IntNotNull_Int32_ScalarTest()
	{
		await ScalarTest<int>("IntNotNull", typeof(Int32Materializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task IntNull_Int32_ScalarTest()
	{
		await ScalarTest<int>("IntNull", typeof(Int32Materializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task MoneyNotNull_Decimal_ScalarTest()
	{
		await ScalarTest<decimal>("MoneyNotNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task MoneyNull_Decimal_ScalarTest()
	{
		await ScalarTest<decimal>("MoneyNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NCharNotNull_String_ScalarTest()
	{
		await ScalarTest<string>("NCharNotNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NCharNull_String_ScalarTest()
	{
		await ScalarTest<string>("NCharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NTextNotNull_String_ScalarTest()
	{
		await ScalarTest<string>("NTextNotNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NTextNull_String_ScalarTest()
	{
		await ScalarTest<string>("NTextNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NumericNotNull_Decimal_ScalarTest()
	{
		await ScalarTest<decimal>("NumericNotNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NumericNull_Decimal_ScalarTest()
	{
		await ScalarTest<decimal>("NumericNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NVarCharNotNull_String_ScalarTest()
	{
		await ScalarTest<string>("NVarCharNotNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NVarCharNull_String_ScalarTest()
	{
		await ScalarTest<string>("NVarCharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task RealNotNull_Single_ScalarTest()
	{
		await ScalarTest<float>("RealNotNull", typeof(SingleMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task RealNull_Single_ScalarTest()
	{
		await ScalarTest<float>("RealNull", typeof(SingleMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmalldatetimeNotNull_DateTime_ScalarTest()
	{
		await ScalarTest<DateTime>("SmalldatetimeNotNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmalldatetimeNull_DateTime_ScalarTest()
	{
		await ScalarTest<DateTime>("SmalldatetimeNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallIntNotNull_Int16_ScalarTest()
	{
		await ScalarTest<short>("SmallIntNotNull", typeof(Int16Materializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallIntNull_Int16_ScalarTest()
	{
		await ScalarTest<short>("SmallIntNull", typeof(Int16Materializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallMoneyNotNull_Decimal_ScalarTest()
	{
		await ScalarTest<decimal>("SmallMoneyNotNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallMoneyNull_Decimal_ScalarTest()
	{
		await ScalarTest<decimal>("SmallMoneyNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TextNotNull_String_ScalarTest()
	{
		await ScalarTest<string>("TextNotNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TextNull_String_ScalarTest()
	{
		await ScalarTest<string>("TextNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TimeNotNull_TimeSpan_ScalarTest()
	{
		await ScalarTest<TimeSpan>("TimeNotNull", typeof(TimeSpanMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TimeNull_TimeSpan_ScalarTest()
	{
		await ScalarTest<TimeSpan>("TimeNull", typeof(TimeSpanMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TinyIntNotNull_Byte_ScalarTest()
	{
		await ScalarTest<byte>("TinyIntNotNull", typeof(ByteMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TinyIntNull_Byte_ScalarTest()
	{
		await ScalarTest<byte>("TinyIntNull", typeof(ByteMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task UniqueidentifierNotNull_Guid_ScalarTest()
	{
		await ScalarTest<Guid>("UniqueidentifierNotNull", typeof(GuidMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task UniqueidentifierNull_Guid_ScalarTest()
	{
		await ScalarTest<Guid>("UniqueidentifierNull", typeof(GuidMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task VarBinaryNotNull_ByteArray_ScalarTest()
	{
		await ScalarTest<byte[]>("VarBinaryNotNull", typeof(ByteArrayMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task VarBinaryNull_ByteArray_ScalarTest()
	{
		await ScalarTest<byte[]>("VarBinaryNull", typeof(ByteArrayMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task VarCharNotNull_String_ScalarTest()
	{
		await ScalarTest<string>("VarCharNotNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task VarCharNull_String_ScalarTest()
	{
		await ScalarTest<string>("VarCharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task XmlNotNull_String_ScalarTest()
	{
		await ScalarTest<string>("XmlNotNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task XmlNotNull_XElement_ScalarTest()
	{
		await ScalarTest<XElement>("XmlNotNull", typeof(XElementMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task XmlNull_String_ScalarTest()
	{
		await ScalarTest<string>("XmlNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task XmlNull_XElement_ScalarTest()
	{
		await ScalarTest<XElement>("XmlNull", typeof(XElementMaterializer<DbCommand, DbParameter>));
	}

	async Task ScalarTest<TResult>(string columnName, Type materializerType)
	{
		var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id = 1");
		var materializer1 = (ILink<TResult>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName });
		var result1 = materializer1.Execute();
		var result1a = await materializer1.ExecuteAsync();

		if (typeof(TResult) == typeof(byte[]))
		{
			var result1Cast = (byte[])(object)result1;
			var result1aCast = (byte[])(object)result1a;
			Assert.AreEqual(result1Cast.Length, result1aCast.Length);
		}
		else if (result1 is XElement)
		{
			Assert.AreEqual(result1.ToString(), result1a.ToString(), "Results don't match");
		}
		else
		{
			Assert.AreEqual(result1, result1a, "Results don't match");
		}
	}
}
