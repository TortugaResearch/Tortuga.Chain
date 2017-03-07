//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Xunit;
//using Xunit.Abstractions;

//#if SQL_SERVER || POSTGRESQL || OLE_SQL_SERVER

//namespace Tests.CommandBuilders
//{
//    public class TableFunctionTests : TestBase
//    {
//        public static BasicData Prime = new BasicData(s_PrimaryDataSource);
//#if SQL_SERVER || OLE_SQL_SERVER
//        static object Parameter1 = new { @State = "CA" };
//        static object DictParameter1a = new Dictionary<string, object>() { { "State", "CA" } };
//        static object DictParameter1b = new Dictionary<string, object>() { { "@State", "CA" } };
//#elif POSTGRESQL
//        static object Parameter1 = new { @param_state = "CA" };
//        static object DictParameter1a = new Dictionary<string, object>() { { "param_state", "CA" } };
//        static object DictParameter1b = new Dictionary<string, object>() { { "@param_state", "CA" } };
//#endif


//        public TableFunctionTests(ITestOutputHelper output) : base(output)
//        {
//        }


//#if SQL_SERVER || OLE_SQL_SERVER
//        //Only SQL Server has inline functions.

//        [Theory, MemberData("Prime")]
//        public void TableFunction2_Object(string assemblyName, string dataSourceName, DataSourceType mode)
//        {
//            var dataSource = DataSource(dataSourceName, mode);
//            try
//            {
//                var result = dataSource.TableFunction(TableFunction2Name, Parameter1).ToTable().Execute();
//            }
//            finally
//            {
//                Release(dataSource);
//            }
//        }
//#endif
//        [Theory, MemberData("Prime")]
//        public void TableFunction1_Scalar(string assemblyName, string dataSourceName, DataSourceType mode)
//        {
//            var dataSource = DataSource(dataSourceName, mode);
//            try
//            {
//                var result = dataSource.TableFunction(TableFunction1Name, Parameter1).ToInt32().Execute();
//            }
//            finally
//            {
//                Release(dataSource);
//            }
//        }

//        [Theory, MemberData("Prime")]
//        public void TableFunction1_Object_Limit(string assemblyName, string dataSourceName, DataSourceType mode)
//        {
//            var dataSource = DataSource(dataSourceName, mode);
//            try
//            {
//                var result = dataSource.TableFunction(TableFunction1Name, Parameter1).WithLimits(1).ToTable().Execute();
//            }
//            finally
//            {
//                Release(dataSource);
//            }
//        }

//        [Theory, MemberData("Prime")]
//        public void TableFunction1_Object_Filter(string assemblyName, string dataSourceName, DataSourceType mode)
//        {
//            var dataSource = DataSource(dataSourceName, mode);
//            try
//            {
//                var result = dataSource.TableFunction(TableFunction1Name, Parameter1).WithFilter(new { @FullName = "Tom Jones" }).ToTable().Execute();
//            }
//            finally
//            {
//                Release(dataSource);
//            }
//        }

//        [Theory, MemberData("Prime")]
//        public void TableFunction1_Object_Sort(string assemblyName, string dataSourceName, DataSourceType mode)
//        {
//            var dataSource = DataSource(dataSourceName, mode);
//            try
//            {
//                var result = dataSource.TableFunction(TableFunction1Name, Parameter1).WithSorting("FullName").ToTable().Execute();
//            }
//            finally
//            {
//                Release(dataSource);
//            }
//        }

//        [Theory, MemberData("Prime")]
//        public void TableFunction1_Object(string assemblyName, string dataSourceName, DataSourceType mode)
//        {
//            var dataSource = DataSource(dataSourceName, mode);
//            try
//            {
//                var result = dataSource.TableFunction(TableFunction1Name, Parameter1).ToTable().Execute();
//            }
//            finally
//            {
//                Release(dataSource);
//            }
//        }

//        [Theory, MemberData("Prime")]
//        public async Task TableFunction1_Object_Async(string assemblyName, string dataSourceName, DataSourceType mode)
//        {
//            var dataSource = DataSource(dataSourceName, mode);
//            try
//            {
//                var result = await dataSource.TableFunction(TableFunction1Name, Parameter1).ToTable().ExecuteAsync();
//            }
//            finally
//            {
//                Release(dataSource);
//            }
//        }


//        [Theory, MemberData("Prime")]
//        public void TableFunction1_Dictionary(string assemblyName, string dataSourceName, DataSourceType mode)
//        {
//            var dataSource = DataSource(dataSourceName, mode);
//            try
//            {
//                var result = dataSource.TableFunction(TableFunction1Name, DictParameter1a).ToTable().Execute();
//            }
//            finally
//            {
//                Release(dataSource);
//            }
//        }

//        [Theory, MemberData("Prime")]
//        public async Task TableFunction1_Dictionary_Async(string assemblyName, string dataSourceName, DataSourceType mode)
//        {
//            var dataSource = DataSource(dataSourceName, mode);
//            try
//            {
//                var result = await dataSource.TableFunction(TableFunction1Name, DictParameter1a).ToTable().ExecuteAsync();
//            }
//            finally
//            {
//                Release(dataSource);
//            }
//        }

//        [Theory, MemberData("Prime")]
//        public void TableFunction1_Dictionary2(string assemblyName, string dataSourceName, DataSourceType mode)
//        {
//            var dataSource = DataSource(dataSourceName, mode);
//            try
//            {
//                var result = dataSource.TableFunction(TableFunction1Name, DictParameter1b).ToTable().Execute();
//            }
//            finally
//            {
//                Release(dataSource);
//            }
//        }

//        [Theory, MemberData("Prime")]
//        public async Task TableFunction1_Dictionary2_Async(string assemblyName, string dataSourceName, DataSourceType mode)
//        {
//            var dataSource = DataSource(dataSourceName, mode);
//            try
//            {
//                var result = await dataSource.TableFunction(TableFunction1Name, DictParameter1b).ToTable().ExecuteAsync();
//            }
//            finally
//            {
//                Release(dataSource);
//            }
//        }

//    }
//}

//#endif

