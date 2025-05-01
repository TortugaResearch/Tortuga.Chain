using System.Data.Common;
using Tortuga.Chain.Materializers;

#nullable disable

namespace Tortuga.Chain.Core.Tests;

[TestClass]
public class ListDiscardNullTests : MaterializerTestBase
{
	[TestMethod]
	public async Task BigIntNotNull_Int64_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<long>("BigIntNotNull", typeof(Int64ListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BigIntNull_Int64_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<long>("BigIntNull", typeof(Int64ListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BinaryNotNull_ByteArray_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<byte[]>("BinaryNotNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BinaryNull_ByteArray_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<byte[]>("BinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BitNull_Boolean_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<bool>("BitNull", typeof(BooleanListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNotNull_Char_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<char>("CharNotNull", typeof(CharListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNotNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<string>("CharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNull_Char_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<char>("CharNull", typeof(CharListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<string>("CharNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharOneNotNull_Char_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<char>("CharOneNotNull", typeof(CharListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharOneNull_Char_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<char>("CharOneNull", typeof(CharListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DateNotNull_DateTime_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<DateTime>("DateNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DateNull_DateTime_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<DateTime>("DateNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Datetime2NotNull_DateTime_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<DateTime>("Datetime2NotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Datetime2Null_DateTime_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<DateTime>("Datetime2Null", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeNotNull_DateTime_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<DateTime>("DatetimeNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeNull_DateTime_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<DateTime>("DatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeoffsetNotNull_DateTimeOffset_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<DateTimeOffset>("DatetimeoffsetNotNull", typeof(DateTimeOffsetListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeoffsetNull_DateTimeOffset_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DecimalNotNull_Decimal_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<decimal>("DecimalNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DecimalNull_Decimal_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<decimal>("DecimalNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task FloatNotNull_Double_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<double>("FloatNotNull", typeof(DoubleListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task FloatNull_Double_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<double>("FloatNull", typeof(DoubleListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task IntNotNull_Int32_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<int>("IntNotNull", typeof(Int32ListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task IntNull_Int32_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<int>("IntNull", typeof(Int32ListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task MoneyNotNull_Decimal_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<decimal>("MoneyNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task MoneyNull_Decimal_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<decimal>("MoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NCharNotNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<string>("NCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NCharNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<string>("NCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NTextNotNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<string>("NTextNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NTextNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<string>("NTextNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NumericNotNull_Decimal_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<decimal>("NumericNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NumericNull_Decimal_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<decimal>("NumericNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NVarCharNotNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<string>("NVarCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NVarCharNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<string>("NVarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task RealNotNull_Single_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<float>("RealNotNull", typeof(SingleListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task RealNull_Single_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<float>("RealNull", typeof(SingleListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmalldatetimeNotNull_DateTime_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<DateTime>("SmalldatetimeNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmalldatetimeNull_DateTime_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<DateTime>("SmalldatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallIntNotNull_Int16_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<short>("SmallIntNotNull", typeof(Int16ListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallIntNull_Int16_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<short>("SmallIntNull", typeof(Int16ListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallMoneyNotNull_Decimal_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<decimal>("SmallMoneyNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallMoneyNull_Decimal_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<decimal>("SmallMoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TextNotNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<string>("TextNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TextNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<string>("TextNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TimeNotNull_TimeSpan_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<TimeSpan>("TimeNotNull", typeof(TimeSpanListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TimeNull_TimeSpan_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<TimeSpan>("TimeNull", typeof(TimeSpanListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TinyIntNotNull_Byte_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<byte>("TinyIntNotNull", typeof(ByteListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TinyIntNull_Byte_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<byte>("TinyIntNull", typeof(ByteListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task UniqueidentifierNotNull_Guid_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<Guid>("UniqueidentifierNotNull", typeof(GuidListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task UniqueidentifierNull_Guid_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<Guid>("UniqueidentifierNull", typeof(GuidListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task VarBinaryNotNull_ByteArray_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<byte[]>("VarBinaryNotNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task VarBinaryNull_ByteArray_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<byte[]>("VarBinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task VarCharNotNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<string>("VarCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task VarCharNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<string>("VarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task XmlNotNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<string>("XmlNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task XmlNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTestAsync<string>("XmlNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	async Task ListDiscardNullTestAsync<TResult>(string columnName, Type materializerType)
	{
		var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (1, 2, 3, 4)");
		var materializer1 = (ILink<List<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.DiscardNulls });
		var result1 = materializer1.Execute();
		var result1a = await materializer1.ExecuteAsync().ConfigureAwait(false);
		Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
		Assert.IsTrue(result1.All(x => x != null));
	}
}
