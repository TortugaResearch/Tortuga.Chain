using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tests.Models;
using Tortuga.Chain;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.MySql;

namespace Tests
{
    public abstract partial class TestBase
    {
        static public readonly string AssemblyName = "MySql";
        static protected readonly Dictionary<string, MySqlDataSource> s_DataSources = new Dictionary<string, MySqlDataSource>();
        protected static readonly MySqlDataSource s_PrimaryDataSource;

        static TestBase()
        {
            Setup.AssemblyInit();
            foreach (ConnectionStringSettings con in ConfigurationManager.ConnectionStrings)
            {
                var ds = new MySqlDataSource(con.Name, con.ConnectionString);
                if (s_PrimaryDataSource == null) s_PrimaryDataSource = ds;
                s_DataSources.Add(con.Name, ds);
            }
            BuildEmployeeSearchKey1000(s_PrimaryDataSource);
        }

        public static string CustomerTableName { get { return "Sales.Customer"; } }

        public static string EmployeeTableName { get { return "HR.Employee"; } }

        public string MultiResultSetProc1Name { get { return "Sales.CustomerWithOrdersByState"; } }

        //public string TableFunction1Name { get { return "Sales.CustomersByState"; } }

        //public string TableFunction2Name { get { return "Sales.CustomersByStateInline"; } }

        public string ScalarFunction1Name { get { return "HR.EmployeeCount"; } }

        public MySqlDataSource AttachRules(MySqlDataSource source)
        {
            return source.WithRules(
                new DateTimeRule("CreatedDate", DateTimeKind.Local, OperationTypes.Insert),
                new DateTimeRule("UpdatedDate", DateTimeKind.Local, OperationTypes.InsertOrUpdate),
                new UserDataRule("CreatedByKey", "EmployeeKey", OperationTypes.Insert),
                new UserDataRule("UpdatedByKey", "EmployeeKey", OperationTypes.InsertOrUpdate),
                new ValidateWithValidatable(OperationTypes.InsertOrUpdate)
                );
        }

        public MySqlDataSource AttachSoftDeleteRulesWithUser(MySqlDataSource source)
        {
            var currentUser1 = source.From(EmployeeTableName).WithLimits(1).ToObject<Employee>().Execute();

            return source.WithRules(
                new SoftDeleteRule("DeletedFlag", true, OperationTypes.SelectOrDelete),
                new UserDataRule("DeletedByKey", "EmployeeKey", OperationTypes.Delete),
                new DateTimeRule("DeletedDate", DateTimeKind.Local, OperationTypes.Delete)
                ).WithUser(currentUser1);
        }

        public MySqlDataSource DataSource(string name, [CallerMemberName] string caller = null)
        {
            WriteLine($"{caller} requested Data Source {name}");

            return AttachTracers(s_DataSources[name]);
        }

        public MySqlDataSourceBase DataSource(string name, DataSourceType mode, [CallerMemberName] string caller = null)
        {
            WriteLine($"{caller} requested Data Source {name} with mode {mode}");

            var ds = s_DataSources[name];
            switch (mode)
            {
                case DataSourceType.Normal: return AttachTracers(ds);
                case DataSourceType.Strict: return AttachTracers(ds).WithSettings(new MySqlDataSourceSettings() { StrictMode = true });
                case DataSourceType.Transactional: return AttachTracers(ds.BeginTransaction());
                case DataSourceType.Open:
                    var root = (IRootDataSource)ds;
                    return AttachTracers((MySqlDataSourceBase)root.CreateOpenDataSource(root.CreateConnection(), null));
            }
            throw new ArgumentException($"Unkown mode {mode}");
        }

        public async Task<MySqlDataSourceBase> DataSourceAsync(string name, DataSourceType mode, [CallerMemberName] string caller = null)
        {
            WriteLine($"{caller} requested Data Source {name} with mode {mode}");

            var ds = s_DataSources[name];
            switch (mode)
            {
                case DataSourceType.Normal: return AttachTracers(ds);
                case DataSourceType.Strict: return AttachTracers(ds).WithSettings(new MySqlDataSourceSettings() { StrictMode = true });
                case DataSourceType.Transactional: return AttachTracers(await ds.BeginTransactionAsync());
                case DataSourceType.Open:
                    var root = (IRootDataSource)ds;
                    return AttachTracers((MySqlDataSourceBase)root.CreateOpenDataSource(await root.CreateConnectionAsync(), null));
            }
            throw new ArgumentException($"Unkown mode {mode}");
        }

        private void WriteDetails(ExecutionEventArgs e)
        {
            if (e.ExecutionDetails is MySqlCommandExecutionToken)
            {
                WriteLine("");
                WriteLine("Command text: ");
                WriteLine(e.ExecutionDetails.CommandText);
                WriteLine("CommandType: " + e.ExecutionDetails.CommandType);
                //Indent();
                foreach (var item in ((MySqlCommandExecutionToken)e.ExecutionDetails).Parameters)
                    WriteLine(item.ParameterName + ": " + (item.Value == null || item.Value == DBNull.Value ? "<NULL>" : item.Value));
                //Unindent();
                WriteLine("******");
                WriteLine("");
            }
        }
    }
}