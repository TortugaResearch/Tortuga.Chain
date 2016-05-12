using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Core
{
    public class DataSourceTest : TestBase
    {
        public static BasicData BasicData = new BasicData(s_DataSources.Values);

        public DataSourceTest(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [MemberData("BasicData")]
        public void TestConnection(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                dataSource.TestConnection();
            }
            finally
            {
                Release(dataSource);
            }

        }

        [Theory]
        [MemberData("BasicData")]
        public async Task TestConnectionAsync(string assemblyName, string dataSourceName, DataSourceType mode)
        {
            var dataSource = DataSource(dataSourceName, mode);
            try
            {
                await dataSource.TestConnectionAsync();
            }
            finally
            {
                Release(dataSource);
            }

        }
    }


}


