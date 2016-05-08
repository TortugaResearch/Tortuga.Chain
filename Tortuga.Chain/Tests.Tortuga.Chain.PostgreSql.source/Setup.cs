using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using System;
using System.Diagnostics;
using Tortuga.Chain;
using Tortuga.Chain.Core;
using Tortuga.Chain.DataSources;



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
            CompiledMaterializers.MaterializerCompilerFailed += CompiledMaterializers_MaterializerCompiled;


            //TODO - setup database?
            using (var con = new NpgsqlConnection(@"User ID = postgres;
                                             Password = toor; 
                                             Host = localhost; 
                                             Port = 5432;
                                             Database = tortugachaintestdb;
                                             Pooling = true;"))
            {
                con.Open();

                string sql = @"
DROP TABLE IF EXISTS hr.employee;
DROP SCHEMA IF EXISTS hr;
CREATE SCHEMA hr;
CREATE TABLE hr.employee
(
	EmployeeKey SERIAL PRIMARY KEY,
	FirstName VARCHAR(50) NOT NULL,
	MiddleName VARCHAR(50) NULL,
	LastName VARCHAR(50) NOT NULL,
	Title VARCHAR(100) null,
	ManagerKey INTEGER NULL REFERENCES HR.Employee(EmployeeKey),
    CreatedDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedDate TIMESTAMP NULL
)";

                string sql2 = @"
DROP TABLE IF EXISTS sales.customer;
DROP SCHEMA IF EXISTS sales;
CREATE SCHEMA sales;
CREATE TABLE sales.customer
(
	CustomerKey SERIAL PRIMARY KEY, 
    FullName VARCHAR(150) NULL,
	State CHAR(2) NOT NULL,

    CreatedByKey INTEGER NULL,
    UpdatedByKey INTEGER NULL,

	CreatedDate TIMESTAMP NULL,
    UpdatedDate TIMESTAMP NULL,

	DeletedFlag boolean NOT NULL DEFAULT FALSE,
	DeletedDate TIMESTAMP NULL,
	DeletedByKey INTEGER NULL
)";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, con))
                    cmd.ExecuteNonQuery();

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql2, con))
                    cmd.ExecuteNonQuery();

            }

        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            /*
            using (var con = new NpgsqlConnection(@"User ID = postgres;
                                             Password = toor; 
                                             Host = localhost; 
                                             Port = 5432;
                                             Database = tortugachaintestdb;
                                             Pooling = true;"))
            {
                con.Open();

                string sql = "DROP TABLE HR.Employee; DROP SCHEMA HR;";
                string sql2 = "DROP TABLE Sales.Customer; DROP SCHEMA Sales;"; 
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, con))
                    cmd.ExecuteNonQuery();

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql2, con))
                    cmd.ExecuteNonQuery();

            }*/
        }



        static void DefaultDispatcher_ExecutionCanceled(object sender, ExecutionEventArgs e)
        {
            Debug.WriteLine("******");
            Debug.WriteLine($"Execution canceled: {e.ExecutionDetails.OperationName}. Duration: {e.Duration.Value.TotalSeconds.ToString("N3")} sec.");
            //WriteDetails(e);
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
            Debug.WriteLine("");
            Debug.WriteLine("Command text: ");
            Debug.WriteLine(e.ExecutionDetails.CommandText);
            Debug.Indent();
            foreach (var item in ((CommandExecutionToken<NpgsqlCommand, NpgsqlParameter>)e.ExecutionDetails).Parameters)
                Debug.WriteLine(item.ParameterName + ": " + (item.Value == null || item.Value == DBNull.Value ? "<NULL>" : item.Value));
            Debug.Unindent();
            Debug.WriteLine("******");
            Debug.WriteLine("");
        }
    }
}
