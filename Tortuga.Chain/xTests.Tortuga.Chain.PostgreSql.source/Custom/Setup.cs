using Npgsql;
using System;
using System.Configuration;
using System.Diagnostics;
using Tortuga.Chain;
using Tortuga.Chain.Core;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Tests
{
    public class Setup
    {

        //TODO: Redesign this to adhere to xUnit conventions.


        public static void AssemblyInit()
        {
            //TODO - setup database?
            using (var con = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["PostgreSqlTestDatabase"].ConnectionString))
            {
                con.Open();
                string sql1 = @"DROP SCHEMA IF Exists Sales Cascade; DROP SCHEMA IF Exists hr Cascade";

                string sql2 = @"
CREATE SCHEMA hr;
CREATE TABLE hr.employee
(
	EmployeeKey SERIAL PRIMARY KEY,
	FirstName VARCHAR(50) NOT NULL,
	MiddleName VARCHAR(50) NULL,
	LastName VARCHAR(50) NOT NULL,
	Title VARCHAR(100) null,
	ManagerKey INTEGER NULL REFERENCES HR.Employee(EmployeeKey),
	OfficePhone VARCHAR(15) NULL ,
	CellPhone VARCHAR(15) NULL ,
	CreatedDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	UpdatedDate TIMESTAMP NULL
)";

                string sql3 = @"
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

                string viewSql = @"CREATE VIEW HR.EmployeeWithManager
AS
SELECT  e.EmployeeKey ,
		e.FirstName ,
		e.MiddleName ,
		e.LastName ,
		e.Title ,
		e.ManagerKey ,
		e.OfficePhone ,
		e.CellPhone ,
		e.CreatedDate ,
		e.UpdatedDate ,
		m.EmployeeKey AS ManagerEmployeeKey ,
		m.FirstName AS ManagerFirstName ,
		m.MiddleName AS ManagerMiddleName ,
		m.LastName AS ManagerLastName ,
		m.Title AS ManagerTitle ,
		m.ManagerKey AS ManagerManagerKey ,
		m.OfficePhone AS ManagerOfficePhone ,
		m.CellPhone AS ManagerCellPhone ,
		m.CreatedDate AS ManagerCreatedDate ,
		m.UpdatedDate AS ManagerUpdatedDate
FROM    HR.Employee e
		LEFT JOIN HR.Employee m ON m.EmployeeKey = e.ManagerKey;";

                var orderSql = @"CREATE TABLE sales.order
(
	OrderKey SERIAL PRIMARY KEY,
	CustomerKey INT NOT NULL References Sales.Customer(CustomerKey),
	OrderDate TIMESTAMP NOT NULL
);";

                var function1 = @"CREATE FUNCTION Sales.CustomersByState ( param_state CHAR(2) ) RETURNS TABLE
    (
      CustomerKey INT,
      FullName VARCHAR(150) ,
      State CHAR(2)  ,
      CreatedByKey INT ,
      UpdatedByKey INT ,
      CreatedDate TIMESTAMP ,
      UpdatedDate TIMESTAMP ,
      DeletedFlag BOOLEAN,
      DeletedDate TIMESTAMP ,
      DeletedByKey INT 
    )
    AS $$
    BEGIN
	  RETURN QUERY SELECT    
	        c.CustomerKey ,
                c.FullName ,
                c.State ,
                c.CreatedByKey ,
                c.UpdatedByKey ,
                c.CreatedDate ,
                c.UpdatedDate ,
                c.DeletedFlag ,
                c.DeletedDate ,
                c.DeletedByKey
      FROM      Sales.Customer c
      WHERE     c.State = param_state;
    END;
    $$ LANGUAGE plpgsql;
";

                var proc1 = @"CREATE FUNCTION Sales.CustomerWithOrdersByState(param_state CHAR(2)) RETURNS SETOF refcursor AS $$
    DECLARE
      ref1 refcursor;           -- Declare cursor variables
      ref2 refcursor;                             
    BEGIN
      OPEN ref1 FOR  SELECT  c.CustomerKey ,
            c.FullName ,
            c.State ,
            c.CreatedByKey ,
            c.UpdatedByKey ,
            c.CreatedDate ,
            c.UpdatedDate ,
            c.DeletedFlag ,
            c.DeletedDate ,
            c.DeletedByKey
    FROM    Sales.Customer c
    WHERE   c.State = param_state;
    RETURN NEXT ref1;                                                                              
 
    OPEN ref2 FOR   SELECT  o.OrderKey ,
            o.CustomerKey ,
            o.OrderDate
    FROM    Sales.Order o
            INNER JOIN Sales.Customer c ON o.CustomerKey = c.CustomerKey
    WHERE   c.State = param_state;
    RETURN NEXT ref2; 

    END;
    $$ LANGUAGE plpgsql;
";

                //string proc1 = @"";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql1, con))
                    cmd.ExecuteNonQuery();

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql2, con))
                    cmd.ExecuteNonQuery();

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql3, con))
                    cmd.ExecuteNonQuery();

                using (NpgsqlCommand cmd = new NpgsqlCommand(viewSql, con))
                    cmd.ExecuteNonQuery();

                using (NpgsqlCommand cmd = new NpgsqlCommand(orderSql, con))
                    cmd.ExecuteNonQuery();

                using (NpgsqlCommand cmd = new NpgsqlCommand(function1, con))
                    cmd.ExecuteNonQuery();

                using (NpgsqlCommand cmd = new NpgsqlCommand(proc1, con))
                    cmd.ExecuteNonQuery();
            }

        }

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
