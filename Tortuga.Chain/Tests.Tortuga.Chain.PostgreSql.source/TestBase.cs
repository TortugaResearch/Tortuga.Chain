using System;
using Tortuga.Chain;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.PostgreSql;

namespace Tests
{
    public abstract class TestBase
    {
        private static PostgreSqlDataSource s_DataSource;
        private static readonly PostgreSqlDataSource s_StrictDataSource;

        static TestBase()
        {
            s_DataSource = PostgreSqlDataSource.CreateFromConfig("PostgreSqlTestDatabase");
            s_StrictDataSource = s_DataSource.WithSettings(new PostgreSqlDataSourceSettings() { StrictMode = true });
        }

        public static PostgreSqlDataSource DataSource
        {
            get { return s_DataSource; }
        }

        public static PostgreSqlDataSource StrictDataSource
        {
            get { return s_StrictDataSource; }
        }

        public static string EmployeeTableName { get { return "hr.employee"; } }
        public static string CustomerTableName { get { return "sales.customer"; } }


        protected static PostgreSqlDataSource DataSourceWithAuditRules()
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
