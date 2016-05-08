using System;
using Tortuga.Chain;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.SQLite;

namespace Tests
{
    public abstract class TestBase
    {
        private static SQLiteDataSource s_DataSource;
        private static readonly SQLiteDataSource s_StrictDataSource;

        static TestBase()
        {
            s_DataSource = new SQLiteDataSource(System.Configuration.ConfigurationManager.ConnectionStrings["SQLiteTestDatabase"].ConnectionString);
            s_StrictDataSource = s_DataSource.WithSettings(new SQLiteDataSourceSettings() { StrictMode = true });
        }

        public static SQLiteDataSource DataSource
        {
            get { return s_DataSource; }
        }

        public static SQLiteDataSource StrictDataSource
        {
            get { return s_StrictDataSource; }
        }

        public static string EmployeeTableName { get { return "Employee"; } }
        public static string CustomerTableName { get { return "Customer"; } }


        protected static SQLiteDataSource DataSourceWithAuditRules()
        {
            return DataSource.WithRules(
                new DateTimeRule("CreatedDate", DateTimeKind.Local, OperationTypes.Insert),
                new DateTimeRule("UpdatedDate", DateTimeKind.Local, OperationTypes.InsertOrUpdate),
                new UserDataRule("CreatedByKey", "EmployeeKey", OperationTypes.Insert),
                new UserDataRule("UpdatedByKey", "EmployeeKey", OperationTypes.InsertOrUpdate),
                new ValidateWithValidatable(OperationTypes.InsertOrUpdate)
                );
        }
    }
}
