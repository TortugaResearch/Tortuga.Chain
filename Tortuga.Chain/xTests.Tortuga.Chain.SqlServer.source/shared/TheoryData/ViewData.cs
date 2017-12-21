using System.Collections.Generic;
using System.Linq;
using Tortuga.Chain.DataSources;
using Xunit;

namespace Tests
{
    public class ViewData : TheoryData<string, string, DataSourceType, string>
    {
        public ViewData(IEnumerable<DataSource> dataSources)
        {
            foreach (var ds in dataSources)
            {
                ds.DatabaseMetadata.Preload();
                foreach (var view in ds.DatabaseMetadata.GetTablesAndViews().Where(t => !t.IsTable))
                {
                    Add(TestBase.AssemblyName, ds.Name, DataSourceType.Normal, view.Name);
                    Add(TestBase.AssemblyName, ds.Name, DataSourceType.Open, view.Name);
                    Add(TestBase.AssemblyName, ds.Name, DataSourceType.Transactional, view.Name);
                    Add(TestBase.AssemblyName, ds.Name, DataSourceType.Strict, view.Name);
                }
            }
        }
    }
}
