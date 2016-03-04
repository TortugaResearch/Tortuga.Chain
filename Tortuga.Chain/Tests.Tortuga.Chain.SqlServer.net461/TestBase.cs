using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Tortuga.Chain;

namespace Tests
{
    public abstract class TestBase
    {
        private readonly SqlServerDataSource m_DataSource;

        protected TestBase()
        {
            m_DataSource = new SqlServerDataSource(System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerTestDatabase"].ConnectionString);
        }

        public IClass1DataSource Class1DataSource
        {
            get { return m_DataSource; }
        }

        public SqlServerDataSource DataSource
        {
            get { return m_DataSource; }
        }
        public string EmployeeTableName { get { return "HR.Employee"; } }

    }

}

