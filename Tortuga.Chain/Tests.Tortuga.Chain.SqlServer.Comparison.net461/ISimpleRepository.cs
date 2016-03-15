using System.Collections.Generic;
using Tests.Models;

namespace Tests
{
    public interface ISimpleEmployeeRepository
    {
        Employee Get(int employeeKey);

        IList<Employee> GetAll();

        IList<Employee> GetByManager(int managerKey);

        int Insert(Employee employee);

        Employee InsertAndReturn(Employee employee);

        
        void Update(Employee employee);

        void Delete(int employeeKey);

        IList<EmployeeOfficePhone> GetOfficePhoneNumbers();

        void Update(EmployeeOfficePhone employee);

        int Upsert(Employee employee);

    }
}


