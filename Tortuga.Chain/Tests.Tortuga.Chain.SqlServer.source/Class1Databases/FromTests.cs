using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Tests.Models;

namespace Tests.Class1Databases
{
    [TestClass]
    public class FromTests : TestBase
    {
        [TestMethod]
        public void FromTests_GetByKey()
        {
            var emp1 = new Employee() { FirstName = "A", LastName = "1" };
            var emp2 = new Employee() { FirstName = "B", LastName = "2" };
            var emp3 = new Employee() { FirstName = "C", LastName = "3" };
            var emp4 = new Employee() { FirstName = "D", LastName = "4" };

            emp1 = DataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
            emp2 = DataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
            emp3 = DataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
            emp4 = DataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

            var find2 = DataSource.GetByKey(EmployeeTableName, emp2.EmployeeKey).ToObject<Employee>().Execute();
            Assert.AreEqual(emp2.EmployeeKey, find2.EmployeeKey, "The wrong employee was returned");

            var list = DataSource.GetByKey(EmployeeTableName, emp2.EmployeeKey, emp3.EmployeeKey, emp4.EmployeeKey).ToCollection<Employee>().Execute();
            Assert.AreEqual(3, list.Count, "GetByKey returned the wrong number of rows");
            Assert.IsTrue(list.Any(e => e.EmployeeKey == emp2.EmployeeKey));
            Assert.IsTrue(list.Any(e => e.EmployeeKey == emp3.EmployeeKey));
            Assert.IsTrue(list.Any(e => e.EmployeeKey == emp4.EmployeeKey));

        }
    }
}
