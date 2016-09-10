using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Tests.Models;
using Tortuga.Chain;

namespace Tests
{
    [TestClass]
    public class ComparisonTests
    {
        static int Iterations = 1000;
        static bool Warmup = true;
        static bool DiscardHighLow = true;

        static EmployeeRepositoryDapper s_DapperRepo;
        static EmployeeRepositoryChain s_ChainRepo;
        static EmployeeRepositoryChainCompiled s_ChainCompiledRepo;
        static EmployeeRepositoryEF_Intermediate s_EFIntermediateRepo;
        static EmployeeRepositoryEF_Intermediate_NoTrack s_EFIntermediateNoTrackRepo;
        static EmployeeRepositoryEF_Novice s_EFNoviceRepo;
        static SqlServerDataSource s_DataSource;

        [ClassInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            s_DataSource = SqlServerDataSource.CreateFromConfig("CodeFirstModels");
            s_DapperRepo = new EmployeeRepositoryDapper(ConfigurationManager.ConnectionStrings["CodeFirstModels"].ConnectionString);
            s_ChainRepo = new EmployeeRepositoryChain(SqlServerDataSource.CreateFromConfig("CodeFirstModels"));
            s_ChainCompiledRepo = new EmployeeRepositoryChainCompiled(SqlServerDataSource.CreateFromConfig("CodeFirstModels"));
            s_EFIntermediateRepo = new EmployeeRepositoryEF_Intermediate();
            s_EFIntermediateNoTrackRepo = new EmployeeRepositoryEF_Intermediate_NoTrack();
            s_EFNoviceRepo = new EmployeeRepositoryEF_Novice();

            if (Warmup)
            {
                CrudTestCore(s_DapperRepo);
                CrudTestCore(s_ChainRepo);
                CrudTestCore(s_ChainCompiledRepo);
                CrudTestCore(s_EFIntermediateRepo);
                CrudTestCore(s_EFIntermediateNoTrackRepo);
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
        public void ChainCompiled_CrudTest()
        {
            CrudTest(s_ChainCompiledRepo);
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

        [TestMethod]
        public void EF_Intermediate_NoTrack_CrudTest()
        {
            CrudTest(s_EFIntermediateNoTrackRepo);

        }

        static void CrudTest(ISimpleEmployeeRepository repo)
        {
            s_DataSource.Sql(@"DELETE FROM Sales.Customer;DELETE FROM HR.Employee;").Execute();

            //actual
            var spans = new List<double>(Iterations);
            for (var i = 0; i < Iterations; i++)
            {
                var sw = Stopwatch.StartNew();
                CrudTestCore(repo);
                sw.Stop();
                spans.Add(sw.Elapsed.TotalMilliseconds);
            }
            Trace.WriteLine("Run Duration: " + spans.Average().ToString("N2") + " ms per iteration. Min: " + spans.Min().ToString("N2") + " ms. Max: " + spans.Max().ToString("N2") + " ms.");

            Trace.WriteLine("");
            Trace.WriteLine("");
            //foreach (var span in spans)
            //    Trace.WriteLine("    " + span.ToString("N2"));

            if (DiscardHighLow && Iterations > 10)
            {
                //Remove the highest and lowest two to reduce OS effects
                spans.Remove(spans.Max());
                spans.Remove(spans.Max());
                spans.Remove(spans.Min());
                spans.Remove(spans.Min());
            }

            Trace.WriteLine("Run Duration: " + spans.Average().ToString("N2") + " ms per iteration. Min: " + spans.Min().ToString("N2") + " ms. Max: " + spans.Max().ToString("N2") + " ms.");

            long frequency = Stopwatch.Frequency;
            Trace.WriteLine($"  Timer frequency in ticks per second = {frequency}");
            long nanosecPerTick = (1000L * 1000L * 1000L) / frequency;
            Trace.WriteLine($"  Timer is accurate within {nanosecPerTick} nanoseconds");
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
