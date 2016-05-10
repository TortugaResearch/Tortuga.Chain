using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tortuga.Chain;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.PostgreSql;
using Xunit.Abstractions;

namespace Tests
{
    public abstract partial class TestBase
    {
        public TestBase(ITestOutputHelper output)
        {
            m_Output = output;
        }

        protected readonly ITestOutputHelper m_Output;

        static protected readonly Dictionary<string, PostgreSqlDataSource> s_DataSources = new Dictionary<string, PostgreSqlDataSource>();
        static public readonly string AssemblyName = "PostgreSql";

        static TestBase()
        {
            Setup.AssemblyInit();
            foreach (ConnectionStringSettings con in ConfigurationManager.ConnectionStrings)
            {
                var ds = new PostgreSqlDataSource(con.Name, con.ConnectionString);
                s_DataSources.Add(con.Name, ds);
            }
        }

        public PostgreSqlDataSourceBase DataSource(string name, DataSourceType mode, [CallerMemberName] string caller = null)
        {
            m_Output.WriteLine($"{caller} requested Data Source {name} with mode {mode}");

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
            m_Output.WriteLine($"{caller} requested Data Source {name} with mode {mode}");

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
            if (e.ExecutionDetails is PostgreSqlExecutionToken)
            {
                m_Output.WriteLine("");
                m_Output.WriteLine("Command text: ");
                m_Output.WriteLine(e.ExecutionDetails.CommandText);
                //m_Output.Indent();
                foreach (var item in ((PostgreSqlExecutionToken)e.ExecutionDetails).Parameters)
                    m_Output.WriteLine(item.ParameterName + ": " + (item.Value == null || item.Value == DBNull.Value ? "<NULL>" : item.Value));
                //m_Output.Unindent();
                m_Output.WriteLine("******");
                m_Output.WriteLine("");
            }
        }
    }
}

