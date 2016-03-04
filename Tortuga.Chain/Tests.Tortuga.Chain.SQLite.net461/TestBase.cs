using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        public IClass1DataSource Class1DataSource
        {
            get { return m_DataSource; }
        }

        public string EmployeeTableName { get { return "Employee"; } }

    }
}
