using System;
using Tests.Models;
using Tortuga.Chain;

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Tests.Class1Databases
{
    [TestClass]
    public class DeleteTests : TestBase
    {

        [TestMethod]
        public void DeleteTests_Delete()
        {

            var original = new Employee()
            {
                FirstName = "Test",
                LastName = "Employee" + DateTime.Now.Ticks,
                Title = "Mail Room"
            };

            var key = DataSource.Insert(EmployeeTableName, original).ToInt32().Execute();
            var inserted = DataSource.GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();

            DataSource.Delete(EmployeeTableName, inserted).Execute();
        }

        [TestMethod]
        public void DeleteTests_Delete_Attribute()
        {

            var original = new Employee()
            {
                FirstName = "Test",
                LastName = "Employee" + DateTime.Now.Ticks,
                Title = "Mail Room"
            };

            var key = DataSource.Insert(EmployeeTableName, original).ToInt32().Execute();
            var inserted = DataSource.GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();

            DataSource.Delete(inserted).Execute();

            try
            {
                DataSource.GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();
                Assert.Fail("Excpected a missing data exception");
            }
            catch (MissingDataException) { }
        }

        [TestMethod]
        public void DeleteTests_Delete_Implied()
        {

            var original = new Employee()
            {
                FirstName = "Test",
                LastName = "Employee" + DateTime.Now.Ticks,
                Title = "Mail Room"
            };

            var key = DataSource.Insert(EmployeeTableName, original).ToInt32().Execute();

            DataSource.Delete(new HR.Employee() { EmployeeKey = key }).Execute();

            try
            {
                DataSource.GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();
                Assert.Fail("Excpected a missing data exception");
            }
            catch (MissingDataException) { }
        }



    }


    namespace HR
    {
        public class Employee
        {
            public int EmployeeKey { get; set; }
        }
    }
}
