using Microsoft.Extensions.Configuration;
using Tests.Models;
using Tortuga.Chain;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.MySql;

namespace Tests;

public abstract partial class TestBase
{
	public const string EmployeeTableName = "HR.Employee";
	public const string EmployeeViewName = "HR.EmployeeWithManager";
	internal static readonly Dictionary<string, MySqlDataSource> s_DataSources = new Dictionary<string, MySqlDataSource>();
	protected internal static MySqlDataSource s_PrimaryDataSource;

	public static string CustomerTableName { get { return "Sales.Customer"; } }

	public string MultiResultSetProc1Name { get { return "Sales.CustomerWithOrdersByState"; } }

	public string ScalarFunction1Name { get { return "HR.EmployeeCount"; } }

	//public string TableFunction2Name { get { return "Sales.CustomersByStateInline"; } }
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

	//public string TableFunction1Name { get { return "Sales.CustomersByState"; } }
	public MySqlDataSource AttachSoftDeleteRulesWithUser(MySqlDataSource source)
	{
		var currentUser1 = source.From(EmployeeTableName).WithLimits(1).ToObject<Employee>().Execute();

		return source.WithRules(
			new SoftDeleteRule("DeletedFlag", true),
			new UserDataRule("DeletedByKey", "EmployeeKey", OperationTypes.Delete),
			new DateTimeRule("DeletedDate", DateTimeKind.Local, OperationTypes.Delete)
			).WithUser(currentUser1);
	}

	public MySqlDataSource DataSource(string name, [CallerMemberName] string caller = null)
	{
		//WriteLine($"{caller} requested Data Source {name}");

		return AttachTracers(s_DataSources[name]);
	}

	public MySqlDataSourceBase DataSource(string name, DataSourceType mode, [CallerMemberName] string caller = null)
	{
		//WriteLine($"{caller} requested Data Source {name} with mode {mode}");

		var ds = s_DataSources[name];
		switch (mode)
		{
			case DataSourceType.Normal: return AttachTracers(ds);
			case DataSourceType.Strict: return AttachTracers(ds).WithSettings(new MySqlDataSourceSettings() { StrictMode = true });
			case DataSourceType.SequentialAccess: return AttachTracers(ds).WithSettings(new MySqlDataSourceSettings() { SequentialAccessMode = true });
			case DataSourceType.Transactional: return AttachTracers(ds.BeginTransaction());
			case DataSourceType.Open:
				var root = (IRootDataSource)ds;
				return AttachTracers((MySqlDataSourceBase)root.CreateOpenDataSource(root.CreateConnection(), null));
		}
		throw new ArgumentException($"Unknown mode {mode}");
	}

	public async Task<MySqlDataSourceBase> DataSourceAsync(string name, DataSourceType mode, [CallerMemberName] string caller = null)
	{
		//WriteLine($"{caller} requested Data Source {name} with mode {mode}");

		var ds = s_DataSources[name];
		switch (mode)
		{
			case DataSourceType.Normal: return AttachTracers(ds);
			case DataSourceType.Strict: return AttachTracers(ds).WithSettings(new MySqlDataSourceSettings() { StrictMode = true });
			case DataSourceType.SequentialAccess: return AttachTracers(ds).WithSettings(new MySqlDataSourceSettings() { SequentialAccessMode = true });
			case DataSourceType.Transactional: return AttachTracers(await ds.BeginTransactionAsync().ConfigureAwait(false));
			case DataSourceType.Open:
				var root = (IRootDataSource)ds;
				return AttachTracers((MySqlDataSourceBase)root.CreateOpenDataSource(await root.CreateConnectionAsync().ConfigureAwait(false), null));
		}
		throw new ArgumentException($"Unknown mode {mode}");
	}

	internal static void SetupTestBase()
	{
		if (s_PrimaryDataSource != null)
			return; //run once check

		Setup.CreateDatabase();

		var configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json").Build();

		foreach (var con in configuration.GetSection("ConnectionStrings").GetChildren())
		{
			var ds = new MySqlDataSource(con.Key, con.Value);
			if (s_PrimaryDataSource == null)
				s_PrimaryDataSource = ds;

			s_DataSources.Add(con.Key, ds);
		}
		BuildEmployeeSearchKey1000(s_PrimaryDataSource);
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
