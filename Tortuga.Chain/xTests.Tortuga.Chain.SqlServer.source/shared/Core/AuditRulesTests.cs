using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using Tests.Models;
using Tortuga.Chain;
using Tortuga.Chain.AuditRules;
using Xunit;
using Xunit.Abstractions;

namespace Tests.shared.Core
{
    public class AuditRulesTests : TestBase
    {
        public static RootData Prime = new RootData(s_PrimaryDataSource);

        public AuditRulesTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory, MemberData("Prime")]
        public void AuditRulesTests_AddUser(string assemblyName, string dataSourceName)
        {
            var dataSource = DataSource(dataSourceName);
            try
            {
                var currentUser = dataSource.From(EmployeeTableName).ToObject<Employee>(RowOptions.DiscardExtraRows).Execute();
                var ds =  AttachRules(dataSource).WithUser(currentUser);
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public void AuditRulesTests_CheckValidation(string assemblyName, string dataSourceName)
        {
            var dataSource = DataSource(dataSourceName);
            try
            {
                var currentUser = dataSource.From(EmployeeTableName).ToObject<Employee>(RowOptions.DiscardExtraRows).Execute();

                var ds = AttachRules(dataSource).WithUser(currentUser);

                var cust = new CustomerWithValidation();

                try
                {
                    ds.Insert(CustomerTableName, cust).Execute();
                    Assert.Fail("Expected a validation exception");
                }
                catch (ValidationException)
                {
                    //expected
                }
            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public void AuditRulesTests_InsertUpdateRules(string assemblyName, string dataSourceName)
        {

            var dataSource = DataSource(dataSourceName);
            try
            {
                var key = Guid.NewGuid().ToString();
                for (var i = 0; i < 2; i++)
                    dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

                var users = dataSource.From(EmployeeTableName).ToCollection<Employee>().Execute();
                var currentUser1 = users.First();
                var currentUser2 = users.Skip(1).First();

                var dsWithRules = AttachRules(dataSource);
                var ds1 = dsWithRules.WithUser(currentUser1);
                var ds2 = dsWithRules.WithUser(currentUser2);

                var cust1 = new CustomerWithValidation() { FullName = "Test Customer " + DateTime.Now.Ticks, State = "CA" };

                var cust2 = ds1.Insert(CustomerTableName, cust1).ToObject<CustomerWithValidation>().Execute();
                Assert.AreEqual(cust1.FullName, cust2.FullName, "Fullname was not set");
                Assert.AreEqual(currentUser1.EmployeeKey, cust2.CreatedByKey, "CreatedBy was not set");
                Assert.AreEqual(currentUser1.EmployeeKey, cust2.UpdatedByKey, "UpdatedBy was not set");
                Assert.IsNotNull(cust2.CreatedDate, "CreatedDate was not set");
                Assert.IsNotNull(cust2.UpdatedDate, "UpdatedDate was not set");

                Thread.Sleep(100); //make sure the current time is different enough for the database to notice

                cust2.State = "NV";
                var cust3 = ds2.Update(CustomerTableName, cust2).ToObject<CustomerWithValidation>().Execute();
                Assert.AreEqual(currentUser1.EmployeeKey, cust2.CreatedByKey, "CreatedBy was not set");
                Assert.AreEqual(currentUser2.EmployeeKey, cust3.UpdatedByKey, "UpdatedBy was not changed");
                Assert.AreEqual(cust2.CreatedDate, cust3.CreatedDate, "CreatedDate was not suposed to change");
                Assert.AreNotEqual(cust2.UpdatedDate, cust3.UpdatedDate, "UpdatedDate was suposed to change");

            }
            finally
            {
                Release(dataSource);
            }
        }

        [Theory, MemberData("Prime")]
        public void AuditRulesTests_SoftDelete(string assemblyName, string dataSourceName)
        {
            var dataSource = DataSource(dataSourceName);
            try
            {
                var key = Guid.NewGuid().ToString();
                for (var i = 0; i < 2; i++)
                    dataSource.Insert(EmployeeTableName, new Employee() { FirstName = i.ToString("0000"), LastName = "Z" + (int.MaxValue - i), Title = key, MiddleName = i % 2 == 0 ? "A" + i : null }).ToObject<Employee>().Execute();

                var users = dataSource.From(EmployeeTableName).ToCollection<Employee>().Execute();
                var currentUser1 = users.First();
                var currentUser2 = users.Skip(1).First();

                var dsWithRules = dataSource.WithRules(
                    new SoftDeleteRule("DeletedFlag", true, OperationTypes.SelectOrDelete),
                    new UserDataRule("DeletedByKey", "EmployeeKey", OperationTypes.Delete),
                    new DateTimeRule("DeletedDate", DateTimeKind.Local, OperationTypes.Delete)
                    );

                var ds1 = dsWithRules.WithUser(currentUser1);

                var cust1 = new CustomerWithValidation() { FullName = "Test Customer " + DateTime.Now.Ticks, State = "CA" };

                var cust2 = ds1.Insert(CustomerTableName, cust1).ToObject<CustomerWithValidation>().Execute();
                var customerKey = cust2.CustomerKey;
                Assert.False(cust2.DeletedFlag, "Deleted flag should be is clear");
                Assert.IsNull(cust2.DeletedDate, "Deleted date should be null");
                Assert.IsNull(cust2.DeletedByKey, "Deleted by key should be null");

                ds1.Delete(CustomerTableName, new { CustomerKey = customerKey }).Execute();

                var deletedRecord = dataSource.From(CustomerTableName, new { CustomerKey = customerKey }).ToObject<CustomerWithValidation>().Execute();

                Assert.True(deletedRecord.DeletedFlag, "Deleted flag should be set");
                Assert.IsNotNull(deletedRecord.DeletedDate, "Deleted date should be set");
                Assert.AreEqual(currentUser1.EmployeeKey, deletedRecord.DeletedByKey, "Deleted by key should be set");

                var misingRecord = ds1.From(CustomerTableName, new { CustomerKey = customerKey }).ToObject<CustomerWithValidation>(RowOptions.AllowEmptyResults).Execute();

                Assert.IsNull(misingRecord, "The soft delete rule should prevent this record from being returned.");

                var misingRecord2 = ds1.From(CustomerTableName, "CustomerKey = @CustomerKey", new { CustomerKey = customerKey }).ToObject<CustomerWithValidation>(RowOptions.AllowEmptyResults).Execute();

                Assert.IsNull(misingRecord2, "The soft delete rule should prevent this record from being returned.");

                var misingRecords = ds1.From(CustomerTableName).ToCollection<CustomerWithValidation>().Execute();

                Assert.False(misingRecords.Any(r => r.CustomerKey == customerKey), "The soft delete rule should prevent this record from being returned.");
            }
            finally
            {
                Release(dataSource);
            }
        }

    }
}
