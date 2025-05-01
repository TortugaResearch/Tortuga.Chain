using Microsoft.Extensions.Configuration;
using Tests.Models;
using Tortuga.Chain;
using Tortuga.Chain.AuditRules;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.SQLite;

namespace Tests;

public abstract partial class TestBase
{
	public const string EmployeeTableName = "Employee";
	public const string EmployeeViewName = "EmployeeWithManager";
	internal static readonly Dictionary<string, SQLiteDataSource> s_DataSources = new Dictionary<string, SQLiteDataSource>();
	internal static SQLiteDataSource s_PrimaryDataSource;

	public static string CustomerTableName { get { return "Customer"; } }

	public SQLiteDataSource AttachRules(SQLiteDataSource source)
	{
		return source.WithRules(
			new DateTimeRule("CreatedDate", DateTimeKind.Local, OperationTypes.Insert),
			new DateTimeRule("UpdatedDate", DateTimeKind.Local, OperationTypes.InsertOrUpdate),
			new UserDataRule("CreatedByKey", "EmployeeKey", OperationTypes.Insert),
			new UserDataRule("UpdatedByKey", "EmployeeKey", OperationTypes.InsertOrUpdate),
			new ValidateWithValidatable(OperationTypes.InsertOrUpdate)
			);
	}

	public SQLiteDataSource AttachSoftDeleteRulesWithUser(SQLiteDataSource source)
	{
		var currentUser1 = source.From(EmployeeTableName).WithLimits(1).ToObject<Employee>().Execute();

		return source.WithRules(
			new SoftDeleteRule("DeletedFlag", true),
			new UserDataRule("DeletedByKey", "EmployeeKey", OperationTypes.Delete),
			new DateTimeRule("DeletedDate", DateTimeKind.Local, OperationTypes.Delete)
			).WithUser(currentUser1);
	}

	public SQLiteDataSource DataSource(string name, [CallerMemberName] string caller = null)
	{
		//WriteLine($"{caller} requested Data Source {name}");

		return AttachTracers(s_DataSources[name]);
	}

	public SQLiteDataSourceBase DataSource(string name, DataSourceType mode, [CallerMemberName] string caller = null)
	{
		//WriteLine($"{caller} requested Data Source {name} with mode {mode}");

		var ds = s_DataSources[name];
		switch (mode)
		{
			case DataSourceType.Normal: return AttachTracers(ds);
			case DataSourceType.Strict: return AttachTracers(ds).WithSettings(new SQLiteDataSourceSettings() { StrictMode = true });
			case DataSourceType.SequentialAccess: return AttachTracers(ds).WithSettings(new SQLiteDataSourceSettings() { SequentialAccessMode = true });
			case DataSourceType.Transactional: return AttachTracers(ds.BeginTransaction());
			case DataSourceType.Open:
				var root = (IRootDataSource)ds;
				return AttachTracers((SQLiteDataSourceBase)root.CreateOpenDataSource(root.CreateConnection(), null));
		}
		throw new ArgumentException($"Unknown mode {mode}");
	}

	public async Task<SQLiteDataSourceBase> DataSourceAsync(string name, DataSourceType mode, [CallerMemberName] string caller = null)
	{
		//WriteLine($"{caller} requested Data Source {name} with mode {mode}");

		var ds = s_DataSources[name];
		switch (mode)
		{
			case DataSourceType.Normal: return AttachTracers(ds);
			case DataSourceType.Strict: return AttachTracers(ds).WithSettings(new SQLiteDataSourceSettings() { StrictMode = true });
			case DataSourceType.SequentialAccess: return AttachTracers(ds).WithSettings(new SQLiteDataSourceSettings() { SequentialAccessMode = true });
			case DataSourceType.Transactional: return AttachTracers(await ds.BeginTransactionAsync().ConfigureAwait(false));
			case DataSourceType.Open:
				var root = (IRootDataSource)ds;
				return AttachTracers((SQLiteDataSourceBase)root.CreateOpenDataSource(await root.CreateConnectionAsync().ConfigureAwait(false), null));
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
			var ds = new SQLiteDataSource(con.Key, con.Value);
			if (s_PrimaryDataSource == null)
				s_PrimaryDataSource = ds;

			s_DataSources.Add(con.Key, ds);
		}

		BuildEmployeeSearchKey1000(s_PrimaryDataSource);
	}

	void WriteDetails(ExecutionEventArgs e)
	{
		if (e.ExecutionDetails is SQLiteCommandExecutionToken)
		{
			WriteLine("");
			WriteLine("Command text: ");
			WriteLine(e.ExecutionDetails.CommandText);
			//m_Output.Indent();
			foreach (var item in ((SQLiteCommandExecutionToken)e.ExecutionDetails).Parameters)
				WriteLine(item.ParameterName + ": " + (item.Value == null || item.Value == DBNull.Value ? "<NULL>" : item.Value));
			//m_Output.Unindent();
			WriteLine("******");
			WriteLine("");
		}
	}
}
