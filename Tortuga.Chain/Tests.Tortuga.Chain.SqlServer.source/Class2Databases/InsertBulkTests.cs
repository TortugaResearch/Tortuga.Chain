//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using Tests.Models;

//namespace Tests.Class2Databases
//{
//    [TestClass]
//    public class InsertBulkTests : TestBase
//    {
//        [ClassInitialize()]
//        public static void ClassInitialize(TestContext context)
//        {
//            var key1000 = Guid.NewGuid().ToString();
//            var employeeList = new List<Employee>();

//            for (var i = 0; i < 5; i++)
//                employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

//            var count = DataSource.InsertBulk(EmployeeTableName, employeeList).Execute();
//        }

//        [TestMethod]
//        public void InsertBulk()
//        {
//            var key1000 = Guid.NewGuid().ToString();
//            var employeeList = new List<Employee>();

//            for (var i = 0; i < 1000; i++)
//                employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

//            var count = DataSource.InsertBulk(EmployeeTableName, employeeList).Execute();
//            Assert.AreEqual(1000, count);
//        }

//        [TestMethod]
//        public void InsertBulk_WithStreaming()
//        {
//            var key = Guid.NewGuid().ToString();
//            var employeeList = new List<Employee>();

//            var count = DataSource.InsertBulk(EmployeeTableName, StreamRecords(key, 1000)).WithStreaming().Execute();
//            Assert.AreEqual(-1, count); //streaming prevents returning a row count;
//        }

//        [TestMethod]
//        public void InsertBulk_WithEvents()
//        {
//            long runningCount = 0;
//            var key = Guid.NewGuid().ToString();
//            var employeeList = new List<Employee>();

//            var count = DataSource.InsertBulk(EmployeeTableName, StreamRecords(key, 1000)).WithBatchSize(75).WithNotifications((s, e) =>
//            {
//                Debug.WriteLine($"Copied {e.RowsCopied} rows");
//                runningCount = e.RowsCopied;
//            }, 90).Execute();
//            Assert.AreEqual(-1, count); //streaming prevents returning a row count;
//            Assert.AreNotEqual(0, runningCount); //but we can get it another way
//        }

//        IEnumerable<Employee> StreamRecords(string key, int maxRecords)
//        {
//            var i = 0;
//            while (i < maxRecords)
//            {
//                yield return new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key };
//                i++;
//            }
//        }

//    }
//}
