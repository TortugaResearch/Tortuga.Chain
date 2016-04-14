using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Tests.Models;
using Tortuga.Chain;

namespace Tests.Class1Databases
{
    [TestClass]
    public class FromTests : TestBase
    {
        [TestMethod]
        public void FromTests_Sorting()
        {
            var uniqueKey = Guid.NewGuid().ToString();

            var emp1 = new Employee() { FirstName = "A", LastName = "2", Title = uniqueKey };
            var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
            var emp3 = new Employee() { FirstName = "C", LastName = "1", Title = uniqueKey };
            var emp4 = new Employee() { FirstName = "D", LastName = "1", Title = uniqueKey };

            emp1 = DataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
            emp2 = DataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
            emp3 = DataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
            emp4 = DataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

            var test1 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("FirstName").ToCollection<Employee>().Execute();
            Assert.AreEqual(emp1.EmployeeKey, test1[0].EmployeeKey);
            Assert.AreEqual(emp2.EmployeeKey, test1[1].EmployeeKey);
            Assert.AreEqual(emp3.EmployeeKey, test1[2].EmployeeKey);
            Assert.AreEqual(emp4.EmployeeKey, test1[3].EmployeeKey);

            var test2 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting(new SortExpression("FirstName", SortDirection.Descending)).ToCollection<Employee>().Execute();
            Assert.AreEqual(emp4.EmployeeKey, test2[0].EmployeeKey);
            Assert.AreEqual(emp3.EmployeeKey, test2[1].EmployeeKey);
            Assert.AreEqual(emp2.EmployeeKey, test2[2].EmployeeKey);
            Assert.AreEqual(emp1.EmployeeKey, test2[3].EmployeeKey);

            var test3 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("LastName", "FirstName").ToCollection<Employee>().Execute();
            Assert.AreEqual(emp3.EmployeeKey, test3[0].EmployeeKey);
            Assert.AreEqual(emp4.EmployeeKey, test3[1].EmployeeKey);
            Assert.AreEqual(emp1.EmployeeKey, test3[2].EmployeeKey);
            Assert.AreEqual(emp2.EmployeeKey, test3[3].EmployeeKey);

        }

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
