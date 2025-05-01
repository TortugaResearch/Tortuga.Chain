#nullable disable

using System.Data.Common;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Core.Tests;

[TestClass]
public class ScalarOrNullTests : MaterializerTestBase
{
	[TestMethod]
	public async Task BigIntNull_Int64_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<long?>("BigIntNull", typeof(Int64OrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BinaryNull_ByteArray_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<byte[]>("BinaryNull", typeof(ByteArrayMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task BitNull_Boolean_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<bool?>("BitNull", typeof(BooleanOrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNull_Char_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<char>("CharNull", typeof(CharMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task CharNull_String_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<string>("CharNull", typeof(StringMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DateNull_DateTime_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<DateTime?>("DateNull", typeof(DateTimeOrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task Datetime2Null_DateTime_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<DateTime?>("Datetime2Null", typeof(DateTimeOrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeNull_DateTime_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<DateTime?>("DatetimeNull", typeof(DateTimeOrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DatetimeoffsetNull_DateTimeOffset_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<DateTimeOffset?>("DatetimeoffsetNull", typeof(DateTimeOffsetOrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task DecimalNull_Decimal_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<decimal?>("DecimalNull", typeof(DecimalOrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task FloatNull_Double_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<double?>("FloatNull", typeof(DoubleOrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task IntNull_Int32_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<int?>("IntNull", typeof(Int32OrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task MoneyNull_Decimal_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<decimal?>("MoneyNull", typeof(DecimalOrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NCharNull_String_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<string>("NCharNull", typeof(StringMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NTextNull_String_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<string>("NTextNull", typeof(StringMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NumericNull_Decimal_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<decimal?>("NumericNull", typeof(DecimalOrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task NVarCharNull_String_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<string>("NVarCharNull", typeof(StringMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task RealNull_Single_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<float?>("RealNull", typeof(SingleOrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmalldatetimeNull_DateTime_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<DateTime?>("SmalldatetimeNull", typeof(DateTimeOrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallIntNull_Int16_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<short?>("SmallIntNull", typeof(Int16OrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task SmallMoneyNull_Decimal_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<decimal?>("SmallMoneyNull", typeof(DecimalOrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TextNull_String_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<string>("TextNull", typeof(StringMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TimeNull_TimeSpan_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<TimeSpan?>("TimeNull", typeof(TimeSpanOrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task TinyIntNull_Byte_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<byte?>("TinyIntNull", typeof(ByteOrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task UniqueidentifierNull_Guid_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<Guid?>("UniqueidentifierNull", typeof(GuidOrNullMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task VarBinaryNull_ByteArray_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<byte[]>("VarBinaryNull", typeof(ByteArrayMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task VarCharNull_String_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<string>("VarCharNull", typeof(StringMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	[TestMethod]
	public async Task XmlNull_String_ScalarOrNullTest()
	{
		await ScalarOrNullTestAsync<string>("XmlNull", typeof(StringMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
	}

	async Task ScalarOrNullTestAsync<TResult>(string columnName, Type materializerType)
	{
		var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id = 1");
		var materializer1 = (ILink<TResult>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName });
		var result1 = materializer1.Execute();
		var result1a = await materializer1.ExecuteAsync().ConfigureAwait(false);
		if (typeof(TResult) == typeof(byte[]))
		{
			var result1Cast = (byte[])(object)result1;
			var result1aCast = (byte[])(object)result1a;
			Assert.AreEqual(result1Cast.Length, result1aCast.Length);
		}
		else
		{
			Assert.AreEqual(result1, result1a, "Results don't match");
		}

		var materializer3 = (ILink<TResult>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName });
		var result3 = materializer3.Execute();
		var result3a = await materializer3.ExecuteAsync().ConfigureAwait(false);
		if (typeof(TResult) == typeof(byte[]))
		{
			var result3Cast = (byte[])(object)result3;
			var result3aCast = (byte[])(object)result3a;
			Assert.AreEqual(result3Cast.Length, result3aCast.Length);
		}
		else
		{
			Assert.AreEqual(result3, result3a, "Results don't match");
		}
	}
}
