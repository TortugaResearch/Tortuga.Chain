using System.Data.Common;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Core.Tests
{
	partial class GenericDbDataSource3_MaterializerTests
	{
		[TestMethod]
		public async Task BigIntNotNull_Int64_ListOrNullTest()
		{
			await ListOrNullTest<long?>("BigIntNotNull", typeof(Int64OrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task BigIntNull_Int64_ListOrNullTest()
		{
			await ListOrNullTest<long?>("BigIntNull", typeof(Int64OrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task BinaryNotNull_ByteArray_ListOrNullTest()
		{
			await ListOrNullTest<byte[]?>("BinaryNotNull", typeof(ByteArrayOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task BinaryNull_ByteArray_ListOrNullTest()
		{
			await ListOrNullTest<byte[]?>("BinaryNull", typeof(ByteArrayOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task BitNotNull_Boolean_ListOrNullTest()
		{
			await ListOrNullTest<bool?>("BitNotNull", typeof(BooleanOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task BitNull_Boolean_ListOrNullTest()
		{
			await ListOrNullTest<bool?>("BitNull", typeof(BooleanOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task CharNotNull_String_ListOrNullTest()
		{
			await ListOrNullTest<string?>("CharNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task CharNull_String_ListOrNullTest()
		{
			await ListOrNullTest<string?>("CharNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DateNotNull_DateTime_ListOrNullTest()
		{
			await ListOrNullTest<DateTime?>("DateNotNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DateNull_DateTime_ListOrNullTest()
		{
			await ListOrNullTest<DateTime?>("DateNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task Datetime2NotNull_DateTime_ListOrNullTest()
		{
			await ListOrNullTest<DateTime?>("Datetime2NotNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task Datetime2Null_DateTime_ListOrNullTest()
		{
			await ListOrNullTest<DateTime?>("Datetime2Null", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DatetimeNotNull_DateTime_ListOrNullTest()
		{
			await ListOrNullTest<DateTime?>("DatetimeNotNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DatetimeNull_DateTime_ListOrNullTest()
		{
			await ListOrNullTest<DateTime?>("DatetimeNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DatetimeoffsetNotNull_DateTimeOffset_ListOrNullTest()
		{
			await ListOrNullTest<DateTimeOffset?>("DatetimeoffsetNotNull", typeof(DateTimeOffsetOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DatetimeoffsetNull_DateTimeOffset_ListOrNullTest()
		{
			await ListOrNullTest<DateTimeOffset?>("DatetimeoffsetNull", typeof(DateTimeOffsetOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DecimalNotNull_Decimal_ListOrNullTest()
		{
			await ListOrNullTest<decimal?>("DecimalNotNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DecimalNull_Decimal_ListOrNullTest()
		{
			await ListOrNullTest<decimal?>("DecimalNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task FloatNotNull_Double_ListOrNullTest()
		{
			await ListOrNullTest<double?>("FloatNotNull", typeof(DoubleOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task FloatNull_Double_ListOrNullTest()
		{
			await ListOrNullTest<double?>("FloatNull", typeof(DoubleOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task IntNotNull_Int32_ListOrNullTest()
		{
			await ListOrNullTest<int?>("IntNotNull", typeof(Int32OrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task IntNull_Int32_ListOrNullTest()
		{
			await ListOrNullTest<int?>("IntNull", typeof(Int32OrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task MoneyNotNull_Decimal_ListOrNullTest()
		{
			await ListOrNullTest<decimal?>("MoneyNotNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task MoneyNull_Decimal_ListOrNullTest()
		{
			await ListOrNullTest<decimal?>("MoneyNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NCharNotNull_String_ListOrNullTest()
		{
			await ListOrNullTest<string?>("NCharNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NCharNull_String_ListOrNullTest()
		{
			await ListOrNullTest<string?>("NCharNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NTextNotNull_String_ListOrNullTest()
		{
			await ListOrNullTest<string?>("NTextNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NTextNull_String_ListOrNullTest()
		{
			await ListOrNullTest<string?>("NTextNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NumericNotNull_Decimal_ListOrNullTest()
		{
			await ListOrNullTest<decimal?>("NumericNotNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NumericNull_Decimal_ListOrNullTest()
		{
			await ListOrNullTest<decimal?>("NumericNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NVarCharNotNull_String_ListOrNullTest()
		{
			await ListOrNullTest<string?>("NVarCharNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NVarCharNull_String_ListOrNullTest()
		{
			await ListOrNullTest<string?>("NVarCharNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task RealNotNull_Single_ListOrNullTest()
		{
			await ListOrNullTest<float?>("RealNotNull", typeof(SingleOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task RealNull_Single_ListOrNullTest()
		{
			await ListOrNullTest<float?>("RealNull", typeof(SingleOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task SmalldatetimeNotNull_DateTime_ListOrNullTest()
		{
			await ListOrNullTest<DateTime?>("SmalldatetimeNotNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task SmalldatetimeNull_DateTime_ListOrNullTest()
		{
			await ListOrNullTest<DateTime?>("SmalldatetimeNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task SmallIntNull_Int16_ListOrNullTest()
		{
			await ListOrNullTest<short?>("SmallIntNull", typeof(Int16OrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task SmallMoneyNotNull_Decimal_ListOrNullTest()
		{
			await ListOrNullTest<decimal?>("SmallMoneyNotNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task SmallMoneyNull_Decimal_ListOrNullTest()
		{
			await ListOrNullTest<decimal?>("SmallMoneyNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task TextNotNull_String_ListOrNullTest()
		{
			await ListOrNullTest<string?>("TextNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task TextNull_String_ListOrNullTest()
		{
			await ListOrNullTest<string?>("TextNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task TimeNotNull_TimeSpan_ListOrNullTest()
		{
			await ListOrNullTest<TimeSpan?>("TimeNotNull", typeof(TimeSpanOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task TimeNull_TimeSpan_ListOrNullTest()
		{
			await ListOrNullTest<TimeSpan?>("TimeNull", typeof(TimeSpanOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task TinyIntNotNull_Byte_ListOrNullTest()
		{
			await ListOrNullTest<byte?>("TinyIntNotNull", typeof(ByteOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task TinyIntNull_Byte_ListOrNullTest()
		{
			await ListOrNullTest<byte?>("TinyIntNull", typeof(ByteOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task UniqueidentifierNotNull_Guid_ListOrNullTest()
		{
			await ListOrNullTest<Guid?>("UniqueidentifierNotNull", typeof(GuidOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task UniqueidentifierNull_Guid_ListOrNullTest()
		{
			await ListOrNullTest<Guid?>("UniqueidentifierNull", typeof(GuidOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task VarBinaryNotNull_ByteArray_ListOrNullTest()
		{
			await ListOrNullTest<byte[]?>("VarBinaryNotNull", typeof(ByteArrayOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task VarBinaryNull_ByteArray_ListOrNullTest()
		{
			await ListOrNullTest<byte[]?>("VarBinaryNull", typeof(ByteArrayOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task VarCharNotNull_String_ListOrNullTest()
		{
			await ListOrNullTest<string?>("VarCharNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		//*****************************
		[TestMethod]
		public async Task VarCharNull_String_ListOrNullTest()
		{
			await ListOrNullTest<string?>("VarCharNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task XmlNotNull_String_ListOrNullTest()
		{
			await ListOrNullTest<string?>("XmlNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task XmlNull_String_ListOrNullTest()
		{
			await ListOrNullTest<string?>("XmlNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		async Task ListOrNullTest<TResult>(string columnName, Type materializerType)
		{
			var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (1, 2, 3, 4)");
			ILink<List<TResult>> materializer1 = (ILink<List<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.None })!;
			var result1 = materializer1.Execute();
			var result1a = await materializer1.ExecuteAsync();
			Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
			Assert.IsTrue(result1.Any(x => x != null));
		}
	}
}
