using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Tests.Models;
using Tortuga.Chain;

namespace Tests.Class2Databases
{
    [TestClass]
    public class InsertBatchTests : TestBase
    {
        const string TableType = "HR.EmployeeTable";
        [TestMethod]
        public void InsertBatch()
        {

            var key1000 = Guid.NewGuid().ToString();
            var employeeList = new List<Employee>();

            for (var i = 0; i < 1000; i++)
                employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

            DataSource.InsertBatch(EmployeeTableName, employeeList, TableType).Execute();
        }

        [TestMethod]
        public void InsertBatch_Streaming()
        {

            var key1000 = Guid.NewGuid().ToString();
            //var employeeList = new List<Employee>();

            //for (var i = 0; i < 1000; i++)
            //    employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

            DataSource.InsertBatch(EmployeeTableName, StreamRecords(key1000, 1000), TableType).Execute();
        }

        IEnumerable<Employee> StreamRecords(string key, int maxRecords)
        {
            var i = 0;
            while (i < maxRecords)
            {
                yield return new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key };
                i++;
            }
        }

        [TestMethod]
        public void InsertBatch_SelectBack()
        {

            var key1000 = Guid.NewGuid().ToString();
            var employeeList = new List<Employee>();

            for (var i = 0; i < 1000; i++)
                employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

            var employeeList2 = DataSource.InsertBatch(EmployeeTableName, employeeList, TableType).ToCollection<EmployeeLookup>(CollectionOptions.InferConstructor).Execute();

            Assert.AreEqual(employeeList.Count, employeeList2.Count);
        }

        [TestMethod]
        public void InsertBatch_SelectKeys()
        {

            var key1000 = Guid.NewGuid().ToString();
            var employeeList = new List<Employee>();

            for (var i = 0; i < 1000; i++)
                employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

            var employeeList2 = DataSource.InsertBatch(EmployeeTableName, employeeList, TableType).ToInt32List().Execute();

            Assert.AreEqual(employeeList.Count, employeeList2.Count);
        }

        [TestMethod]
        public void InsertBatch_AuditRules()
        {

            var key1000 = Guid.NewGuid().ToString();
            var employeeList = new List<Employee>();

            for (var i = 0; i < 1000; i++)
                employeeList.Add(new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key1000 });

            var employeeList2 = DataSourceWithAuditRules().InsertBatch(EmployeeTableName, employeeList, TableType).ToCollection<Employee>().Execute();
            Assert.AreEqual(employeeList.Count, employeeList2.Count);
            Assert.IsNotNull(employeeList2[0].UpdatedDate);

        }
    }
}

