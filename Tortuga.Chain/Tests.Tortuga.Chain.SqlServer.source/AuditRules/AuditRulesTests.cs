//using System;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Threading;
//using Tests.Models;
//using Tortuga.Chain;
//using Tortuga.Chain.AuditRules;

//#if MSTest
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//#elif WINDOWS_UWP 
//using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
//#endif

//namespace Tests.AuditRules
//{
//    [TestClass]
//    public class AuditRulesTests : TestBase
//    {

//        [TestMethod]
//        public void AuditRulesTests_SetupRules()
//        {
//            var dataSource = DataSourceWithAuditRules();
//        }

//        [TestMethod]
//        public void AuditRulesTests_AddUser()
//        {
//            var currentUser = DataSource.From(EmployeeTableName).ToObject<Employee>(RowOptions.DiscardExtraRows).Execute();
//            var ds = DataSourceWithAuditRules().WithUser(currentUser);
//        }

//        [TestMethod]
//        public void AuditRulesTests_CheckValidation()
//        {
//            var currentUser = DataSource.From(EmployeeTableName).ToObject<Employee>(RowOptions.DiscardExtraRows).Execute();

//            var ds = DataSourceWithAuditRules().WithUser(currentUser);

//            var cust = new CustomerWithValidation();

//            try
//            {
//                ds.Insert(CustomerTableName, cust).Execute();
//                Assert.Fail("Expected a validation exception");
//            }
//            catch (ValidationException)
//            {
//                //expected
//            }
//        }

//#if !WINDOWS_UWP
//        [TestMethod]
//        public void AuditRulesTests_InsertUpdateRules()
//        {

//            var key = Guid.NewGuid().ToString();
//            for (var i = 0; i < 2; i++)
//                DataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

//            var users = DataSource.From(EmployeeTableName).ToCollection<Employee>().Execute();
//            var currentUser1 = users.First();
//            var currentUser2 = users.Skip(1).First();

//            var dsWithRules = DataSourceWithAuditRules();
//            var ds1 = dsWithRules.WithUser(currentUser1);
//            var ds2 = dsWithRules.WithUser(currentUser2);

//            var cust1 = new CustomerWithValidation() { FullName = "Test Customer " + DateTime.Now.Ticks, State = "CA" };

//            var cust2 = ds1.Insert(CustomerTableName, cust1).ToObject<CustomerWithValidation>().Execute();
//            Assert.AreEqual(cust1.FullName, cust2.FullName, "Fullname was not set");
//            Assert.AreEqual(currentUser1.EmployeeKey, cust2.CreatedByKey, "CreatedBy was not set");
//            Assert.AreEqual(currentUser1.EmployeeKey, cust2.UpdatedByKey, "UpdatedBy was not set");
//            Assert.IsNotNull(cust2.CreatedDate, "CreatedDate was not set");
//            Assert.IsNotNull(cust2.UpdatedDate, "UpdatedDate was not set");

//            Thread.Sleep(100); //make sure the current time is different enough for the database to notice

//            cust2.State = "NV";
//            var cust3 = ds2.Update(CustomerTableName, cust2).ToObject<CustomerWithValidation>().Execute();
//            Assert.AreEqual(currentUser1.EmployeeKey, cust2.CreatedByKey, "CreatedBy was not set");
//            Assert.AreEqual(currentUser2.EmployeeKey, cust3.UpdatedByKey, "UpdatedBy was not changed");
//            Assert.AreEqual(cust2.CreatedDate, cust3.CreatedDate, "CreatedDate was not supposed to change");
//            Assert.AreNotEqual(cust2.UpdatedDate, cust3.UpdatedDate, "UpdatedDate was supposed to change");

//        }
//#endif 
//        [TestMethod]
//        public void AuditRulesTests_SoftDelete()
//        {
//            var key = Guid.NewGuid().ToString();
//            for (var i = 0; i < 2; i++)
//                DataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

//            var users = DataSource.From(EmployeeTableName).ToCollection<Employee>().Execute();
//            var currentUser1 = users.First();
//            var currentUser2 = users.Skip(1).First();

//            var dsWithRules = DataSourceWithAuditRules().WithRules(
//                new SoftDeleteRule("DeletedFlag", true, OperationTypes.SelectOrDelete),
//                new UserDataRule("DeletedByKey", "EmployeeKey", OperationTypes.Delete),
//                new DateTimeRule("DeletedDate", DateTimeKind.Local, OperationTypes.Delete)
//                );

//            var ds1 = dsWithRules.WithUser(currentUser1);

//            var cust1 = new CustomerWithValidation() { FullName = "Test Customer " + DateTime.Now.Ticks, State = "CA" };

//            var cust2 = ds1.Insert(CustomerTableName, cust1).ToObject<CustomerWithValidation>().Execute();
//            var customerKey = cust2.CustomerKey;
//            Assert.IsFalse(cust2.DeletedFlag, "Deleted flag should be is clear");
//            Assert.IsNull(cust2.DeletedDate, "Deleted date should be null");
//            Assert.IsNull(cust2.DeletedByKey, "Deleted by key should be null");

//            ds1.Delete(CustomerTableName, new { CustomerKey = customerKey }).Execute();

//            var deletedRecord = DataSource.From(CustomerTableName, new { CustomerKey = customerKey }).ToObject<CustomerWithValidation>().Execute();

//            Assert.IsTrue(deletedRecord.DeletedFlag, "Deleted flag should be set");
//            Assert.IsNotNull(deletedRecord.DeletedDate, "Deleted date should be set");
//            Assert.AreEqual(currentUser1.EmployeeKey, deletedRecord.DeletedByKey, "Deleted by key should be set");

//            var misingRecord = ds1.From(CustomerTableName, new { CustomerKey = customerKey }).ToObject<CustomerWithValidation>(RowOptions.AllowEmptyResults).Execute();

//            Assert.IsNull(misingRecord, "The soft delete rule should prevent this record from being returned.");

//            var misingRecord2 = ds1.From(CustomerTableName, "CustomerKey = @CustomerKey", new { CustomerKey = customerKey }).ToObject<CustomerWithValidation>(RowOptions.AllowEmptyResults).Execute();

//            Assert.IsNull(misingRecord2, "The soft delete rule should prevent this record from being returned.");

//            var misingRecords = ds1.From(CustomerTableName).ToCollection<CustomerWithValidation>().Execute();

//            Assert.IsFalse(misingRecords.Any(r => r.CustomerKey == customerKey), "The soft delete rule should prevent this record from being returned.");
//        }

//    }
//}


