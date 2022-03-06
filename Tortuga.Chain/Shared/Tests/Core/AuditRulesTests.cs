using System.ComponentModel.DataAnnotations;
using Tests.Models;
using Tortuga.Chain;
using Tortuga.Chain.AuditRules;

namespace Tests.shared.Core;

[TestClass]
public class AuditRulesTests : TestBase
{
	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void AuditRulesTests_AddUser(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			var currentUser = dataSource.From(EmployeeTableName).ToObject<Employee>(RowOptions.DiscardExtraRows).Execute();
			var ds = AttachRules(dataSource).WithUser(currentUser);
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void AuditRulesTests_CheckValidation(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			dataSource.Insert(EmployeeTableName, new Employee() { FirstName = "X", LastName = "Z", Title = Guid.NewGuid().ToString(), MiddleName = "A" }).Execute();
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

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void AuditRulesTests_InsertUpdateRules(string dataSourceName)
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
			Assert.AreEqual(cust1.FullName, cust2.FullName, "FullName was not set");
			Assert.AreEqual(currentUser1.EmployeeKey, cust2.CreatedByKey, "CreatedBy was not set");
			Assert.AreEqual(currentUser1.EmployeeKey, cust2.UpdatedByKey, "UpdatedBy was not set");
			Assert.IsNotNull(cust2.CreatedDate, "CreatedDate was not set");
			Assert.IsNotNull(cust2.UpdatedDate, "UpdatedDate was not set");

			Thread.Sleep(1000); //make sure the current time is different enough for the database to notice

			cust2.State = "NV";
			var cust3 = ds2.Update(CustomerTableName, cust2).ToObject<CustomerWithValidation>().Execute();
			Assert.AreEqual(currentUser1.EmployeeKey, cust2.CreatedByKey, "CreatedBy was not set");
			Assert.AreEqual(currentUser2.EmployeeKey, cust3.UpdatedByKey, "UpdatedBy was not changed");
			Assert.AreEqual(cust2.CreatedDate, cust3.CreatedDate, "CreatedDate was not supposed to change");
			Assert.AreNotEqual(cust2.UpdatedDate, cust3.UpdatedDate, "UpdatedDate was supposed to change");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void AuditRulesTests_SoftDelete(string dataSourceName)
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
				new SoftDeleteRule("DeletedFlag", true),
				new UserDataRule("DeletedByKey", "EmployeeKey", OperationTypes.Delete),
				new DateTimeRule("DeletedDate", DateTimeKind.Local, OperationTypes.Delete)
				);

			var ds1 = dsWithRules.WithUser(currentUser1);

			var cust1 = new CustomerWithValidation() { FullName = "Test Customer " + DateTime.Now.Ticks, State = "CA" };

			var cust2 = ds1.Insert(CustomerTableName, cust1).ToObject<CustomerWithValidation>().Execute();
			var customerKey = cust2.CustomerKey;
			Assert.IsFalse(cust2.DeletedFlag, "Deleted flag should be is clear");
			Assert.IsNull(cust2.DeletedDate, "Deleted date should be null");
			Assert.IsNull(cust2.DeletedByKey, "Deleted by key should be null");

			ds1.Delete(CustomerTableName, new { CustomerKey = customerKey }).Execute();

			var deletedRecord = dataSource.From(CustomerTableName, new { CustomerKey = customerKey }).ToObject<CustomerWithValidation>().Execute();

			Assert.IsTrue(deletedRecord.DeletedFlag, "Deleted flag should be set");
			Assert.IsNotNull(deletedRecord.DeletedDate, "Deleted date should be set");
			Assert.AreEqual(currentUser1.EmployeeKey, deletedRecord.DeletedByKey, "Deleted by key should be set");

			var misingRecord = ds1.From(CustomerTableName, new { CustomerKey = customerKey }).ToObjectOrNull<CustomerWithValidation>().Execute();

			Assert.IsNull(misingRecord, "The soft delete rule should prevent this record from being returned.");

#if SQL_SERVER_OLEDB
			var misingRecord2 = ds1.From(CustomerTableName, "CustomerKey = ?", new { CustomerKey = customerKey }).ToObjectOrNull<CustomerWithValidation>().Execute();
#else
			var misingRecord2 = ds1.From(CustomerTableName, "CustomerKey = @CustomerKey", new { CustomerKey = customerKey }).ToObjectOrNull<CustomerWithValidation>().Execute();
#endif

			Assert.IsNull(misingRecord2, "The soft delete rule should prevent this record from being returned.");

			var misingRecords = ds1.From(CustomerTableName).ToCollection<CustomerWithValidation>().Execute();

			Assert.IsFalse(misingRecords.Any(r => r.CustomerKey == customerKey), "The soft delete rule should prevent this record from being returned.");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void AuditRulesTests_SoftDelete_2(string dataSourceName)
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
				new SoftDeleteRule("DeletedFlag", true),
				new UserDataRule("DeletedByKey", "EmployeeKey", OperationTypes.Delete),
				new DateTimeRule("DeletedDate", DateTimeKind.Local, OperationTypes.Delete)
				);

			var ds1 = dsWithRules.WithUser(currentUser1);

			var cust1 = new Customer() { FullName = "Test Customer " + DateTime.Now.Ticks, State = "CA" }; //Note the difference in class name from the original test.

			var cust2 = ds1.Insert(CustomerTableName, cust1).ToObject<CustomerWithValidation>().Execute();
			var customerKey = cust2.CustomerKey;
			Assert.IsFalse(cust2.DeletedFlag, "Deleted flag should be is clear");
			Assert.IsNull(cust2.DeletedDate, "Deleted date should be null");
			Assert.IsNull(cust2.DeletedByKey, "Deleted by key should be null");

			ds1.Delete(CustomerTableName, new { CustomerKey = customerKey }).Execute();

			var deletedRecord = dataSource.From(CustomerTableName, new { CustomerKey = customerKey }).ToObject<CustomerWithValidation>().Execute();

			Assert.IsTrue(deletedRecord.DeletedFlag, "Deleted flag should be set");
			Assert.IsNotNull(deletedRecord.DeletedDate, "Deleted date should be set");
			Assert.AreEqual(currentUser1.EmployeeKey, deletedRecord.DeletedByKey, "Deleted by key should be set");

			var misingRecord = ds1.From(CustomerTableName, new { CustomerKey = customerKey }).ToObjectOrNull<CustomerWithValidation>().Execute();

			Assert.IsNull(misingRecord, "The soft delete rule should prevent this record from being returned.");

#if SQL_SERVER_OLEDB
			var misingRecord2 = ds1.From(CustomerTableName, "CustomerKey = ?", new { CustomerKey = customerKey }).ToObjectOrNull<CustomerWithValidation>().Execute();
#else
			var misingRecord2 = ds1.From(CustomerTableName, "CustomerKey = @CustomerKey", new { CustomerKey = customerKey }).ToObjectOrNull<CustomerWithValidation>().Execute();
#endif

			Assert.IsNull(misingRecord2, "The soft delete rule should prevent this record from being returned.");

			var misingRecords = ds1.From(CustomerTableName).ToCollection<CustomerWithValidation>().Execute();

			Assert.IsFalse(misingRecords.Any(r => r.CustomerKey == customerKey), "The soft delete rule should prevent this record from being returned.");
		}
		finally
		{
			Release(dataSource);
		}
	}

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void AuditRulesTests_RestrictedColumn(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			var key = dataSource.Insert(EmployeeTableName, new Employee() { FirstName = "A", MiddleName = "B", LastName = "C" }).ToInt32().Execute();
			var goodUser = new UserToken(true);
			var badUser = new UserToken(false);
			ExceptWhenPredicate isAdminCheck = user => ((UserToken)user).IsAdmin;
			var dsReadCheck = dataSource.WithRules(new RestrictColumn("MiddleName", OperationTypes.Select, isAdminCheck));
			var dsWriteCheck = dataSource.WithRules(new RestrictColumn("MiddleName", OperationTypes.Update, isAdminCheck));

			//SELECT
			{
				var shouldBeSet = dsReadCheck.WithUser(goodUser).GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();
				Assert.AreEqual("B", shouldBeSet.MiddleName, "MiddleName was to be set");
			}
			{
				var shouldBeMissing = dsReadCheck.WithUser(badUser).GetByKey(EmployeeTableName, key).ToObject<Employee>().Execute();
				Assert.IsNull(shouldBeMissing.MiddleName, "MiddleName was supposed to be clear");
			}
			//UPDATE
			{
				var shouldNotBeChanged = dsWriteCheck.WithUser(badUser).Update(EmployeeTableName, new { FirstName = "AA", MiddleName = "Z", EmployeeKey = key }).ToObject<Employee>().Execute();
				Assert.AreEqual("B", shouldNotBeChanged.MiddleName, "MiddleName was not supposed to be changed");
			}
			{
				var shouldBeChanged = dsWriteCheck.WithUser(goodUser).Update(EmployeeTableName, new { FirstName = "BB", MiddleName = "X", EmployeeKey = key }).ToObject<Employee>().Execute();
				Assert.AreEqual("X", shouldBeChanged.MiddleName, "MiddleName was supposed to be changed");
			}
			//SELECT after UPDATE
			{
				var shouldBeSet = dsReadCheck.WithUser(goodUser).Update(EmployeeTableName, new { FirstName = "AA", MiddleName = "B", EmployeeKey = key }).ToObject<Employee>().Execute();
				Assert.AreEqual("B", shouldBeSet.MiddleName, "MiddleName was to be set");
			}
			{
				var shouldBeMissing = dsReadCheck.WithUser(badUser).Update(EmployeeTableName, new { FirstName = "BB", MiddleName = "X", EmployeeKey = key }).ToObject<Employee>().Execute();
				Assert.IsNull(shouldBeMissing.MiddleName, "MiddleName was supposed to be clear");
			}
		}
		finally
		{
			Release(dataSource);
		}
	}

#if SQL_SERVER_SDS || SQL_SERVER_MDS || SQL_SERVER_OLEDB

	[DataTestMethod, RootData(DataSourceGroup.Primary)]
	public void SoftDeleteByKey(string dataSourceName)
	{
		var dataSource = DataSource(dataSourceName);
		try
		{
			var users = dataSource.From(EmployeeTableName).ToCollection<Employee>().Execute();
			var currentUser1 = users.First();
			var currentUser2 = users.Skip(1).First();

			var dsWithRules = dataSource.WithRules(
				new SoftDeleteRule("DeletedFlag", true),
				new UserDataRule("DeletedByKey", "EmployeeKey", OperationTypes.Delete),
				new DateTimeRule("DeletedDate", DateTimeKind.Local, OperationTypes.Delete)
				);

			var ds1 = dsWithRules.WithUser(currentUser1);

			var lookupKey = Guid.NewGuid().ToString();
			for (var i = 0; i < 10; i++)
				dataSource.Insert(CustomerTableName, new CustomerWithValidation() { FullName = lookupKey, State = "CA" }).ToObject<CustomerWithValidation>().Execute();

			var allKeys = dataSource.From(CustomerTableName, new { FullName = lookupKey }).ToInt32List("CustomerKey").Execute();
			var keysToUpdate = allKeys.Take(5).ToList();

			var updatedRows = ds1.DeleteByKeyList(CustomerTableName, keysToUpdate).ToCollection<CustomerWithValidation>().Execute();

			foreach (var row in updatedRows)
				Assert.IsTrue(row.DeletedFlag, "Deleted flag was not set");

			Assert.AreEqual(5, updatedRows.Count, "The wrong number of rows were deleted");

			var allRows = ds1.From(CustomerTableName, new { FullName = lookupKey }).ToCollection<CustomerWithValidation>().Execute();
			Assert.AreEqual(5, allRows.Count, "The wrong number of rows remain");

			var allRowsBypass = dataSource.From(CustomerTableName, new { FullName = lookupKey }).ToCollection<CustomerWithValidation>().Execute();
			Assert.AreEqual(10, allRowsBypass.Count, "The rows were supposed to be soft-deleted only");
		}
		finally
		{
			Release(dataSource);
		}
	}

#endif

	private class UserToken
	{
		public UserToken(bool isAdmin)
		{
			IsAdmin = isAdmin;
		}

		public bool IsAdmin { get; set; }
	}
}
