using System;
using Tests.Models;

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
    }
}