using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tests.Models;

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif


namespace Tests.Repository
{
    [TestClass]
    public class TransactionalAsyncRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task BasicCrud()
        {
            using (var trans = DataSource.BeginTransaction())
            {
                var repo = new AsyncRepository<Employee, int>(trans, EmployeeTableName);

                var emp1 = new Employee() { FirstName = "Tom", LastName = "Jones", Title = "President" };
                var echo1 = await repo.InsertAsync(emp1);

                Assert.AreNotEqual(0, echo1.EmployeeKey, "EmployeeKey was not set");
                Assert.AreEqual(emp1.FirstName, echo1.FirstName, "FirstName");
                Assert.AreEqual(emp1.LastName, echo1.LastName, "LastName");
                Assert.AreEqual(emp1.Title, echo1.Title, "Title");

                echo1.MiddleName = "G";
                await repo.UpdateAsync(echo1);

                var emp2 = new Employee() { FirstName = "Lisa", LastName = "Green", Title = "VP Transportation", ManagerKey = echo1.EmployeeKey };
                var echo2 = await repo.InsertAsync(emp2);
                Assert.AreNotEqual(0, echo2.EmployeeKey, "EmployeeKey was not set");
                Assert.AreEqual(emp2.FirstName, echo2.FirstName, "FirstName");
                Assert.AreEqual(emp2.LastName, echo2.LastName, "LastName");
                Assert.AreEqual(emp2.Title, echo2.Title, "Title");
                Assert.AreEqual(emp2.ManagerKey, echo2.ManagerKey, "ManagerKey");

                var list = await repo.GetAllAsync();
                Assert.IsTrue(list.Any(e => e.EmployeeKey == echo1.EmployeeKey), "Employee 1 is missing");
                Assert.IsTrue(list.Any(e => e.EmployeeKey == echo2.EmployeeKey), "Employee 2 is missing");

                var get1 = await repo.GetAsync(echo1.EmployeeKey.Value);
                Assert.AreEqual(echo1.EmployeeKey, get1.EmployeeKey);



                var whereSearch1 = await repo.QueryAsync("FirstName = @FN", new { FN = "Tom" });
                Assert.IsTrue(whereSearch1.Any(x => x.EmployeeKey == echo1.EmployeeKey), "Emp1 should have been returned");
                Assert.IsTrue(whereSearch1.All(x => x.FirstName == "Tom"), "Checking for incorrect return values");

                var whereSearch2 = await repo.QueryAsync(new { FirstName = "Tom" });
                Assert.IsTrue(whereSearch2.Any(x => x.EmployeeKey == echo1.EmployeeKey), "Emp1 should have been returned");
                Assert.IsTrue(whereSearch2.All(x => x.FirstName == "Tom"), "Checking for incorrect return values");


                await repo.DeleteAsync(echo2.EmployeeKey.Value);
                await repo.DeleteAsync(echo1.EmployeeKey.Value);

                var list2 = await repo.GetAllAsync();
                Assert.AreEqual(list.Count - 2, list2.Count);

            }
        }

        [TestMethod]
        public async Task InsertWithDictionary()
        {
            using (var trans = DataSource.BeginTransaction())
            {
                var repo = new AsyncRepository<Employee, int>(trans, EmployeeTableName);

                var emp1 = new Dictionary<string, object>() { { "FirstName", "Tom" }, { "LastName", "Jones" }, { "Title", "President" } };
                var echo1 = await repo.InsertAsync(emp1);

                Assert.AreNotEqual(0, echo1.EmployeeKey, "EmployeeKey was not set");
                Assert.AreEqual(emp1["FirstName"], echo1.FirstName, "FirstName");
                Assert.AreEqual(emp1["LastName"], echo1.LastName, "LastName");
                Assert.AreEqual(emp1["Title"], echo1.Title, "Title");

                await repo.DeleteAsync(echo1.EmployeeKey.Value);

            }
        }

        [TestMethod]
        public async Task UpdateWithDictionary()
        {
            using (var trans = DataSource.BeginTransaction())
            {
                var repo = new AsyncRepository<Employee, int>(trans, EmployeeTableName);

                var emp1 = new Dictionary<string, object>() { { "FirstName", "Tom" }, { "LastName", "Jones" }, { "Title", "President" } };
                var echo1 = await repo.InsertAsync(emp1);

                Assert.AreNotEqual(0, echo1.EmployeeKey, "EmployeeKey was not set");
                Assert.AreEqual(emp1["FirstName"], echo1.FirstName, "FirstName");
                Assert.AreEqual(emp1["LastName"], echo1.LastName, "LastName");
                Assert.AreEqual(emp1["Title"], echo1.Title, "Title");

                var emp2 = new Dictionary<string, object>() { { "EmployeeKey", echo1.EmployeeKey }, { "LastName", "Brown" } };
                await repo.UpdateAsync(emp2);
                var echo2 = await repo.GetAsync(echo1.EmployeeKey.Value);

                //these were changed
                Assert.AreEqual(echo1.EmployeeKey, echo2.EmployeeKey, "EmployeeKey was not set");
                Assert.AreEqual(emp2["LastName"], echo2.LastName, "LastName");

                //these should be unchanged
                Assert.AreEqual(emp1["FirstName"], echo2.FirstName, "FirstName");
                Assert.AreEqual(emp1["Title"], echo2.Title, "Title");


                await repo.DeleteAsync(echo1.EmployeeKey.Value);

            }
        }
    }
}
