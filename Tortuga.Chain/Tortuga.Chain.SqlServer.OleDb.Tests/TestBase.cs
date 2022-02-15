using Microsoft.Extensions.Configuration;
using System.Data.OleDb;
using System.Runtime.CompilerServices;
using Tests.Models;
using Tortuga.Chain;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.SqlServer;

namespace Tests
{
	public abstract partial class TestBase
	{
		public const string EmployeeTableName = "HR.Employee";
		public const string EmployeeViewName = "HR.EmployeeWithManager";
		internal static readonly Dictionary<string, OleDbSqlServerDataSource> s_DataSources = new Dictionary<string, OleDbSqlServerDataSource>();
		internal static OleDbSqlServerDataSource s_PrimaryDataSource;

		public static string CustomerTableName { get { return "Sales.Customer"; } }

		public static string EmployeeTableName_Trigger { get { return "HR.EmployeeWithTrigger"; } }

		public string MultiResultSetProc1Name { get { return "Sales.CustomerWithOrdersByState"; } }

		public string ScalarFunction1Name { get { return "HR.EmployeeCount"; } }

		public string TableFunction1Name { get { return "Sales.CustomersByState"; } }

		public string TableFunction2Name { get { return "Sales.CustomersByStateInline"; } }

		public OleDbSqlServerDataSource AttachRules(OleDbSqlServerDataSource source)
		{
			return source.WithRules(
				new DateTimeRule("CreatedDate", DateTimeKind.Local, OperationTypes.Insert),
				new DateTimeRule("UpdatedDate", DateTimeKind.Local, OperationTypes.InsertOrUpdate),
				new UserDataRule("CreatedByKey", "EmployeeKey", OperationTypes.Insert),
				new UserDataRule("UpdatedByKey", "EmployeeKey", OperationTypes.InsertOrUpdate),
				new ValidateWithValidatable(OperationTypes.InsertOrUpdate)
				);
		}

		public OleDbSqlServerDataSource AttachSoftDeleteRulesWithUser(OleDbSqlServerDataSource source)
		{
			var currentUser1 = source.From(EmployeeTableName).WithLimits(1).ToObject<Employee>().Execute();

			return source.WithRules(
				new SoftDeleteRule("DeletedFlag", true),
				new UserDataRule("DeletedByKey", "EmployeeKey", OperationTypes.Delete),
				new DateTimeRule("DeletedDate", DateTimeKind.Local, OperationTypes.Delete)
				).WithUser(currentUser1);
		}

		public OleDbSqlServerDataSource DataSource(string name, [CallerMemberName] string caller = null)
		{
			//WriteLine($"{caller} requested Data Source {name}");

			return AttachTracers(s_DataSources[name]);
		}

		public OleDbSqlServerDataSourceBase DataSource(string name, DataSourceType mode, [CallerMemberName] string caller = null)
		{
			//WriteLine($"{caller} requested Data Source {name} with mode {mode}");

			var ds = s_DataSources[name];
			switch (mode)
			{
				case DataSourceType.Normal: return AttachTracers(ds);
				case DataSourceType.Strict: return AttachTracers(ds).WithSettings(new SqlServerDataSourceSettings() { StrictMode = true });
				case DataSourceType.SequentialAccess: return AttachTracers(ds).WithSettings(new SqlServerDataSourceSettings() { SequentialAccessMode = true });
				case DataSourceType.Transactional: return AttachTracers(ds.BeginTransaction());
				case DataSourceType.Open:
					var root = (IRootDataSource)ds;
					return AttachTracers((OleDbSqlServerDataSourceBase)root.CreateOpenDataSource(root.CreateConnection(), null));
			}
			throw new ArgumentException($"Unknown mode {mode}");
		}

		public async Task<OleDbSqlServerDataSourceBase> DataSourceAsync(string name, DataSourceType mode, [CallerMemberName] string caller = null)
		{
			//WriteLine($"{caller} requested Data Source {name} with mode {mode}");

			var ds = s_DataSources[name];
			switch (mode)
			{
				case DataSourceType.Normal: return AttachTracers(ds);
				case DataSourceType.Strict: return AttachTracers(ds).WithSettings(new SqlServerDataSourceSettings() { StrictMode = true });
				case DataSourceType.SequentialAccess: return AttachTracers(ds).WithSettings(new SqlServerDataSourceSettings() { SequentialAccessMode = true });
				case DataSourceType.Transactional: return AttachTracers(await ds.BeginTransactionAsync());
				case DataSourceType.Open:
					var root = (IRootDataSource)ds;
					return AttachTracers((OleDbSqlServerDataSourceBase)root.CreateOpenDataSource(await root.CreateConnectionAsync(), null));
			}
			throw new ArgumentException($"Unknown mode {mode}");
		}

		internal static void SetupTestBase()
		{
			if (s_PrimaryDataSource != null)
				return; //run once check

			var configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json").Build();

			foreach (var con in configuration.GetSection("ConnectionStrings").GetChildren())
			{
				var ds = new OleDbSqlServerDataSource(con.Key, con.Value);
				s_DataSources.Add(con.Key, ds);
				if (s_PrimaryDataSource == null) s_PrimaryDataSource = ds;
			}
			BuildEmployeeSearchKey1000_NoTrans(s_PrimaryDataSource);
			//BuildEmployeeSearchKey1000(s_PrimaryDataSource);
		}

		void WriteDetails(ExecutionEventArgs e)
		{
			if (e.ExecutionDetails is CommandExecutionToken<OleDbCommand, OleDbParameter>)
			{
				WriteLine("");
				WriteLine("Command text: ");
				WriteLine(e.ExecutionDetails.CommandText);
				//Indent();
				foreach (var item in ((CommandExecutionToken<OleDbCommand, OleDbParameter>)e.ExecutionDetails).Parameters)
					WriteLine(item.ParameterName + ": " + (item.Value == null || item.Value == DBNull.Value ? "<NULL>" : item.Value) + " [" + item.DbType + "]");
				//Unindent();
				WriteLine("******");
				WriteLine("");
			}
		}
	}
}