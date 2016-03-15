using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Tests.Models;
using Tortuga.Chain;

namespace Tests.Tortuga.Chain.SqlServer.Comparison.net461
{
    [TestClass]
    public class ComparisonTests
    {
        static int Iterations = 1;
        static bool Warmup = false;


        static EmployeeRepositoryDapper s_DapperRepo;
        static EmployeeRepositoryChain s_ChainRepo;
        static EmployeeRepositoryEF_Intermediate s_EFIntermediateRepo;
        static EmployeeRepositoryEF_Novice s_EFNoviceRepo;

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            s_DapperRepo = new EmployeeRepositoryDapper(ConfigurationManager.ConnectionStrings["CodeFirstModels"].ConnectionString);
            s_ChainRepo = new EmployeeRepositoryChain(SqlServerDataSource.CreateFromConfig("CodeFirstModels"));
            s_EFIntermediateRepo = new EmployeeRepositoryEF_Intermediate();
            s_EFNoviceRepo = new EmployeeRepositoryEF_Novice();

            if (Warmup)
            {
                CrudTestCore(s_DapperRepo);
                CrudTestCore(s_ChainRepo);
                CrudTestCore(s_EFIntermediateRepo);
                CrudTestCore(s_EFNoviceRepo);
            }
        }


        [TestMethod]
        public void Dapper_CrudTest()
        {
            var repo = new EmployeeRepositoryDapper(ConfigurationManager.ConnectionStrings["CodeFirstModels"].ConnectionString);
            CrudTest(s_DapperRepo);
        }

        [TestMethod]
        public void Chain_CrudTest()
        {
            CrudTest(s_ChainRepo);
        }

        [TestMethod]
        public void EF_Novice_CrudTest()
        {
            CrudTest(s_EFNoviceRepo);
        }

        [TestMethod]
        public void EF_Intermediate_CrudTest()
        {
            CrudTest(s_EFIntermediateRepo);

        }

        static void CrudTest(ISimpleEmployeeRepository repo)
        {

            ////warmup
            //const int warmups = 10;
            //var sw = Stopwatch.StartNew();
            //for (var i = 0; i < warmups; i++)
            //{
            //    CrudTestCore(repo);
            //}
            //sw.Stop();
            //Debug.WriteLine("Warmup Duration: " + sw.Elapsed.TotalMilliseconds.ToString("N2") + " ms");

            //actual
            var sw2 = Stopwatch.StartNew();
            for (var i = 0; i < Iterations; i++)
            {
                CrudTestCore(repo);
            }
            sw2.Stop();
            Debug.WriteLine("Run Duration: " + (sw2.Elapsed.TotalMilliseconds / Iterations).ToString("N2") + " ms per iteration.");

        }

        static void CrudTestCore(ISimpleEmployeeRepository repo)
        {
            var emp1 = new Employee() { FirstName = "Tom", LastName = "Jones", Title = "President" };
            var employeeKey1 = repo.Insert(emp1);
            var echo1 = repo.Get(employeeKey1);

            Assert.AreNotEqual(0, echo1.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp1.FirstName, echo1.FirstName, "FirstName");
            Assert.AreEqual(emp1.LastName, echo1.LastName, "LastName");
            Assert.AreEqual(emp1.Title, echo1.Title, "Title");

            echo1.MiddleName = "G";
            repo.Update(echo1);
            var echo1b = repo.Get(employeeKey1);
            Assert.AreEqual("G", echo1b.MiddleName);


            var emp2 = new Employee() { FirstName = "Lisa", LastName = "Green", Title = "VP Transportation", ManagerKey = echo1.EmployeeKey };
            var echo2 = repo.InsertAndReturn(emp2);

            Assert.AreNotEqual(0, echo2.EmployeeKey, "EmployeeKey was not set");
            Assert.AreEqual(emp2.FirstName, echo2.FirstName, "FirstName");
            Assert.AreEqual(emp2.LastName, echo2.LastName, "LastName");
            Assert.AreEqual(emp2.Title, echo2.Title, "Title");
            Assert.AreEqual(emp2.ManagerKey, echo2.ManagerKey, "ManagerKey");
            Assert.IsNotNull(echo2.CreatedDate);

            var list = repo.GetAll();
            Assert.IsTrue(list.Any(e => e.EmployeeKey == echo1.EmployeeKey), "Employee 1 is missing");
            Assert.IsTrue(list.Any(e => e.EmployeeKey == echo2.EmployeeKey), "Employee 2 is missing");

            var whereSearch1 = repo.GetByManager(employeeKey1);
            Assert.IsFalse(whereSearch1.Any(x => x.EmployeeKey == echo1.EmployeeKey), "Emp1 should not have been returned");
            Assert.IsTrue(whereSearch1.Any(x => x.EmployeeKey == echo2.EmployeeKey), "Emp2 should have been returned");

            var projection = repo.GetOfficePhoneNumbers();
            Assert.IsTrue(projection.Any(e => e.EmployeeKey == echo1.EmployeeKey), "Employee 1 is missing");
            Assert.IsTrue(projection.Any(e => e.EmployeeKey == echo2.EmployeeKey), "Employee 2 is missing");

            var projection1 = projection.Single(e => e.EmployeeKey == echo1.EmployeeKey);
            projection1.OfficePhone = "123-456-7890";
            repo.Update(projection1);

            var echo1c = repo.Get(employeeKey1);
            Assert.AreEqual("123-456-7890", echo1c.OfficePhone);

            repo.Delete(echo2.EmployeeKey);
            repo.Delete(echo1.EmployeeKey);

            var list2 = repo.GetAll();
            Assert.AreEqual(list.Count - 2, list2.Count);
        }
    }
}
