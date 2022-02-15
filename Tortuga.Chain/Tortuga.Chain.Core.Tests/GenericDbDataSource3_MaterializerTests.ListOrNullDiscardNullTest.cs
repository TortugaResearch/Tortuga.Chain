using System.Data.Common;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Core.Tests
{
	partial class GenericDbDataSource3_MaterializerTests
	{
		[TestMethod]
		public async Task BigIntNotNull_Int64_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<long?>("BigIntNotNull", typeof(Int64OrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task BigIntNull_Int64_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<long?>("BigIntNull", typeof(Int64OrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task BinaryNotNull_ByteArray_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<byte[]?>("BinaryNotNull", typeof(ByteArrayOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task BinaryNull_ByteArray_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<byte[]?>("BinaryNull", typeof(ByteArrayOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task BitNotNull_Boolean_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<bool?>("BitNotNull", typeof(BooleanOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task BitNull_Boolean_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<bool?>("BitNull", typeof(BooleanOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task CharNotNull_String_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<string?>("CharNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task CharNull_String_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<string?>("CharNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DateNotNull_DateTime_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<DateTime?>("DateNotNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DateNull_DateTime_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<DateTime?>("DateNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task Datetime2NotNull_DateTime_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<DateTime?>("Datetime2NotNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task Datetime2Null_DateTime_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<DateTime?>("Datetime2Null", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DatetimeNotNull_DateTime_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<DateTime?>("DatetimeNotNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DatetimeNull_DateTime_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<DateTime?>("DatetimeNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DatetimeoffsetNotNull_DateTimeOffset_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<DateTimeOffset?>("DatetimeoffsetNotNull", typeof(DateTimeOffsetOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DatetimeoffsetNull_DateTimeOffset_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<DateTimeOffset?>("DatetimeoffsetNull", typeof(DateTimeOffsetOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DecimalNotNull_Decimal_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<decimal?>("DecimalNotNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DecimalNull_Decimal_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<decimal?>("DecimalNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task FloatNotNull_Double_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<double?>("FloatNotNull", typeof(DoubleOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task FloatNull_Double_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<double?>("FloatNull", typeof(DoubleOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task IntNotNull_Int32_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<int?>("IntNotNull", typeof(Int32OrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task IntNull_Int32_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<int?>("IntNull", typeof(Int32OrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task MoneyNotNull_Decimal_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<decimal?>("MoneyNotNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task MoneyNull_Decimal_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<decimal?>("MoneyNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NCharNotNull_String_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<string?>("NCharNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NCharNull_String_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<string?>("NCharNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NTextNotNull_String_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<string?>("NTextNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NTextNull_String_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<string?>("NTextNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NumericNotNull_Decimal_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<decimal?>("NumericNotNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NumericNull_Decimal_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<decimal?>("NumericNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NVarCharNotNull_String_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<string?>("NVarCharNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NVarCharNull_String_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<string?>("NVarCharNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task RealNotNull_Single_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<float?>("RealNotNull", typeof(SingleOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task RealNull_Single_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<float?>("RealNull", typeof(SingleOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task SmalldatetimeNotNull_DateTime_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<DateTime?>("SmalldatetimeNotNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task SmalldatetimeNull_DateTime_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<DateTime?>("SmalldatetimeNull", typeof(DateTimeOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task SmallIntNull_Int16_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<short?>("SmallIntNull", typeof(Int16OrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task SmallMoneyNotNull_Decimal_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<decimal?>("SmallMoneyNotNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task SmallMoneyNull_Decimal_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<decimal?>("SmallMoneyNull", typeof(DecimalOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task TextNotNull_String_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<string?>("TextNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task TextNull_String_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<string?>("TextNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task TimeNotNull_TimeSpan_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<TimeSpan?>("TimeNotNull", typeof(TimeSpanOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task TimeNull_TimeSpan_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<TimeSpan?>("TimeNull", typeof(TimeSpanOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task TinyIntNotNull_Byte_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<byte?>("TinyIntNotNull", typeof(ByteOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task TinyIntNull_Byte_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<byte?>("TinyIntNull", typeof(ByteOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task UniqueidentifierNotNull_Guid_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<Guid?>("UniqueidentifierNotNull", typeof(GuidOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task UniqueidentifierNull_Guid_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<Guid?>("UniqueidentifierNull", typeof(GuidOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task VarBinaryNotNull_ByteArray_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<byte[]?>("VarBinaryNotNull", typeof(ByteArrayOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task VarBinaryNull_ByteArray_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<byte[]?>("VarBinaryNull", typeof(ByteArrayOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task VarCharNotNull_String_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<string?>("VarCharNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		//*****************************
		[TestMethod]
		public async Task VarCharNull_String_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<string?>("VarCharNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task XmlNotNull_String_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<string?>("XmlNotNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task XmlNull_String_ListOrNullDiscardNullTest()
		{
			await ListOrNullDiscardNullTest<string?>("XmlNull", typeof(StringOrNullListMaterializer<DbCommand, DbParameter>));
		}

		async Task ListOrNullDiscardNullTest<TResult>(string columnName, Type materializerType)
		{
			var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (1, 2, 3, 4)");
			ILink<List<TResult>> materializer1 = (ILink<List<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.DiscardNulls })!;
			var result1 = materializer1.Execute();
			var result1a = await materializer1.ExecuteAsync();
			Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
			Assert.IsTrue(result1.All(x => x != null));
		}
	}
}
