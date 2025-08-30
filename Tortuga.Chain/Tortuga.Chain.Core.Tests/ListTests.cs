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
		await ListTestAsync<long>("BigIntNotNull", typeof(Int64ListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BigIntNull_Int64_ListTest()
	{
		await ListTestAsync<long>("BigIntNull", typeof(Int64ListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BinaryNotNull_ByteArray_ListTest()
	{
		await ListTestAsync<byte[]>("BinaryNotNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BinaryNull_ByteArray_ListTest()
	{
		await ListTestAsync<byte[]>("BinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BitNotNull_Boolean_ListTest()
	{
		await ListTestAsync<bool>("BitNotNull", typeof(BooleanListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BitNull_Boolean_ListTest()
	{
		await ListTestAsync<bool>("BitNull", typeof(BooleanListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNotNull_Char_ListTest()
	{
		await ListTestAsync<char>("CharNotNull", typeof(CharListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNotNull_String_ListTest()
	{
		await ListTestAsync<string>("CharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNull_Char_ListTest()
	{
		await ListTestAsync<char>("CharNull", typeof(CharListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNull_String_ListTest()
	{
		await ListTestAsync<string>("CharNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharOneNotNull_Char_ListTest()
	{
		await ListTestAsync<char>("CharOneNotNull", typeof(CharListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharOneNull_Char_ListTest()
	{
		await ListTestAsync<char>("CharOneNull", typeof(CharListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DateNotNull_DateTime_ListTest()
	{
		await ListTestAsync<DateTime>("DateNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DateNull_DateTime_ListTest()
	{
		await ListTestAsync<DateTime>("DateNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Datetime2NotNull_DateTime_ListTest()
	{
		await ListTestAsync<DateTime>("Datetime2NotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Datetime2Null_DateTime_ListTest()
	{
		await ListTestAsync<DateTime>("Datetime2Null", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeNotNull_DateTime_ListTest()
	{
		await ListTestAsync<DateTime>("DatetimeNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeNull_DateTime_ListTest()
	{
		await ListTestAsync<DateTime>("DatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeoffsetNotNull_DateTimeOffset_ListTest()
	{
		await ListTestAsync<DateTimeOffset>("DatetimeoffsetNotNull", typeof(DateTimeOffsetListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeoffsetNull_DateTimeOffset_ListTest()
	{
		await ListTestAsync<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DecimalNotNull_Decimal_ListTest()
	{
		await ListTestAsync<decimal>("DecimalNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DecimalNull_Decimal_ListTest()
	{
		await ListTestAsync<decimal>("DecimalNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task FloatNotNull_Double_ListTest()
	{
		await ListTestAsync<double>("FloatNotNull", typeof(DoubleListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task FloatNull_Double_ListTest()
	{
		await ListTestAsync<double>("FloatNull", typeof(DoubleListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task IntNotNull_Int32_ListTest()
	{
		await ListTestAsync<int>("IntNotNull", typeof(Int32ListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task IntNull_Int32_ListTest()
	{
		await ListTestAsync<int>("IntNull", typeof(Int32ListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task MoneyNotNull_Decimal_ListTest()
	{
		await ListTestAsync<decimal>("MoneyNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task MoneyNull_Decimal_ListTest()
	{
		await ListTestAsync<decimal>("MoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NCharNotNull_String_ListTest()
	{
		await ListTestAsync<string>("NCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NCharNull_String_ListTest()
	{
		await ListTestAsync<string>("NCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NTextNotNull_String_ListTest()
	{
		await ListTestAsync<string>("NTextNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NTextNull_String_ListTest()
	{
		await ListTestAsync<string>("NTextNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NumericNotNull_Decimal_ListTest()
	{
		await ListTestAsync<decimal>("NumericNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NumericNull_Decimal_ListTest()
	{
		await ListTestAsync<decimal>("NumericNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NVarCharNotNull_String_ListTest()
	{
		await ListTestAsync<string>("NVarCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NVarCharNull_String_ListTest()
	{
		await ListTestAsync<string>("NVarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task RealNotNull_Single_ListTest()
	{
		await ListTestAsync<float>("RealNotNull", typeof(SingleListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task RealNull_Single_ListTest()
	{
		await ListTestAsync<float>("RealNull", typeof(SingleListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmalldatetimeNotNull_DateTime_ListTest()
	{
		await ListTestAsync<DateTime>("SmalldatetimeNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmalldatetimeNull_DateTime_ListTest()
	{
		await ListTestAsync<DateTime>("SmalldatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallIntNotNull_Int16_ListTest()
	{
		await ListTestAsync<short>("SmallIntNotNull", typeof(Int16ListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallIntNull_Int16_ListTest()
	{
		await ListTestAsync<short>("SmallIntNull", typeof(Int16ListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallMoneyNotNull_Decimal_ListTest()
	{
		await ListTestAsync<decimal>("SmallMoneyNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallMoneyNull_Decimal_ListTest()
	{
		await ListTestAsync<decimal>("SmallMoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Table()
	{
		var materializer = DataSource.Sql($"SELECT * FROM dbo.AllTypes").ToTable();
		var result1 = materializer.Execute();
		var result1a = await materializer.ExecuteAsync().ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TextNotNull_String_ListTest()
	{
		await ListTestAsync<string>("TextNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TextNull_String_ListTest()
	{
		await ListTestAsync<string>("TextNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TimeNotNull_TimeSpan_ListTest()
	{
		await ListTestAsync<TimeSpan>("TimeNotNull", typeof(TimeSpanListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TimeNull_TimeSpan_ListTest()
	{
		await ListTestAsync<TimeSpan>("TimeNull", typeof(TimeSpanListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TinyIntNotNull_Byte_ListTest()
	{
		await ListTestAsync<byte>("TinyIntNotNull", typeof(ByteListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TinyIntNull_Byte_ListTest()
	{
		await ListTestAsync<byte>("TinyIntNull", typeof(ByteListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task UniqueidentifierNotNull_Guid_ListTest()
	{
		await ListTestAsync<Guid>("UniqueidentifierNotNull", typeof(GuidListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task UniqueidentifierNull_Guid_ListTest()
	{
		await ListTestAsync<Guid>("UniqueidentifierNull", typeof(GuidListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task VarBinaryNotNull_ByteArray_ListTest()
	{
		await ListTestAsync<byte[]>("VarBinaryNotNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task VarBinaryNull_ByteArray_ListTest()
	{
		await ListTestAsync<byte[]>("VarBinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task VarCharNotNull_String_ListTest()
	{
		await ListTestAsync<string>("VarCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task VarCharNull_String_ListTest()
	{
		await ListTestAsync<string>("VarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task XmlNotNull_String_ListTest()
	{
		await ListTestAsync<string>("XmlNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task XmlNotNull_XElement_ListTest()
	{
		await ListTestAsync<XElement>("XmlNotNull", typeof(XElementListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task XmlNull_String_ListTest()
	{
		await ListTestAsync<string>("XmlNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task XmlNull_XElement_ListTest()
	{
		await ListTestAsync<XElement>("XmlNull", typeof(XElementListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	async Task ListTestAsync<TResult>(string columnName, Type materializerType)
	{
		var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (1, 2, 4)");
		var materializer1 = (ILink<List<TResult>>)Activator.CreateInstance(materializerType, [cb1, columnName, ListOptions.None]);
		var result1 = materializer1.Execute();
		var result1a = await materializer1.ExecuteAsync().ConfigureAwait(false);
		Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
		Assert.AreEqual(3, result1.Count, "Count is wrong.");
	}













	[TestMethod]
	public async Task DateNotNull_DateOnly_ListTest()
	{
		await ListTestAsync<DateOnly>("DateNotNull", typeof(DateOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DateNull_DateOnly_ListTest()
	{
		await ListTestAsync<DateOnly>("DateNull", typeof(DateOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Datetime2NotNull_DateOnly_ListTest()
	{
		await ListTestAsync<DateOnly>("Datetime2NotNull", typeof(DateOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Datetime2Null_DateOnly_ListTest()
	{
		await ListTestAsync<DateOnly>("Datetime2Null", typeof(DateOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeNotNull_DateOnly_ListTest()
	{
		await ListTestAsync<DateOnly>("DatetimeNotNull", typeof(DateOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeNull_DateOnly_ListTest()
	{
		await ListTestAsync<DateOnly>("DatetimeNull", typeof(DateOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmalldatetimeNotNull_DateOnly_ListTest()
	{
		await ListTestAsync<DateOnly>("SmalldatetimeNotNull", typeof(DateOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmalldatetimeNull_DateOnly_ListTest()
	{
		await ListTestAsync<DateOnly>("SmalldatetimeNull", typeof(DateOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}




































	[TestMethod]
	public async Task Datetime2NotNull_TimeOnly_ListTest()
	{
		await ListTestAsync<TimeOnly>("Datetime2NotNull", typeof(TimeOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Datetime2Null_TimeOnly_ListTest()
	{
		await ListTestAsync<TimeOnly>("Datetime2Null", typeof(TimeOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeNotNull_TimeOnly_ListTest()
	{
		await ListTestAsync<TimeOnly>("DatetimeNotNull", typeof(TimeOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeNull_TimeOnly_ListTest()
	{
		await ListTestAsync<TimeOnly>("DatetimeNull", typeof(TimeOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmalldatetimeNotNull_TimeOnly_ListTest()
	{
		await ListTestAsync<TimeOnly>("SmalldatetimeNotNull", typeof(TimeOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmalldatetimeNull_TimeOnly_ListTest()
	{
		await ListTestAsync<TimeOnly>("SmalldatetimeNull", typeof(TimeOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}
	[TestMethod]
	public async Task TimeNotNull_TimeOnly_ListTest()
	{
		await ListTestAsync<TimeOnly>("TimeNotNull", typeof(TimeOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TimeNull_TimeOnly_ListTest()
	{
		await ListTestAsync<TimeOnly>("TimeNull", typeof(TimeOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}
}
