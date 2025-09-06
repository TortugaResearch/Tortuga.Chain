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
		await ScalarFailOnNullTestAsync<long>("BigIntNull", typeof(Int64Materializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BitNull_Boolean_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<bool>("BitNull", typeof(BooleanMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNull_Char_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<char>("CharNull", typeof(CharMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNull_String_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<string>("CharNull", typeof(StringMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharOneNull_Char_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<char>("CharOneNull", typeof(CharMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharOneNull_String_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<string>("CharOneNull", typeof(StringMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DateNull_DateOnly_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<DateOnly>("DateNull", typeof(DateOnlyMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DateNull_DateTime_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<DateTime>("DateNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Datetime2Null_DateOnly_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<DateOnly>("Datetime2Null", typeof(DateOnlyMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Datetime2Null_DateTime_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<DateTime>("Datetime2Null", typeof(DateTimeMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Datetime2Null_TimeOnly_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<TimeOnly>("Datetime2Null", typeof(TimeOnlyMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeNull_DateOnly_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<DateOnly>("DatetimeNull", typeof(DateOnlyMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeNull_DateTime_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<DateTime>("DatetimeNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeNull_TimeOnly_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<TimeOnly>("DatetimeNull", typeof(TimeOnlyMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeoffsetNull_DateTimeOffset_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DecimalNull_Decimal_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<decimal>("DecimalNull", typeof(DecimalMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task FloatNull_Double_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<double>("FloatNull", typeof(DoubleMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task IntNull_Int32_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<int>("IntNull", typeof(Int32Materializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task MoneyNull_Decimal_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<decimal>("MoneyNull", typeof(DecimalMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NumericNull_Decimal_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<decimal>("NumericNull", typeof(DecimalMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task RealNull_Single_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<float>("RealNull", typeof(SingleMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmalldatetimeNull_DateOnly_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<DateOnly>("SmalldatetimeNull", typeof(DateOnlyMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmalldatetimeNull_DateTime_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<DateTime>("SmalldatetimeNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmalldatetimeNull_TimeOnly_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<TimeOnly>("SmalldatetimeNull", typeof(TimeOnlyMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallIntNull_Int16_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<short>("SmallIntNull", typeof(Int16Materializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallMoneyNull_Decimal_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<decimal>("SmallMoneyNull", typeof(DecimalMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TimeNull_TimeOnly_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<TimeOnly>("TimeNull", typeof(TimeOnlyMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TimeNull_TimeSpan_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<TimeSpan>("TimeNull", typeof(TimeSpanMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TinyIntNull_Byte_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<byte>("TinyIntNull", typeof(ByteMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task UniqueidentifierNull_Guid_ScalarFailOnNullTest()
	{
		await ScalarFailOnNullTestAsync<Guid>("UniqueidentifierNull", typeof(GuidMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	async Task ScalarFailOnNullTestAsync<TResult>(string columnName, Type materializerType)
	{
		var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (3)");

		var materializer1 = (ILink<TResult>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName });
		try
		{
			materializer1.Execute();
			Assert.Fail("Exception expected");
		}
		catch (MissingDataException)
		{
			//Expected
		}
		try
		{
			await materializer1.ExecuteAsync().ConfigureAwait(false);
			Assert.Fail("Exception expected");
		}
		catch (MissingDataException)
		{
			//Expected
		}
	}
}
