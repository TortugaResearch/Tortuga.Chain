
using System;
using System.Linq;
using Tests.Models;
using Tortuga.Chain;
using System.Collections.Concurrent;

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Tests.Class1Databases
{
    [TestClass]
    public class FromTests : TestBase
    {
        //static string Key10;
        //static string Key100;
        static string s_Key1000;
        //static string Key10000;

        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
        {
            using (var trans = DataSource.BeginTransaction())
            {
                //Key10 = Guid.NewGuid().ToString();
                //for (var i = 0; i < 100 i++)
                //    DataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = Key10 }).ToObject<Employee>().Execute();

                //Key100 = Guid.NewGuid().ToString();
                //for (var i = 0; i < 100; i++)
                //    DataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = Key100 }).ToObject<Employee>().Execute();

                s_Key1000 = Guid.NewGuid().ToString();
                for (var i = 0; i < 1000; i++)
                    trans.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = s_Key1000, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

                //Key10000 = Guid.NewGuid().ToString();
                //for (var i = 0; i < 10000; i++)
                //    DataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = Key10000 }).ToObject<Employee>().Execute();

                trans.Commit();
            }
        }


        [TestMethod]
        public void FromTests_Counts()
        {
            var count = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).AsCount().Execute();
            var columnCount = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).AsCount("Title").Execute();
            var columnCount2 = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).AsCount("MiddleName").Execute();
            var distinctColumnCount = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).AsCount("Title", true).Execute();
            var distinctColumnCount2 = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).AsCount("LastName", true).Execute();

            Assert.AreEqual(1000, count, "All of the rows");
            Assert.AreEqual(1000, columnCount, "No nulls");
            Assert.AreEqual(500, columnCount2, "Half of the rows are nul");
            Assert.AreEqual(1, distinctColumnCount, "Only one distinct value");
            Assert.AreEqual(1000, distinctColumnCount2, "Every value is distinct");
        }



#if SQLite
        [TestMethod]
        public void FromTests_ToImmutableObject()
        {
            var uniqueKey = Guid.NewGuid().ToString();

            var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
            DataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();

            var lookup = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToObject<EmployeeLookup>().WithConstructor<long, string, string>().Execute();

            Assert.AreEqual("A", lookup.FirstName);
            Assert.AreEqual("1", lookup.LastName);

        }

#else
        [TestMethod]
        public void FromTests_ToImmutableObject()
        {
            var uniqueKey = Guid.NewGuid().ToString();

            var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
            DataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();

            var lookup = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToObject<EmployeeLookup>().WithConstructor<int, string, string>().Execute();

            Assert.AreEqual("A", lookup.FirstName);
            Assert.AreEqual("1", lookup.LastName);

        }
#endif


        [TestMethod]
        public void FromTests_ToInferredObject()
        {
            var uniqueKey = Guid.NewGuid().ToString();

            var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
            DataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();

            var lookup = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToObject<EmployeeLookup>(RowOptions.InferConstructor).Execute();

            Assert.AreEqual("A", lookup.FirstName);
            Assert.AreEqual("1", lookup.LastName);

        }

#if !SQLite

        [TestMethod]
        public void FromTests_ToDictionary_ImmutableObject()
        {
            var uniqueKey = Guid.NewGuid().ToString();

            var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
            var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
            var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = uniqueKey };
            var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = uniqueKey };

            DataSource.Insert(EmployeeTableName, emp1).WithRefresh().Execute();
            DataSource.Insert(EmployeeTableName, emp2).WithRefresh().Execute();
            DataSource.Insert(EmployeeTableName, emp3).WithRefresh().Execute();
            DataSource.Insert(EmployeeTableName, emp4).WithRefresh().Execute();

            var test1 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup>("FirstName").WithConstructor<int, string, string>().Execute();

            Assert.AreEqual("1", test1["A"].LastName);
            Assert.AreEqual("2", test1["B"].LastName);
            Assert.AreEqual("3", test1["C"].LastName);
            Assert.AreEqual("4", test1["D"].LastName);

            var test2 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName)).WithConstructor<int, string, string>().Execute();

            Assert.AreEqual("A", test2[1].FirstName);
            Assert.AreEqual("B", test2[2].FirstName);
            Assert.AreEqual("C", test2[3].FirstName);
            Assert.AreEqual("D", test2[4].FirstName);

            var test3 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup, ConcurrentDictionary<string, EmployeeLookup>>("FirstName").WithConstructor<int, string, string>().Execute();
            Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
            Assert.AreEqual("1", test3["A"].LastName);
            Assert.AreEqual("2", test3["B"].LastName);
            Assert.AreEqual("3", test3["C"].LastName);
            Assert.AreEqual("4", test3["D"].LastName);

            var test4 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup, ConcurrentDictionary<int, EmployeeLookup>>(e => int.Parse(e.LastName)).WithConstructor<int, string, string>().Execute();
            Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
            Assert.AreEqual("A", test4[1].FirstName);
            Assert.AreEqual("B", test4[2].FirstName);
            Assert.AreEqual("C", test4[3].FirstName);
            Assert.AreEqual("D", test4[4].FirstName);

            var test5 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<string, EmployeeLookup>("FirstName").WithConstructor<int, string, string>().Execute();
            Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
            Assert.AreEqual("1", test5["A"].LastName);
            Assert.AreEqual("2", test5["B"].LastName);
            Assert.AreEqual("3", test5["C"].LastName);
            Assert.AreEqual("4", test5["D"].LastName);

            var test6 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName)).WithConstructor<int, string, string>().Execute();
            Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
            Assert.AreEqual("A", test6[1].FirstName);
            Assert.AreEqual("B", test6[2].FirstName);
            Assert.AreEqual("C", test6[3].FirstName);
            Assert.AreEqual("D", test6[4].FirstName);

        }
#endif

        [TestMethod]
        public void FromTests_ToDictionary_InferredObject()
        {
            var uniqueKey = Guid.NewGuid().ToString();

            var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
            var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
            var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = uniqueKey };
            var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = uniqueKey };

            emp1 = DataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
            emp2 = DataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
            emp3 = DataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
            emp4 = DataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

            var test1 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup>("FirstName", DictionaryOptions.InferConstructor).Execute();

            Assert.AreEqual("1", test1["A"].LastName);
            Assert.AreEqual("2", test1["B"].LastName);
            Assert.AreEqual("3", test1["C"].LastName);
            Assert.AreEqual("4", test1["D"].LastName);

            var test2 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName), DictionaryOptions.InferConstructor).Execute();

            Assert.AreEqual("A", test2[1].FirstName);
            Assert.AreEqual("B", test2[2].FirstName);
            Assert.AreEqual("C", test2[3].FirstName);
            Assert.AreEqual("D", test2[4].FirstName);

            var test3 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, EmployeeLookup, ConcurrentDictionary<string, EmployeeLookup>>("FirstName", DictionaryOptions.InferConstructor).Execute();
            Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
            Assert.AreEqual("1", test3["A"].LastName);
            Assert.AreEqual("2", test3["B"].LastName);
            Assert.AreEqual("3", test3["C"].LastName);
            Assert.AreEqual("4", test3["D"].LastName);

            var test4 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, EmployeeLookup, ConcurrentDictionary<int, EmployeeLookup>>(e => int.Parse(e.LastName), DictionaryOptions.InferConstructor).Execute();
            Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
            Assert.AreEqual("A", test4[1].FirstName);
            Assert.AreEqual("B", test4[2].FirstName);
            Assert.AreEqual("C", test4[3].FirstName);
            Assert.AreEqual("D", test4[4].FirstName);

            var test5 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<string, EmployeeLookup>("FirstName", DictionaryOptions.InferConstructor).Execute();
            Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, EmployeeLookup>));
            Assert.AreEqual("1", test5["A"].LastName);
            Assert.AreEqual("2", test5["B"].LastName);
            Assert.AreEqual("3", test5["C"].LastName);
            Assert.AreEqual("4", test5["D"].LastName);

            var test6 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<int, EmployeeLookup>(e => int.Parse(e.LastName), DictionaryOptions.InferConstructor).Execute();
            Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, EmployeeLookup>));
            Assert.AreEqual("A", test6[1].FirstName);
            Assert.AreEqual("B", test6[2].FirstName);
            Assert.AreEqual("C", test6[3].FirstName);
            Assert.AreEqual("D", test6[4].FirstName);

        }

        [TestMethod]
        public void FromTests_ToDictionary()
        {
            var uniqueKey = Guid.NewGuid().ToString();

            var emp1 = new Employee() { FirstName = "A", LastName = "1", Title = uniqueKey };
            var emp2 = new Employee() { FirstName = "B", LastName = "2", Title = uniqueKey };
            var emp3 = new Employee() { FirstName = "C", LastName = "3", Title = uniqueKey };
            var emp4 = new Employee() { FirstName = "D", LastName = "4", Title = uniqueKey };

            emp1 = DataSource.Insert(EmployeeTableName, emp1).ToObject<Employee>().Execute();
            emp2 = DataSource.Insert(EmployeeTableName, emp2).ToObject<Employee>().Execute();
            emp3 = DataSource.Insert(EmployeeTableName, emp3).ToObject<Employee>().Execute();
            emp4 = DataSource.Insert(EmployeeTableName, emp4).ToObject<Employee>().Execute();

            var test1 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, Employee>("FirstName").Execute();

            Assert.AreEqual("1", test1["A"].LastName);
            Assert.AreEqual("2", test1["B"].LastName);
            Assert.AreEqual("3", test1["C"].LastName);
            Assert.AreEqual("4", test1["D"].LastName);

            var test2 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, Employee>(e => int.Parse(e.LastName)).Execute();

            Assert.AreEqual("A", test2[1].FirstName);
            Assert.AreEqual("B", test2[2].FirstName);
            Assert.AreEqual("C", test2[3].FirstName);
            Assert.AreEqual("D", test2[4].FirstName);

            var test3 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<string, Employee, ConcurrentDictionary<string, Employee>>("FirstName").Execute();
            Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, Employee>));
            Assert.AreEqual("1", test3["A"].LastName);
            Assert.AreEqual("2", test3["B"].LastName);
            Assert.AreEqual("3", test3["C"].LastName);
            Assert.AreEqual("4", test3["D"].LastName);

            var test4 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToDictionary<int, Employee, ConcurrentDictionary<int, Employee>>(e => int.Parse(e.LastName)).Execute();
            Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, Employee>));
            Assert.AreEqual("A", test4[1].FirstName);
            Assert.AreEqual("B", test4[2].FirstName);
            Assert.AreEqual("C", test4[3].FirstName);
            Assert.AreEqual("D", test4[4].FirstName);

            var test5 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<string, Employee>("FirstName").Execute();
            Assert.IsInstanceOfType(test3, typeof(ConcurrentDictionary<string, Employee>));
            Assert.AreEqual("1", test5["A"].LastName);
            Assert.AreEqual("2", test5["B"].LastName);
            Assert.AreEqual("3", test5["C"].LastName);
            Assert.AreEqual("4", test5["D"].LastName);

            var test6 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).ToImmutableDictionary<int, Employee>(e => int.Parse(e.LastName)).Execute();
            Assert.IsInstanceOfType(test4, typeof(ConcurrentDictionary<int, Employee>));
            Assert.AreEqual("A", test6[1].FirstName);
            Assert.AreEqual("B", test6[2].FirstName);
            Assert.AreEqual("C", test6[3].FirstName);
            Assert.AreEqual("D", test6[4].FirstName);

        }


        [TestMethod]
        public void FromTests_Sorting_InferredCollection()
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

            var test1 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("FirstName").ToCollection<EmployeeLookup>(CollectionOptions.InferConstructor).Execute();
            Assert.AreEqual(emp1.EmployeeKey, test1[0].EmployeeKey);
            Assert.AreEqual(emp2.EmployeeKey, test1[1].EmployeeKey);
            Assert.AreEqual(emp3.EmployeeKey, test1[2].EmployeeKey);
            Assert.AreEqual(emp4.EmployeeKey, test1[3].EmployeeKey);

            var test2 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting(new SortExpression("FirstName", SortDirection.Descending)).ToCollection<EmployeeLookup>(CollectionOptions.InferConstructor).Execute();
            Assert.AreEqual(emp4.EmployeeKey, test2[0].EmployeeKey);
            Assert.AreEqual(emp3.EmployeeKey, test2[1].EmployeeKey);
            Assert.AreEqual(emp2.EmployeeKey, test2[2].EmployeeKey);
            Assert.AreEqual(emp1.EmployeeKey, test2[3].EmployeeKey);

            var test3 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("LastName", "FirstName").ToCollection<EmployeeLookup>(CollectionOptions.InferConstructor).Execute();
            Assert.AreEqual(emp3.EmployeeKey, test3[0].EmployeeKey);
            Assert.AreEqual(emp4.EmployeeKey, test3[1].EmployeeKey);
            Assert.AreEqual(emp1.EmployeeKey, test3[2].EmployeeKey);
            Assert.AreEqual(emp2.EmployeeKey, test3[3].EmployeeKey);

        }

#if !SQLite
        [TestMethod]
        public void FromTests_Sorting_ImmutableCollection()
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

            var test1 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("FirstName").ToCollection<EmployeeLookup>().WithConstructor<int, string, string>().Execute();
            Assert.AreEqual(emp1.EmployeeKey, test1[0].EmployeeKey);
            Assert.AreEqual(emp2.EmployeeKey, test1[1].EmployeeKey);
            Assert.AreEqual(emp3.EmployeeKey, test1[2].EmployeeKey);
            Assert.AreEqual(emp4.EmployeeKey, test1[3].EmployeeKey);

            var test2 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting(new SortExpression("FirstName", SortDirection.Descending)).ToCollection<EmployeeLookup>().WithConstructor<int, string, string>().Execute();
            Assert.AreEqual(emp4.EmployeeKey, test2[0].EmployeeKey);
            Assert.AreEqual(emp3.EmployeeKey, test2[1].EmployeeKey);
            Assert.AreEqual(emp2.EmployeeKey, test2[2].EmployeeKey);
            Assert.AreEqual(emp1.EmployeeKey, test2[3].EmployeeKey);

            var test3 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("LastName", "FirstName").ToCollection<EmployeeLookup>().WithConstructor<int, string, string>().Execute();
            Assert.AreEqual(emp3.EmployeeKey, test3[0].EmployeeKey);
            Assert.AreEqual(emp4.EmployeeKey, test3[1].EmployeeKey);
            Assert.AreEqual(emp1.EmployeeKey, test3[2].EmployeeKey);
            Assert.AreEqual(emp2.EmployeeKey, test3[3].EmployeeKey);

        }
#endif

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
        public void FromTests_ImmutableArray()
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

            var test1 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("FirstName").ToImmutableArray<Employee>().Execute();
            Assert.AreEqual(emp1.EmployeeKey, test1[0].EmployeeKey);
            Assert.AreEqual(emp2.EmployeeKey, test1[1].EmployeeKey);
            Assert.AreEqual(emp3.EmployeeKey, test1[2].EmployeeKey);
            Assert.AreEqual(emp4.EmployeeKey, test1[3].EmployeeKey);

            var test2 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting(new SortExpression("FirstName", SortDirection.Descending)).ToImmutableArray<Employee>().Execute();
            Assert.AreEqual(emp4.EmployeeKey, test2[0].EmployeeKey);
            Assert.AreEqual(emp3.EmployeeKey, test2[1].EmployeeKey);
            Assert.AreEqual(emp2.EmployeeKey, test2[2].EmployeeKey);
            Assert.AreEqual(emp1.EmployeeKey, test2[3].EmployeeKey);

            var test3 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("LastName", "FirstName").ToImmutableArray<Employee>().Execute();
            Assert.AreEqual(emp3.EmployeeKey, test3[0].EmployeeKey);
            Assert.AreEqual(emp4.EmployeeKey, test3[1].EmployeeKey);
            Assert.AreEqual(emp1.EmployeeKey, test3[2].EmployeeKey);
            Assert.AreEqual(emp2.EmployeeKey, test3[3].EmployeeKey);

        }

        [TestMethod]
        public void FromTests_ImmutableList()
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

            var test1 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("FirstName").ToImmutableList<Employee>().Execute();
            Assert.AreEqual(emp1.EmployeeKey, test1[0].EmployeeKey);
            Assert.AreEqual(emp2.EmployeeKey, test1[1].EmployeeKey);
            Assert.AreEqual(emp3.EmployeeKey, test1[2].EmployeeKey);
            Assert.AreEqual(emp4.EmployeeKey, test1[3].EmployeeKey);

            var test2 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting(new SortExpression("FirstName", SortDirection.Descending)).ToImmutableList<Employee>().Execute();
            Assert.AreEqual(emp4.EmployeeKey, test2[0].EmployeeKey);
            Assert.AreEqual(emp3.EmployeeKey, test2[1].EmployeeKey);
            Assert.AreEqual(emp2.EmployeeKey, test2[2].EmployeeKey);
            Assert.AreEqual(emp1.EmployeeKey, test2[3].EmployeeKey);

            var test3 = DataSource.From(EmployeeTableName, new { Title = uniqueKey }).WithSorting("LastName", "FirstName").ToImmutableList<Employee>().Execute();
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

            var find2 = DataSource.GetByKey(EmployeeTableName, emp2.EmployeeKey.Value).ToObject<Employee>().Execute();
            Assert.AreEqual(emp2.EmployeeKey, find2.EmployeeKey, "The wrong employee was returned");

            var list = DataSource.GetByKey(EmployeeTableName, emp2.EmployeeKey.Value, emp3.EmployeeKey.Value, emp4.EmployeeKey.Value).ToCollection<Employee>().Execute();
            Assert.AreEqual(3, list.Count, "GetByKey returned the wrong number of rows");
            Assert.IsTrue(list.Any(e => e.EmployeeKey == emp2.EmployeeKey));
            Assert.IsTrue(list.Any(e => e.EmployeeKey == emp3.EmployeeKey));
            Assert.IsTrue(list.Any(e => e.EmployeeKey == emp4.EmployeeKey));
        }


        [TestMethod]
        public void FromTests_Take_NoSort()
        {

            var result = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).WithLimits(10).ToCollection<Employee>().Execute();
            Assert.AreEqual(10, result.Count, "Count");
            foreach (var item in result)
            {
                Assert.AreEqual(s_Key1000, item.Title, "Filter");
                Assert.IsTrue(int.Parse(item.FirstName) >= 0, "Range");
                Assert.IsTrue(int.Parse(item.FirstName) < 10, "Range");
            }
        }

        [TestMethod]
        public void FromTests_Take()
        {

            var result = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).WithSorting("FirstName").WithLimits(10).ToCollection<Employee>().Execute();
            Assert.AreEqual(10, result.Count, "Count");
            foreach (var item in result)
            {
                Assert.AreEqual(s_Key1000, item.Title, "Filter");
                Assert.IsTrue(int.Parse(item.FirstName) >= 0, "Range");
                Assert.IsTrue(int.Parse(item.FirstName) < 10, "Range");
            }
        }

        [TestMethod]
        public void FromTests_SkipTake()
        {
            var result = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).WithSorting("FirstName").WithLimits(10, 15).ToCollection<Employee>().Execute();
            Assert.AreEqual(15, result.Count, "Count");
            foreach (var item in result)
            {
                Assert.AreEqual(s_Key1000, item.Title, "Filter");
                Assert.IsTrue(int.Parse(item.FirstName) >= 10, "Range");
                Assert.IsTrue(int.Parse(item.FirstName) < 25, "Range");
            }
        }



#if SqlServer
        [TestMethod]
        public void FromTests_TakePercent()
        {
            var result = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).WithSorting("FirstName").WithLimits(10, SqlServerLimitOption.Percentage).ToCollection<Employee>().Execute();
            Assert.AreEqual(100, result.Count, "Count");
            foreach (var item in result)
            {
                Assert.AreEqual(s_Key1000, item.Title, "Filter");
                Assert.IsTrue(int.Parse(item.FirstName) >= 0, "Range");
                Assert.IsTrue(int.Parse(item.FirstName) < 100, "Range");
            }


        }

        [TestMethod]
        public void FromTests_TakePercentWithTies()
        {

            var result = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).WithSorting("FirstName").WithLimits(10, SqlServerLimitOption.PercentageWithTies).ToCollection<Employee>().Execute();
            Assert.AreEqual(100, result.Count, "Count");
            foreach (var item in result)
            {
                Assert.AreEqual(s_Key1000, item.Title, "Filter");
                Assert.IsTrue(int.Parse(item.FirstName) >= 0, "Range");
                Assert.IsTrue(int.Parse(item.FirstName) < 100, "Range");
            }
        }

        [TestMethod]
        public void FromTests_TakeWithTies()
        {
            var result = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).WithSorting("FirstName").WithLimits(10, SqlServerLimitOption.RowsWithTies).ToCollection<Employee>().Execute();
            Assert.AreEqual(10, result.Count, "Count");
            foreach (var item in result)
            {
                Assert.AreEqual(s_Key1000, item.Title, "Filter");
                Assert.IsTrue(int.Parse(item.FirstName) >= 0, "Range");
                Assert.IsTrue(int.Parse(item.FirstName) < 10, "Range");
            }
        }

        [TestMethod]
        public void FromTests_TableSampleSystemPercentage()
        {
            var result = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).WithLimits(100, SqlServerLimitOption.TableSampleSystemPercentage).ToCollection<Employee>().Execute();

            //SQL Server is really inaccurate here for low row counts. We could get 0 rows, 55 rows, or 175 rows depending on where the data lands on the page.
            foreach (var item in result)
            {
                Assert.AreEqual(s_Key1000, item.Title, "Filter");
            }
        }

        [TestMethod]
        public void FromTests_TableSampleSystemRows()
        {
            var result = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).WithLimits(100, SqlServerLimitOption.TableSampleSystemRows).ToCollection<Employee>().Execute();

            //SQL Server is really inaccurate here for low row counts. We could get 0 rows, 55 rows, or 175 rows depending on where the data lands on the page.
            foreach (var item in result)
            {
                Assert.AreEqual(s_Key1000, item.Title, "Filter");
            }
        }

        [TestMethod]
        public void FromTests_TableSampleSystemPercentage_Repeatable()
        {
            var seed = 1;
            var result1 = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).WithLimits(100, SqlServerLimitOption.TableSampleSystemPercentage, seed).ToCollection<Employee>().Execute();
            var result2 = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).WithLimits(100, SqlServerLimitOption.TableSampleSystemPercentage, seed).ToCollection<Employee>().Execute();

            Assert.AreEqual(result1.Count, result2.Count, "Row count");
        }

        [TestMethod]
        public void FromTests_TableSampleSystemRows_Repeatable()
        {
            var seed = 1;
            var result1 = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).WithLimits(100, SqlServerLimitOption.TableSampleSystemRows, seed).ToCollection<Employee>().Execute();
            var result2 = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).WithLimits(100, SqlServerLimitOption.TableSampleSystemRows, seed).ToCollection<Employee>().Execute();

            Assert.AreEqual(result1.Count, result2.Count, "Row count");
        }

#endif

#if SQLite


        [TestMethod]
        public void FromTests_TakeRandom()
        {
            var result = DataSource.From(EmployeeTableName, new { Title = s_Key1000 }).WithLimits(100, SQLiteLimitOption.RandomSampleRows).ToCollection<Employee>().Execute();
            Assert.AreEqual(100, result.Count, "Count");
            foreach (var item in result)
            {
                Assert.AreEqual(s_Key1000, item.Title, "Filter");
            }
        }
#endif


    }
}
