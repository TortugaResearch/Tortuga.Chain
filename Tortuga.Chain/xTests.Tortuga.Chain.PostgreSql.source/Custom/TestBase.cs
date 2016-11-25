using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tortuga.Chain;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.PostgreSql;

namespace Tests
{
    public abstract partial class TestBase
    {

        static public readonly string AssemblyName = "PostgreSql";
        static protected readonly Dictionary<string, PostgreSqlDataSource> s_DataSources = new Dictionary<string, PostgreSqlDataSource>();
        protected static readonly PostgreSqlDataSource s_PrimaryDataSource;

        static TestBase()
        {
            Setup.AssemblyInit();
            foreach (ConnectionStringSettings con in ConfigurationManager.ConnectionStrings)
            {
                var ds = new PostgreSqlDataSource(con.Name, con.ConnectionString);
                if (s_PrimaryDataSource == null) s_PrimaryDataSource = ds;
                s_DataSources.Add(con.Name, ds);
            }
        }

        public static string CustomerTableName { get { return "Sales.Customer"; } }

        public static string EmployeeTableName { get { return "HR.Employee"; } }

        public string MultiResultSetProc1Name { get { return "Sales.CustomerWithOrdersByState"; } }

        public string TableFunction1Name { get { return "Sales.CustomersByState"; } }

        //public string TableFunction2Name { get { return "Sales.CustomersByStateInline"; } }

        public string ScalarFunction1Name { get { return "HR.EmployeeCount"; } }

        public PostgreSqlDataSource AttachRules(PostgreSqlDataSource source)
        {
            return source.WithRules(
                new DateTimeRule("CreatedDate", DateTimeKind.Local, OperationTypes.Insert),
                new DateTimeRule("UpdatedDate", DateTimeKind.Local, OperationTypes.InsertOrUpdate),
                new UserDataRule("CreatedByKey", "EmployeeKey", OperationTypes.Insert),
                new UserDataRule("UpdatedByKey", "EmployeeKey", OperationTypes.InsertOrUpdate),
                new ValidateWithValidatable(OperationTypes.InsertOrUpdate)
                );
        }

        public PostgreSqlDataSource DataSource(string name, [CallerMemberName] string caller = null)
        {
            WriteLine($"{caller} requested Data Source {name}");

            return AttachTracers(s_DataSources[name]);
        }

        public PostgreSqlDataSourceBase DataSource(string name, DataSourceType mode, [CallerMemberName] string caller = null)
        {
            WriteLine($"{caller} requested Data Source {name} with mode {mode}");

            var ds = s_DataSources[name];
            switch (mode)
            {
                case DataSourceType.Normal: return AttachTracers(ds);
                case DataSourceType.Transactional: return AttachTracers(ds.BeginTransaction());
                case DataSourceType.Open:
                    var root = (IRootDataSource)ds;
                    return AttachTracers((PostgreSqlDataSourceBase)root.CreateOpenDataSource(root.CreateConnection(), null));
            }
            throw new ArgumentException($"Unkown mode {mode}");
        }

        public async Task<PostgreSqlDataSourceBase> DataSourceAsync(string name, DataSourceType mode, [CallerMemberName] string caller = null)
        {
            WriteLine($"{caller} requested Data Source {name} with mode {mode}");

            var ds = s_DataSources[name];
            switch (mode)
            {
                case DataSourceType.Normal: return AttachTracers(ds);
                case DataSourceType.Transactional: return AttachTracers(await ds.BeginTransactionAsync());
                case DataSourceType.Open:
                    var root = (IRootDataSource)ds;
                    return AttachTracers((PostgreSqlDataSourceBase)root.CreateOpenDataSource(await root.CreateConnectionAsync(), null));
            }
            throw new ArgumentException($"Unkown mode {mode}");
        }

        void WriteDetails(ExecutionEventArgs e)
        {
            if (e.ExecutionDetails is PostgreSqlCommandExecutionToken)
            {
                WriteLine("");
                WriteLine("Command text: ");
                WriteLine(e.ExecutionDetails.CommandText);
                WriteLine("CommandType: " + e.ExecutionDetails.CommandType);
                //Indent();
                foreach (var item in ((PostgreSqlCommandExecutionToken)e.ExecutionDetails).Parameters)
                    WriteLine(item.ParameterName + ": " + (item.Value == null || item.Value == DBNull.Value ? "<NULL>" : item.Value));
                //Unindent();
                WriteLine("******");
                WriteLine("");
            }
        }
    }
}

