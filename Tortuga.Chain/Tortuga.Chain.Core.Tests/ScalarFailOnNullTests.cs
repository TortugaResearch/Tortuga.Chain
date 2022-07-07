#nullable disable

using System.Data.Common;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Core.Tests;

[TestClass]
public class ScalarFailOnNullTests : MaterializerTestBase
{
	[TestMethod]
	public async Task BigIntNull_Int64_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<long>("BigIntNull", typeof(Int64Materializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BitNull_Boolean_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<bool>("BitNull", typeof(BooleanMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNull_Char_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<char>("CharNull", typeof(CharMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNull_String_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<string>("CharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharOneNull_Char_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<char>("CharOneNull", typeof(CharMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharOneNull_String_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<string>("CharOneNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DateNull_DateTime_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<DateTime>("DateNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task Datetime2Null_DateTime_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<DateTime>("Datetime2Null", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeNull_DateTime_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<DateTime>("DatetimeNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeoffsetNull_DateTimeOffset_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DecimalNull_Decimal_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<decimal>("DecimalNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task FloatNull_Double_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<double>("FloatNull", typeof(DoubleMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task IntNull_Int32_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<int>("IntNull", typeof(Int32Materializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task MoneyNull_Decimal_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<decimal>("MoneyNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NumericNull_Decimal_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<decimal>("NumericNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task RealNull_Single_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<float>("RealNull", typeof(SingleMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmalldatetimeNull_DateTime_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<DateTime>("SmalldatetimeNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallIntNull_Int16_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<short>("SmallIntNull", typeof(Int16Materializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallMoneyNull_Decimal_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<decimal>("SmallMoneyNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TimeNull_TimeSpan_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<TimeSpan>("TimeNull", typeof(TimeSpanMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TinyIntNull_Byte_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<byte>("TinyIntNull", typeof(ByteMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task UniqueidentifierNull_Guid_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTest<Guid>("UniqueidentifierNull", typeof(GuidMaterializer<DbCommand, DbParameter>));
	}

	async Task ScalarFailOnNullTest<TResult>(string columnName, Type materializerType)
	{
		var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (3)");

		var materializer1 = (ILink<TResult>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName });
		try
		{
			var result1 = materializer1.Execute();
			Assert.Fail("Exception expected");
		}
		catch (MissingDataException)
		{
			//Expected
		}
		try
		{
			var result1a = await materializer1.ExecuteAsync();
			Assert.Fail("Exception expected");
		}
		catch (MissingDataException)
		{
			//Expected
		}
	}
}
