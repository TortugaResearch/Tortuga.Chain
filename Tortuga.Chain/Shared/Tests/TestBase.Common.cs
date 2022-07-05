using System.Diagnostics;
using System.Reflection;
using Tests.Models;
using Tortuga.Chain;
using Tortuga.Chain.DataSources;

namespace Tests;

partial class TestBase
{
	private static int s_DataSourceCount;

	public T AttachTracers<T>(T dataSource) where T : DataSource
	{
		Interlocked.Increment(ref s_DataSourceCount);
		//WriteLine("Data source count: " + s_DataSourceCount);

		dataSource.ExecutionCanceled += DefaultDispatcher_ExecutionCanceled;
		dataSource.ExecutionError += DefaultDispatcher_ExecutionError;
		dataSource.ExecutionFinished += DefaultDispatcher_ExecutionFinished;
		dataSource.ExecutionStarted += DefaultDispatcher_ExecutionStarted;
		CompiledMaterializers.MaterializerCompiled += CompiledMaterializers_MaterializerCompiled;
		CompiledMaterializers.MaterializerCompilerFailed += CompiledMaterializers_MaterializerCompiled;

		return dataSource;
	}

#if CLASS_2

	public IAdvancedCrudDataSource DataSource2(string name, DataSourceType mode, [CallerMemberName] string caller = null)
	{
		return DataSource(name, mode, caller);
	}

#endif

#if CLASS_3

	public AbstractDataSource DataSource3(string name, DataSourceType mode, [CallerMemberName] string caller = null)
	{
		return DataSource(name, mode, caller);
	}

#endif

	public void Release(IDataSource dataSource)
	{
		//WriteLine($"Releasing data source {dataSource.Name} ({dataSource.GetType().Name})");

		dataSource.ExecutionCanceled -= DefaultDispatcher_ExecutionCanceled;
		dataSource.ExecutionError -= DefaultDispatcher_ExecutionError;
		dataSource.ExecutionFinished -= DefaultDispatcher_ExecutionFinished;
		dataSource.ExecutionStarted -= DefaultDispatcher_ExecutionStarted;

		CompiledMaterializers.MaterializerCompiled -= CompiledMaterializers_MaterializerCompiled;
		CompiledMaterializers.MaterializerCompilerFailed -= CompiledMaterializers_MaterializerCompiled;

		switch (dataSource)
		{
			case ITransactionalDataSource trans:
				trans.Commit();
				break;

			case IOpenDataSource open:
				open.TryCommit();
				open.Close();
				break;
		}

		Interlocked.Decrement(ref s_DataSourceCount);
		//WriteLine("Data source count: " + s_DataSourceCount);
	}

	protected void WriteLine(string message)
	{
#if DEBUG
		Debug.WriteLine(message);
#endif
	}

	private void CompiledMaterializers_MaterializerCompiled(object sender, MaterializerCompilerEventArgs e)
	{
		WriteLine("******");
		WriteLine("Compiled Materializer");
		WriteLine("SQL");
		WriteLine(e.Sql);
		WriteLine("Code");
		WriteLine(e.Code);
	}

	private void DefaultDispatcher_ExecutionCanceled(object sender, ExecutionEventArgs e)
	{
		//WriteLine("******");
		//WriteLine($"Execution canceled: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec.");
		//WriteDetails(e);
	}

	private void DefaultDispatcher_ExecutionError(object sender, ExecutionEventArgs e)
	{
		WriteLine("******");
		WriteLine($"Execution error: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec.");
		WriteDetails(e);
	}

	private void DefaultDispatcher_ExecutionFinished(object sender, ExecutionEventArgs e)
	{
		//This really slows down the tests. Only turn it on when we need it.

		//WriteLine("******");
		//WriteLine($"Execution finished: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec. Rows affected: {(e.RowsAffected != null ? e.RowsAffected.Value.ToString("N0") : "<NULL>")}.");
		//WriteDetails(e);
	}

	private void DefaultDispatcher_ExecutionStarted(object sender, ExecutionEventArgs e)
	{
		//This really slows down the tests. Only turn it on when we need it.

		//WriteLine("******");
		//WriteLine($"Execution started: {e.ExecutionDetails.OperationName}");
		//WriteDetails(e);
	}

	static void BuildEmployeeSearchKey1000(IRootDataSource dataSource)
	{
		var trans = dataSource.BeginTransaction();
		{
			BuildEmployeeSearchKey1000_NoTrans((ICrudDataSource)trans);
			trans.Commit();
		}
	}

	static void BuildEmployeeSearchKey1000_NoTrans(ICrudDataSource dataSource)
	{
		EmployeeSearchKey1000 = Guid.NewGuid().ToString();
		var rows = new List<Employee>();

		for (var i = 0; i < 1000; i++)
			rows.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = EmployeeSearchKey1000, MiddleName = i % 2 == 0 ? "A" + i : null });

		if (dataSource is ISupportsInsertBulk bulk)
			bulk.InsertBulk(EmployeeTableName, rows).Execute();
		else if (dataSource is ISupportsInsertBatch batch)
			batch.InsertMultipleBatch(EmployeeTableName, rows).Execute();
		else
			foreach (var row in rows)
				dataSource.Insert(EmployeeTableName, row).Execute();
	}

	protected static string EmployeeSearchKey1000;

	protected DirectoryInfo GetOutputFolder(string folderName)
	{
		var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
		var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
		var dirPath = Path.GetDirectoryName(codeBasePath);
		var result = new DirectoryInfo(Path.Combine(dirPath, "TestOutput", folderName));
		result.Create();
		return result;
	}
}
