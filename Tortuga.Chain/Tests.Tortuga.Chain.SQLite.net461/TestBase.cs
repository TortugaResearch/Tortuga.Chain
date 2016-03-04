using Tortuga.Chain;

namespace Tests
{
    public abstract class TestBase
    {
        private readonly SQLiteDataSource m_DataSource;

        protected TestBase()
        {
            m_DataSource = new SQLiteDataSource(System.Configuration.ConfigurationManager.ConnectionStrings["SQLiteTestDatabase"].ConnectionString);
        }

        public SQLiteDataSource DataSource
        {
            get { return m_DataSource; }
        }

        public string EmployeeTableName { get { return "Employee"; } }

    }
}
