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
		await SetTestAsync<long>("BigIntNotNull", typeof(Int64SetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BigIntNull_Int64_SetTest()
	{
		await SetTestAsync<long>("BigIntNull", typeof(Int64SetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNotNull_Char_SetTest()
	{
		await SetTestAsync<char>("CharNotNull", typeof(CharSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNotNull_String_SetTest()
	{
		await SetTestAsync<string>("CharNotNull", typeof(StringSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNull_Char_SetTest()
	{
		await SetTestAsync<char>("CharNull", typeof(CharSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNull_String_SetTest()
	{
		await SetTestAsync<string>("CharNull", typeof(StringSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharOneNotNull_Char_SetTest()
	{
		await SetTestAsync<char>("CharOneNotNull", typeof(CharSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharOneNull_Char_SetTest()
	{
		await SetTestAsync<char>("CharOneNull", typeof(CharSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DataTable()
	{
		var materializer = DataSource.Sql($"SELECT * FROM dbo.AllTypes").ToDataTable();
		var result1 = materializer.Execute();
		var result1a = await materializer.ExecuteAsync().ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DateNotNull_DateTime_SetTest()
	{
		await SetTestAsync<DateTime>("DateNotNull", typeof(DateTimeSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DateNull_DateTime_SetTest()
	{
		await SetTestAsync<DateTime>("DateNull", typeof(DateTimeSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Datetime2NotNull_DateTime_SetTest()
	{
		await SetTestAsync<DateTime>("Datetime2NotNull", typeof(DateTimeSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Datetime2Null_DateTime_SetTest()
	{
		await SetTestAsync<DateTime>("Datetime2Null", typeof(DateTimeSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeNotNull_DateTime_SetTest()
	{
		await SetTestAsync<DateTime>("DatetimeNotNull", typeof(DateTimeSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeNull_DateTime_SetTest()
	{
		await SetTestAsync<DateTime>("DatetimeNull", typeof(DateTimeSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeoffsetNotNull_DateTimeOffset_SetTest()
	{
		await SetTestAsync<DateTimeOffset>("DatetimeoffsetNotNull", typeof(DateTimeOffsetSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeoffsetNull_DateTimeOffset_SetTest()
	{
		await SetTestAsync<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DecimalNotNull_Decimal_SetTest()
	{
		await SetTestAsync<decimal>("DecimalNotNull", typeof(DecimalSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DecimalNull_Decimal_SetTest()
	{
		await SetTestAsync<decimal>("DecimalNull", typeof(DecimalSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task FloatNotNull_Double_SetTest()
	{
		await SetTestAsync<double>("FloatNotNull", typeof(DoubleSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task FloatNull_Double_SetTest()
	{
		await SetTestAsync<double>("FloatNull", typeof(DoubleSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task IntNotNull_Int32_SetTest()
	{
		await SetTestAsync<int>("IntNotNull", typeof(Int32SetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task IntNull_Int32_SetTest()
	{
		await SetTestAsync<int>("IntNull", typeof(Int32SetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task MoneyNotNull_Decimal_SetTest()
	{
		await SetTestAsync<decimal>("MoneyNotNull", typeof(DecimalSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task MoneyNull_Decimal_SetTest()
	{
		await SetTestAsync<decimal>("MoneyNull", typeof(DecimalSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NCharNotNull_String_SetTest()
	{
		await SetTestAsync<string>("NCharNotNull", typeof(StringSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NCharNull_String_SetTest()
	{
		await SetTestAsync<string>("NCharNull", typeof(StringSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NTextNotNull_String_SetTest()
	{
		await SetTestAsync<string>("NTextNotNull", typeof(StringSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NTextNull_String_SetTest()
	{
		await SetTestAsync<string>("NTextNull", typeof(StringSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NumericNotNull_Decimal_SetTest()
	{
		await SetTestAsync<decimal>("NumericNotNull", typeof(DecimalSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NumericNull_Decimal_SetTest()
	{
		await SetTestAsync<decimal>("NumericNull", typeof(DecimalSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NVarCharNotNull_String_SetTest()
	{
		await SetTestAsync<string>("NVarCharNotNull", typeof(StringSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NVarCharNull_String_SetTest()
	{
		await SetTestAsync<string>("NVarCharNull", typeof(StringSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task RealNotNull_Single_SetTest()
	{
		await SetTestAsync<float>("RealNotNull", typeof(SingleSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task RealNull_Single_SetTest()
	{
		await SetTestAsync<float>("RealNull", typeof(SingleSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmalldatetimeNotNull_DateTime_SetTest()
	{
		await SetTestAsync<DateTime>("SmalldatetimeNotNull", typeof(DateTimeSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmalldatetimeNull_DateTime_SetTest()
	{
		await SetTestAsync<DateTime>("SmalldatetimeNull", typeof(DateTimeSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallIntNotNull_Int16_SetTest()
	{
		await SetTestAsync<short>("SmallIntNotNull", typeof(Int16SetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallIntNull_Int16_SetTest()
	{
		await SetTestAsync<short>("SmallIntNull", typeof(Int16SetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallMoneyNotNull_Decimal_SetTest()
	{
		await SetTestAsync<decimal>("SmallMoneyNotNull", typeof(DecimalSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallMoneyNull_Decimal_SetTest()
	{
		await SetTestAsync<decimal>("SmallMoneyNull", typeof(DecimalSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TextNotNull_String_SetTest()
	{
		await SetTestAsync<string>("TextNotNull", typeof(StringSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TextNull_String_SetTest()
	{
		await SetTestAsync<string>("TextNull", typeof(StringSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TimeNotNull_TimeSpan_SetTest()
	{
		await SetTestAsync<TimeSpan>("TimeNotNull", typeof(TimeSpanSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TimeNull_TimeSpan_SetTest()
	{
		await SetTestAsync<TimeSpan>("TimeNull", typeof(TimeSpanSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TinyIntNotNull_Byte_SetTest()
	{
		await SetTestAsync<byte>("TinyIntNotNull", typeof(ByteSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TinyIntNull_Byte_SetTest()
	{
		await SetTestAsync<byte>("TinyIntNull", typeof(ByteSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task UniqueidentifierNotNull_Guid_SetTest()
	{
		await SetTestAsync<Guid>("UniqueidentifierNotNull", typeof(GuidSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task UniqueidentifierNull_Guid_SetTest()
	{
		await SetTestAsync<Guid>("UniqueidentifierNull", typeof(GuidSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task VarCharNotNull_String_SetTest()
	{
		await SetTestAsync<string>("VarCharNotNull", typeof(StringSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task VarCharNull_String_SetTest()
	{
		await SetTestAsync<string>("VarCharNull", typeof(StringSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task XmlNotNull_String_SetTest()
	{
		await SetTestAsync<string>("XmlNotNull", typeof(StringSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task XmlNull_String_SetTest()
	{
		await SetTestAsync<string>("XmlNull", typeof(StringSetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	async Task SetTestAsync<TResult>(string columnName, Type materializerType)
	{
		var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (1, 2, 4)");
		var materializer1 = (ILink<HashSet<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.None });
		var result1 = materializer1.Execute();
		var result1a = await materializer1.ExecuteAsync().ConfigureAwait(false);
		Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
	}
}
