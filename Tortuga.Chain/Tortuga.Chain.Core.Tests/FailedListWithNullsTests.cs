#nullable disable

using System.Data.Common;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Core.Tests
{
	[TestClass]
	public class FailedListWithNullsTests : GenericDbDataSource3_MaterializerTests
	{
		[TestMethod]
		public async Task BigIntNull_Int64_ListWithNullsTest()
		{
			await FailedListWithNullsTest<long>("BigIntNull", typeof(Int64ListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task BinaryNull_ByteArray_ListWithNullsTest()
		{
			await FailedListWithNullsTest<byte[]>("BinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task BitNull_Boolean_ListWithNullsTest()
		{
			await FailedListWithNullsTest<bool>("BitNull", typeof(BooleanListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task CharNull_Char_ListWithNullsTest()
		{
			await FailedListWithNullsTest<char>("CharNull", typeof(CharListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task CharNull_CharOne_ListWithNullsTest()
		{
			await FailedListWithNullsTest<char>("CharOneNull", typeof(CharListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task CharNull_String_ListWithNullsTest()
		{
			await FailedListWithNullsTest<string>("CharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DateNull_DateTime_ListWithNullsTest()
		{
			await FailedListWithNullsTest<DateTime>("DateNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task Datetime2Null_DateTime_ListWithNullsTest()
		{
			await FailedListWithNullsTest<DateTime>("Datetime2Null", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DatetimeNull_DateTime_ListWithNullsTest()
		{
			await FailedListWithNullsTest<DateTime>("DatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DatetimeoffsetNull_DateTimeOffset_ListWithNullsTest()
		{
			await FailedListWithNullsTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task DecimalNull_Decimal_ListWithNullsTest()
		{
			await FailedListWithNullsTest<decimal>("DecimalNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task FloatNull_Double_ListWithNullsTest()
		{
			await FailedListWithNullsTest<double>("FloatNull", typeof(DoubleListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task IntNull_Int32_ListWithNullsTest()
		{
			await FailedListWithNullsTest<int>("IntNull", typeof(Int32ListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task MoneyNull_Decimal_ListWithNullsTest()
		{
			await FailedListWithNullsTest<decimal>("MoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NCharNull_String_ListWithNullsTest()
		{
			await FailedListWithNullsTest<string>("NCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NTextNull_String_ListWithNullsTest()
		{
			await FailedListWithNullsTest<string>("NTextNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NumericNull_Decimal_ListWithNullsTest()
		{
			await FailedListWithNullsTest<decimal>("NumericNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task NVarCharNull_String_ListWithNullsTest()
		{
			await FailedListWithNullsTest<string>("NVarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task RealNull_Single_ListWithNullsTest()
		{
			await FailedListWithNullsTest<float>("RealNull", typeof(SingleListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task SmalldatetimeNull_DateTime_ListWithNullsTest()
		{
			await FailedListWithNullsTest<DateTime>("SmalldatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task SmallIntNull_Int16_ListWithNullsTest()
		{
			await FailedListWithNullsTest<short>("SmallIntNull", typeof(Int16ListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task SmallMoneyNull_Decimal_ListWithNullsTest()
		{
			await FailedListWithNullsTest<decimal>("SmallMoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task TextNull_String_ListWithNullsTest()
		{
			await FailedListWithNullsTest<string>("TextNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task TimeNull_TimeSpan_ListWithNullsTest()
		{
			await FailedListWithNullsTest<TimeSpan>("TimeNull", typeof(TimeSpanListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task TinyIntNull_Byte_ListWithNullsTest()
		{
			await FailedListWithNullsTest<byte>("TinyIntNull", typeof(ByteListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task UniqueidentifierNull_Guid_ListWithNullsTest()
		{
			await FailedListWithNullsTest<Guid>("UniqueidentifierNull", typeof(GuidListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task VarBinaryNull_ByteArray_ListWithNullsTest()
		{
			await FailedListWithNullsTest<byte[]>("VarBinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task VarCharNull_String_ListWithNullsTest()
		{
			await FailedListWithNullsTest<string>("VarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
		}

		[TestMethod]
		public async Task XmlNull_String_ListWithNullsTest()
		{
			await FailedListWithNullsTest<string>("XmlNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
		}

		async Task FailedListWithNullsTest<TResult>(string columnName, Type materializerType)
		{
			var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (1, 2, 3, 4)");

			ILink<List<TResult>> materializer1 = (ILink<List<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.None });
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
}
