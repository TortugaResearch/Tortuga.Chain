using MySql.Data.MySqlClient;
using System.Configuration;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Tests
{
    public class Setup
    {
        public static void AssemblyCleanup()
        {
        }

        public static void AssemblyInit()
        {
            using (var con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlTestDatabase"].ConnectionString))
            {
                con.Open();

                string sql = @"
                DROP VIEW IF EXISTS hr.EmployeeWithManager;
                DROP TABLE IF EXISTS sales.order;
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
                	OfficePhone VARCHAR(15) NULL ,
                	CellPhone VARCHAR(15) NULL ,
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

                string function1 = @"
CREATE FUNCTION hr.EmployeeCount(p_managerKey integer) RETURNS integer
    READS SQL DATA
BEGIN
    DECLARE emp_count integer;

	IF (p_managerKey IS NOT NULL) THEN
		SELECT COUNT(*) INTO emp_count
		FROM HR.Employee e
		WHERE	e.ManagerKey = p_managerKey;
	ELSE
		SELECT COUNT(*) INTO emp_count
		FROM	HR.Employee e;
	END IF;
	RETURN emp_count;

END; ";

                string proc1 = @"
CREATE PROCEDURE Sales.CustomerWithOrdersByState
(IN p_State CHAR(2))
BEGIN
    SELECT  *
    FROM    Sales.Customer c
    WHERE   c.State = p_State;

    SELECT  o.*
    FROM    sales.order o
            INNER JOIN Sales.Customer c ON o.CustomerKey = c.CustomerKey
    WHERE   c.State = p_State;
END;";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    cmd.ExecuteNonQuery();

                using (MySqlCommand cmd = new MySqlCommand(sql2, con))
                    cmd.ExecuteNonQuery();

                using (MySqlCommand cmd = new MySqlCommand(viewSql, con))
                    cmd.ExecuteNonQuery();

                using (MySqlCommand cmd = new MySqlCommand(orderSql, con))
                    cmd.ExecuteNonQuery();

                using (MySqlCommand cmd = new MySqlCommand(function1, con))
                    cmd.ExecuteNonQuery();

                using (MySqlCommand cmd = new MySqlCommand(proc1, con))
                    cmd.ExecuteNonQuery();
            }
        }
    }
}