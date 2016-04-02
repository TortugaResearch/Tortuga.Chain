using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Tests.Models;

namespace Tests
{
    public class EmployeeRepositoryEF_Intermediate_NoTrack : ISimpleEmployeeRepository
    {
        public void Delete(int employeeKey)
        {
            using (var context = new CodeFirstModels())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM HR.Employee WHERE EmployeeKey = @p0", employeeKey);
            }
        }

        public Employee Get(int employeeKey)
        {
            using (var context = new CodeFirstModels())
            {
                return context.Employees.Where(e => e.EmployeeKey == employeeKey).AsNoTracking().First();
            }
        }

        public IList<Employee> GetAll()
        {
            using (var context = new CodeFirstModels())
            {
                return context.Employees.AsNoTracking().ToList();
            }
        }

        public IList<Employee> GetByManager(int managerKey)
        {
            using (var context = new CodeFirstModels())
            {
                return context.Employees.Where(e => e.ManagerKey == managerKey).AsNoTracking().ToList();
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
                }).AsNoTracking().ToList();
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
            const string sql = @"UPDATE	HR.Employee
SET		FirstName = @p0,
		LastName = @p1,
		OfficePhone = @p2
WHERE	EmployeeKey = @p3
";
            using (var context = new CodeFirstModels())
            {
                context.Database.ExecuteSqlCommand(sql, employee.FirstName, employee.LastName, employee.OfficePhone, employee.EmployeeKey);
            }
        }

        public void Update(Employee employee)
        {
            using (var context = new CodeFirstModels())
            {
                context.Entry(employee).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public int Upsert(Employee employee)
        {
            using (var context = new CodeFirstModels())
            {
                if (employee.EmployeeKey == 0)
                    context.Entry(employee).State = EntityState.Added;
                else
                    context.Entry(employee).State = EntityState.Modified;
                context.SaveChanges();
                return employee.EmployeeKey;
            }
        }
    }
}
