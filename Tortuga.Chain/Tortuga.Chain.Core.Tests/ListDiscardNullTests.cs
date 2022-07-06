using System.Data.Common;
using Tortuga.Chain.Materializers;

#nullable disable

namespace Tortuga.Chain.Core.Tests;

[TestClass]
public class ListDiscardNullTests : GenericDbDataSource3_MaterializerTests
{
	[TestMethod]
	public async Task BigIntNotNull_Int64_ListDiscardNullTest()
	{
		await ListDiscardNullTest<long>("BigIntNotNull", typeof(Int64ListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BigIntNull_Int64_ListDiscardNullTest()
	{
		await ListDiscardNullTest<long>("BigIntNull", typeof(Int64ListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BinaryNotNull_ByteArray_ListDiscardNullTest()
	{
		await ListDiscardNullTest<byte[]>("BinaryNotNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BinaryNull_ByteArray_ListDiscardNullTest()
	{
		await ListDiscardNullTest<byte[]>("BinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BitNull_Boolean_ListDiscardNullTest()
	{
		await ListDiscardNullTest<bool>("BitNull", typeof(BooleanListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNotNull_Char_ListDiscardNullTest()
	{
		await ListDiscardNullTest<char>("CharNotNull", typeof(CharListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNotNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTest<string>("CharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNull_Char_ListDiscardNullTest()
	{
		await ListDiscardNullTest<char>("CharNull", typeof(CharListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTest<string>("CharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharOneNotNull_Char_ListDiscardNullTest()
	{
		await ListDiscardNullTest<char>("CharOneNotNull", typeof(CharListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharOneNull_Char_ListDiscardNullTest()
	{
		await ListDiscardNullTest<char>("CharOneNull", typeof(CharListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DateNotNull_DateTime_ListDiscardNullTest()
	{
		await ListDiscardNullTest<DateTime>("DateNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DateNull_DateTime_ListDiscardNullTest()
	{
		await ListDiscardNullTest<DateTime>("DateNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task Datetime2NotNull_DateTime_ListDiscardNullTest()
	{
		await ListDiscardNullTest<DateTime>("Datetime2NotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task Datetime2Null_DateTime_ListDiscardNullTest()
	{
		await ListDiscardNullTest<DateTime>("Datetime2Null", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeNotNull_DateTime_ListDiscardNullTest()
	{
		await ListDiscardNullTest<DateTime>("DatetimeNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeNull_DateTime_ListDiscardNullTest()
	{
		await ListDiscardNullTest<DateTime>("DatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeoffsetNotNull_DateTimeOffset_ListDiscardNullTest()
	{
		await ListDiscardNullTest<DateTimeOffset>("DatetimeoffsetNotNull", typeof(DateTimeOffsetListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeoffsetNull_DateTimeOffset_ListDiscardNullTest()
	{
		await ListDiscardNullTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DecimalNotNull_Decimal_ListDiscardNullTest()
	{
		await ListDiscardNullTest<decimal>("DecimalNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DecimalNull_Decimal_ListDiscardNullTest()
	{
		await ListDiscardNullTest<decimal>("DecimalNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task FloatNotNull_Double_ListDiscardNullTest()
	{
		await ListDiscardNullTest<double>("FloatNotNull", typeof(DoubleListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task FloatNull_Double_ListDiscardNullTest()
	{
		await ListDiscardNullTest<double>("FloatNull", typeof(DoubleListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task IntNotNull_Int32_ListDiscardNullTest()
	{
		await ListDiscardNullTest<int>("IntNotNull", typeof(Int32ListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task IntNull_Int32_ListDiscardNullTest()
	{
		await ListDiscardNullTest<int>("IntNull", typeof(Int32ListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task MoneyNotNull_Decimal_ListDiscardNullTest()
	{
		await ListDiscardNullTest<decimal>("MoneyNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task MoneyNull_Decimal_ListDiscardNullTest()
	{
		await ListDiscardNullTest<decimal>("MoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NCharNotNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTest<string>("NCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NCharNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTest<string>("NCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NTextNotNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTest<string>("NTextNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NTextNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTest<string>("NTextNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NumericNotNull_Decimal_ListDiscardNullTest()
	{
		await ListDiscardNullTest<decimal>("NumericNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NumericNull_Decimal_ListDiscardNullTest()
	{
		await ListDiscardNullTest<decimal>("NumericNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NVarCharNotNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTest<string>("NVarCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NVarCharNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTest<string>("NVarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task RealNotNull_Single_ListDiscardNullTest()
	{
		await ListDiscardNullTest<float>("RealNotNull", typeof(SingleListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task RealNull_Single_ListDiscardNullTest()
	{
		await ListDiscardNullTest<float>("RealNull", typeof(SingleListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmalldatetimeNotNull_DateTime_ListDiscardNullTest()
	{
		await ListDiscardNullTest<DateTime>("SmalldatetimeNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmalldatetimeNull_DateTime_ListDiscardNullTest()
	{
		await ListDiscardNullTest<DateTime>("SmalldatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallIntNotNull_Int16_ListDiscardNullTest()
	{
		await ListDiscardNullTest<short>("SmallIntNotNull", typeof(Int16ListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallIntNull_Int16_ListDiscardNullTest()
	{
		await ListDiscardNullTest<short>("SmallIntNull", typeof(Int16ListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallMoneyNotNull_Decimal_ListDiscardNullTest()
	{
		await ListDiscardNullTest<decimal>("SmallMoneyNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallMoneyNull_Decimal_ListDiscardNullTest()
	{
		await ListDiscardNullTest<decimal>("SmallMoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TextNotNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTest<string>("TextNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TextNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTest<string>("TextNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TimeNotNull_TimeSpan_ListDiscardNullTest()
	{
		await ListDiscardNullTest<TimeSpan>("TimeNotNull", typeof(TimeSpanListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TimeNull_TimeSpan_ListDiscardNullTest()
	{
		await ListDiscardNullTest<TimeSpan>("TimeNull", typeof(TimeSpanListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TinyIntNotNull_Byte_ListDiscardNullTest()
	{
		await ListDiscardNullTest<byte>("TinyIntNotNull", typeof(ByteListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TinyIntNull_Byte_ListDiscardNullTest()
	{
		await ListDiscardNullTest<byte>("TinyIntNull", typeof(ByteListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task UniqueidentifierNotNull_Guid_ListDiscardNullTest()
	{
		await ListDiscardNullTest<Guid>("UniqueidentifierNotNull", typeof(GuidListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task UniqueidentifierNull_Guid_ListDiscardNullTest()
	{
		await ListDiscardNullTest<Guid>("UniqueidentifierNull", typeof(GuidListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task VarBinaryNotNull_ByteArray_ListDiscardNullTest()
	{
		await ListDiscardNullTest<byte[]>("VarBinaryNotNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task VarBinaryNull_ByteArray_ListDiscardNullTest()
	{
		await ListDiscardNullTest<byte[]>("VarBinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task VarCharNotNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTest<string>("VarCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task VarCharNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTest<string>("VarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task XmlNotNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTest<string>("XmlNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task XmlNull_String_ListDiscardNullTest()
	{
		await ListDiscardNullTest<string>("XmlNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
	}

	async Task ListDiscardNullTest<TResult>(string columnName, Type materializerType)
	{
		var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (1, 2, 3, 4)");
		ILink<List<TResult>> materializer1 = (ILink<List<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.DiscardNulls });
		var result1 = materializer1.Execute();
		var result1a = await materializer1.ExecuteAsync();
		Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
		Assert.IsTrue(result1.All(x => x != null));
	}
}
