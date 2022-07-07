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
		await ScalarOrNullTest<long?>("BigIntNull", typeof(Int64OrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BinaryNull_ByteArray_ScalarOrNullTest()
	{
		await ScalarOrNullTest<byte[]>("BinaryNull", typeof(ByteArrayMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task BitNull_Boolean_ScalarOrNullTest()
	{
		await ScalarOrNullTest<bool?>("BitNull", typeof(BooleanOrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNull_Char_ScalarOrNullTest()
	{
		await ScalarOrNullTest<char>("CharNull", typeof(CharMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task CharNull_String_ScalarOrNullTest()
	{
		await ScalarOrNullTest<string>("CharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DateNull_DateTime_ScalarOrNullTest()
	{
		await ScalarOrNullTest<DateTime?>("DateNull", typeof(DateTimeOrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task Datetime2Null_DateTime_ScalarOrNullTest()
	{
		await ScalarOrNullTest<DateTime?>("Datetime2Null", typeof(DateTimeOrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeNull_DateTime_ScalarOrNullTest()
	{
		await ScalarOrNullTest<DateTime?>("DatetimeNull", typeof(DateTimeOrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DatetimeoffsetNull_DateTimeOffset_ScalarOrNullTest()
	{
		await ScalarOrNullTest<DateTimeOffset?>("DatetimeoffsetNull", typeof(DateTimeOffsetOrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task DecimalNull_Decimal_ScalarOrNullTest()
	{
		await ScalarOrNullTest<decimal?>("DecimalNull", typeof(DecimalOrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task FloatNull_Double_ScalarOrNullTest()
	{
		await ScalarOrNullTest<double?>("FloatNull", typeof(DoubleOrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task IntNull_Int32_ScalarOrNullTest()
	{
		await ScalarOrNullTest<int?>("IntNull", typeof(Int32OrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task MoneyNull_Decimal_ScalarOrNullTest()
	{
		await ScalarOrNullTest<decimal?>("MoneyNull", typeof(DecimalOrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NCharNull_String_ScalarOrNullTest()
	{
		await ScalarOrNullTest<string>("NCharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NTextNull_String_ScalarOrNullTest()
	{
		await ScalarOrNullTest<string>("NTextNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NumericNull_Decimal_ScalarOrNullTest()
	{
		await ScalarOrNullTest<decimal?>("NumericNull", typeof(DecimalOrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task NVarCharNull_String_ScalarOrNullTest()
	{
		await ScalarOrNullTest<string>("NVarCharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task RealNull_Single_ScalarOrNullTest()
	{
		await ScalarOrNullTest<float?>("RealNull", typeof(SingleOrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmalldatetimeNull_DateTime_ScalarOrNullTest()
	{
		await ScalarOrNullTest<DateTime?>("SmalldatetimeNull", typeof(DateTimeOrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallIntNull_Int16_ScalarOrNullTest()
	{
		await ScalarOrNullTest<short?>("SmallIntNull", typeof(Int16OrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task SmallMoneyNull_Decimal_ScalarOrNullTest()
	{
		await ScalarOrNullTest<decimal?>("SmallMoneyNull", typeof(DecimalOrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TextNull_String_ScalarOrNullTest()
	{
		await ScalarOrNullTest<string>("TextNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TimeNull_TimeSpan_ScalarOrNullTest()
	{
		await ScalarOrNullTest<TimeSpan?>("TimeNull", typeof(TimeSpanOrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task TinyIntNull_Byte_ScalarOrNullTest()
	{
		await ScalarOrNullTest<byte?>("TinyIntNull", typeof(ByteOrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task UniqueidentifierNull_Guid_ScalarOrNullTest()
	{
		await ScalarOrNullTest<Guid?>("UniqueidentifierNull", typeof(GuidOrNullMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task VarBinaryNull_ByteArray_ScalarOrNullTest()
	{
		await ScalarOrNullTest<byte[]>("VarBinaryNull", typeof(ByteArrayMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task VarCharNull_String_ScalarOrNullTest()
	{
		await ScalarOrNullTest<string>("VarCharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	[TestMethod]
	public async Task XmlNull_String_ScalarOrNullTest()
	{
		await ScalarOrNullTest<string>("XmlNull", typeof(StringMaterializer<DbCommand, DbParameter>));
	}

	async Task ScalarOrNullTest<TResult>(string columnName, Type materializerType)
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
		else
		{
			Assert.AreEqual(result1, result1a, "Results don't match");
		}

		var materializer3 = (ILink<TResult>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName });
		var result3 = materializer3.Execute();
		var result3a = await materializer3.ExecuteAsync();
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
