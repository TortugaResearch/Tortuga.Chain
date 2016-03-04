using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Tortuga.Chain;

namespace Tests
{
    [TestClass]
    public class Setup
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)

        {
            Tortuga.Chain.DataSources.DataSource.GlobalExecutionCanceled += DefaultDispatcher_OperationCanceled;
            Tortuga.Chain.DataSources.DataSource.GlobalExecutionError += DefaultDispatcher_OperationError;
            Tortuga.Chain.DataSources.DataSource.GlobalExecutionFinished += DefaultDispatcher_OperationFinished;
            Tortuga.Chain.DataSources.DataSource.GlobalExecutionStarted += DefaultDispatcher_OperationStarted;
        }

        static void DefaultDispatcher_OperationCanceled(object sender, ExecutionEventArgs e)
        {
            Debug.WriteLine($"Execution canceled: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec.");
            WriteDetails(e);
        }

        static void DefaultDispatcher_OperationError(object sender, ExecutionEventArgs e)
        {
            Debug.WriteLine($"Execution error: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec.");
            WriteDetails(e);
        }

        static void DefaultDispatcher_OperationFinished(object sender, ExecutionEventArgs e)
        {
            Debug.WriteLine($"Execution finished: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec. Rows affected: {(e.RowsAffected != null ? e.RowsAffected.Value.ToString("N0") : "<NULL>")}.");
            WriteDetails(e);
        }

        static void DefaultDispatcher_OperationStarted(object sender, ExecutionEventArgs e)
        {
            Debug.WriteLine($"Execution started: {e.ExecutionDetails.OperationName}");
            WriteDetails(e);
        }

        static void WriteDetails(ExecutionEventArgs e)
        {
            Debug.WriteLine("");
            Debug.WriteLine("Command text: ");
            Debug.WriteLine(e.ExecutionDetails.CommandText);
            //TODO: add parameter dump
            Debug.WriteLine("");
            Debug.WriteLine("");
        }
    }
}
