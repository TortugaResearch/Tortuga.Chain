using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Tests.Models;
using Tortuga.Chain;
using Tortuga.Chain.DataSources;
using Xunit;
using Xunit.Abstractions;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Tests
{
    partial class TestBase
    {
        private readonly ITestOutputHelper m_Output;

        private static int s_DataSourceCount;

        public TestBase(ITestOutputHelper output)
        {
            m_Output = output;
        }

        public T AttachTracers<T>(T dataSource) where T : DataSource
        {
            Interlocked.Increment(ref s_DataSourceCount);
            WriteLine("Data source count: " + s_DataSourceCount);

            dataSource.ExecutionCanceled += DefaultDispatcher_ExecutionCanceled;
            dataSource.ExecutionError += DefaultDispatcher_ExecutionError;
            dataSource.ExecutionFinished += DefaultDispatcher_ExecutionFinished;
            dataSource.ExecutionStarted += DefaultDispatcher_ExecutionStarted;
#if !Roslyn_Missing
            CompiledMaterializers.MaterializerCompiled += CompiledMaterializers_MaterializerCompiled;
            CompiledMaterializers.MaterializerCompilerFailed += CompiledMaterializers_MaterializerCompiled;
#endif
            return dataSource;
        }

        public IClass2DataSource DataSource2(string name, DataSourceType mode, [CallerMemberName] string caller = null)
        {
            return (IClass2DataSource)DataSource(name, mode, caller);
        }

        public void Release(IDataSource dataSource)
        {
            WriteLine($"Releasing data source {dataSource.Name} ({dataSource.GetType().Name})");

            dataSource.ExecutionCanceled -= DefaultDispatcher_ExecutionCanceled;
            dataSource.ExecutionError -= DefaultDispatcher_ExecutionError;
            dataSource.ExecutionFinished -= DefaultDispatcher_ExecutionFinished;
            dataSource.ExecutionStarted -= DefaultDispatcher_ExecutionStarted;
#if !Roslyn_Missing
            CompiledMaterializers.MaterializerCompiled -= CompiledMaterializers_MaterializerCompiled;
            CompiledMaterializers.MaterializerCompilerFailed -= CompiledMaterializers_MaterializerCompiled;
#endif
            var trans = dataSource as ITransactionalDataSource;
            trans?.Commit();

            var open = dataSource as IOpenDataSource;
            open?.TryCommit();
            open?.Close();

            Interlocked.Decrement(ref s_DataSourceCount);
            WriteLine("Data source count: " + s_DataSourceCount);
        }

        protected void WriteLine(string message)
        {
#if DEBUG
            //This really slows down the tests. Only turn it on when we need it.

            try
            {
                m_Output.WriteLine(message);
            }
            catch
            {
                Debug.WriteLine("Error writing to xUnit log");
            }
            Debug.WriteLine(message);
#endif
        }

#if !Roslyn_Missing

        private void CompiledMaterializers_MaterializerCompiled(object sender, MaterializerCompilerEventArgs e)
        {
            WriteLine("******");
            WriteLine("Compiled Materializer");
            WriteLine("SQL");
            WriteLine(e.Sql);
            WriteLine("Code");
            WriteLine(e.Code);
        }

#endif

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
            //WriteLine("******");
            //WriteLine($"Execution finished: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec. Rows affected: {(e.RowsAffected != null ? e.RowsAffected.Value.ToString("N0") : "<NULL>")}.");
            //WriteDetails(e);
        }

        private void DefaultDispatcher_ExecutionStarted(object sender, ExecutionEventArgs e)
        {
            //WriteLine("******");
            //WriteLine($"Execution started: {e.ExecutionDetails.OperationName}");
            //WriteDetails(e);
        }

        static void BuildEmployeeSearchKey1000(IRootDataSource dataSource)
        {
            //using (var trans = dataSource.BeginTransaction())
            var trans = dataSource.BeginTransaction();
            {
                var cds = (IClass1DataSource)trans;
                EmployeeSearchKey1000 = Guid.NewGuid().ToString();
                for (var i = 0; i < 1000; i++)
                    cds.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = EmployeeSearchKey1000, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

                trans.Commit();
            }
        }

        protected static string EmployeeSearchKey1000;
    }
}