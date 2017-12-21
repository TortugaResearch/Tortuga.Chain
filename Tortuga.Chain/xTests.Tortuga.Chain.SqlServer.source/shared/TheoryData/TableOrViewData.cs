using System.Collections.Generic;
using System.Linq;
using Tortuga.Chain.DataSources;
using Xunit;

namespace Tests
{
    public class TablesAndViewData : TheoryData<string, string, DataSourceType, string>
    {
        public TablesAndViewData(IEnumerable<DataSource> dataSources)
        {
            foreach (var ds in dataSources)
            {
                ds.DatabaseMetadata.Preload();
                foreach (var table in ds.DatabaseMetadata.GetTablesAndViews().Where(t => t.IsTable))
                {
                    Add(TestBase.AssemblyName, ds.Name, DataSourceType.Normal, table.Name);
                    Add(TestBase.AssemblyName, ds.Name, DataSourceType.Open, table.Name);
                    Add(TestBase.AssemblyName, ds.Name, DataSourceType.Transactional, table.Name);
                    Add(TestBase.AssemblyName, ds.Name, DataSourceType.Strict, table.Name);
                }
            }
        }
    }


    public class TablesAndViewColumnsData : TheoryData<string, string, DataSourceType, string, string>
    {
        public TablesAndViewColumnsData(IEnumerable<DataSource> dataSources)
        {
            foreach (var ds in dataSources)
            {
                ds.DatabaseMetadata.Preload();
                foreach (var table in ds.DatabaseMetadata.GetTablesAndViews().Where(t => t.IsTable))
                {
                    foreach (var column in table.Columns)
                    {
                        Add(TestBase.AssemblyName, ds.Name, DataSourceType.Normal, table.Name, column.SqlName);
                        Add(TestBase.AssemblyName, ds.Name, DataSourceType.Open, table.Name, column.SqlName);
                        Add(TestBase.AssemblyName, ds.Name, DataSourceType.Transactional, table.Name, column.SqlName);
                        Add(TestBase.AssemblyName, ds.Name, DataSourceType.Strict, table.Name, column.SqlName);

                        //if (column.SqlName != column.ClrName)
                        //{
                        //    Add(TestBase.AssemblyName, ds.Name, DataSourceType.Normal, table.Name, column.ClrName);
                        //    Add(TestBase.AssemblyName, ds.Name, DataSourceType.Open, table.Name, column.ClrName);
                        //    Add(TestBase.AssemblyName, ds.Name, DataSourceType.Transactional, table.Name, column.ClrName);
                        //}
                    }
                }
            }
        }
    }
}

