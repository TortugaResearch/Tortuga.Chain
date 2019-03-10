using Tortuga.Chain.SQLite;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Core
{
    public class SQLiteDataSourceTest : TestBase
    {
        public static RootData Root = new RootData(s_PrimaryDataSource);

        public SQLiteDataSourceTest(ITestOutputHelper output) : base(output)
        {
        }

        [Theory, MemberData(nameof(Root))]
        public void FK_Disabled(string assemblyName, string dataSourceName)
        {
            var dataSource = DataSource(dataSourceName).WithSettings(new SQLiteDataSourceSettings() { EnforceForeignKeys = false });
            try
            {
                dataSource.TestConnection();
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData(nameof(Root))]
        public void FK_Enabled(string assemblyName, string dataSourceName)
        {
            var dataSource = DataSource(dataSourceName).WithSettings(new SQLiteDataSourceSettings() { EnforceForeignKeys = true });
            try
            {
                dataSource.TestConnection();
            }
            finally
            {
                Release(dataSource);
            }
        }
    }
}
