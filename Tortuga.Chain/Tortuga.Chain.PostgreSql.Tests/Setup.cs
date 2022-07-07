global using AbstractDataSource = Tortuga.Chain.PostgreSql.PostgreSqlDataSourceBase;

using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Tests;

[TestClass]
public static class Setup
{
	static int s_RunOnce;

	[AssemblyCleanup]
	public static void AssemblyCleanup()
	{
	}

	[AssemblyInitialize]
	public static void AssemblyInit(TestContext context)
	{
		TestBase.SetupTestBase();
	}

	public static void CreateDatabase()
	{
		if (Interlocked.CompareExchange(ref s_RunOnce, 1, 0) == 1)
			return;

		var configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json").Build();

		var connectionString = configuration.GetSection("ConnectionStrings").GetChildren().First().Value;

		using (var con = new NpgsqlConnection(connectionString))
		{
			con.Open();
			string sql1 = @"DROP SCHEMA IF Exists Sales Cascade; DROP SCHEMA IF Exists hr Cascade";

			string sql = @"
DROP FUNCTION IF EXISTS hr.EmployeeCount(INTEGER);
DROP FUNCTION IF EXISTS Sales.CustomersByState(char(2));
DROP FUNCTION IF EXISTS Sales.CustomerWithOrdersByState(char(2));
DROP VIEW IF EXISTS hr.EmployeeWithManager;
DROP TABLE IF EXISTS sales.order;
DROP TABLE IF EXISTS public.employee;
DROP TABLE IF EXISTS hr.employee;
DROP TABLE IF EXISTS public.employee;
DROP SCHEMA IF EXISTS hr;";

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
	UpdatedDate TIMESTAMP NULL,
	EmployeeId VARCHAR(50) NOT NULL,
	Gender Char(1) NOT NULL,
	Status Char(1) NULL
)";

			string index = @"CREATE UNIQUE INDEX UX_Employee_EmployeeId ON hr.employee(EmployeeId);";

			string sql2b = @"
CREATE TABLE public.employee
(
	EmployeeKey SERIAL PRIMARY KEY,
	FirstName VARCHAR(50) NOT NULL,
	MiddleName VARCHAR(50) NULL,
	LastName VARCHAR(50) NOT NULL,
	Title VARCHAR(100) null,
	ManagerKey INTEGER NULL REFERENCES public.employee(EmployeeKey),
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
	DeletedByKey INTEGER NULL,
	BirthDay DATE NULL,
	PreferredCallTime TIME NULL
)";

			string sql4 = @"
CREATE TABLE sales.location
(
	LocationKey SERIAL PRIMARY KEY,
	LocationName VARCHAR(150) NULL
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
		e.Gender ,
		m.EmployeeKey AS ManagerEmployeeKey ,
		m.FirstName AS ManagerFirstName ,
		m.MiddleName AS ManagerMiddleName ,
		m.LastName AS ManagerLastName ,
		m.Title AS ManagerTitle ,
		m.ManagerKey AS ManagerManagerKey ,
		m.OfficePhone AS ManagerOfficePhone ,
		m.CellPhone AS ManagerCellPhone ,
		m.CreatedDate AS ManagerCreatedDate ,
		m.UpdatedDate AS ManagerUpdatedDate,
		m.Gender AS ManagerGender,
		e.EmployeeId
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

			var scalarFunc = @"CREATE OR REPLACE FUNCTION hr.EmployeeCount(p_managerKey integer) RETURNS integer AS $$
		DECLARE
		result INTEGER;
		BEGIN
		IF (p_managerKey IS NOT NULL) THEN
			result := COUNT(*)
			FROM HR.Employee e
			WHERE	e.ManagerKey = p_managerKey;
		ELSE
			result := COUNT(*)
			FROM	HR.Employee e;
		END IF;
		RETURN result;

		END;
$$ LANGUAGE plpgsql;";

			using (NpgsqlCommand cmd = new NpgsqlCommand(sql, con))
				cmd.ExecuteNonQuery();

			using (NpgsqlCommand cmd = new NpgsqlCommand(sql1, con))
				cmd.ExecuteNonQuery();

			using (NpgsqlCommand cmd = new NpgsqlCommand(sql2, con))
				cmd.ExecuteNonQuery();

			using (NpgsqlCommand cmd = new NpgsqlCommand(index, con))
				cmd.ExecuteNonQuery();

			using (NpgsqlCommand cmd = new NpgsqlCommand(sql2b, con))
				cmd.ExecuteNonQuery();

			using (NpgsqlCommand cmd = new NpgsqlCommand(sql3, con))
				cmd.ExecuteNonQuery();

			using (NpgsqlCommand cmd = new NpgsqlCommand(sql4, con))
				cmd.ExecuteNonQuery();

			using (NpgsqlCommand cmd = new NpgsqlCommand(viewSql, con))
				cmd.ExecuteNonQuery();

			using (NpgsqlCommand cmd = new NpgsqlCommand(orderSql, con))
				cmd.ExecuteNonQuery();

			using (NpgsqlCommand cmd = new NpgsqlCommand(function1, con))
				cmd.ExecuteNonQuery();

			using (NpgsqlCommand cmd = new NpgsqlCommand(proc1, con))
				cmd.ExecuteNonQuery();

			using (NpgsqlCommand cmd = new NpgsqlCommand(scalarFunc, con))
				cmd.ExecuteNonQuery();
		}
	}
}
