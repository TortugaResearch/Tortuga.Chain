using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tortuga.Chain;
using Tortuga.Chain.Materializers;

namespace Tests.Materializers
{
    [TestClass]
    public class MaterializerTests : TestBase
    {


        async Task ScalarTest<TResult>(string columnName, Type materializerType)
        {
            var cb1 = DataSource.From("dbo.AllTypes", "Id = 1");
            ILink<TResult> materializer1 = (ILink<TResult>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName });
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
        }


        async Task ScalarFailOnNullTest<TResult>(string columnName, Type materializerType)
        {

            var cb1 = DataSource.From("dbo.AllTypes", "Id In (3)");

            ILink<TResult> materializer1 = (ILink<TResult>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName });
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

        async Task ScalarOrNullTest<TResult>(string columnName, Type materializerType)
        {
            var cb1 = DataSource.From("dbo.AllTypes", "Id = 1");
            ILink<TResult> materializer1 = (ILink<TResult>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName });
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

            var cb3 = DataSource.From("dbo.AllTypes", "Id = 3");
            ILink<TResult> materializer3 = (ILink<TResult>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName });
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

        async Task ListTest<TResult>(string columnName, Type materializerType)
        {
            var cb1 = DataSource.From("dbo.AllTypes", "Id In (1, 2, 4)");
            ILink<List<TResult>> materializer1 = (ILink<List<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.None });
            var result1 = materializer1.Execute();
            var result1a = await materializer1.ExecuteAsync();
            Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
        }

        async Task ListDiscardNullTest<TResult>(string columnName, Type materializerType)
        {
            var cb1 = DataSource.From("dbo.AllTypes", "Id In (1, 2, 3, 4)");
            ILink<List<TResult>> materializer1 = (ILink<List<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.DiscardNulls });
            var result1 = materializer1.Execute();
            var result1a = await materializer1.ExecuteAsync();
            Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
            Assert.IsTrue(result1.All(x => x != null));
        }

        async Task ListWithNullsTest<TResult>(string columnName, Type materializerType)
        {
            var cb1 = DataSource.From("dbo.AllTypes", "Id In (1, 2, 3, 4)");
            ILink<List<TResult>> materializer1 = (ILink<List<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.None });
            var result1 = materializer1.Execute();
            var result1a = await materializer1.ExecuteAsync();
            Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
        }

        async Task SetTest<TResult>(string columnName, Type materializerType)
        {
            var cb1 = DataSource.From("dbo.AllTypes", "Id In (1, 2, 4)");
            ILink<HashSet<TResult>> materializer1 = (ILink<HashSet<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.None });
            var result1 = materializer1.Execute();
            var result1a = await materializer1.ExecuteAsync();
            Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
        }

        async Task SetDiscardNullTest<TResult>(string columnName, Type materializerType)
        {
            var cb1 = DataSource.From("dbo.AllTypes", "Id In (1, 2, 3, 4)");
            ILink<HashSet<TResult>> materializer1 = (ILink<HashSet<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.DiscardNulls });
            var result1 = materializer1.Execute();
            var result1a = await materializer1.ExecuteAsync();
            Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
            Assert.IsTrue(result1.All(x => x != null));
        }

        async Task SetWithNullsTest<TResult>(string columnName, Type materializerType)
        {
            var cb1 = DataSource.From("dbo.AllTypes", "Id In (1, 2, 3, 4)");
            ILink<HashSet<TResult>> materializer1 = (ILink<HashSet<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.None });
            var result1 = materializer1.Execute();
            var result1a = await materializer1.ExecuteAsync();
            Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
        }

        async Task FailedListWithNullsTest<TResult>(string columnName, Type materializerType)
        {
            var cb1 = DataSource.From("dbo.AllTypes", "Id In (1, 2, 3, 4)");

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

        async Task FailedSetWithNullsTest<TResult>(string columnName, Type materializerType)
        {
            var cb1 = DataSource.From("dbo.AllTypes", "Id In (1, 2, 3, 4)");

            ILink<HashSet<TResult>> materializer1 = (ILink<HashSet<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.None });
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



        //*****************************


        [TestMethod]
        public async Task CharNull_String_ScalarTestTest()
        {
            await ScalarTest<string>("CharNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task CharNotNull_String_ScalarTestTest()
        {
            await ScalarTest<string>("CharNotNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarCharNull_String_ScalarTestTest()
        {
            await ScalarTest<string>("VarCharNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarCharNotNull_String_ScalarTestTest()
        {
            await ScalarTest<string>("VarCharNotNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NCharNull_String_ScalarTestTest()
        {
            await ScalarTest<string>("NCharNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NCharNotNull_String_ScalarTestTest()
        {
            await ScalarTest<string>("NCharNotNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NVarCharNull_String_ScalarTestTest()
        {
            await ScalarTest<string>("NVarCharNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NVarCharNotNull_String_ScalarTestTest()
        {
            await ScalarTest<string>("NVarCharNotNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BitNull_Boolean_ScalarTestTest()
        {
            await ScalarTest<bool>("BitNull", typeof(BooleanMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BitNotNull_Boolean_ScalarTestTest()
        {
            await ScalarTest<bool>("BitNotNull", typeof(BooleanMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TinyIntNull_Byte_ScalarTestTest()
        {
            await ScalarTest<byte>("TinyIntNull", typeof(ByteMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TinyIntNotNull_Byte_ScalarTestTest()
        {
            await ScalarTest<byte>("TinyIntNotNull", typeof(ByteMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task IntNull_Int32_ScalarTestTest()
        {
            await ScalarTest<int>("IntNull", typeof(Int32Materializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task IntNotNull_Int32_ScalarTestTest()
        {
            await ScalarTest<int>("IntNotNull", typeof(Int32Materializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BigIntNull_Int64_ScalarTestTest()
        {
            await ScalarTest<long>("BigIntNull", typeof(Int64Materializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BigIntNotNull_Int64_ScalarTestTest()
        {
            await ScalarTest<long>("BigIntNotNull", typeof(Int64Materializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NumericNull_Decimal_ScalarTestTest()
        {
            await ScalarTest<decimal>("NumericNull", typeof(DecimalMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NumericNotNull_Decimal_ScalarTestTest()
        {
            await ScalarTest<decimal>("NumericNotNull", typeof(DecimalMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallIntNull_Int16_ScalarTestTest()
        {
            await ScalarTest<short>("SmallIntNull", typeof(Int16Materializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallIntNotNull_Int16_ScalarTestTest()
        {
            await ScalarTest<short>("SmallIntNotNull", typeof(Int16Materializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DecimalNull_Decimal_ScalarTestTest()
        {
            await ScalarTest<decimal>("DecimalNull", typeof(DecimalMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DecimalNotNull_Decimal_ScalarTestTest()
        {
            await ScalarTest<decimal>("DecimalNotNull", typeof(DecimalMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallMoneyNull_Decimal_ScalarTestTest()
        {
            await ScalarTest<decimal>("SmallMoneyNull", typeof(DecimalMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallMoneyNotNull_Decimal_ScalarTestTest()
        {
            await ScalarTest<decimal>("SmallMoneyNotNull", typeof(DecimalMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task MoneyNull_Decimal_ScalarTestTest()
        {
            await ScalarTest<decimal>("MoneyNull", typeof(DecimalMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task MoneyNotNull_Decimal_ScalarTestTest()
        {
            await ScalarTest<decimal>("MoneyNotNull", typeof(DecimalMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task FloatNull_Double_ScalarTestTest()
        {
            await ScalarTest<double>("FloatNull", typeof(DoubleMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task FloatNotNull_Double_ScalarTestTest()
        {
            await ScalarTest<double>("FloatNotNull", typeof(DoubleMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task RealNull_Single_ScalarTestTest()
        {
            await ScalarTest<float>("RealNull", typeof(SingleMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task RealNotNull_Single_ScalarTestTest()
        {
            await ScalarTest<float>("RealNotNull", typeof(SingleMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DateNull_DateTime_ScalarTestTest()
        {
            await ScalarTest<DateTime>("DateNull", typeof(DateTimeMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DateNotNull_DateTime_ScalarTestTest()
        {
            await ScalarTest<DateTime>("DateNotNull", typeof(DateTimeMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeoffsetNull_DateTimeOffset_ScalarTestTest()
        {
            await ScalarTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeoffsetNotNull_DateTimeOffset_ScalarTestTest()
        {
            await ScalarTest<DateTimeOffset>("DatetimeoffsetNotNull", typeof(DateTimeOffsetMaterializer<SqlCommand, SqlParameter>));
        }
        [TestMethod]
        public async Task Datetime2Null_DateTime_ScalarTestTest()
        {
            await ScalarTest<DateTime>("Datetime2Null", typeof(DateTimeMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task Datetime2NotNull_DateTime_ScalarTestTest()
        {
            await ScalarTest<DateTime>("Datetime2NotNull", typeof(DateTimeMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmalldatetimeNull_DateTime_ScalarTestTest()
        {
            await ScalarTest<DateTime>("SmalldatetimeNull", typeof(DateTimeMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmalldatetimeNotNull_DateTime_ScalarTestTest()
        {
            await ScalarTest<DateTime>("SmalldatetimeNotNull", typeof(DateTimeMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeNull_DateTime_ScalarTestTest()
        {
            await ScalarTest<DateTime>("DatetimeNull", typeof(DateTimeMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeNotNull_DateTime_ScalarTestTest()
        {
            await ScalarTest<DateTime>("DatetimeNotNull", typeof(DateTimeMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TimeNull_TimeSpan_ScalarTestTest()
        {
            await ScalarTest<TimeSpan>("TimeNull", typeof(TimeSpanMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TimeNotNull_TimeSpan_ScalarTestTest()
        {
            await ScalarTest<TimeSpan>("TimeNotNull", typeof(TimeSpanMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TextNull_String_ScalarTestTest()
        {
            await ScalarTest<string>("TextNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TextNotNull_String_ScalarTestTest()
        {
            await ScalarTest<string>("TextNotNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NTextNull_String_ScalarTestTest()
        {
            await ScalarTest<string>("NTextNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NTextNotNull_String_ScalarTestTest()
        {
            await ScalarTest<string>("NTextNotNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BinaryNull_ByteArray_ScalarTestTest()
        {
            await ScalarTest<byte[]>("BinaryNull", typeof(ByteArrayMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BinaryNotNull_ByteArray_ScalarTestTest()
        {
            await ScalarTest<byte[]>("BinaryNotNull", typeof(ByteArrayMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarBinaryNull_ByteArray_ScalarTestTest()
        {
            await ScalarTest<byte[]>("VarBinaryNull", typeof(ByteArrayMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarBinaryNotNull_ByteArray_ScalarTestTest()
        {
            await ScalarTest<byte[]>("VarBinaryNotNull", typeof(ByteArrayMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task UniqueidentifierNull_Guid_ScalarTestTest()
        {
            await ScalarTest<Guid>("UniqueidentifierNull", typeof(GuidMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task UniqueidentifierNotNull_Guid_ScalarTestTest()
        {
            await ScalarTest<Guid>("UniqueidentifierNotNull", typeof(GuidMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task XmlNull_String_ScalarTestTest()
        {
            await ScalarTest<string>("XmlNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task XmlNotNull_String_ScalarTestTest()
        {
            await ScalarTest<string>("XmlNotNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        //*****************************


        [TestMethod]
        public async Task CharNull_String_ScalarOrNullTest()
        {
            await ScalarOrNullTest<string>("CharNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarCharNull_String_ScalarOrNullTest()
        {
            await ScalarOrNullTest<string>("VarCharNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NCharNull_String_ScalarOrNullTest()
        {
            await ScalarOrNullTest<string>("NCharNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NVarCharNull_String_ScalarOrNullTest()
        {
            await ScalarOrNullTest<string>("NVarCharNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BitNull_Boolean_ScalarOrNullTest()
        {
            await ScalarOrNullTest<bool?>("BitNull", typeof(BooleanOrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TinyIntNull_Byte_ScalarOrNullTest()
        {
            await ScalarOrNullTest<byte?>("TinyIntNull", typeof(ByteOrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task IntNull_Int32_ScalarOrNullTest()
        {
            await ScalarOrNullTest<int?>("IntNull", typeof(Int32OrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BigIntNull_Int64_ScalarOrNullTest()
        {
            await ScalarOrNullTest<long?>("BigIntNull", typeof(Int64OrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NumericNull_Decimal_ScalarOrNullTest()
        {
            await ScalarOrNullTest<decimal?>("NumericNull", typeof(DecimalOrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallIntNull_Int16_ScalarOrNullTest()
        {
            await ScalarOrNullTest<short?>("SmallIntNull", typeof(Int16OrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DecimalNull_Decimal_ScalarOrNullTest()
        {
            await ScalarOrNullTest<decimal?>("DecimalNull", typeof(DecimalOrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallMoneyNull_Decimal_ScalarOrNullTest()
        {
            await ScalarOrNullTest<decimal?>("SmallMoneyNull", typeof(DecimalOrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task MoneyNull_Decimal_ScalarOrNullTest()
        {
            await ScalarOrNullTest<decimal?>("MoneyNull", typeof(DecimalOrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task FloatNull_Double_ScalarOrNullTest()
        {
            await ScalarOrNullTest<double?>("FloatNull", typeof(DoubleOrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task RealNull_Single_ScalarOrNullTest()
        {
            await ScalarOrNullTest<float?>("RealNull", typeof(SingleOrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DateNull_DateTime_ScalarOrNullTest()
        {
            await ScalarOrNullTest<DateTime?>("DateNull", typeof(DateTimeOrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeoffsetNull_DateTimeOffset_ScalarOrNullTest()
        {
            await ScalarOrNullTest<DateTimeOffset?>("DatetimeoffsetNull", typeof(DateTimeOffsetOrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task Datetime2Null_DateTime_ScalarOrNullTest()
        {
            await ScalarOrNullTest<DateTime?>("Datetime2Null", typeof(DateTimeOrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmalldatetimeNull_DateTime_ScalarOrNullTest()
        {
            await ScalarOrNullTest<DateTime?>("SmalldatetimeNull", typeof(DateTimeOrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeNull_DateTime_ScalarOrNullTest()
        {
            await ScalarOrNullTest<DateTime?>("DatetimeNull", typeof(DateTimeOrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TimeNull_TimeSpan_ScalarOrNullTest()
        {
            await ScalarOrNullTest<TimeSpan?>("TimeNull", typeof(TimeSpanOrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TextNull_String_ScalarOrNullTest()
        {
            await ScalarOrNullTest<string>("TextNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NTextNull_String_ScalarOrNullTest()
        {
            await ScalarOrNullTest<string>("NTextNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BinaryNull_ByteArray_ScalarOrNullTest()
        {
            await ScalarOrNullTest<byte[]>("BinaryNull", typeof(ByteArrayMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarBinaryNull_ByteArray_ScalarOrNullTest()
        {
            await ScalarOrNullTest<byte[]>("VarBinaryNull", typeof(ByteArrayMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task UniqueidentifierNull_Guid_ScalarOrNullTest()
        {
            await ScalarOrNullTest<Guid?>("UniqueidentifierNull", typeof(GuidOrNullMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task XmlNull_String_ScalarOrNullTest()
        {
            await ScalarOrNullTest<string>("XmlNull", typeof(StringMaterializer<SqlCommand, SqlParameter>));
        }


        //*****************************


        [TestMethod]
        public async Task CharNull_String_ListTest()
        {
            await ListTest<string>("CharNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task CharNotNull_String_ListTest()
        {
            await ListTest<string>("CharNotNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarCharNull_String_ListTest()
        {
            await ListTest<string>("VarCharNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarCharNotNull_String_ListTest()
        {
            await ListTest<string>("VarCharNotNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NCharNull_String_ListTest()
        {
            await ListTest<string>("NCharNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NCharNotNull_String_ListTest()
        {
            await ListTest<string>("NCharNotNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NVarCharNull_String_ListTest()
        {
            await ListTest<string>("NVarCharNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NVarCharNotNull_String_ListTest()
        {
            await ListTest<string>("NVarCharNotNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BitNull_Boolean_ListTest()
        {
            await ListTest<bool>("BitNull", typeof(BooleanListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BitNotNull_Boolean_ListTest()
        {
            await ListTest<bool>("BitNotNull", typeof(BooleanListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TinyIntNull_Byte_ListTest()
        {
            await ListTest<byte>("TinyIntNull", typeof(ByteListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TinyIntNotNull_Byte_ListTest()
        {
            await ListTest<byte>("TinyIntNotNull", typeof(ByteListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task IntNull_Int32_ListTest()
        {
            await ListTest<int>("IntNull", typeof(Int32ListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task IntNotNull_Int32_ListTest()
        {
            await ListTest<int>("IntNotNull", typeof(Int32ListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BigIntNull_Int64_ListTest()
        {
            await ListTest<long>("BigIntNull", typeof(Int64ListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BigIntNotNull_Int64_ListTest()
        {
            await ListTest<long>("BigIntNotNull", typeof(Int64ListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NumericNull_Decimal_ListTest()
        {
            await ListTest<decimal>("NumericNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NumericNotNull_Decimal_ListTest()
        {
            await ListTest<decimal>("NumericNotNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallIntNull_Int16_ListTest()
        {
            await ListTest<short>("SmallIntNull", typeof(Int16ListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallIntNotNull_Int16_ListTest()
        {
            await ListTest<short>("SmallIntNotNull", typeof(Int16ListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DecimalNull_Decimal_ListTest()
        {
            await ListTest<decimal>("DecimalNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DecimalNotNull_Decimal_ListTest()
        {
            await ListTest<decimal>("DecimalNotNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallMoneyNull_Decimal_ListTest()
        {
            await ListTest<decimal>("SmallMoneyNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallMoneyNotNull_Decimal_ListTest()
        {
            await ListTest<decimal>("SmallMoneyNotNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task MoneyNull_Decimal_ListTest()
        {
            await ListTest<decimal>("MoneyNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task MoneyNotNull_Decimal_ListTest()
        {
            await ListTest<decimal>("MoneyNotNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task FloatNull_Double_ListTest()
        {
            await ListTest<double>("FloatNull", typeof(DoubleListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task FloatNotNull_Double_ListTest()
        {
            await ListTest<double>("FloatNotNull", typeof(DoubleListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task RealNull_Single_ListTest()
        {
            await ListTest<float>("RealNull", typeof(SingleListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task RealNotNull_Single_ListTest()
        {
            await ListTest<float>("RealNotNull", typeof(SingleListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DateNull_DateTime_ListTest()
        {
            await ListTest<DateTime>("DateNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DateNotNull_DateTime_ListTest()
        {
            await ListTest<DateTime>("DateNotNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeoffsetNull_DateTimeOffset_ListTest()
        {
            await ListTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeoffsetNotNull_DateTimeOffset_ListTest()
        {
            await ListTest<DateTimeOffset>("DatetimeoffsetNotNull", typeof(DateTimeOffsetListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task Datetime2Null_DateTime_ListTest()
        {
            await ListTest<DateTime>("Datetime2Null", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task Datetime2NotNull_DateTime_ListTest()
        {
            await ListTest<DateTime>("Datetime2NotNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmalldatetimeNull_DateTime_ListTest()
        {
            await ListTest<DateTime>("SmalldatetimeNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmalldatetimeNotNull_DateTime_ListTest()
        {
            await ListTest<DateTime>("SmalldatetimeNotNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeNull_DateTime_ListTest()
        {
            await ListTest<DateTime>("DatetimeNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeNotNull_DateTime_ListTest()
        {
            await ListTest<DateTime>("DatetimeNotNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TimeNull_TimeSpan_ListTest()
        {
            await ListTest<TimeSpan>("TimeNull", typeof(TimeSpanListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TimeNotNull_TimeSpan_ListTest()
        {
            await ListTest<TimeSpan>("TimeNotNull", typeof(TimeSpanListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TextNull_String_ListTest()
        {
            await ListTest<string>("TextNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TextNotNull_String_ListTest()
        {
            await ListTest<string>("TextNotNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NTextNull_String_ListTest()
        {
            await ListTest<string>("NTextNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NTextNotNull_String_ListTest()
        {
            await ListTest<string>("NTextNotNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BinaryNull_ByteArray_ListTest()
        {
            await ListTest<byte[]>("BinaryNull", typeof(ByteArrayListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BinaryNotNull_ByteArray_ListTest()
        {
            await ListTest<byte[]>("BinaryNotNull", typeof(ByteArrayListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarBinaryNull_ByteArray_ListTest()
        {
            await ListTest<byte[]>("VarBinaryNull", typeof(ByteArrayListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarBinaryNotNull_ByteArray_ListTest()
        {
            await ListTest<byte[]>("VarBinaryNotNull", typeof(ByteArrayListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task UniqueidentifierNull_Guid_ListTest()
        {
            await ListTest<Guid>("UniqueidentifierNull", typeof(GuidListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task UniqueidentifierNotNull_Guid_ListTest()
        {
            await ListTest<Guid>("UniqueidentifierNotNull", typeof(GuidListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task XmlNull_String_ListTest()
        {
            await ListTest<string>("XmlNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task XmlNotNull_String_ListTest()
        {
            await ListTest<string>("XmlNotNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }



        //*****************************




        [TestMethod]
        public async Task CharNull_String_ListDiscardNullTest()
        {
            await ListDiscardNullTest<string>("CharNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task CharNotNull_String_ListDiscardNullTest()
        {
            await ListDiscardNullTest<string>("CharNotNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarCharNull_String_ListDiscardNullTest()
        {
            await ListDiscardNullTest<string>("VarCharNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarCharNotNull_String_ListDiscardNullTest()
        {
            await ListDiscardNullTest<string>("VarCharNotNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NCharNull_String_ListDiscardNullTest()
        {
            await ListDiscardNullTest<string>("NCharNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NCharNotNull_String_ListDiscardNullTest()
        {
            await ListDiscardNullTest<string>("NCharNotNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NVarCharNull_String_ListDiscardNullTest()
        {
            await ListDiscardNullTest<string>("NVarCharNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NVarCharNotNull_String_ListDiscardNullTest()
        {
            await ListDiscardNullTest<string>("NVarCharNotNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BitNull_Boolean_ListDiscardNullTest()
        {
            await ListDiscardNullTest<bool>("BitNull", typeof(BooleanListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BitNotNull_Boolean_ListDiscardNullTest()
        {
            await ListDiscardNullTest<bool>("BitNotNull", typeof(BooleanListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TinyIntNull_Byte_ListDiscardNullTest()
        {
            await ListDiscardNullTest<byte>("TinyIntNull", typeof(ByteListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TinyIntNotNull_Byte_ListDiscardNullTest()
        {
            await ListDiscardNullTest<byte>("TinyIntNotNull", typeof(ByteListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task IntNull_Int32_ListDiscardNullTest()
        {
            await ListDiscardNullTest<int>("IntNull", typeof(Int32ListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task IntNotNull_Int32_ListDiscardNullTest()
        {
            await ListDiscardNullTest<int>("IntNotNull", typeof(Int32ListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BigIntNull_Int64_ListDiscardNullTest()
        {
            await ListDiscardNullTest<long>("BigIntNull", typeof(Int64ListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BigIntNotNull_Int64_ListDiscardNullTest()
        {
            await ListDiscardNullTest<long>("BigIntNotNull", typeof(Int64ListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NumericNull_Decimal_ListDiscardNullTest()
        {
            await ListDiscardNullTest<decimal>("NumericNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NumericNotNull_Decimal_ListDiscardNullTest()
        {
            await ListDiscardNullTest<decimal>("NumericNotNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallIntNull_Int16_ListDiscardNullTest()
        {
            await ListDiscardNullTest<short>("SmallIntNull", typeof(Int16ListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallIntNotNull_Int16_ListDiscardNullTest()
        {
            await ListDiscardNullTest<short>("SmallIntNotNull", typeof(Int16ListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DecimalNull_Decimal_ListDiscardNullTest()
        {
            await ListDiscardNullTest<decimal>("DecimalNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DecimalNotNull_Decimal_ListDiscardNullTest()
        {
            await ListDiscardNullTest<decimal>("DecimalNotNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallMoneyNull_Decimal_ListDiscardNullTest()
        {
            await ListDiscardNullTest<decimal>("SmallMoneyNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallMoneyNotNull_Decimal_ListDiscardNullTest()
        {
            await ListDiscardNullTest<decimal>("SmallMoneyNotNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task MoneyNull_Decimal_ListDiscardNullTest()
        {
            await ListDiscardNullTest<decimal>("MoneyNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task MoneyNotNull_Decimal_ListDiscardNullTest()
        {
            await ListDiscardNullTest<decimal>("MoneyNotNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task FloatNull_Double_ListDiscardNullTest()
        {
            await ListDiscardNullTest<double>("FloatNull", typeof(DoubleListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task FloatNotNull_Double_ListDiscardNullTest()
        {
            await ListDiscardNullTest<double>("FloatNotNull", typeof(DoubleListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task RealNull_Single_ListDiscardNullTest()
        {
            await ListDiscardNullTest<float>("RealNull", typeof(SingleListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task RealNotNull_Single_ListDiscardNullTest()
        {
            await ListDiscardNullTest<float>("RealNotNull", typeof(SingleListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DateNull_DateTime_ListDiscardNullTest()
        {
            await ListDiscardNullTest<DateTime>("DateNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DateNotNull_DateTime_ListDiscardNullTest()
        {
            await ListDiscardNullTest<DateTime>("DateNotNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeoffsetNull_DateTimeOffset_ListDiscardNullTest()
        {
            await ListDiscardNullTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetListMaterializer<SqlCommand, SqlParameter>));
        }

        [TestMethod]
        public async Task DatetimeoffsetNotNull_DateTimeOffset_ListDiscardNullTest()
        {
            await ListDiscardNullTest<DateTimeOffset>("DatetimeoffsetNotNull", typeof(DateTimeOffsetListMaterializer<SqlCommand, SqlParameter>));
        }

        [TestMethod]
        public async Task Datetime2Null_DateTime_ListDiscardNullTest()
        {
            await ListDiscardNullTest<DateTime>("Datetime2Null", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task Datetime2NotNull_DateTime_ListDiscardNullTest()
        {
            await ListDiscardNullTest<DateTime>("Datetime2NotNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmalldatetimeNull_DateTime_ListDiscardNullTest()
        {
            await ListDiscardNullTest<DateTime>("SmalldatetimeNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmalldatetimeNotNull_DateTime_ListDiscardNullTest()
        {
            await ListDiscardNullTest<DateTime>("SmalldatetimeNotNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeNull_DateTime_ListDiscardNullTest()
        {
            await ListDiscardNullTest<DateTime>("DatetimeNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeNotNull_DateTime_ListDiscardNullTest()
        {
            await ListDiscardNullTest<DateTime>("DatetimeNotNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TimeNull_TimeSpan_ListDiscardNullTest()
        {
            await ListDiscardNullTest<TimeSpan>("TimeNull", typeof(TimeSpanListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TimeNotNull_TimeSpan_ListDiscardNullTest()
        {
            await ListDiscardNullTest<TimeSpan>("TimeNotNull", typeof(TimeSpanListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TextNull_String_ListDiscardNullTest()
        {
            await ListDiscardNullTest<string>("TextNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TextNotNull_String_ListDiscardNullTest()
        {
            await ListDiscardNullTest<string>("TextNotNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NTextNull_String_ListDiscardNullTest()
        {
            await ListDiscardNullTest<string>("NTextNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NTextNotNull_String_ListDiscardNullTest()
        {
            await ListDiscardNullTest<string>("NTextNotNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BinaryNull_ByteArray_ListDiscardNullTest()
        {
            await ListDiscardNullTest<byte[]>("BinaryNull", typeof(ByteArrayListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BinaryNotNull_ByteArray_ListDiscardNullTest()
        {
            await ListDiscardNullTest<byte[]>("BinaryNotNull", typeof(ByteArrayListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarBinaryNull_ByteArray_ListDiscardNullTest()
        {
            await ListDiscardNullTest<byte[]>("VarBinaryNull", typeof(ByteArrayListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarBinaryNotNull_ByteArray_ListDiscardNullTest()
        {
            await ListDiscardNullTest<byte[]>("VarBinaryNotNull", typeof(ByteArrayListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task UniqueidentifierNull_Guid_ListDiscardNullTest()
        {
            await ListDiscardNullTest<Guid>("UniqueidentifierNull", typeof(GuidListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task UniqueidentifierNotNull_Guid_ListDiscardNullTest()
        {
            await ListDiscardNullTest<Guid>("UniqueidentifierNotNull", typeof(GuidListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task XmlNull_String_ListDiscardNullTest()
        {
            await ListDiscardNullTest<string>("XmlNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task XmlNotNull_String_ListDiscardNullTest()
        {
            await ListDiscardNullTest<string>("XmlNotNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }



        //*****************************


        [TestMethod]
        public async Task CharNull_String_ListWithNullsTest()
        {
            await ListWithNullsTest<string>("CharNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarCharNull_String_ListWithNullsTest()
        {
            await ListWithNullsTest<string>("VarCharNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NCharNull_String_ListWithNullsTest()
        {
            await ListWithNullsTest<string>("NCharNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NVarCharNull_String_ListWithNullsTest()
        {
            await ListWithNullsTest<string>("NVarCharNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BitNull_Boolean_ListWithNullsTest()
        {
            await FailedListWithNullsTest<bool>("BitNull", typeof(BooleanListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TinyIntNull_Byte_ListWithNullsTest()
        {
            await FailedListWithNullsTest<byte>("TinyIntNull", typeof(ByteListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task IntNull_Int32_ListWithNullsTest()
        {
            await FailedListWithNullsTest<int>("IntNull", typeof(Int32ListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BigIntNull_Int64_ListWithNullsTest()
        {
            await FailedListWithNullsTest<long>("BigIntNull", typeof(Int64ListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NumericNull_Decimal_ListWithNullsTest()
        {
            await FailedListWithNullsTest<decimal>("NumericNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallIntNull_Int16_ListWithNullsTest()
        {
            await FailedListWithNullsTest<short>("SmallIntNull", typeof(Int16ListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DecimalNull_Decimal_ListWithNullsTest()
        {
            await FailedListWithNullsTest<decimal>("DecimalNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallMoneyNull_Decimal_ListWithNullsTest()
        {
            await FailedListWithNullsTest<decimal>("SmallMoneyNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task MoneyNull_Decimal_ListWithNullsTest()
        {
            await FailedListWithNullsTest<decimal>("MoneyNull", typeof(DecimalListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task FloatNull_Double_ListWithNullsTest()
        {
            await FailedListWithNullsTest<double>("FloatNull", typeof(DoubleListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task RealNull_Single_ListWithNullsTest()
        {
            await FailedListWithNullsTest<float>("RealNull", typeof(SingleListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DateNull_DateTime_ListWithNullsTest()
        {
            await FailedListWithNullsTest<DateTime>("DateNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeoffsetNull_DateTimeOffset_ListWithNullsTest()
        {
            await FailedListWithNullsTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task Datetime2Null_DateTime_ListWithNullsTest()
        {
            await FailedListWithNullsTest<DateTime>("Datetime2Null", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmalldatetimeNull_DateTime_ListWithNullsTest()
        {
            await FailedListWithNullsTest<DateTime>("SmalldatetimeNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeNull_DateTime_ListWithNullsTest()
        {
            await FailedListWithNullsTest<DateTime>("DatetimeNull", typeof(DateTimeListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TimeNull_TimeSpan_ListWithNullsTest()
        {
            await FailedListWithNullsTest<TimeSpan>("TimeNull", typeof(TimeSpanListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TextNull_String_ListWithNullsTest()
        {
            await ListWithNullsTest<string>("TextNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NTextNull_String_ListWithNullsTest()
        {
            await ListWithNullsTest<string>("NTextNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BinaryNull_ByteArray_ListWithNullsTest()
        {
            await FailedListWithNullsTest<byte[]>("BinaryNull", typeof(ByteArrayListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarBinaryNull_ByteArray_ListWithNullsTest()
        {
            await FailedListWithNullsTest<byte[]>("VarBinaryNull", typeof(ByteArrayListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task UniqueidentifierNull_Guid_ListWithNullsTest()
        {
            await FailedListWithNullsTest<Guid>("UniqueidentifierNull", typeof(GuidListMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task XmlNull_String_ListWithNullsTest()
        {
            await ListWithNullsTest<string>("XmlNull", typeof(StringListMaterializer<SqlCommand, SqlParameter>));
        }




        //*****************************


        [TestMethod]
        public async Task BitNull_Boolean_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<bool>("BitNull", typeof(BooleanMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TinyIntNull_Byte_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<byte>("TinyIntNull", typeof(ByteMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task IntNull_Int32_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<int>("IntNull", typeof(Int32Materializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BigIntNull_Int64_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<long>("BigIntNull", typeof(Int64Materializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NumericNull_Decimal_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<decimal>("NumericNull", typeof(DecimalMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallIntNull_Int16_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<short>("SmallIntNull", typeof(Int16Materializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DecimalNull_Decimal_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<decimal>("DecimalNull", typeof(DecimalMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallMoneyNull_Decimal_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<decimal>("SmallMoneyNull", typeof(DecimalMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task MoneyNull_Decimal_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<decimal>("MoneyNull", typeof(DecimalMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task FloatNull_Double_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<double>("FloatNull", typeof(DoubleMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task RealNull_Single_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<float>("RealNull", typeof(SingleMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DateNull_DateTime_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<DateTime>("DateNull", typeof(DateTimeMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeoffsetNull_DateTimeOffset_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task Datetime2Null_DateTime_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<DateTime>("Datetime2Null", typeof(DateTimeMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmalldatetimeNull_DateTime_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<DateTime>("SmalldatetimeNull", typeof(DateTimeMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeNull_DateTime_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<DateTime>("DatetimeNull", typeof(DateTimeMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TimeNull_TimeSpan_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<TimeSpan>("TimeNull", typeof(TimeSpanMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task UniqueidentifierNull_Guid_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<Guid>("UniqueidentifierNull", typeof(GuidMaterializer<SqlCommand, SqlParameter>));
        }



        //*****************************


        [TestMethod]
        public async Task Table()
        {
            var materializer = DataSource.From("dbo.AllTypes").ToTable();
            var result1 = materializer.Execute();
            var result1a = await materializer.ExecuteAsync();
        }



        [TestMethod]
        public async Task DataTable()
        {
            var materializer = DataSource.From("dbo.AllTypes").ToDataTable();
            var result1 = materializer.Execute();
            var result1a = await materializer.ExecuteAsync();
        }


        [TestMethod]
        public async Task TableSet()
        {
            var materializer = DataSource.Procedure("Sales.CustomerWithOrdersByState", new { State = "CA" }).ToTableSet();
            var result1 = materializer.Execute();
            var result1a = await materializer.ExecuteAsync();
        }




        [TestMethod]
        public async Task TableSet2()
        {
            var materializer = DataSource.Procedure("Sales.CustomerWithOrdersByState", new { State = "CA" }).ToTableSet("Customers", "Orders");
            var result1 = materializer.Execute();
            var result1a = await materializer.ExecuteAsync();
        }


        [TestMethod]
        public async Task DataTableSet()
        {
            var materializer = DataSource.Procedure("Sales.CustomerWithOrdersByState", new { State = "CA" }).ToDataSet("Customers", "Orders");
            var result1 = materializer.Execute();
            var result1a = await materializer.ExecuteAsync();
        }



        [TestMethod]
        public async Task TableSet_Dict()
        {
            var materializer = DataSource.Procedure("Sales.CustomerWithOrdersByState", new Dictionary<string, object>() { { "State", "CA" } }).ToTableSet();
            var result1 = materializer.Execute();
            var result1a = await materializer.ExecuteAsync();
        }



        [TestMethod]
        public async Task DataTableSet_Dict()
        {
            var materializer = DataSource.Procedure("Sales.CustomerWithOrdersByState", new Dictionary<string, object>() { { "State", "CA" } }).ToDataSet("Customers", "Orders");
            var result1 = materializer.Execute();
            var result1a = await materializer.ExecuteAsync();
        }


        //--------------------------------------------


        //*****************************




        [TestMethod]
        public async Task CharNull_String_SetDiscardNullTest()
        {
            await SetDiscardNullTest<string>("CharNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task CharNotNull_String_SetDiscardNullTest()
        {
            await SetDiscardNullTest<string>("CharNotNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarCharNull_String_SetDiscardNullTest()
        {
            await SetDiscardNullTest<string>("VarCharNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarCharNotNull_String_SetDiscardNullTest()
        {
            await SetDiscardNullTest<string>("VarCharNotNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NCharNull_String_SetDiscardNullTest()
        {
            await SetDiscardNullTest<string>("NCharNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NCharNotNull_String_SetDiscardNullTest()
        {
            await SetDiscardNullTest<string>("NCharNotNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NVarCharNull_String_SetDiscardNullTest()
        {
            await SetDiscardNullTest<string>("NVarCharNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NVarCharNotNull_String_SetDiscardNullTest()
        {
            await SetDiscardNullTest<string>("NVarCharNotNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }



        [TestMethod]
        public async Task IntNull_Int32_SetDiscardNullTest()
        {
            await SetDiscardNullTest<int>("IntNull", typeof(Int32SetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task IntNotNull_Int32_SetDiscardNullTest()
        {
            await SetDiscardNullTest<int>("IntNotNull", typeof(Int32SetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BigIntNull_Int64_SetDiscardNullTest()
        {
            await SetDiscardNullTest<long>("BigIntNull", typeof(Int64SetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BigIntNotNull_Int64_SetDiscardNullTest()
        {
            await SetDiscardNullTest<long>("BigIntNotNull", typeof(Int64SetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NumericNull_Decimal_SetDiscardNullTest()
        {
            await SetDiscardNullTest<decimal>("NumericNull", typeof(DecimalSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NumericNotNull_Decimal_SetDiscardNullTest()
        {
            await SetDiscardNullTest<decimal>("NumericNotNull", typeof(DecimalSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallIntNull_Int16_SetDiscardNullTest()
        {
            await SetDiscardNullTest<short>("SmallIntNull", typeof(Int16SetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallIntNotNull_Int16_SetDiscardNullTest()
        {
            await SetDiscardNullTest<short>("SmallIntNotNull", typeof(Int16SetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DecimalNull_Decimal_SetDiscardNullTest()
        {
            await SetDiscardNullTest<decimal>("DecimalNull", typeof(DecimalSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DecimalNotNull_Decimal_SetDiscardNullTest()
        {
            await SetDiscardNullTest<decimal>("DecimalNotNull", typeof(DecimalSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallMoneyNull_Decimal_SetDiscardNullTest()
        {
            await SetDiscardNullTest<decimal>("SmallMoneyNull", typeof(DecimalSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallMoneyNotNull_Decimal_SetDiscardNullTest()
        {
            await SetDiscardNullTest<decimal>("SmallMoneyNotNull", typeof(DecimalSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task MoneyNull_Decimal_SetDiscardNullTest()
        {
            await SetDiscardNullTest<decimal>("MoneyNull", typeof(DecimalSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task MoneyNotNull_Decimal_SetDiscardNullTest()
        {
            await SetDiscardNullTest<decimal>("MoneyNotNull", typeof(DecimalSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task FloatNull_Double_SetDiscardNullTest()
        {
            await SetDiscardNullTest<double>("FloatNull", typeof(DoubleSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task FloatNotNull_Double_SetDiscardNullTest()
        {
            await SetDiscardNullTest<double>("FloatNotNull", typeof(DoubleSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task RealNull_Single_SetDiscardNullTest()
        {
            await SetDiscardNullTest<float>("RealNull", typeof(SingleSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task RealNotNull_Single_SetDiscardNullTest()
        {
            await SetDiscardNullTest<float>("RealNotNull", typeof(SingleSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DateNull_DateTime_SetDiscardNullTest()
        {
            await SetDiscardNullTest<DateTime>("DateNull", typeof(DateTimeSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DateNotNull_DateTime_SetDiscardNullTest()
        {
            await SetDiscardNullTest<DateTime>("DateNotNull", typeof(DateTimeSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeoffsetNull_DateTimeOffset_SetDiscardNullTest()
        {
            await SetDiscardNullTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetSetMaterializer<SqlCommand, SqlParameter>));
        }

        [TestMethod]
        public async Task DatetimeoffsetNotNull_DateTimeOffset_SetDiscardNullTest()
        {
            await SetDiscardNullTest<DateTimeOffset>("DatetimeoffsetNotNull", typeof(DateTimeOffsetSetMaterializer<SqlCommand, SqlParameter>));
        }

        [TestMethod]
        public async Task Datetime2Null_DateTime_SetDiscardNullTest()
        {
            await SetDiscardNullTest<DateTime>("Datetime2Null", typeof(DateTimeSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task Datetime2NotNull_DateTime_SetDiscardNullTest()
        {
            await SetDiscardNullTest<DateTime>("Datetime2NotNull", typeof(DateTimeSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmalldatetimeNull_DateTime_SetDiscardNullTest()
        {
            await SetDiscardNullTest<DateTime>("SmalldatetimeNull", typeof(DateTimeSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmalldatetimeNotNull_DateTime_SetDiscardNullTest()
        {
            await SetDiscardNullTest<DateTime>("SmalldatetimeNotNull", typeof(DateTimeSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeNull_DateTime_SetDiscardNullTest()
        {
            await SetDiscardNullTest<DateTime>("DatetimeNull", typeof(DateTimeSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeNotNull_DateTime_SetDiscardNullTest()
        {
            await SetDiscardNullTest<DateTime>("DatetimeNotNull", typeof(DateTimeSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TimeNull_TimeSpan_SetDiscardNullTest()
        {
            await SetDiscardNullTest<TimeSpan>("TimeNull", typeof(TimeSpanSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TimeNotNull_TimeSpan_SetDiscardNullTest()
        {
            await SetDiscardNullTest<TimeSpan>("TimeNotNull", typeof(TimeSpanSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TextNull_String_SetDiscardNullTest()
        {
            await SetDiscardNullTest<string>("TextNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TextNotNull_String_SetDiscardNullTest()
        {
            await SetDiscardNullTest<string>("TextNotNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NTextNull_String_SetDiscardNullTest()
        {
            await SetDiscardNullTest<string>("NTextNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NTextNotNull_String_SetDiscardNullTest()
        {
            await SetDiscardNullTest<string>("NTextNotNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task UniqueidentifierNull_Guid_SetDiscardNullTest()
        {
            await SetDiscardNullTest<Guid>("UniqueidentifierNull", typeof(GuidSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task UniqueidentifierNotNull_Guid_SetDiscardNullTest()
        {
            await SetDiscardNullTest<Guid>("UniqueidentifierNotNull", typeof(GuidSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task XmlNull_String_SetDiscardNullTest()
        {
            await SetDiscardNullTest<string>("XmlNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task XmlNotNull_String_SetDiscardNullTest()
        {
            await SetDiscardNullTest<string>("XmlNotNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }



        //*****************************


        [TestMethod]
        public async Task CharNull_String_SetWithNullsTest()
        {
            await SetWithNullsTest<string>("CharNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task VarCharNull_String_SetWithNullsTest()
        {
            await SetWithNullsTest<string>("VarCharNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NCharNull_String_SetWithNullsTest()
        {
            await SetWithNullsTest<string>("NCharNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NVarCharNull_String_SetWithNullsTest()
        {
            await SetWithNullsTest<string>("NVarCharNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


              


        [TestMethod]
        public async Task IntNull_Int32_SetWithNullsTest()
        {
            await FailedSetWithNullsTest<int>("IntNull", typeof(Int32SetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task BigIntNull_Int64_SetWithNullsTest()
        {
            await FailedSetWithNullsTest<long>("BigIntNull", typeof(Int64SetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NumericNull_Decimal_SetWithNullsTest()
        {
            await FailedSetWithNullsTest<decimal>("NumericNull", typeof(DecimalSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallIntNull_Int16_SetWithNullsTest()
        {
            await FailedSetWithNullsTest<short>("SmallIntNull", typeof(Int16SetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DecimalNull_Decimal_SetWithNullsTest()
        {
            await FailedSetWithNullsTest<decimal>("DecimalNull", typeof(DecimalSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmallMoneyNull_Decimal_SetWithNullsTest()
        {
            await FailedSetWithNullsTest<decimal>("SmallMoneyNull", typeof(DecimalSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task MoneyNull_Decimal_SetWithNullsTest()
        {
            await FailedSetWithNullsTest<decimal>("MoneyNull", typeof(DecimalSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task FloatNull_Double_SetWithNullsTest()
        {
            await FailedSetWithNullsTest<double>("FloatNull", typeof(DoubleSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task RealNull_Single_SetWithNullsTest()
        {
            await FailedSetWithNullsTest<float>("RealNull", typeof(SingleSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DateNull_DateTime_SetWithNullsTest()
        {
            await FailedSetWithNullsTest<DateTime>("DateNull", typeof(DateTimeSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeoffsetNull_DateTimeOffset_SetWithNullsTest()
        {
            await FailedSetWithNullsTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task Datetime2Null_DateTime_SetWithNullsTest()
        {
            await FailedSetWithNullsTest<DateTime>("Datetime2Null", typeof(DateTimeSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task SmalldatetimeNull_DateTime_SetWithNullsTest()
        {
            await FailedSetWithNullsTest<DateTime>("SmalldatetimeNull", typeof(DateTimeSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task DatetimeNull_DateTime_SetWithNullsTest()
        {
            await FailedSetWithNullsTest<DateTime>("DatetimeNull", typeof(DateTimeSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TimeNull_TimeSpan_SetWithNullsTest()
        {
            await FailedSetWithNullsTest<TimeSpan>("TimeNull", typeof(TimeSpanSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task TextNull_String_SetWithNullsTest()
        {
            await SetWithNullsTest<string>("TextNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }


        [TestMethod]
        public async Task NTextNull_String_SetWithNullsTest()
        {
            await SetWithNullsTest<string>("NTextNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }

        [TestMethod]
        public async Task UniqueidentifierNull_Guid_SetWithNullsTest()
        {
            await FailedSetWithNullsTest<Guid>("UniqueidentifierNull", typeof(GuidSetMaterializer<SqlCommand, SqlParameter>));
        }

        [TestMethod]
        public async Task XmlNull_String_SetWithNullsTest()
        {
            await SetWithNullsTest<string>("XmlNull", typeof(StringSetMaterializer<SqlCommand, SqlParameter>));
        }

    }
}



