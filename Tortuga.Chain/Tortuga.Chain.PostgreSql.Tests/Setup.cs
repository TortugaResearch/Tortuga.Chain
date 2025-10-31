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
			string sql1 = @"DROP SCHEMA IF Exists Sales Cascade; DROP SCHEMA IF Exists hr Cascade; DROP SCHEMA IF Exists public Cascade";

			//			string sql = @"
			//DROP FUNCTION IF EXISTS hr.EmployeeCount(INTEGER);
			//DROP FUNCTION IF EXISTS Sales.CustomersByState(char(2));
			//DROP FUNCTION IF EXISTS Sales.CustomerWithOrdersByState(char(2));
			//DROP VIEW IF EXISTS hr.EmployeeWithManager;
			//DROP TABLE IF EXISTS sales.order;
			//DROP TABLE IF EXISTS public.employee;
			//DROP TABLE IF EXISTS hr.employee;
			//DROP TABLE IF EXISTS public.employee;
			//DROP SCHEMA IF EXISTS hr;";

			string sql2 = @"
create schema hr;
create table hr.employee
(
	employee_key serial primary key,
	first_name varchar(50) not null,
	middle_name varchar(50) null,
	last_name varchar(50) not null,
	title varchar(100) null,
	manager_key integer null references hr.employee(employee_key),
	office_phone varchar(15) null ,
	cell_phone varchar(15) null ,
	created_date timestamp not null default current_timestamp,
	updated_date timestamp null,
	employee_id varchar(50) not null,
	gender char(1) not null,
	status char(1) null
)";



			string index = @"CREATE UNIQUE INDEX UX_Employee_EmployeeId ON hr.employee(employee_id);";

			string sql2b = @"
CREATE SCHEMA public;
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
);

COMMENT ON TABLE sales.customer IS 'This table holds US customers';

COMMENT ON COLUMN sales.customer.CustomerKey IS 'What if we run out of keys?';

COMMENT ON COLUMN sales.customer.State IS 'Use NA for non-US customers.';

";

			string sql4 = @"
CREATE TABLE sales.location
(
	LocationKey SERIAL PRIMARY KEY,
	LocationName VARCHAR(150) NULL
)";

			string viewSql = @"CREATE VIEW HR.EmployeeWithManager
AS
SELECT  e.Employee_Key ,
		e.First_Name ,
		e.Middle_Name ,
		e.Last_Name ,
		e.Title ,
		e.Manager_Key ,
		e.Office_Phone ,
		e.Cell_Phone ,
		e.Created_Date ,
		e.Updated_Date ,
		e.Gender ,
		m.Employee_Key AS Manager_Employee_Key ,
		m.First_Name AS Manager_First_Name ,
		m.Middle_Name AS Manager_Middle_Name ,
		m.Last_Name AS Manager_Last_Name ,
		m.Title AS Manager_Title ,
		m.Manager_Key AS Manager_Manager_Key ,
		m.Office_Phone AS Manager_Office_Phone ,
		m.Cell_Phone AS Manager_Cell_Phone ,
		m.Created_Date AS Manager_Created_Date ,
		m.Updated_Date AS Manager_Updated_Date,
		m.Gender AS Manager_Gender,
		e.Employee_Id
FROM    HR.Employee e
		LEFT JOIN HR.Employee m ON m.Employee_Key = e.Manager_Key;";

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
			WHERE	e.Manager_Key = p_managerKey;
		ELSE
			result := COUNT(*)
			FROM	HR.Employee e;
		END IF;
		RETURN result;

		END;
$$ LANGUAGE plpgsql;";

			//using (NpgsqlCommand cmd = new NpgsqlCommand(sql, con))
			//	cmd.ExecuteNonQuery();

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
