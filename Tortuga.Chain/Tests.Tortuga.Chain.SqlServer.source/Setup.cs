using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using Tortuga.Chain;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.SqlServer;

namespace Tests
{
    [TestClass]
    public class Setup
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)

        {
            DataSource.GlobalExecutionCanceled += DefaultDispatcher_ExecutionCanceled;
            DataSource.GlobalExecutionError += DefaultDispatcher_ExecutionError;
            DataSource.GlobalExecutionFinished += DefaultDispatcher_ExecutionFinished;
            DataSource.GlobalExecutionStarted += DefaultDispatcher_ExecutionStarted;

            CompiledMaterializers.MaterializerCompiled += CompiledMaterializers_MaterializerCompiled;
        }

        private static void CompiledMaterializers_MaterializerCompiled(object sender, MaterializerCompilerEventArgs e)
        {
            Debug.WriteLine("******");
            Debug.WriteLine("Compiled Materializer");
            Debug.Indent();
            Debug.WriteLine("SQL");
            Debug.WriteLine(e.Sql);
            Debug.WriteLine("Code");
            Debug.WriteLine(e.Code);
            Debug.Unindent();
        }

        static void DefaultDispatcher_ExecutionCanceled(object sender, ExecutionEventArgs e)
        {
            Debug.WriteLine("******");
            Debug.WriteLine($"Execution canceled: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec.");
            //WriteDetails(e);
        }

        static void DefaultDispatcher_ExecutionError(object sender, ExecutionEventArgs e)
        {
            Debug.WriteLine("******");
            Debug.WriteLine($"Execution error: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec.");
            //WriteDetails(e);
        }

        static void DefaultDispatcher_ExecutionFinished(object sender, ExecutionEventArgs e)
        {
            Debug.WriteLine("******");
            Debug.WriteLine($"Execution finished: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec. Rows affected: {(e.RowsAffected != null ? e.RowsAffected.Value.ToString("N0") : "<NULL>")}.");
            //WriteDetails(e);
        }

        static void DefaultDispatcher_ExecutionStarted(object sender, ExecutionEventArgs e)
        {
            Debug.WriteLine("******");
            Debug.WriteLine($"Execution started: {e.ExecutionDetails.OperationName}");
            WriteDetails(e);
        }

        static void WriteDetails(ExecutionEventArgs e)
        {
            if (e.ExecutionDetails is SqlServerExecutionToken)
            {
                Debug.WriteLine("");
                Debug.WriteLine("Command text: ");
                Debug.WriteLine(e.ExecutionDetails.CommandText);
                Debug.Indent();
                foreach (var item in ((SqlServerExecutionToken)e.ExecutionDetails).Parameters)
                    Debug.WriteLine(item.ParameterName + ": " + (item.Value == null || item.Value == DBNull.Value ? "<NULL>" : item.Value));
                Debug.Unindent();
                Debug.WriteLine("******");
                Debug.WriteLine("");
            }
        }
    }
}
