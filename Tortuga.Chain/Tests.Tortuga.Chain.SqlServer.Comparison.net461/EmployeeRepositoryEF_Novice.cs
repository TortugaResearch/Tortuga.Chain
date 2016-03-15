using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Models;

namespace Tests
{
    public class EmployeeRepositoryEF_Novice : ISimpleEmployeeRepository
    {
        public void Delete(int employeeKey)
        {
            using (var context = new CodeFirstModels())
            {
                var employee = context.Employees.Where(e => e.EmployeeKey == employeeKey).First();
                context.Employees.Remove(employee);
                context.SaveChanges();
            }
        }

        public Employee Get(int employeeKey)
        {
            using (var context = new CodeFirstModels())
            {
                return context.Employees.Where(e => e.EmployeeKey == employeeKey).First();
            }
        }

        public IList<Employee> GetAll()
        {
            using (var context = new CodeFirstModels())
            {
                return context.Employees.ToList();
            }
        }

        public IList<Employee> GetByManager(int managerKey)
        {
            using (var context = new CodeFirstModels())
            {
                return context.Employees.Where(e => e.ManagerKey == managerKey).ToList();
            }
        }

        public IList<EmployeeOfficePhone> GetOfficePhoneNumbers()
        {
            using (var context = new CodeFirstModels())
            {
                return context.Employees.Select(e => new EmployeeOfficePhone()
                {
                    EmployeeKey = e.EmployeeKey,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    OfficePhone = e.OfficePhone
                }).ToList();
            }
        }

        public int Insert(Employee employee)
        {
            using (var context = new CodeFirstModels())
            {
                context.Employees.Add(employee);
                context.SaveChanges();
                return employee.EmployeeKey;
            }
        }

        public Employee InsertAndReturn(Employee employee)
        {
            using (var context = new CodeFirstModels())
            {
                context.Employees.Add(employee);
                context.SaveChanges();
                return employee;
            }
        }

        public void Update(EmployeeOfficePhone employee)
        {
            using (var context = new CodeFirstModels())
            {
                var entity = context.Employees.Where(e => e.EmployeeKey == employee.EmployeeKey).First();
                entity.FirstName = employee.FirstName;
                entity.LastName = employee.LastName;
                entity.OfficePhone = employee.OfficePhone;
                context.SaveChanges();
            }
        }

        public void Update(Employee employee)
        {
            using (var context = new CodeFirstModels())
            {
                var entity = context.Employees.Where(e => e.EmployeeKey == employee.EmployeeKey).First();
                entity.CellPhone = employee.CellPhone;
                entity.FirstName = employee.FirstName;
                entity.LastName = employee.LastName;
                entity.ManagerKey = employee.ManagerKey;
                entity.MiddleName = employee.MiddleName;
                entity.OfficePhone = employee.OfficePhone;
                entity.Title = employee.Title;
                context.SaveChanges();
            }
        }

        public int Upsert(Employee employee)
        {
            throw new NotImplementedException();
        }
    }
}
