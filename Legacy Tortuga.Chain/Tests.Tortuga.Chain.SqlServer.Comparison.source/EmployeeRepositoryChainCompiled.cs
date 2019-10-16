using System.Collections.Generic;
using Tests.Models;
using Tortuga.Chain;

namespace Tests
{
    public class EmployeeRepositoryChainCompiled : ISimpleEmployeeRepository
    {
        readonly SqlServerDataSource m_DataSource;
        const string TableName = "HR.Employee";

        public EmployeeRepositoryChainCompiled(SqlServerDataSource dataSource)
        {
            m_DataSource = dataSource;
        }

        public void Delete(int employeeKey)
        {
            m_DataSource.Delete(TableName, new { @EmployeeKey = employeeKey }).Execute();
        }

        public Employee Get(int employeeKey)
        {
            return m_DataSource.GetByKey(TableName, employeeKey).Compile().ToObject<Employee>().Execute();
        }

        public IList<Employee> GetAll()
        {
            return m_DataSource.From(TableName).Compile().ToCollection<Employee>().Execute();
        }

        public IList<Employee> GetByManager(int managerKey)
        {
            return m_DataSource.From(TableName, new { @ManagerKey = managerKey }).Compile().ToCollection<Employee>().Execute();
        }

        public IList<EmployeeOfficePhone> GetOfficePhoneNumbers()
        {
            return m_DataSource.From(TableName).Compile().ToCollection<EmployeeOfficePhone>().Execute();
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
            return m_DataSource.Insert(TableName, employee).Compile().ToObject<Employee>().Execute();
        }

        public int Upsert(Employee employee)
        {
            return m_DataSource.Upsert(TableName, employee).ToInt32().Execute();
        }
    }
}
