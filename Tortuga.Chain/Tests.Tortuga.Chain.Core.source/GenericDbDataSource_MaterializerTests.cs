using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tortuga.Chain;
using Tortuga.Chain.Materializers;

#if JSON_CONFIG

using Microsoft.Extensions.Configuration;

#else
using System.Configuration;

#endif

namespace Tests.Materializers
{
    [TestClass]
    public class GenericDbDataSource_MaterializerTests
    {
        const string CustomerWithOrdersByState = @"SELECT  *
    FROM    Sales.Customer c
    WHERE   c.State = @State;

    SELECT  o.*
    FROM    Sales.[Order] o
            INNER JOIN Sales.Customer c ON o.CustomerKey = c.CustomerKey
    WHERE   c.State = @State;";

        static readonly GenericDbDataSource s_DataSource;

        static GenericDbDataSource_MaterializerTests()
        {
#if JSON_CONFIG
            var configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json").Build();
            var connectionString = configuration.GetConnectionString("SqlServerTestDatabase");
#else
            var connectionString = ConfigurationManager.ConnectionStrings["SqlServerTestDatabase"].ConnectionString;
#endif
            s_DataSource = new GenericDbDataSource(SqlClientFactory.Instance, "SqlServerTestDatabase", connectionString);
        }

        public static GenericDbDataSource DataSource
        {
            get { return s_DataSource; }
        }

        [TestMethod]
        public async Task BigIntNotNull_Int64_ListDisardNullTest()
        {
            await ListDisardNullTest<long>("BigIntNotNull", typeof(Int64ListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BigIntNotNull_Int64_ListTest()
        {
            await ListTest<long>("BigIntNotNull", typeof(Int64ListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BigIntNotNull_Int64_ScalarTest()
        {
            await ScalarTest<long>("BigIntNotNull", typeof(Int64Materializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BigIntNull_Int64_ListDisardNullTest()
        {
            await ListDisardNullTest<long>("BigIntNull", typeof(Int64ListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BigIntNull_Int64_ListTest()
        {
            await ListTest<long>("BigIntNull", typeof(Int64ListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BigIntNull_Int64_ListWithNullsTest()
        {
            await FailedListWithNullsTest<long>("BigIntNull", typeof(Int64ListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BigIntNull_Int64_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<long>("BigIntNull", typeof(Int64Materializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BigIntNull_Int64_ScalarOrNullTest()
        {
            await ScalarOrNullTest<long?>("BigIntNull", typeof(Int64OrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BigIntNull_Int64_ScalarTest()
        {
            await ScalarTest<long>("BigIntNull", typeof(Int64Materializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BinaryNotNull_ByteArray_ListDisardNullTest()
        {
            await ListDisardNullTest<byte[]>("BinaryNotNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BinaryNotNull_ByteArray_ListTest()
        {
            await ListTest<byte[]>("BinaryNotNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BinaryNotNull_ByteArray_ScalarTest()
        {
            await ScalarTest<byte[]>("BinaryNotNull", typeof(ByteArrayMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BinaryNull_ByteArray_ListDisardNullTest()
        {
            await ListDisardNullTest<byte[]>("BinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BinaryNull_ByteArray_ListTest()
        {
            await ListTest<byte[]>("BinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BinaryNull_ByteArray_ListWithNullsTest()
        {
            await FailedListWithNullsTest<byte[]>("BinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BinaryNull_ByteArray_ScalarOrNullTest()
        {
            await ScalarOrNullTest<byte[]>("BinaryNull", typeof(ByteArrayMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BinaryNull_ByteArray_ScalarTest()
        {
            await ScalarTest<byte[]>("BinaryNull", typeof(ByteArrayMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BitNotNull_Boolean_ListDisardNullTest()
        {
            await ListDisardNullTest<bool>("BitNotNull", typeof(BooleanListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BitNotNull_Boolean_ListTest()
        {
            await ListTest<bool>("BitNotNull", typeof(BooleanListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BitNotNull_Boolean_ScalarTest()
        {
            await ScalarTest<bool>("BitNotNull", typeof(BooleanMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BitNull_Boolean_ListDisardNullTest()
        {
            await ListDisardNullTest<bool>("BitNull", typeof(BooleanListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BitNull_Boolean_ListTest()
        {
            await ListTest<bool>("BitNull", typeof(BooleanListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BitNull_Boolean_ListWithNullsTest()
        {
            await FailedListWithNullsTest<bool>("BitNull", typeof(BooleanListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BitNull_Boolean_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<bool>("BitNull", typeof(BooleanMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BitNull_Boolean_ScalarOrNullTest()
        {
            await ScalarOrNullTest<bool?>("BitNull", typeof(BooleanOrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task BitNull_Boolean_ScalarTest()
        {
            await ScalarTest<bool>("BitNull", typeof(BooleanMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task CharNotNull_String_ListDisardNullTest()
        {
            await ListDisardNullTest<string>("CharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task CharNotNull_String_ListTest()
        {
            await ListTest<string>("CharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task CharNotNull_String_ScalarTest()
        {
            await ScalarTest<string>("CharNotNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task CharNull_String_ListDisardNullTest()
        {
            await ListDisardNullTest<string>("CharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task CharNull_String_ListTest()
        {
            await ListTest<string>("CharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task CharNull_String_ListWithNullsTest()
        {
            await ListWithNullsTest<string>("CharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task CharNull_String_ScalarOrNullTest()
        {
            await ScalarOrNullTest<string>("CharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task CharNull_String_ScalarTest()
        {
            await ScalarTest<string>("CharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DataTable()
        {
            var materializer = DataSource.Sql($"SELECT * FROM dbo.AllTypes").ToDataTable();
            var result1 = materializer.Execute();
            var result1a = await materializer.ExecuteAsync();
        }

        [TestMethod]
        public async Task DataTableSet()
        {
            var materializer = DataSource.Sql(CustomerWithOrdersByState, new { State = "CA" }).ToDataSet("Customers", "Orders");
            var result1 = materializer.Execute();
            var result1a = await materializer.ExecuteAsync();
        }

        [TestMethod]
        public async Task DataTableSet_Dict()
        {
            var materializer = DataSource.Sql(CustomerWithOrdersByState, new Dictionary<string, object>() { { "State", "CA" } }).ToDataSet("Customers", "Orders");
            var result1 = materializer.Execute();
            var result1a = await materializer.ExecuteAsync();
        }

        [TestMethod]
        public async Task DataTableSet_Dict2()
        {
            var materializer = DataSource.Sql(CustomerWithOrdersByState, new Dictionary<string, object>() { { "@State", "CA" } }).ToDataSet("Customers", "Orders");
            var result1 = materializer.Execute();
            var result1a = await materializer.ExecuteAsync();
        }

        [TestMethod]
        public async Task DateNotNull_DateTime_ListDisardNullTest()
        {
            await ListDisardNullTest<DateTime>("DateNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DateNotNull_DateTime_ListTest()
        {
            await ListTest<DateTime>("DateNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DateNotNull_DateTime_ScalarTest()
        {
            await ScalarTest<DateTime>("DateNotNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DateNull_DateTime_ListDisardNullTest()
        {
            await ListDisardNullTest<DateTime>("DateNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DateNull_DateTime_ListTest()
        {
            await ListTest<DateTime>("DateNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DateNull_DateTime_ListWithNullsTest()
        {
            await FailedListWithNullsTest<DateTime>("DateNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DateNull_DateTime_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<DateTime>("DateNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DateNull_DateTime_ScalarOrNullTest()
        {
            await ScalarOrNullTest<DateTime?>("DateNull", typeof(DateTimeOrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DateNull_DateTime_ScalarTest()
        {
            await ScalarTest<DateTime>("DateNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task Datetime2NotNull_DateTime_ListDisardNullTest()
        {
            await ListDisardNullTest<DateTime>("Datetime2NotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task Datetime2NotNull_DateTime_ListTest()
        {
            await ListTest<DateTime>("Datetime2NotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task Datetime2NotNull_DateTime_ScalarTest()
        {
            await ScalarTest<DateTime>("Datetime2NotNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task Datetime2Null_DateTime_ListDisardNullTest()
        {
            await ListDisardNullTest<DateTime>("Datetime2Null", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task Datetime2Null_DateTime_ListTest()
        {
            await ListTest<DateTime>("Datetime2Null", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task Datetime2Null_DateTime_ListWithNullsTest()
        {
            await FailedListWithNullsTest<DateTime>("Datetime2Null", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task Datetime2Null_DateTime_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<DateTime>("Datetime2Null", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task Datetime2Null_DateTime_ScalarOrNullTest()
        {
            await ScalarOrNullTest<DateTime?>("Datetime2Null", typeof(DateTimeOrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task Datetime2Null_DateTime_ScalarTest()
        {
            await ScalarTest<DateTime>("Datetime2Null", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeNotNull_DateTime_ListDisardNullTest()
        {
            await ListDisardNullTest<DateTime>("DatetimeNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeNotNull_DateTime_ListTest()
        {
            await ListTest<DateTime>("DatetimeNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeNotNull_DateTime_ScalarTest()
        {
            await ScalarTest<DateTime>("DatetimeNotNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeNull_DateTime_ListDisardNullTest()
        {
            await ListDisardNullTest<DateTime>("DatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeNull_DateTime_ListTest()
        {
            await ListTest<DateTime>("DatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeNull_DateTime_ListWithNullsTest()
        {
            await FailedListWithNullsTest<DateTime>("DatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeNull_DateTime_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<DateTime>("DatetimeNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeNull_DateTime_ScalarOrNullTest()
        {
            await ScalarOrNullTest<DateTime?>("DatetimeNull", typeof(DateTimeOrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeNull_DateTime_ScalarTest()
        {
            await ScalarTest<DateTime>("DatetimeNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeoffsetNotNull_DateTimeOffset_ListDisardNullTest()
        {
            await ListDisardNullTest<DateTimeOffset>("DatetimeoffsetNotNull", typeof(DateTimeOffsetListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeoffsetNotNull_DateTimeOffset_ListTest()
        {
            await ListTest<DateTimeOffset>("DatetimeoffsetNotNull", typeof(DateTimeOffsetListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeoffsetNotNull_DateTimeOffset_ScalarTest()
        {
            await ScalarTest<DateTimeOffset>("DatetimeoffsetNotNull", typeof(DateTimeOffsetMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeoffsetNull_DateTimeOffset_ListDisardNullTest()
        {
            await ListDisardNullTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeoffsetNull_DateTimeOffset_ListTest()
        {
            await ListTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeoffsetNull_DateTimeOffset_ListWithNullsTest()
        {
            await FailedListWithNullsTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeoffsetNull_DateTimeOffset_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeoffsetNull_DateTimeOffset_ScalarOrNullTest()
        {
            await ScalarOrNullTest<DateTimeOffset?>("DatetimeoffsetNull", typeof(DateTimeOffsetOrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DatetimeoffsetNull_DateTimeOffset_ScalarTest()
        {
            await ScalarTest<DateTimeOffset>("DatetimeoffsetNull", typeof(DateTimeOffsetMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DecimalNotNull_Decimal_ListDisardNullTest()
        {
            await ListDisardNullTest<decimal>("DecimalNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DecimalNotNull_Decimal_ListTest()
        {
            await ListTest<decimal>("DecimalNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DecimalNotNull_Decimal_ScalarTest()
        {
            await ScalarTest<decimal>("DecimalNotNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DecimalNull_Decimal_ListDisardNullTest()
        {
            await ListDisardNullTest<decimal>("DecimalNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DecimalNull_Decimal_ListTest()
        {
            await ListTest<decimal>("DecimalNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DecimalNull_Decimal_ListWithNullsTest()
        {
            await FailedListWithNullsTest<decimal>("DecimalNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DecimalNull_Decimal_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<decimal>("DecimalNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DecimalNull_Decimal_ScalarOrNullTest()
        {
            await ScalarOrNullTest<decimal?>("DecimalNull", typeof(DecimalOrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task DecimalNull_Decimal_ScalarTest()
        {
            await ScalarTest<decimal>("DecimalNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task FloatNotNull_Double_ListDisardNullTest()
        {
            await ListDisardNullTest<double>("FloatNotNull", typeof(DoubleListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task FloatNotNull_Double_ListTest()
        {
            await ListTest<double>("FloatNotNull", typeof(DoubleListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task FloatNotNull_Double_ScalarTest()
        {
            await ScalarTest<double>("FloatNotNull", typeof(DoubleMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task FloatNull_Double_ListDisardNullTest()
        {
            await ListDisardNullTest<double>("FloatNull", typeof(DoubleListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task FloatNull_Double_ListTest()
        {
            await ListTest<double>("FloatNull", typeof(DoubleListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task FloatNull_Double_ListWithNullsTest()
        {
            await FailedListWithNullsTest<double>("FloatNull", typeof(DoubleListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task FloatNull_Double_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<double>("FloatNull", typeof(DoubleMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task FloatNull_Double_ScalarOrNullTest()
        {
            await ScalarOrNullTest<double?>("FloatNull", typeof(DoubleOrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task FloatNull_Double_ScalarTest()
        {
            await ScalarTest<double>("FloatNull", typeof(DoubleMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task IntNotNull_Int32_ListDisardNullTest()
        {
            await ListDisardNullTest<int>("IntNotNull", typeof(Int32ListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task IntNotNull_Int32_ListTest()
        {
            await ListTest<int>("IntNotNull", typeof(Int32ListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task IntNotNull_Int32_ScalarTest()
        {
            await ScalarTest<int>("IntNotNull", typeof(Int32Materializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task IntNull_Int32_ListDisardNullTest()
        {
            await ListDisardNullTest<int>("IntNull", typeof(Int32ListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task IntNull_Int32_ListTest()
        {
            await ListTest<int>("IntNull", typeof(Int32ListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task IntNull_Int32_ListWithNullsTest()
        {
            await FailedListWithNullsTest<int>("IntNull", typeof(Int32ListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task IntNull_Int32_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<int>("IntNull", typeof(Int32Materializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task IntNull_Int32_ScalarOrNullTest()
        {
            await ScalarOrNullTest<int?>("IntNull", typeof(Int32OrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task IntNull_Int32_ScalarTest()
        {
            await ScalarTest<int>("IntNull", typeof(Int32Materializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task MoneyNotNull_Decimal_ListDisardNullTest()
        {
            await ListDisardNullTest<decimal>("MoneyNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task MoneyNotNull_Decimal_ListTest()
        {
            await ListTest<decimal>("MoneyNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task MoneyNotNull_Decimal_ScalarTest()
        {
            await ScalarTest<decimal>("MoneyNotNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task MoneyNull_Decimal_ListDisardNullTest()
        {
            await ListDisardNullTest<decimal>("MoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task MoneyNull_Decimal_ListTest()
        {
            await ListTest<decimal>("MoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task MoneyNull_Decimal_ListWithNullsTest()
        {
            await FailedListWithNullsTest<decimal>("MoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task MoneyNull_Decimal_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<decimal>("MoneyNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task MoneyNull_Decimal_ScalarOrNullTest()
        {
            await ScalarOrNullTest<decimal?>("MoneyNull", typeof(DecimalOrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task MoneyNull_Decimal_ScalarTest()
        {
            await ScalarTest<decimal>("MoneyNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NCharNotNull_String_ListDisardNullTest()
        {
            await ListDisardNullTest<string>("NCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NCharNotNull_String_ListTest()
        {
            await ListTest<string>("NCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NCharNotNull_String_ScalarTest()
        {
            await ScalarTest<string>("NCharNotNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NCharNull_String_ListDisardNullTest()
        {
            await ListDisardNullTest<string>("NCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NCharNull_String_ListTest()
        {
            await ListTest<string>("NCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NCharNull_String_ListWithNullsTest()
        {
            await ListWithNullsTest<string>("NCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NCharNull_String_ScalarOrNullTest()
        {
            await ScalarOrNullTest<string>("NCharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NCharNull_String_ScalarTest()
        {
            await ScalarTest<string>("NCharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NTextNotNull_String_ListDisardNullTest()
        {
            await ListDisardNullTest<string>("NTextNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NTextNotNull_String_ListTest()
        {
            await ListTest<string>("NTextNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NTextNotNull_String_ScalarTest()
        {
            await ScalarTest<string>("NTextNotNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NTextNull_String_ListDisardNullTest()
        {
            await ListDisardNullTest<string>("NTextNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NTextNull_String_ListTest()
        {
            await ListTest<string>("NTextNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NTextNull_String_ListWithNullsTest()
        {
            await ListWithNullsTest<string>("NTextNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NTextNull_String_ScalarOrNullTest()
        {
            await ScalarOrNullTest<string>("NTextNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NTextNull_String_ScalarTest()
        {
            await ScalarTest<string>("NTextNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NumericNotNull_Decimal_ListDisardNullTest()
        {
            await ListDisardNullTest<decimal>("NumericNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NumericNotNull_Decimal_ListTest()
        {
            await ListTest<decimal>("NumericNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NumericNotNull_Decimal_ScalarTest()
        {
            await ScalarTest<decimal>("NumericNotNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NumericNull_Decimal_ListDisardNullTest()
        {
            await ListDisardNullTest<decimal>("NumericNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NumericNull_Decimal_ListTest()
        {
            await ListTest<decimal>("NumericNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NumericNull_Decimal_ListWithNullsTest()
        {
            await FailedListWithNullsTest<decimal>("NumericNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NumericNull_Decimal_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<decimal>("NumericNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NumericNull_Decimal_ScalarOrNullTest()
        {
            await ScalarOrNullTest<decimal?>("NumericNull", typeof(DecimalOrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NumericNull_Decimal_ScalarTest()
        {
            await ScalarTest<decimal>("NumericNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NVarCharNotNull_String_ListDisardNullTest()
        {
            await ListDisardNullTest<string>("NVarCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NVarCharNotNull_String_ListTest()
        {
            await ListTest<string>("NVarCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NVarCharNotNull_String_ScalarTest()
        {
            await ScalarTest<string>("NVarCharNotNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NVarCharNull_String_ListDisardNullTest()
        {
            await ListDisardNullTest<string>("NVarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NVarCharNull_String_ListTest()
        {
            await ListTest<string>("NVarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NVarCharNull_String_ListWithNullsTest()
        {
            await ListWithNullsTest<string>("NVarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NVarCharNull_String_ScalarOrNullTest()
        {
            await ScalarOrNullTest<string>("NVarCharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task NVarCharNull_String_ScalarTest()
        {
            await ScalarTest<string>("NVarCharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task RealNotNull_Single_ListDisardNullTest()
        {
            await ListDisardNullTest<float>("RealNotNull", typeof(SingleListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task RealNotNull_Single_ListTest()
        {
            await ListTest<float>("RealNotNull", typeof(SingleListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task RealNotNull_Single_ScalarTest()
        {
            await ScalarTest<float>("RealNotNull", typeof(SingleMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task RealNull_Single_ListDisardNullTest()
        {
            await ListDisardNullTest<float>("RealNull", typeof(SingleListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task RealNull_Single_ListTest()
        {
            await ListTest<float>("RealNull", typeof(SingleListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task RealNull_Single_ListWithNullsTest()
        {
            await FailedListWithNullsTest<float>("RealNull", typeof(SingleListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task RealNull_Single_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<float>("RealNull", typeof(SingleMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task RealNull_Single_ScalarOrNullTest()
        {
            await ScalarOrNullTest<float?>("RealNull", typeof(SingleOrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task RealNull_Single_ScalarTest()
        {
            await ScalarTest<float>("RealNull", typeof(SingleMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmalldatetimeNotNull_DateTime_ListDisardNullTest()
        {
            await ListDisardNullTest<DateTime>("SmalldatetimeNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmalldatetimeNotNull_DateTime_ListTest()
        {
            await ListTest<DateTime>("SmalldatetimeNotNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmalldatetimeNotNull_DateTime_ScalarTest()
        {
            await ScalarTest<DateTime>("SmalldatetimeNotNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmalldatetimeNull_DateTime_ListDisardNullTest()
        {
            await ListDisardNullTest<DateTime>("SmalldatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmalldatetimeNull_DateTime_ListTest()
        {
            await ListTest<DateTime>("SmalldatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmalldatetimeNull_DateTime_ListWithNullsTest()
        {
            await FailedListWithNullsTest<DateTime>("SmalldatetimeNull", typeof(DateTimeListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmalldatetimeNull_DateTime_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<DateTime>("SmalldatetimeNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmalldatetimeNull_DateTime_ScalarOrNullTest()
        {
            await ScalarOrNullTest<DateTime?>("SmalldatetimeNull", typeof(DateTimeOrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmalldatetimeNull_DateTime_ScalarTest()
        {
            await ScalarTest<DateTime>("SmalldatetimeNull", typeof(DateTimeMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallIntNotNull_Int16_ListDisardNullTest()
        {
            await ListDisardNullTest<short>("SmallIntNotNull", typeof(Int16ListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallIntNotNull_Int16_ListTest()
        {
            await ListTest<short>("SmallIntNotNull", typeof(Int16ListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallIntNotNull_Int16_ScalarTest()
        {
            await ScalarTest<short>("SmallIntNotNull", typeof(Int16Materializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallIntNull_Int16_ListDisardNullTest()
        {
            await ListDisardNullTest<short>("SmallIntNull", typeof(Int16ListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallIntNull_Int16_ListTest()
        {
            await ListTest<short>("SmallIntNull", typeof(Int16ListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallIntNull_Int16_ListWithNullsTest()
        {
            await FailedListWithNullsTest<short>("SmallIntNull", typeof(Int16ListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallIntNull_Int16_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<short>("SmallIntNull", typeof(Int16Materializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallIntNull_Int16_ScalarOrNullTest()
        {
            await ScalarOrNullTest<short?>("SmallIntNull", typeof(Int16OrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallIntNull_Int16_ScalarTest()
        {
            await ScalarTest<short>("SmallIntNull", typeof(Int16Materializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallMoneyNotNull_Decimal_ListDisardNullTest()
        {
            await ListDisardNullTest<decimal>("SmallMoneyNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallMoneyNotNull_Decimal_ListTest()
        {
            await ListTest<decimal>("SmallMoneyNotNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallMoneyNotNull_Decimal_ScalarTest()
        {
            await ScalarTest<decimal>("SmallMoneyNotNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallMoneyNull_Decimal_ListDisardNullTest()
        {
            await ListDisardNullTest<decimal>("SmallMoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallMoneyNull_Decimal_ListTest()
        {
            await ListTest<decimal>("SmallMoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallMoneyNull_Decimal_ListWithNullsTest()
        {
            await FailedListWithNullsTest<decimal>("SmallMoneyNull", typeof(DecimalListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallMoneyNull_Decimal_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<decimal>("SmallMoneyNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallMoneyNull_Decimal_ScalarOrNullTest()
        {
            await ScalarOrNullTest<decimal?>("SmallMoneyNull", typeof(DecimalOrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task SmallMoneyNull_Decimal_ScalarTest()
        {
            await ScalarTest<decimal>("SmallMoneyNull", typeof(DecimalMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task Table()
        {
            var materializer = DataSource.Sql($"SELECT * FROM dbo.AllTypes").ToTable();
            var result1 = materializer.Execute();
            var result1a = await materializer.ExecuteAsync();
        }

        //*****************************
        [TestMethod]
        public async Task TableSet()
        {
            var materializer = DataSource.Sql(CustomerWithOrdersByState, new { State = "CA" }).ToTableSet();
            var result1 = materializer.Execute();
            var result1a = await materializer.ExecuteAsync();
        }

        [TestMethod]
        public async Task TableSet_Dict()
        {
            var materializer = DataSource.Sql(CustomerWithOrdersByState, new Dictionary<string, object>() { { "State", "CA" } }).ToTableSet();
            var result1 = materializer.Execute();
            var result1a = await materializer.ExecuteAsync();
        }

        [TestMethod]
        public async Task TableSet_Dict2()
        {
            var materializer = DataSource.Sql(CustomerWithOrdersByState, new Dictionary<string, object>() { { "@State", "CA" } }).ToTableSet();
            var result1 = materializer.Execute();
            var result1a = await materializer.ExecuteAsync();
        }

        [TestMethod]
        public async Task TableSet2()
        {
            var materializer = DataSource.Sql(CustomerWithOrdersByState, new { State = "CA" }).ToTableSet("Customers", "Orders");
            var result1 = materializer.Execute();
            var result1a = await materializer.ExecuteAsync();
        }

        [TestMethod]
        public async Task TextNotNull_String_ListDisardNullTest()
        {
            await ListDisardNullTest<string>("TextNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TextNotNull_String_ListTest()
        {
            await ListTest<string>("TextNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TextNotNull_String_ScalarTest()
        {
            await ScalarTest<string>("TextNotNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TextNull_String_ListDisardNullTest()
        {
            await ListDisardNullTest<string>("TextNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TextNull_String_ListTest()
        {
            await ListTest<string>("TextNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TextNull_String_ListWithNullsTest()
        {
            await ListWithNullsTest<string>("TextNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TextNull_String_ScalarOrNullTest()
        {
            await ScalarOrNullTest<string>("TextNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TextNull_String_ScalarTest()
        {
            await ScalarTest<string>("TextNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TimeNotNull_TimeSpan_ListDisardNullTest()
        {
            await ListDisardNullTest<TimeSpan>("TimeNotNull", typeof(TimeSpanListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TimeNotNull_TimeSpan_ListTest()
        {
            await ListTest<TimeSpan>("TimeNotNull", typeof(TimeSpanListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TimeNotNull_TimeSpan_ScalarTest()
        {
            await ScalarTest<TimeSpan>("TimeNotNull", typeof(TimeSpanMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TimeNull_TimeSpan_ListDisardNullTest()
        {
            await ListDisardNullTest<TimeSpan>("TimeNull", typeof(TimeSpanListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TimeNull_TimeSpan_ListTest()
        {
            await ListTest<TimeSpan>("TimeNull", typeof(TimeSpanListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TimeNull_TimeSpan_ListWithNullsTest()
        {
            await FailedListWithNullsTest<TimeSpan>("TimeNull", typeof(TimeSpanListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TimeNull_TimeSpan_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<TimeSpan>("TimeNull", typeof(TimeSpanMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TimeNull_TimeSpan_ScalarOrNullTest()
        {
            await ScalarOrNullTest<TimeSpan?>("TimeNull", typeof(TimeSpanOrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TimeNull_TimeSpan_ScalarTest()
        {
            await ScalarTest<TimeSpan>("TimeNull", typeof(TimeSpanMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TinyIntNotNull_Byte_ListDisardNullTest()
        {
            await ListDisardNullTest<byte>("TinyIntNotNull", typeof(ByteListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TinyIntNotNull_Byte_ListTest()
        {
            await ListTest<byte>("TinyIntNotNull", typeof(ByteListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TinyIntNotNull_Byte_ScalarTest()
        {
            await ScalarTest<byte>("TinyIntNotNull", typeof(ByteMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TinyIntNull_Byte_ListDisardNullTest()
        {
            await ListDisardNullTest<byte>("TinyIntNull", typeof(ByteListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TinyIntNull_Byte_ListTest()
        {
            await ListTest<byte>("TinyIntNull", typeof(ByteListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TinyIntNull_Byte_ListWithNullsTest()
        {
            await FailedListWithNullsTest<byte>("TinyIntNull", typeof(ByteListMaterializer<DbCommand, DbParameter>));
        }

        //*****************************
        [TestMethod]
        public async Task TinyIntNull_Byte_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<byte>("TinyIntNull", typeof(ByteMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TinyIntNull_Byte_ScalarOrNullTest()
        {
            await ScalarOrNullTest<byte?>("TinyIntNull", typeof(ByteOrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task TinyIntNull_Byte_ScalarTest()
        {
            await ScalarTest<byte>("TinyIntNull", typeof(ByteMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task UniqueidentifierNotNull_Guid_ListDisardNullTest()
        {
            await ListDisardNullTest<Guid>("UniqueidentifierNotNull", typeof(GuidListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task UniqueidentifierNotNull_Guid_ListTest()
        {
            await ListTest<Guid>("UniqueidentifierNotNull", typeof(GuidListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task UniqueidentifierNotNull_Guid_ScalarTest()
        {
            await ScalarTest<Guid>("UniqueidentifierNotNull", typeof(GuidMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task UniqueidentifierNull_Guid_ListDisardNullTest()
        {
            await ListDisardNullTest<Guid>("UniqueidentifierNull", typeof(GuidListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task UniqueidentifierNull_Guid_ListTest()
        {
            await ListTest<Guid>("UniqueidentifierNull", typeof(GuidListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task UniqueidentifierNull_Guid_ListWithNullsTest()
        {
            await FailedListWithNullsTest<Guid>("UniqueidentifierNull", typeof(GuidListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task UniqueidentifierNull_Guid_ScalarFailOnNullTest()
        {
            await ScalarFailOnNullTest<Guid>("UniqueidentifierNull", typeof(GuidMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task UniqueidentifierNull_Guid_ScalarOrNullTest()
        {
            await ScalarOrNullTest<Guid?>("UniqueidentifierNull", typeof(GuidOrNullMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task UniqueidentifierNull_Guid_ScalarTest()
        {
            await ScalarTest<Guid>("UniqueidentifierNull", typeof(GuidMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task VarBinaryNotNull_ByteArray_ListDisardNullTest()
        {
            await ListDisardNullTest<byte[]>("VarBinaryNotNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task VarBinaryNotNull_ByteArray_ListTest()
        {
            await ListTest<byte[]>("VarBinaryNotNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task VarBinaryNotNull_ByteArray_ScalarTest()
        {
            await ScalarTest<byte[]>("VarBinaryNotNull", typeof(ByteArrayMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task VarBinaryNull_ByteArray_ListDisardNullTest()
        {
            await ListDisardNullTest<byte[]>("VarBinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task VarBinaryNull_ByteArray_ListTest()
        {
            await ListTest<byte[]>("VarBinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task VarBinaryNull_ByteArray_ListWithNullsTest()
        {
            await FailedListWithNullsTest<byte[]>("VarBinaryNull", typeof(ByteArrayListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task VarBinaryNull_ByteArray_ScalarOrNullTest()
        {
            await ScalarOrNullTest<byte[]>("VarBinaryNull", typeof(ByteArrayMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task VarBinaryNull_ByteArray_ScalarTest()
        {
            await ScalarTest<byte[]>("VarBinaryNull", typeof(ByteArrayMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task VarCharNotNull_String_ListDisardNullTest()
        {
            await ListDisardNullTest<string>("VarCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task VarCharNotNull_String_ListTest()
        {
            await ListTest<string>("VarCharNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task VarCharNotNull_String_ScalarTest()
        {
            await ScalarTest<string>("VarCharNotNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        //*****************************
        [TestMethod]
        public async Task VarCharNull_String_ListDisardNullTest()
        {
            await ListDisardNullTest<string>("VarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task VarCharNull_String_ListTest()
        {
            await ListTest<string>("VarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        //*****************************
        [TestMethod]
        public async Task VarCharNull_String_ListWithNullsTest()
        {
            await ListWithNullsTest<string>("VarCharNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        //*****************************
        [TestMethod]
        public async Task VarCharNull_String_ScalarOrNullTest()
        {
            await ScalarOrNullTest<string>("VarCharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task VarCharNull_String_ScalarTest()
        {
            await ScalarTest<string>("VarCharNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task XmlNotNull_String_ListDisardNullTest()
        {
            await ListDisardNullTest<string>("XmlNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task XmlNotNull_String_ListTest()
        {
            await ListTest<string>("XmlNotNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task XmlNotNull_String_ScalarTest()
        {
            await ScalarTest<string>("XmlNotNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task XmlNotNull_XElement_ListTest()
        {
            await ListTest<XElement>("XmlNotNull", typeof(XElementListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task XmlNotNull_XElement_ScalarTest()
        {
            await ScalarTest<XElement>("XmlNotNull", typeof(XElementMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task XmlNull_String_ListDisardNullTest()
        {
            await ListDisardNullTest<string>("XmlNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task XmlNull_String_ListTest()
        {
            await ListTest<string>("XmlNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task XmlNull_String_ListWithNullsTest()
        {
            await ListWithNullsTest<string>("XmlNull", typeof(StringListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task XmlNull_String_ScalarOrNullTest()
        {
            await ScalarOrNullTest<string>("XmlNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task XmlNull_String_ScalarTest()
        {
            await ScalarTest<string>("XmlNull", typeof(StringMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task XmlNull_XElement_ListTest()
        {
            await ListTest<XElement>("XmlNull", typeof(XElementListMaterializer<DbCommand, DbParameter>));
        }

        [TestMethod]
        public async Task XmlNull_XElement_ScalarTest()
        {
            await ScalarTest<XElement>("XmlNull", typeof(XElementMaterializer<DbCommand, DbParameter>));
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

        async Task ListDisardNullTest<TResult>(string columnName, Type materializerType)
        {
            var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (1, 2, 3, 4)");
            ILink<List<TResult>> materializer1 = (ILink<List<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.DiscardNulls });
            var result1 = materializer1.Execute();
            var result1a = await materializer1.ExecuteAsync();
            Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
            Assert.IsTrue(result1.All(x => x != null));
        }

        async Task ListTest<TResult>(string columnName, Type materializerType)
        {
            var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (1, 2, 4)");
            ILink<List<TResult>> materializer1 = (ILink<List<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.None });
            var result1 = materializer1.Execute();
            var result1a = await materializer1.ExecuteAsync();
            Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
        }

        async Task ListWithNullsTest<TResult>(string columnName, Type materializerType)
        {
            var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (1, 2, 3, 4)");
            ILink<List<TResult>> materializer1 = (ILink<List<TResult>>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName, ListOptions.None });
            var result1 = materializer1.Execute();
            var result1a = await materializer1.ExecuteAsync();
            Assert.AreEqual(result1.Count, result1a.Count, "Results don't match");
        }

        async Task ScalarFailOnNullTest<TResult>(string columnName, Type materializerType)
        {
            var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id In (3)");

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
            var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id = 1");
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

            var cb3 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id = 3");
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

        async Task ScalarTest<TResult>(string columnName, Type materializerType)
        {
            var cb1 = DataSource.Sql($"SELECT {columnName} FROM dbo.AllTypes WHERE Id = 1");
            ILink<TResult> materializer1 = (ILink<TResult>)Activator.CreateInstance(materializerType, new object[] { cb1, columnName });
            var result1 = materializer1.Execute();
            var result1a = await materializer1.ExecuteAsync();

            if (typeof(TResult) == typeof(byte[]))
            {
                var result1Cast = (byte[])(object)result1;
                var result1aCast = (byte[])(object)result1a;
                Assert.AreEqual(result1Cast.Length, result1aCast.Length);
            }
            else if (result1 is XElement)
            {
                Assert.AreEqual(result1.ToString(), result1a.ToString(), "Results don't match");
            }
            else
            {
                Assert.AreEqual(result1, result1a, "Results don't match");
            }
        }

        //*****************************
        //*****************************
    }
}