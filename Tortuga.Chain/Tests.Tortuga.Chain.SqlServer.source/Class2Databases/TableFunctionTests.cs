using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Class2Databases
{
    [TestClass]
    public class TableFunctionTests : TestBase
    {
        [TestMethod]
        public void TableFunction2_Object()
        {
            var result = DataSource.TableFunction(TableFunction2Name, new { @State = "CA" }).ToTable().Execute();
        }


        [TestMethod]
        public void TableFunction1_Object_Limit()
        {
            var result = DataSource.TableFunction(TableFunction1Name, new { @State = "CA" }).WithLimits(1).ToTable().Execute();
        }

        [TestMethod]
        public void TableFunction1_Object_Filter()
        {
            var result = DataSource.TableFunction(TableFunction1Name, new { @State = "CA" }).WithFilter(new { @FullName = "Tom Jones" }).ToTable().Execute();
        }

        [TestMethod]
        public void TableFunction1_Object_Sort()
        {
            var result = DataSource.TableFunction(TableFunction1Name, new { @State = "CA" }).WithSorting("FullName").ToTable().Execute();
        }

        [TestMethod]
        public void TableFunction1_Object()
        {
            var result = DataSource.TableFunction(TableFunction1Name, new { @State = "CA" }).ToTable().Execute();
        }

        [TestMethod]
        public async Task TableFunction1_Object_Async()
        {
            var result = await DataSource.TableFunction(TableFunction1Name, new { @State = "CA" }).ToTable().ExecuteAsync();
        }


        [TestMethod]
        public void TableFunction1_Dictionary()
        {
            var result = DataSource.TableFunction(TableFunction1Name, new Dictionary<string, object>() { { "State", "CA" } }).ToTable().Execute();
        }

        [TestMethod]
        public async Task TableFunction1_Dictionary_Async()
        {
            var result = await DataSource.TableFunction(TableFunction1Name, new Dictionary<string, object>() { { "State", "CA" } }).ToTable().ExecuteAsync();
        }

        [TestMethod]
        public void TableFunction1_Dictionary2()
        {
            var result = DataSource.TableFunction(TableFunction1Name, new Dictionary<string, object>() { { "@State", "CA" } }).ToTable().Execute();
        }

        [TestMethod]
        public async Task TableFunction1_Dictionary2_Async()
        {
            var result = await DataSource.TableFunction(TableFunction1Name, new Dictionary<string, object>() { { "@State", "CA" } }).ToTable().ExecuteAsync();
        }
    }
}

