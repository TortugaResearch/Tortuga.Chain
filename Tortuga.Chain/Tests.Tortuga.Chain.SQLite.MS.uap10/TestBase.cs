using System;
using Tortuga.Chain;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.SQLite;

namespace Tests
{
    public abstract class TestBase
    {
        private static readonly SQLiteDataSource s_DataSource;
        private static readonly SQLiteDataSource s_StrictDataSource;

        static TestBase()
        {
            s_DataSource = new SQLiteDataSource("Data Source=SQLiteTestDatabase.sqlite;");
            s_StrictDataSource = s_DataSource.WithSettings(new SQLiteDataSourceSettings() { StrictMode = true });

        }

        public static SQLiteDataSource DataSource
        {
            get { return s_DataSource; }
        }

        public static string EmployeeTableName { get { return "Employee"; } }
        public static string CustomerTableName { get { return "Customer"; } }

        public static SQLiteDataSource StrictDataSource
        {
            get { return s_StrictDataSource; }
        }

        protected static SQLiteDataSource DataSourceWithAuditRules()
        {
            return DataSource.WithRules(
                new DateTimeRule("CreatedDate", DateTimeKind.Local, OperationTypes.Insert),
                new DateTimeRule("UpdatedDate", DateTimeKind.Local, OperationTypes.InsertOrUpdate),
                new UserDataRule("UpdatedByKey", "EmployeeKey", OperationTypes.Insert),
                new UserDataRule("CreatedByKey", "EmployeeKey", OperationTypes.InsertOrUpdate),
                new ValidateWithValidatable(OperationTypes.InsertOrUpdate)
                );
        }
    }
}
