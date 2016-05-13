using System.Diagnostics;
using System.Threading;
using Tortuga.Chain;
using Tortuga.Chain.DataSources;

namespace Tests
{
    partial class TestBase
    {
        static int s_DataSourceCount;

        public void Release(DataSource dataSource)
        {
            WriteLine($"Releasing data source {dataSource.Name} ({dataSource.GetType().Name})");

            dataSource.ExecutionCanceled -= DefaultDispatcher_ExecutionCanceled;
            dataSource.ExecutionError -= DefaultDispatcher_ExecutionError;
            dataSource.ExecutionFinished -= DefaultDispatcher_ExecutionFinished;
            dataSource.ExecutionStarted -= DefaultDispatcher_ExecutionStarted;
            CompiledMaterializers.MaterializerCompiled -= CompiledMaterializers_MaterializerCompiled;
            CompiledMaterializers.MaterializerCompilerFailed -= CompiledMaterializers_MaterializerCompiled;


            var trans = dataSource as ITransactionalDataSource;
            trans?.Commit();

            var open = dataSource as IOpenDataSource;
            open?.TryCommit();
            open?.Close();

            Interlocked.Decrement(ref s_DataSourceCount);
            WriteLine("Data source count: " + s_DataSourceCount);
        }

        public T AttachTracers<T>(T dataSource) where T : DataSource
        {
            Interlocked.Increment(ref s_DataSourceCount);
            WriteLine("Data source count: " + s_DataSourceCount);

            dataSource.ExecutionCanceled += DefaultDispatcher_ExecutionCanceled;
            dataSource.ExecutionError += DefaultDispatcher_ExecutionError;
            dataSource.ExecutionFinished += DefaultDispatcher_ExecutionFinished;
            dataSource.ExecutionStarted += DefaultDispatcher_ExecutionStarted;
            CompiledMaterializers.MaterializerCompiled += CompiledMaterializers_MaterializerCompiled;
            CompiledMaterializers.MaterializerCompilerFailed += CompiledMaterializers_MaterializerCompiled;

            return dataSource;
        }


        void DefaultDispatcher_ExecutionCanceled(object sender, ExecutionEventArgs e)
        {
            WriteLine("******");
            WriteLine($"Execution canceled: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec.");
            //WriteDetails(e);
        }


        void CompiledMaterializers_MaterializerCompiled(object sender, MaterializerCompilerEventArgs e)
        {
            WriteLine("******");
            WriteLine("Compiled Materializer");
            WriteLine("SQL");
            WriteLine(e.Sql);
            WriteLine("Code");
            WriteLine(e.Code);
        }

        void DefaultDispatcher_ExecutionError(object sender, ExecutionEventArgs e)
        {
            WriteLine("******");
            WriteLine($"Execution error: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec.");
            //WriteDetails(e);
        }

        protected void WriteLine(string message)
        {
            m_Output.WriteLine(message);
            Debug.WriteLine(message);
        }

        void DefaultDispatcher_ExecutionFinished(object sender, ExecutionEventArgs e)
        {
            WriteLine("******");
            WriteLine($"Execution finished: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec. Rows affected: {(e.RowsAffected != null ? e.RowsAffected.Value.ToString("N0") : "<NULL>")}.");
            //WriteDetails(e);
        }

        void DefaultDispatcher_ExecutionStarted(object sender, ExecutionEventArgs e)
        {
            WriteLine("******");
            WriteLine($"Execution started: {e.ExecutionDetails.OperationName}");
            WriteDetails(e);
        }


    }
}
