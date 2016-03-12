using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Repository
{
    [TestClass]
    public class RepositoryTests : TestBase
    {
        [TestMethod]
        public void BasicCrud()
        {
            var repo = new Repository<Employee, int>(DataSource, EmployeeTableName);

            var emp1 = new Employee() { FirstName = "Tom", LastName = "Jones", Title = "President" };
            var echo1 = repo.Insert(emp1);

            Assert.AreNotEqual(0, echo1.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp1.FirstName, echo1.FirstName, "FirstName");
            Assert.AreEqual(emp1.LastName, echo1.LastName, "LastName");
            Assert.AreEqual(emp1.Title, echo1.Title, "Title");

            echo1.MiddleName = "G";
            repo.Update(echo1);

            var emp2 = new Employee() { FirstName = "Lisa", LastName = "Green", Title = "VP Transportation", ManagerKey = echo1.EmployeeKey };
            var echo2 = repo.Insert(emp2);
            Assert.AreNotEqual(0, echo2.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp2.FirstName, echo2.FirstName, "FirstName");
            Assert.AreEqual(emp2.LastName, echo2.LastName, "LastName");
            Assert.AreEqual(emp2.Title, echo2.Title, "Title");
            Assert.AreEqual(emp2.ManagerKey, echo2.ManagerKey, "ManagerKey");

            var list = repo.GetAll();
            Assert.IsTrue(list.Any(e => e.EmployeeKey == echo1.EmployeeKey), "Employee 1 is missing");
            Assert.IsTrue(list.Any(e => e.EmployeeKey == echo2.EmployeeKey), "Employee 2 is missing");

            var get1 = repo.Get(echo1.EmployeeKey.Value);
            Assert.AreEqual(echo1.EmployeeKey, get1.EmployeeKey);



            var whereSearch1 = repo.Query("FirstName = @FN", new { FN = "Tom" });
            Assert.IsTrue(whereSearch1.Any(x => x.EmployeeKey == echo1.EmployeeKey), "Emp1 should have been returned");
            Assert.IsTrue(whereSearch1.All(x => x.FirstName == "Tom"), "Checking for incorrect return values");

            var whereSearch2 = repo.Query(new { FirstName = "Tom" });
            Assert.IsTrue(whereSearch2.Any(x => x.EmployeeKey == echo1.EmployeeKey), "Emp1 should have been returned");
            Assert.IsTrue(whereSearch2.All(x => x.FirstName == "Tom"), "Checking for incorrect return values");


            repo.Delete(echo2.EmployeeKey.Value);
            repo.Delete(echo1.EmployeeKey.Value);

            var list2 = repo.GetAll();
            Assert.AreEqual(list.Count - 2, list2.Count);

        }

        [TestMethod]
        public void InsertWithDictionary()
        {
            var repo = new Repository<Employee, int>(DataSource, EmployeeTableName);

            var emp1 = new Dictionary<string, object>() { { "FirstName", "Tom" }, { "LastName", "Jones" }, { "Title", "President" } };
            var echo1 = repo.Insert(emp1);

            Assert.AreNotEqual(0, echo1.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp1["FirstName"], echo1.FirstName, "FirstName");
            Assert.AreEqual(emp1["LastName"], echo1.LastName, "LastName");
            Assert.AreEqual(emp1["Title"], echo1.Title, "Title");

            repo.Delete(echo1.EmployeeKey.Value);

        }

        [TestMethod]
        public void UpdateWithDictionary()
        {
            var repo = new Repository<Employee, int>(DataSource, EmployeeTableName);

            var emp1 = new Dictionary<string, object>() { { "FirstName", "Tom" }, { "LastName", "Jones" }, { "Title", "President" } };
            var echo1 = repo.Insert(emp1);

            Assert.AreNotEqual(0, echo1.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp1["FirstName"], echo1.FirstName, "FirstName");
            Assert.AreEqual(emp1["LastName"], echo1.LastName, "LastName");
            Assert.AreEqual(emp1["Title"], echo1.Title, "Title");

            var emp2 = new Dictionary<string, object>() { { "EmployeeKey", echo1.EmployeeKey }, { "LastName", "Brown" } };

            var echo2 = repo.Update(emp2);
            //var echo2 = repo.Get(echo1.EmployeeKey.Value);

            //these were changed
            Assert.AreEqual(echo1.EmployeeKey, echo2.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp2["LastName"], echo2.LastName, "LastName");

            //these should be unchanged
            Assert.AreEqual(emp1["FirstName"], echo2.FirstName, "FirstName");
            Assert.AreEqual(emp1["Title"], echo2.Title, "Title");


            repo.Delete(echo1.EmployeeKey.Value);

        }

        [TestMethod]
        public void Upsert()
        {
            var repo = new Repository<Employee, int>(DataSource, EmployeeTableName);

            var emp1 = new Employee() { FirstName = "Tom", LastName = "Jones", Title = "President" };
            var echo1 = repo.Upsert(emp1);

            Assert.AreNotEqual(0, echo1.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp1.FirstName, echo1.FirstName, "FirstName");
            Assert.AreEqual(emp1.LastName, echo1.LastName, "LastName");
            Assert.AreEqual(emp1.Title, echo1.Title, "Title");

            echo1.MiddleName = "G";
            var echo2 = repo.Upsert(echo1);
            Assert.AreEqual("G", echo2.MiddleName);

            var echo3 = repo.Get(echo1.EmployeeKey.Value);
            Assert.AreEqual("G", echo3.MiddleName);

            repo.Delete(echo1.EmployeeKey.Value);
        }

        [TestMethod]
        public void Upsert_Dictionary()
        {
            var repo = new Repository<Employee, int>(DataSource, EmployeeTableName);

            var emp1 = new Dictionary<string, object>() { { "FirstName", "Tom" }, { "LastName", "Jones" }, { "Title", "President" } };
            var echo1 = repo.Upsert(emp1);

            Assert.AreNotEqual(0, echo1.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp1["FirstName"], echo1.FirstName, "FirstName");
            Assert.AreEqual(emp1["LastName"], echo1.LastName, "LastName");
            Assert.AreEqual(emp1["Title"], echo1.Title, "Title");

            echo1.MiddleName = "G";
            repo.Upsert(echo1);

            Assert.AreEqual("G", echo1.MiddleName);

            repo.Delete(echo1.EmployeeKey.Value);
        }

    }
}
