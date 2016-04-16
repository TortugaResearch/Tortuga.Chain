using System;
using Tests.Models;
using Tortuga.Chain;

#if MSTest
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif WINDOWS_UWP 
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Tests.Class1Databases
{
    [TestClass]
    public class UpdateTests : TestBase
    {

        [TestMethod]
        public void UpdateTests_ChangeTrackingTest()
        {

            var original = new ChangeTrackingEmployee()
            {
                FirstName = "Test",
                LastName = "Employee" + DateTime.Now.Ticks,
                Title = "Mail Room"
            };

            var inserted = DataSource.Insert(EmployeeTableName, original).ToObject<ChangeTrackingEmployee>().Execute();
            inserted.FirstName = "Changed";
            inserted.AcceptChanges(); //only properties changed AFTER this line should actually be updated in the database
            inserted.Title = "Also Changed";

            var updated = DataSource.Update(EmployeeTableName, inserted, UpdateOptions.ChangedPropertiesOnly).ToObject<ChangeTrackingEmployee>().Execute();
            Assert.AreEqual(original.FirstName, updated.FirstName, "FirstName shouldn't have changed");
            Assert.AreEqual(original.LastName, updated.LastName, "LastName shouldn't have changed");
            Assert.AreEqual(inserted.Title, updated.Title, "Title should have changed");


        }

        [TestMethod]
        public void UpdateTests_ChangeTrackingTest_NothingChanged()
        {

            var original = new ChangeTrackingEmployee()
            {
                FirstName = "Test",
                LastName = "Employee" + DateTime.Now.Ticks,
                Title = "Mail Room"
            };

            var inserted = DataSource.Insert(EmployeeTableName, original).ToObject<ChangeTrackingEmployee>().Execute();

            try
            {
                var updated = DataSource.Update(EmployeeTableName, inserted, UpdateOptions.ChangedPropertiesOnly).ToObject<ChangeTrackingEmployee>().Execute();
                Assert.Fail("Exception Expected");
            }
            catch (ArgumentException)
            {
                //pass
            }
        }


#if !Roslyn_Missing

        [TestMethod]
        public void UpdateTests_ChangeTrackingTest_Compiled()
        {

            var original = new ChangeTrackingEmployee()
            {
                FirstName = "Test",
                LastName = "Employee" + DateTime.Now.Ticks,
                Title = "Mail Room"
            };

            var inserted = DataSource.Insert(EmployeeTableName, original).Compile().ToObject<ChangeTrackingEmployee>().Execute();
            inserted.FirstName = "Changed";
            inserted.AcceptChanges(); //only properties changed AFTER this line should actually be updated in the database
            inserted.Title = "Also Changed";

            var updated = DataSource.Update(EmployeeTableName, inserted, UpdateOptions.ChangedPropertiesOnly).Compile().ToObject<ChangeTrackingEmployee>().Execute();
            Assert.AreEqual(original.FirstName, updated.FirstName, "FirstName shouldn't have changed");
            Assert.AreEqual(original.LastName, updated.LastName, "LastName shouldn't have changed");
            Assert.AreEqual(inserted.Title, updated.Title, "Title should have changed");


        }

        [TestMethod]
        public void UpdateTests_ChangeTrackingTest_NothingChanged_Compiled()
        {

            var original = new ChangeTrackingEmployee()
            {
                FirstName = "Test",
                LastName = "Employee" + DateTime.Now.Ticks,
                Title = "Mail Room"
            };

            var inserted = DataSource.Insert(EmployeeTableName, original).Compile().ToObject<ChangeTrackingEmployee>().Execute();

            try
            {
                var updated = DataSource.Update(EmployeeTableName, inserted, UpdateOptions.ChangedPropertiesOnly).Compile().ToObject<ChangeTrackingEmployee>().Execute();
                Assert.Fail("Exception Expected");
            }
            catch (ArgumentException)
            {
                //pass
            }
        }

#endif

    }
}
