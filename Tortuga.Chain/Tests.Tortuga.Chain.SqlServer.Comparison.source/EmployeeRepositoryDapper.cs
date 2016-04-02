using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Tests.Models;

namespace Tests
{
    public class EmployeeRepositoryDapper : ISimpleEmployeeRepository
    {
        readonly string m_ConnectionString;

        public EmployeeRepositoryDapper(string connectionString)
        {
            m_ConnectionString = connectionString;
        }

        public void Delete(int employeeKey)
        {
            using (var con = new SqlConnection(m_ConnectionString))
            {
                con.Open();
                con.Execute("DELETE FROM HR.Employee WHERE EmployeeKey = @EmployeeKey", new { @EmployeeKey = employeeKey });
            }
        }

        public Employee Get(int employeeKey)
        {
            using (var con = new SqlConnection(m_ConnectionString))
            {
                con.Open();
                return con.Query<Employee>("SELECT e.EmployeeKey, e.FirstName, e.MiddleName, e.LastName, e.Title, e.ManagerKey, e.OfficePhone, e.CellPhone, e.CreatedDate FROM HR.Employee e WHERE e.EmployeeKey = @EmployeeKey", new { @EmployeeKey = employeeKey }).First();
            }
        }

        public IList<Employee> GetAll()
        {
            using (var con = new SqlConnection(m_ConnectionString))
            {
                con.Open();
                return con.Query<Employee>("SELECT e.EmployeeKey, e.FirstName, e.MiddleName, e.LastName, e.Title, e.ManagerKey, e.OfficePhone, e.CellPhone, e.CreatedDate FROM HR.Employee e").AsList();
            }
        }

        public IList<Employee> GetByManager(int managerKey)
        {
            using (var con = new SqlConnection(m_ConnectionString))
            {
                con.Open();
                return con.Query<Employee>("SELECT e.EmployeeKey, e.FirstName, e.MiddleName, e.LastName, e.Title, e.ManagerKey, e.OfficePhone, e.CellPhone, e.CreatedDate FROM HR.Employee e WHERE e.ManagerKey = @ManagerKey", new { @ManagerKey = managerKey }).AsList();
            }
        }

        public IList<EmployeeOfficePhone> GetOfficePhoneNumbers()
        {
            using (var con = new SqlConnection(m_ConnectionString))
            {
                con.Open();
                return con.Query<EmployeeOfficePhone>("SELECT e.EmployeeKey, e.FirstName, e.LastName, e.OfficePhone, e.CreatedDate FROM HR.Employee e").AsList();
            }
        }

        public int Insert(Employee employee)
        {
            const string sql = @"INSERT	INTO HR.Employee
		(FirstName,
		 MiddleName,
		 LastName,
		 Title,
		 ManagerKey,
		 OfficePhone,
		 CellPhone
		)
VALUES	(@FirstName,
		 @MiddleName,
		 @LastName,
		 @Title,
		 @ManagerKey,
		 @OfficePhone,
		 @CellPhone
		);

SELECT SCOPE_IDENTITY()
";
            using (var con = new SqlConnection(m_ConnectionString))
            {
                con.Open();
                return con.ExecuteScalar<int>(sql, employee);
            }
        }

        public Employee InsertAndReturn(Employee employee)
        {
            const string sql = @"INSERT	INTO HR.Employee
		(FirstName,
		 MiddleName,
		 LastName,
		 Title,
		 ManagerKey,
		 OfficePhone,
		 CellPhone
		)
    OUTPUT 
        Inserted.EmployeeKey,
        Inserted.FirstName,
        Inserted.MiddleName,
        Inserted.LastName,
        Inserted.Title,
        Inserted.ManagerKey,
        Inserted.OfficePhone,
        Inserted.CellPhone,
        Inserted.CreatedDate
VALUES	(@FirstName,
		 @MiddleName,
		 @LastName,
		 @Title,
		 @ManagerKey,
		 @OfficePhone,
		 @CellPhone
		);
";
            using (var con = new SqlConnection(m_ConnectionString))
            {
                con.Open();
                return con.Query<Employee>(sql, employee).First();
            }
        }

        public void Update(EmployeeOfficePhone employee)
        {
            const string sql = @"UPDATE	HR.Employee
SET		FirstName = @FirstName,
		LastName = @LastName,
		OfficePhone = @OfficePhone
WHERE	EmployeeKey = @EmployeeKey
";
            using (var con = new SqlConnection(m_ConnectionString))
            {
                con.Open();
                con.Execute(sql, employee);
            }
        }

        public void Update(Employee employee)
        {
            const string sql = @"UPDATE	HR.Employee
SET		FirstName = @FirstName,
		MiddleName = @MiddleName,
		LastName = @LastName,
		Title = @Title,
		ManagerKey = @ManagerKey,
		OfficePhone = @OfficePhone,
		CellPhone = @CellPhone
WHERE	EmployeeKey = @EmployeeKey
";
            using (var con = new SqlConnection(m_ConnectionString))
            {
                con.Open();
                con.Execute(sql, employee);
            }
        }

        public int Upsert(Employee employee)
        {
            throw new NotImplementedException();
        }
    }
}
