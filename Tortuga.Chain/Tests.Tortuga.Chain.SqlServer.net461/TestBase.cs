using Tortuga.Chain;

namespace Tests
{
    public abstract class TestBase
    {
        private static readonly SqlServerDataSource s_DataSource;

        static TestBase()
        {
            s_DataSource = SqlServerDataSource.CreateFromConfig("SqlServerTestDatabase");
        }

        public static SqlServerDataSource DataSource
        {
            get { return s_DataSource; }
        }

        public string EmployeeTableName { get { return "HR.Employee"; } }

    }

}

