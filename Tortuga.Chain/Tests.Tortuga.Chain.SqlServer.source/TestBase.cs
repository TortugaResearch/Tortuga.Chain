using System;
using Tortuga.Chain;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.SqlServer;

namespace Tests
{
    public abstract class TestBase
    {
        private static readonly SqlServerDataSource s_DataSource;
        private static readonly SqlServerDataSource s_StrictDataSource;

        static TestBase()
        {
            s_DataSource = SqlServerDataSource.CreateFromConfig("SqlServerTestDatabase");
            s_StrictDataSource = s_DataSource.WithSettings(new SqlServerDataSourceSettings() { StrictMode = true });
        }

        public static SqlServerDataSource DataSource
        {
            get { return s_DataSource; }
        }

        public static SqlServerDataSource StrictDataSource
        {
            get { return s_StrictDataSource; }
        }

        public static string EmployeeTableName { get { return "HR.Employee"; } }
        public static string CustomerTableName { get { return "Sales.Customer"; } }

        public string Proc1Name { get { return "Sales.CustomerWithOrdersByState"; } }

        protected static SqlServerDataSource DataSourceWithAuditRules()
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

