using Tortuga.Chain;
using Tortuga.Chain.DataSources;

namespace Tests
{
    partial class TestBase
    {
        public void Release(DataSource dataSource)
        {
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
        }

        public T AttachTracers<T>(T dataSource) where T : DataSource
        {
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
            m_Output.WriteLine("******");
            m_Output.WriteLine($"Execution canceled: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec.");
            //WriteDetails(e);
        }


        void CompiledMaterializers_MaterializerCompiled(object sender, MaterializerCompilerEventArgs e)
        {
            m_Output.WriteLine("******");
            m_Output.WriteLine("Compiled Materializer");
            m_Output.WriteLine("SQL");
            m_Output.WriteLine(e.Sql);
            m_Output.WriteLine("Code");
            m_Output.WriteLine(e.Code);
        }

        void DefaultDispatcher_ExecutionError(object sender, ExecutionEventArgs e)
        {
            m_Output.WriteLine("******");
            m_Output.WriteLine($"Execution error: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec.");
            //WriteDetails(e);
        }

        void DefaultDispatcher_ExecutionFinished(object sender, ExecutionEventArgs e)
        {
            m_Output.WriteLine("******");
            m_Output.WriteLine($"Execution finished: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec. Rows affected: {(e.RowsAffected != null ? e.RowsAffected.Value.ToString("N0") : "<NULL>")}.");
            //WriteDetails(e);
        }

        void DefaultDispatcher_ExecutionStarted(object sender, ExecutionEventArgs e)
        {
            m_Output.WriteLine("******");
            m_Output.WriteLine($"Execution started: {e.ExecutionDetails.OperationName}");
            WriteDetails(e);
        }


    }
}
