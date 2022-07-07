#nullable disable

using System.Data.Common;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Core.Tests;

[TestClass]
public class SetTests : MaterializerTestBase
{
	[TestMethod]
	public async Task BigIntNotNull_Int64_SetTest()
	{
		await SetTest<long>("BigIntNotNull", typeof(Int64SetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BigIntNull_Int64_SetTest()
	{
		await SetTest<long>("BigIntNull", typeof(Int64SetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNotNull_Char_SetTest()
	{
		await SetTest<char>("CharNotNull", typeof(CharSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNotNull_String_SetTest()
	{
		await SetTest<string>("CharNotNull", typeof(StringSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNull_Char_SetTest()
	{
		await SetTest<char>("CharNull", typeof(CharSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNull_String_SetTest()
	{
		await SetTest<string>("CharNull", typeof(StringSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharOneNotNull_Char_SetTest()
	{
		await SetTest<char>("CharOneNotNull", typeof(CharSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharOneNull_Char_SetTest()
	{
		await SetTest<char>("CharOneNull", typeof(CharSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DataTable()
	{
		var materializer = DataSource.Sql($"SELECT * FROM dbo.AllTypes").ToDataTable();
		var result1 = materializer.Execute();
		var result1a = await materializer.ExecuteAsync();
	}

	[TestMethod]
	public async Task DateNotNull_DateTime_SetTest()
	{
		await SetTest<DateTime>("DateNotNull", typeof(DateTimeSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DateNull_DateTime_SetTest()
	{
		await SetTest<DateTime>("DateNull", typeof(DateTimeSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task Datetime2NotNull_DateTime_SetTest()
	{
		await SetTest<DateTime>("Datetime2NotNull", typeof(DateTimeSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task Datetime2Null_DateTime_SetTest()
	{
		await SetTest<DateTime>("Datetime2Null", typeof(DateTimeSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeNotNull_DateTime_SetTest()
	{
		await SetTest<DateTime>("DatetimeNotNull", typeof(DateTimeSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeNull_DateTime_SetTest()
	{
		await SetTest<DateTime>("DatetimeNull", typeof(DateTimeSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeoffsetNotNull_DateTimeOffset_SetTest()
	{
		await SetTest<DateTimeOffset>("DatetimeoffsetNotNull", typeof(DateTimeOffsetSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeoffsetNull_DateTimeOffset_SetTest()
	{
		await SetTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DecimalNotNull_Decimal_SetTest()
	{
		await SetTest<decimal>("DecimalNotNull", typeof(DecimalSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DecimalNull_Decimal_SetTest()
	{
		await SetTest<decimal>("DecimalNull", typeof(DecimalSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task FloatNotNull_Double_SetTest()
	{
		await SetTest<double>("FloatNotNull", typeof(DoubleSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task FloatNull_Double_SetTest()
	{
		await SetTest<double>("FloatNull", typeof(DoubleSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task IntNotNull_Int32_SetTest()
	{
		await SetTest<int>("IntNotNull", typeof(Int32SetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task IntNull_Int32_SetTest()
	{
		await SetTest<int>("IntNull", typeof(Int32SetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task MoneyNotNull_Decimal_SetTest()
	{
		await SetTest<decimal>("MoneyNotNull", typeof(DecimalSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task MoneyNull_Decimal_SetTest()
	{
		await SetTest<decimal>("MoneyNull", typeof(DecimalSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NCharNotNull_String_SetTest()
	{
		await SetTest<string>("NCharNotNull", typeof(StringSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NCharNull_String_SetTest()
	{
		await SetTest<string>("NCharNull", typeof(StringSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NTextNotNull_String_SetTest()
	{
		await SetTest<string>("NTextNotNull", typeof(StringSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NTextNull_String_SetTest()
	{
		await SetTest<string>("NTextNull", typeof(StringSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NumericNotNull_Decimal_SetTest()
	{
		await SetTest<decimal>("NumericNotNull", typeof(DecimalSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NumericNull_Decimal_SetTest()
	{
		await SetTest<decimal>("NumericNull", typeof(DecimalSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NVarCharNotNull_String_SetTest()
	{
		await SetTest<string>("NVarCharNotNull", typeof(StringSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NVarCharNull_String_SetTest()
	{
		await SetTest<string>("NVarCharNull", typeof(StringSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task RealNotNull_Single_SetTest()
	{
		await SetTest<float>("RealNotNull", typeof(SingleSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task RealNull_Single_SetTest()
	{
		await SetTest<float>("RealNull", typeof(SingleSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmalldatetimeNotNull_DateTime_SetTest()
	{
		await SetTest<DateTime>("SmalldatetimeNotNull", typeof(DateTimeSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmalldatetimeNull_DateTime_SetTest()
	{
		await SetTest<DateTime>("SmalldatetimeNull", typeof(DateTimeSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallIntNotNull_Int16_SetTest()
	{
		await SetTest<short>("SmallIntNotNull", typeof(Int16SetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallIntNull_Int16_SetTest()
	{
		await SetTest<short>("SmallIntNull", typeof(Int16SetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallMoneyNotNull_Decimal_SetTest()
	{
		await SetTest<decimal>("SmallMoneyNotNull", typeof(DecimalSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallMoneyNull_Decimal_SetTest()
	{
		await SetTest<decimal>("SmallMoneyNull", typeof(DecimalSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TextNotNull_String_SetTest()
	{
		await SetTest<string>("TextNotNull", typeof(StringSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TextNull_String_SetTest()
	{
		await SetTest<string>("TextNull", typeof(StringSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TimeNotNull_TimeSpan_SetTest()
	{
		await SetTest<TimeSpan>("TimeNotNull", typeof(TimeSpanSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TimeNull_TimeSpan_SetTest()
	{
		await SetTest<TimeSpan>("TimeNull", typeof(TimeSpanSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TinyIntNotNull_Byte_SetTest()
	{
		await SetTest<byte>("TinyIntNotNull", typeof(ByteSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TinyIntNull_Byte_SetTest()
	{
		await SetTest<byte>("TinyIntNull", typeof(ByteSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task UniqueidentifierNotNull_Guid_SetTest()
	{
		await SetTest<Guid>("UniqueidentifierNotNull", typeof(GuidSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task UniqueidentifierNull_Guid_SetTest()
	{
		await SetTest<Guid>("UniqueidentifierNull", typeof(GuidSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task VarCharNotNull_String_SetTest()
	{
		await SetTest<string>("VarCharNotNull", typeof(StringSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task VarCharNull_String_SetTest()
	{
		await SetTest<string>("VarCharNull", typeof(StringSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task XmlNotNull_String_SetTest()
	{
		await SetTest<string>("XmlNotNull", typeof(StringSetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task XmlNull_String_SetTest()
	{
		await SetTest<string>("XmlNull", typeof(StringSetMaterializer<DbCommand, DbParameter>));
	}

	async Task SetTest<TResult>(string columnName, Type materializerType)
	{
		var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (1, 2, 4)");
		var materializer1 = (ILink<HashSet<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.None });
		var result1 = materializer1.Execute();
		var result1a = await materializer1.ExecuteAsync();
		Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
	}
}
