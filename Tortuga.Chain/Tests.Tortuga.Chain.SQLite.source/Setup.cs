using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using Tortuga.Chain;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.SQLite;

namespace Tests
{
    [TestClass]
    public class Setup
    {
        private const string databaseFileName = "SQLiteTestDatabase.sqlite";
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            DataSource.GlobalExecutionCanceled += DefaultDispatcher_ExecutionCanceled;
            DataSource.GlobalExecutionError += DefaultDispatcher_ExecutionError;
            DataSource.GlobalExecutionFinished += DefaultDispatcher_ExecutionFinished;
            DataSource.GlobalExecutionStarted += DefaultDispatcher_ExecutionStarted;

            File.Delete(databaseFileName);

            SQLiteConnection.CreateFile(databaseFileName);
            var m_dbConnection = new SQLiteConnection("Data Source=SQLiteTestDatabase.sqlite;Version=3;");
            m_dbConnection.Open();


            string sql = @"
CREATE TABLE Employee
(
	EmployeeKey INTEGER PRIMARY KEY,
	FirstName nvarChar(25) NOT NULL,
	MiddleName nvarChar(25) NULL,
	LastName nVarChar(25) NOT NULL,
	Title nVarChar(100) null,
	ManagerKey INT NULL REferences Employee(EmployeeKey),
    CreatedDate DateTime NOT NULL DEFAULT CURRENT_TIME,
    UpdatedDate DateTime NULL
)";

            string sql2 = @"CREATE TABLE Customer
(
	CustomerKey INTEGER PRIMARY KEY, 
    FullName NVARCHAR(100) NULL,
	State Char(2) NOT NULL,

    CreatedByKey INTEGER NULL,
    UpdatedByKey INTEGER NULL,

	CreatedDate DATETIME2 NULL,
    UpdatedDate DATETIME2 NULL,

	DeletedFlag BIT NOT NULL Default 0,
	DeletedDate DateTimeOffset NULL,
	DeletedByKey INTEGER NULL
)";

            using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                command.ExecuteNonQuery();

            using (SQLiteCommand command = new SQLiteCommand(sql2, m_dbConnection))
                command.ExecuteNonQuery();

            sql = @"INSERT INTO Employee ([EmployeeKey], [FirstName], [MiddleName], [LastName], [Title], [ManagerKey]) VALUES (@EmployeeKey, @FirstName, @MiddleName, @LastName, @Title, @ManagerKey); SELECT [EmployeeKey], [FirstName], [MiddleName], [LastName], [Title], [ManagerKey] FROM Employee WHERE ROWID = last_insert_rowid();";

            for (var i = 0; i < 10; i++)
                using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                {
                    command.Parameters.AddWithValue("@EmployeeKey", DBNull.Value);
                    command.Parameters.AddWithValue("@FirstName", "Tom");
                    command.Parameters.AddWithValue("@MiddleName", DBNull.Value);
                    command.Parameters.AddWithValue("@LastName", "Jones");
                    command.Parameters.AddWithValue("@Title", "CEO");
                    command.Parameters.AddWithValue("@ManagerKey", DBNull.Value);
                    var key = command.ExecuteScalar();
                }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            File.Delete(databaseFileName);
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
            Debug.WriteLine("");
            Debug.WriteLine("Command text: ");
            Debug.WriteLine(e.ExecutionDetails.CommandText);
            Debug.Indent();
            foreach (var item in ((SQLiteExecutionToken)e.ExecutionDetails).Parameters)
                Debug.WriteLine(item.ParameterName + ": " + (item.Value == null || item.Value == DBNull.Value ? "<NULL>" : item.Value));
            Debug.Unindent();
            Debug.WriteLine("******");
            Debug.WriteLine("");
        }
    }
}
