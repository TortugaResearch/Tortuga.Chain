using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tortuga.Chain;
using Tortuga.Chain.SqlServer;

namespace Tests.Class1Databases
{
    [TestClass]
    public class EdgeCases : TestBase
    {
        [TestMethod]
        public void ColumnWithSpace_Mapped()
        {
            var tag = DateTime.Now.Ticks;
            var ds = DataSource.WithSettings(new SqlServerDataSourceSettings() { StrictMode = true });

            Console.WriteLine(ds.Insert("dbo.ColumnWithSpace", new ColumnWithSpaceA() { ColumnWithSpace = "Test 1" + tag }).ToInt32().CommandText());
            var pk1 = ds.Insert("dbo.ColumnWithSpace", new ColumnWithSpaceA() { ColumnWithSpace = "Test 1" + tag }).ToInt32().Execute();

            Console.WriteLine(ds.GetByKey("dbo.ColumnWithSpace", pk1).ToObject<ColumnWithSpaceA>().CommandText());
            var obj1 = ds.GetByKey("dbo.ColumnWithSpace", pk1).ToObject<ColumnWithSpaceA>().Execute();

            Console.WriteLine(ds.Insert("dbo.ColumnWithSpace", new ColumnWithSpaceA() { ColumnWithSpace = "Test 2" + tag }).ToObject().CommandText());
            var obj2 = ds.Insert("dbo.ColumnWithSpace", new ColumnWithSpaceA() { ColumnWithSpace = "Test 2" + tag }).ToObject().Execute();

            Assert.AreEqual("Test 1" + tag, obj1.ColumnWithSpace);
            Assert.AreEqual("Test 2" + tag, obj2.ColumnWithSpace);
        }

        [TestMethod]
        public void ColumnWithSpace()
        {
            var tag = DateTime.Now.Ticks;
            var ds = DataSource.WithSettings(new SqlServerDataSourceSettings() { StrictMode = true });

            Console.WriteLine(ds.Insert("dbo.ColumnWithSpace", new ColumnWithSpaceB() { ColumnWithSpace = "Test 1" + tag }).ToInt32().CommandText());
            var pk1 = ds.Insert("dbo.ColumnWithSpace", new ColumnWithSpaceB() { ColumnWithSpace = "Test 1" + tag }).ToInt32().Execute();

            Console.WriteLine(ds.GetByKey("dbo.ColumnWithSpace", pk1).ToObject<ColumnWithSpaceB>().CommandText());
            var obj1 = ds.GetByKey("dbo.ColumnWithSpace", pk1).ToObject<ColumnWithSpaceB>().Execute();

            Console.WriteLine(ds.Insert("dbo.ColumnWithSpace", new ColumnWithSpaceB() { ColumnWithSpace = "Test 2" + tag }).ToObject().CommandText());
            var obj2 = ds.Insert("dbo.ColumnWithSpace", new ColumnWithSpaceB() { ColumnWithSpace = "Test 2" + tag }).ToObject().Execute();

            Assert.AreEqual("Test 1" + tag, obj1.ColumnWithSpace);
            Assert.AreEqual("Test 2" + tag, obj2.ColumnWithSpace);
        }

        [TestMethod]
        public void ColumnWithSpace_QuotedMapped_1()
        {
            var tag = DateTime.Now.Ticks;
            var ds = DataSource.WithSettings(new SqlServerDataSourceSettings() { StrictMode = true });

            try
            {
                Console.WriteLine(ds.Insert("dbo.ColumnWithSpace", new ColumnWithSpaceC() { ColumnWithSpace = "Test 1" + tag }).ToInt32().CommandText());
                //var pk1 = ds.Insert("dbo.ColumnWithSpace", new ColumnWithSpaceC() { ColumnWithSpace = "Test 1" + tag }).ToInt32().Execute();


                Assert.Fail("Expected a mapping exception. The column name shouldn't be quoted.");
            }
            catch (MappingException ex)
            {
                Console.WriteLine(ex);
            }

        }

        [TestMethod]
        public void ColumnWithSpace_QuotedMapped_2()
        {
            var tag = DateTime.Now.Ticks;
            var ds = DataSource.WithSettings(new SqlServerDataSourceSettings() { StrictMode = true });

            try
            {
                Console.WriteLine(ds.GetByKey("dbo.ColumnWithSpace", 0).ToObject<ColumnWithSpaceC>().CommandText());
                //var obj1 = ds.GetByKey("dbo.ColumnWithSpace", pk1).ToObject<ColumnWithSpaceC>().Execute();

                Assert.Fail("Expected a mapping exception. The column name shouldn't be quoted.");
            }
            catch (MappingException ex)
            {
                Console.WriteLine(ex);
            }

        }

        [TestMethod]
        public void ColumnWithSpace_QuotedMapped_3()
        {
            var tag = DateTime.Now.Ticks;
            var ds = DataSource.WithSettings(new SqlServerDataSourceSettings() { StrictMode = true });

            try
            {
                Console.WriteLine(ds.Insert("dbo.ColumnWithSpace", new ColumnWithSpaceC() { ColumnWithSpace = "Test 2" + tag }).ToObject().CommandText());
                //var obj2 = ds.Insert("dbo.ColumnWithSpace", new ColumnWithSpaceC() { ColumnWithSpace = "Test 2" + tag }).ToObject().Execute();


                Assert.Fail("Expected a mapping exception. The column name shouldn't be quoted.");
            }
            catch (MappingException ex)
            {
                Console.WriteLine(ex);
            }

        }
    }

    public class ColumnWithSpaceA
    {
        public int Id { get; set; }

        [Column("Column With Space")]
        public string ColumnWithSpace { get; set; }

    }
    public class ColumnWithSpaceB
    {
        public int Id { get; set; }

        //[Column("ColumnWithSpace")]
        public string ColumnWithSpace { get; set; }

    }
    public class ColumnWithSpaceC
    {
        public int Id { get; set; }


        //This won't work. Don't quote the column names.
        [Column("[Column With Space]")]
        public string ColumnWithSpace { get; set; }

    }
}


