#nullable disable

using System.Data.Common;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.Core.Tests
{
	[TestClass]
	public class FailedListWithNullsTests : MaterializerTestBase
	{
		[TestMethod]
		public async Task BigIntNull_Int64_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<long>("BigIntNull", typeof(Int64ListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task BinaryNull_ByteArray_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<byte[]>("BinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task BitNull_Boolean_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<bool>("BitNull", typeof(BooleanListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task CharNull_Char_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<char>("CharNull", typeof(CharListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task CharNull_CharOne_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<char>("CharOneNull", typeof(CharListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task CharNull_String_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<string>("CharNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task DateNull_DateTime_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<DateTime>("DateNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task Datetime2Null_DateTime_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<DateTime>("Datetime2Null", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task DatetimeNull_DateTime_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<DateTime>("DatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task DatetimeoffsetNull_DateTimeOffset_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task DecimalNull_Decimal_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<decimal>("DecimalNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task FloatNull_Double_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<double>("FloatNull", typeof(DoubleListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task IntNull_Int32_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<int>("IntNull", typeof(Int32ListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task MoneyNull_Decimal_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<decimal>("MoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task NCharNull_String_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<string>("NCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task NTextNull_String_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<string>("NTextNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task NumericNull_Decimal_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<decimal>("NumericNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task NVarCharNull_String_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<string>("NVarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task RealNull_Single_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<float>("RealNull", typeof(SingleListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task SmalldatetimeNull_DateTime_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<DateTime>("SmalldatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task SmallIntNull_Int16_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<short>("SmallIntNull", typeof(Int16ListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task SmallMoneyNull_Decimal_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<decimal>("SmallMoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task TextNull_String_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<string>("TextNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task TimeNull_TimeSpan_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<TimeSpan>("TimeNull", typeof(TimeSpanListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task TinyIntNull_Byte_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<byte>("TinyIntNull", typeof(ByteListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task UniqueidentifierNull_Guid_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<Guid>("UniqueidentifierNull", typeof(GuidListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task VarBinaryNull_ByteArray_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<byte[]>("VarBinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task VarCharNull_String_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<string>("VarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task XmlNull_String_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<string>("XmlNull", typeof(StringListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		async Task FailedListWithNullsTestAsync<TResult>(string columnName, Type materializerType)
		{
			var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (1, 2, 3, 4)");

			var materializer1 = (ILink<List<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.None });
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
				var result1a = await materializer1.ExecuteAsync().ConfigureAwait(false);
				Assert.Fail("Exception expected");
			}
			catch (MissingDataException)
			{
				//Expected
			}
		}




		[TestMethod]
		public async Task DateNull_DateOnly_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<DateOnly>("DateNull", typeof(DateOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task Datetime2Null_DateOnly_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<DateOnly>("Datetime2Null", typeof(DateOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task DatetimeNull_DateOnly_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<DateOnly>("DatetimeNull", typeof(DateOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}
		[TestMethod]
		public async Task SmalldatetimeNull_DateOnly_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<DateOnly>("SmalldatetimeNull", typeof(DateOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}




		[TestMethod]
		public async Task Datetime2Null_TimeOnly_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<TimeOnly>("Datetime2Null", typeof(TimeOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}

		[TestMethod]
		public async Task DatetimeNull_TimeOnly_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<TimeOnly>("DatetimeNull", typeof(TimeOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}
		[TestMethod]
		public async Task SmalldatetimeNull_TimeOnly_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<TimeOnly>("SmalldatetimeNull", typeof(TimeOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}
		[TestMethod]
		public async Task TimeNull_TimeOnly_ListWithNullsTest()
		{
			await FailedListWithNullsTestAsync<TimeOnly>("TimeNull", typeof(TimeOnlyListMaterializer<DbCommand, DbParameter>)).ConfigureAwait(false);
		}


	}
}
