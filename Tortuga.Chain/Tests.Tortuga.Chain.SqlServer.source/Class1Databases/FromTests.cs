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
        //static string Key10;
        //static string Key100;
        static string Key1000;
        //static string Key10000;

        [ClassInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            using (var trans = DataSource.BeginTransaction())
            {
                //Key10 = Guid.NewGuid().ToString();
                //for (var i = 0; i < 100 i++)
                //    DataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = Key10 }).ToObject<Employee>().Execute();

                //Key100 = Guid.NewGuid().ToString();
                //for (var i = 0; i < 100; i++)
                //    DataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = Key100 }).ToObject<Employee>().Execute();

                Key1000 = Guid.NewGuid().ToString();
                for (var i = 0; i < 1000; i++)
                    trans.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = Key1000 }).ToObject<Employee>().Execute();

                //Key10000 = Guid.NewGuid().ToString();
                //for (var i = 0; i < 10000; i++)
                //    DataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = Key10000 }).ToObject<Employee>().Execute();

                trans.Commit();
            }
        }

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




        [TestMethod]
        public void FromTests_Take()
        {

            var result = DataSource.From(EmployeeTableName, new { Title = Key1000 }).WithSorting("FirstName").WithLimits(10).ToCollection<Employee>().Execute();
            Assert.AreEqual(10, result.Count, "Count");
            foreach (var item in result)
            {
                Assert.AreEqual(Key1000, item.Title, "Filter");
                Assert.IsTrue(int.Parse(item.FirstName) >= 0, "Range");
                Assert.IsTrue(int.Parse(item.FirstName) < 10, "Range");
            }


        }

        [TestMethod]
        public void FromTests_SkipTake()
        {


            var result = DataSource.From(EmployeeTableName, new { Title = Key1000 }).WithSorting("FirstName").WithLimits(10, 15).ToCollection<Employee>().Execute();
            Assert.AreEqual(15, result.Count, "Count");
            foreach (var item in result)
            {
                Assert.AreEqual(Key1000, item.Title, "Filter");
                Assert.IsTrue(int.Parse(item.FirstName) >= 10, "Range");
                Assert.IsTrue(int.Parse(item.FirstName) < 25, "Range");
            }

        }

#if SqlServer
        [TestMethod]
        public void FromTests_TakePercent()
        {


            var result = DataSource.From(EmployeeTableName, new { Title = Key1000 }).WithSorting("FirstName").WithLimits(10, SqlServerLimitOptions.Percentage).ToCollection<Employee>().Execute();
            Assert.AreEqual(100, result.Count, "Count");
            foreach (var item in result)
            {
                Assert.AreEqual(Key1000, item.Title, "Filter");
                Assert.IsTrue(int.Parse(item.FirstName) >= 0, "Range");
                Assert.IsTrue(int.Parse(item.FirstName) < 100, "Range");
            }


        }

        [TestMethod]
        public void FromTests_TakePercentWithTies()
        {

            var result = DataSource.From(EmployeeTableName, new { Title = Key1000 }).WithSorting("FirstName").WithLimits(10, SqlServerLimitOptions.PercentageWithTies).ToCollection<Employee>().Execute();
            Assert.AreEqual(100, result.Count, "Count");
            foreach (var item in result)
            {
                Assert.AreEqual(Key1000, item.Title, "Filter");
                Assert.IsTrue(int.Parse(item.FirstName) >= 0, "Range");
                Assert.IsTrue(int.Parse(item.FirstName) < 100, "Range");
            }


        }

        [TestMethod]
        public void FromTests_TakeWithTies()
        {
            var result = DataSource.From(EmployeeTableName, new { Title = Key1000 }).WithSorting("FirstName").WithLimits(10, SqlServerLimitOptions.RowsWithTies).ToCollection<Employee>().Execute();
            Assert.AreEqual(10, result.Count, "Count");
            foreach (var item in result)
            {
                Assert.AreEqual(Key1000, item.Title, "Filter");
                Assert.IsTrue(int.Parse(item.FirstName) >= 0, "Range");
                Assert.IsTrue(int.Parse(item.FirstName) < 10, "Range");
            }


        }

        [TestMethod]
        public void FromTests_TableSampleSystemPercentage()
        {
            var result = DataSource.From(EmployeeTableName, new { Title = Key1000 }).WithLimits(100, SqlServerLimitOptions.TableSampleSystemPercentage).ToCollection<Employee>().Execute();

            //SQL Server is really inaccurate here for low row counts. We could get 0 rows, 55 rows, or 175 rows depending on where the data lands on the page.
            foreach (var item in result)
            {
                Assert.AreEqual(Key1000, item.Title, "Filter");
            }
        }

        [TestMethod]
        public void FromTests_TableSampleSystemRows()
        {
            var result = DataSource.From(EmployeeTableName, new { Title = Key1000 }).WithLimits(100, SqlServerLimitOptions.TableSampleSystemRows).ToCollection<Employee>().Execute();

            //SQL Server is really inaccurate here for low row counts. We could get 0 rows, 55 rows, or 175 rows depending on where the data lands on the page.
            foreach (var item in result)
            {
                Assert.AreEqual(Key1000, item.Title, "Filter");
            }
        }

        [TestMethod]
        public void FromTests_TableSampleSystemPercentage_Repeatable()
        {
            var seed = 1;
            var result1 = DataSource.From(EmployeeTableName, new { Title = Key1000 }).WithLimits(100, SqlServerLimitOptions.TableSampleSystemPercentage, seed).ToCollection<Employee>().Execute();
            var result2 = DataSource.From(EmployeeTableName, new { Title = Key1000 }).WithLimits(100, SqlServerLimitOptions.TableSampleSystemPercentage, seed).ToCollection<Employee>().Execute();

            Assert.AreEqual(result1.Count, result2.Count, "Row count");
        }

        [TestMethod]
        public void FromTests_TableSampleSystemRows_Repeatable()
        {
            var seed = 1;
            var result1 = DataSource.From(EmployeeTableName, new { Title = Key1000 }).WithLimits(100, SqlServerLimitOptions.TableSampleSystemRows, seed).ToCollection<Employee>().Execute();
            var result2 = DataSource.From(EmployeeTableName, new { Title = Key1000 }).WithLimits(100, SqlServerLimitOptions.TableSampleSystemRows, seed).ToCollection<Employee>().Execute();

            Assert.AreEqual(result1.Count, result2.Count, "Row count");
        }

#endif

#if SQLite


        [TestMethod]
        public void FromTests_TakeRandom()
        {
            var result = DataSource.From(EmployeeTableName, new { Title = Key1000 }).WithLimits(100, SQLiteLimitOptions.RandomSampleRows).ToCollection<Employee>().Execute();
            Assert.AreEqual(100, result.Count, "Count");
            foreach (var item in result)
            {
                Assert.AreEqual(Key1000, item.Title, "Filter");
            }
        }
#endif


    }
}
