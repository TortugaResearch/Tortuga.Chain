using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Models;
using Tortuga.Chain;

namespace Tests.Materializers
{
    [TestClass]
    public class ModelTests : TestBase
    {
        [TestMethod]
        public void DecomposeTest()
        {
            var ds = DataSource;

            var emp1 = new Employee() { FirstName = "Tom", LastName = "Jones", Title = "President" };
            var emp1Key = ds.Insert(EmployeeTableName, emp1).ToInt32().Execute();

            var emp2 = new Employee() { FirstName = "Lisa", LastName = "Green", Title = "VP Transportation", ManagerKey = emp1Key };
            var emp2Key = ds.Insert(EmployeeTableName, emp2).ToInt32().Execute();

            var echo = ds.From("HR.EmployeeWithManager", new { @EmployeeKey = emp2Key }).ToObject<EmployeeWithManager>().Execute();

            Assert.AreNotEqual(0, echo.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp2.FirstName, echo.FirstName, "FirstName");
            Assert.AreEqual(emp2.LastName, echo.LastName, "LastName");
            Assert.AreEqual(emp2.Title, echo.Title, "Title");

            Assert.AreNotEqual(0, echo.Manager.EmployeeKey, "Manager.EmployeeKey was not set");
            Assert.AreEqual(emp1.FirstName, echo.Manager.FirstName, "Manager.FirstName");
            Assert.AreEqual(emp1.LastName, echo.Manager.LastName, "Manager.LastName");
            Assert.AreEqual(emp1.Title, echo.Manager.Title, "Manager.Title");

            Assert.IsNotNull(echo.AuditInfo.CreatedDate, "CreatedDate via AuditInfo");

        }

        [TestMethod]
        public void CompiledDecomposeTest()
        {
            var ds = DataSource;

            var emp1 = new Employee() { FirstName = "Tom", LastName = "Jones", Title = "President" };
            var emp1Key = ds.Insert(EmployeeTableName, emp1).ToInt32().Execute();

            var emp2 = new Employee() { FirstName = "Lisa", LastName = "Green", Title = "VP Transportation", ManagerKey = emp1Key };
            var emp2Key = ds.Insert(EmployeeTableName, emp2).ToInt32().Execute();

            var echo = ds.From("HR.EmployeeWithManager", new { @EmployeeKey = emp2Key }).Compile().ToObject<EmployeeWithManager>().Execute();

            Assert.AreNotEqual(0, echo.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp2.FirstName, echo.FirstName, "FirstName");
            Assert.AreEqual(emp2.LastName, echo.LastName, "LastName");
            Assert.AreEqual(emp2.Title, echo.Title, "Title");

            Assert.AreNotEqual(0, echo.Manager.EmployeeKey, "Manager.EmployeeKey was not set");
            Assert.AreEqual(emp1.FirstName, echo.Manager.FirstName, "Manager.FirstName");
            Assert.AreEqual(emp1.LastName, echo.Manager.LastName, "Manager.LastName");
            Assert.AreEqual(emp1.Title, echo.Manager.Title, "Manager.Title");

            Assert.IsNotNull(echo.AuditInfo.CreatedDate, "CreatedDate via AuditInfo");
            Assert.IsNotNull(echo.Manager.AuditInfo.CreatedDate, "CreatedDate via Manager.AuditInfo");

        }

        [TestMethod]
        public void CompiledTest()
        {
            var ds = DataSource;

            var emp1 = new Employee() { FirstName = "Tom", LastName = "Jones", Title = "President" };
            var emp1Key = ds.Insert(EmployeeTableName, emp1).ToInt32().Execute();

            var emp2 = new Employee() { FirstName = "Lisa", LastName = "Green", Title = "VP Transportation", ManagerKey = emp1Key };
            var emp2Key = ds.Insert(EmployeeTableName, emp2).ToInt32().Execute();

            var echo = ds.From("HR.EmployeeWithManager", new { @EmployeeKey = emp2Key }).Compile().ToObject<Employee>().Execute();
            var list = ds.From("HR.EmployeeWithManager", new { @EmployeeKey = emp2Key }).Compile().ToCollection<Employee>().Execute();


            Assert.AreNotEqual(0, echo.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp2.FirstName, echo.FirstName, "FirstName");
            Assert.AreEqual(emp2.LastName, echo.LastName, "LastName");
            Assert.AreEqual(emp2.Title, echo.Title, "Title");
        }
    }
}
