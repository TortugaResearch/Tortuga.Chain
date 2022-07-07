#nullable disable

using System.Data.Common;
using System.Xml.Linq;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Core.Tests;

[TestClass]
public class ListTests : MaterializerTestBase
{
	[TestMethod]
	public async Task BigIntNotNull_Int64_ListTest()
	{
		await ListTest<long>("BigIntNotNull", typeof(Int64ListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BigIntNull_Int64_ListTest()
	{
		await ListTest<long>("BigIntNull", typeof(Int64ListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BinaryNotNull_ByteArray_ListTest()
	{
		await ListTest<byte[]>("BinaryNotNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BinaryNull_ByteArray_ListTest()
	{
		await ListTest<byte[]>("BinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BitNotNull_Boolean_ListTest()
	{
		await ListTest<bool>("BitNotNull", typeof(BooleanListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BitNull_Boolean_ListTest()
	{
		await ListTest<bool>("BitNull", typeof(BooleanListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNotNull_Char_ListTest()
	{
		await ListTest<char>("CharNotNull", typeof(CharListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNotNull_String_ListTest()
	{
		await ListTest<string>("CharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNull_Char_ListTest()
	{
		await ListTest<char>("CharNull", typeof(CharListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNull_String_ListTest()
	{
		await ListTest<string>("CharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharOneNotNull_Char_ListTest()
	{
		await ListTest<char>("CharOneNotNull", typeof(CharListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharOneNull_Char_ListTest()
	{
		await ListTest<char>("CharOneNull", typeof(CharListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DateNotNull_DateTime_ListTest()
	{
		await ListTest<DateTime>("DateNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DateNull_DateTime_ListTest()
	{
		await ListTest<DateTime>("DateNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task Datetime2NotNull_DateTime_ListTest()
	{
		await ListTest<DateTime>("Datetime2NotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task Datetime2Null_DateTime_ListTest()
	{
		await ListTest<DateTime>("Datetime2Null", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeNotNull_DateTime_ListTest()
	{
		await ListTest<DateTime>("DatetimeNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeNull_DateTime_ListTest()
	{
		await ListTest<DateTime>("DatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeoffsetNotNull_DateTimeOffset_ListTest()
	{
		await ListTest<DateTimeOffset>("DatetimeoffsetNotNull", typeof(DateTimeOffsetListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeoffsetNull_DateTimeOffset_ListTest()
	{
		await ListTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DecimalNotNull_Decimal_ListTest()
	{
		await ListTest<decimal>("DecimalNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DecimalNull_Decimal_ListTest()
	{
		await ListTest<decimal>("DecimalNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task FloatNotNull_Double_ListTest()
	{
		await ListTest<double>("FloatNotNull", typeof(DoubleListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task FloatNull_Double_ListTest()
	{
		await ListTest<double>("FloatNull", typeof(DoubleListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task IntNotNull_Int32_ListTest()
	{
		await ListTest<int>("IntNotNull", typeof(Int32ListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task IntNull_Int32_ListTest()
	{
		await ListTest<int>("IntNull", typeof(Int32ListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task MoneyNotNull_Decimal_ListTest()
	{
		await ListTest<decimal>("MoneyNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task MoneyNull_Decimal_ListTest()
	{
		await ListTest<decimal>("MoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NCharNotNull_String_ListTest()
	{
		await ListTest<string>("NCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NCharNull_String_ListTest()
	{
		await ListTest<string>("NCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NTextNotNull_String_ListTest()
	{
		await ListTest<string>("NTextNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NTextNull_String_ListTest()
	{
		await ListTest<string>("NTextNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NumericNotNull_Decimal_ListTest()
	{
		await ListTest<decimal>("NumericNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NumericNull_Decimal_ListTest()
	{
		await ListTest<decimal>("NumericNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NVarCharNotNull_String_ListTest()
	{
		await ListTest<string>("NVarCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NVarCharNull_String_ListTest()
	{
		await ListTest<string>("NVarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task RealNotNull_Single_ListTest()
	{
		await ListTest<float>("RealNotNull", typeof(SingleListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task RealNull_Single_ListTest()
	{
		await ListTest<float>("RealNull", typeof(SingleListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmalldatetimeNotNull_DateTime_ListTest()
	{
		await ListTest<DateTime>("SmalldatetimeNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmalldatetimeNull_DateTime_ListTest()
	{
		await ListTest<DateTime>("SmalldatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallIntNotNull_Int16_ListTest()
	{
		await ListTest<short>("SmallIntNotNull", typeof(Int16ListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallIntNull_Int16_ListTest()
	{
		await ListTest<short>("SmallIntNull", typeof(Int16ListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallMoneyNotNull_Decimal_ListTest()
	{
		await ListTest<decimal>("SmallMoneyNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallMoneyNull_Decimal_ListTest()
	{
		await ListTest<decimal>("SmallMoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task Table()
	{
		var materializer = DataSource.Sql($"SELECT * FROM dbo.AllTypes").ToTable();
		var result1 = materializer.Execute();
		var result1a = await materializer.ExecuteAsync();
	}

	[TestMethod]
	public async Task TextNotNull_String_ListTest()
	{
		await ListTest<string>("TextNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TextNull_String_ListTest()
	{
		await ListTest<string>("TextNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TimeNotNull_TimeSpan_ListTest()
	{
		await ListTest<TimeSpan>("TimeNotNull", typeof(TimeSpanListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TimeNull_TimeSpan_ListTest()
	{
		await ListTest<TimeSpan>("TimeNull", typeof(TimeSpanListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TinyIntNotNull_Byte_ListTest()
	{
		await ListTest<byte>("TinyIntNotNull", typeof(ByteListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TinyIntNull_Byte_ListTest()
	{
		await ListTest<byte>("TinyIntNull", typeof(ByteListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task UniqueidentifierNotNull_Guid_ListTest()
	{
		await ListTest<Guid>("UniqueidentifierNotNull", typeof(GuidListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task UniqueidentifierNull_Guid_ListTest()
	{
		await ListTest<Guid>("UniqueidentifierNull", typeof(GuidListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task VarBinaryNotNull_ByteArray_ListTest()
	{
		await ListTest<byte[]>("VarBinaryNotNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task VarBinaryNull_ByteArray_ListTest()
	{
		await ListTest<byte[]>("VarBinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task VarCharNotNull_String_ListTest()
	{
		await ListTest<string>("VarCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task VarCharNull_String_ListTest()
	{
		await ListTest<string>("VarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task XmlNotNull_String_ListTest()
	{
		await ListTest<string>("XmlNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task XmlNotNull_XElement_ListTest()
	{
		await ListTest<XElement>("XmlNotNull", typeof(XElementListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task XmlNull_String_ListTest()
	{
		await ListTest<string>("XmlNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task XmlNull_XElement_ListTest()
	{
		await ListTest<XElement>("XmlNull", typeof(XElementListMaterializer<DbCommand, DbParameter>));
	}

	async Task ListTest<TResult>(string columnName, Type materializerType)
	{
		var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (1, 2, 4)");
		var materializer1 = (ILink<List<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.None });
		var result1 = materializer1.Execute();
		var result1a = await materializer1.ExecuteAsync();
		Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
		Assert.AreEqual(3, result1.Count, "Count is wrong.");
	}
}
