using System.Collections.Generic;
using System.Linq;
using Tortuga.Chain.DataSources;
using Xunit;

namespace Tests
{


    public class TableData : TheoryData<string, string, DataSourceType, string>
    {
        public TableData(IEnumerable<DataSource> dataSources)
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
}