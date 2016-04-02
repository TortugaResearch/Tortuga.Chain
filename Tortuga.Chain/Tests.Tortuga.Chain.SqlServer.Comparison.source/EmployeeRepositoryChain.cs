using System;
using System.Collections.Generic;
using Tests.Models;
using Tortuga.Chain;

namespace Tests
{
    public class EmployeeRepositoryChain : ISimpleEmployeeRepository
    {
        readonly SqlServerDataSource m_DataSource;
        const string TableName = "HR.Employee";

        public EmployeeRepositoryChain(SqlServerDataSource dataSource)
        {
            m_DataSource = dataSource;
        }

        public void Delete(int employeeKey)
        {
            m_DataSource.Delete(TableName, new { @EmployeeKey = employeeKey }).Execute();
        }

        public Employee Get(int employeeKey)
        {
            return m_DataSource.From(TableName, new { @EmployeeKey = employeeKey }).ToObject<Employee>().Execute();
        }

        public IList<Employee> GetAll()
        {
            return m_DataSource.From(TableName).ToCollection<Employee>().Execute();
        }

        public IList<Employee> GetByManager(int managerKey)
        {
            return m_DataSource.From(TableName, new { @ManagerKey = managerKey }).ToCollection<Employee>().Execute();
        }

        public IList<EmployeeOfficePhone> GetOfficePhoneNumbers()
        {
            return m_DataSource.From(TableName).ToCollection<EmployeeOfficePhone>().Execute();
        }

        public int Insert(Employee employee)
        {
            return m_DataSource.Insert(TableName, employee).ToInt32().Execute();
        }

        public void Update(Employee employee)
        {
            m_DataSource.Update(TableName, employee).Execute();
        }

        public void Update(EmployeeOfficePhone employee)
        {
            m_DataSource.Update(TableName, employee).Execute();
        }

        public Employee InsertAndReturn(Employee employee)
        {
            return m_DataSource.Insert(TableName, employee).ToObject<Employee>().Execute();
        }

        public int Upsert(Employee employee)
        {
            return m_DataSource.Upsert(TableName, employee).ToInt32().Execute();
        }
    }
}
