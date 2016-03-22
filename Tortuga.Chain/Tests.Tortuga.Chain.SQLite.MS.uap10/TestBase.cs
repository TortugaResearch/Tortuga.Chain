using Tortuga.Chain;

namespace Tests
{
    public abstract class TestBase
    {
        private readonly SQLiteDataSource m_DataSource;

        protected TestBase()
        {
            m_DataSource = new SQLiteDataSource("Data Source=SQLiteTestDatabase.MS.sqlite;");
        }

        public SQLiteDataSource DataSource
        {
            get { return m_DataSource; }
        }

        public string EmployeeTableName { get { return "Employee"; } }

    }
}
