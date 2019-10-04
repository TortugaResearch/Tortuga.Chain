using System.Collections.Generic;
using System.Linq;
using Tortuga.Chain.DataSources;
using Xunit;

namespace Tests
{
    public class ViewData : TheoryData<string, DataSourceType, string>
    {
        public ViewData(IEnumerable<DataSource> dataSources)
        {
            foreach (var ds in dataSources)
            {
                ds.DatabaseMetadata.Preload();
                foreach (var view in ds.DatabaseMetadata.GetTablesAndViews().Where(t => !t.IsTable))
                {
                    Add(ds.Name, DataSourceType.Normal, view.Name);
                    Add(ds.Name, DataSourceType.Open, view.Name);
                    Add(ds.Name, DataSourceType.Transactional, view.Name);
                    Add(ds.Name, DataSourceType.Strict, view.Name);
                }
            }
        }

        public ViewData(IEnumerable<DataSource> dataSources, DataSourceType dataSourceType) : this(dataSources)
        {
            foreach (var ds in dataSources)
            {
                ds.DatabaseMetadata.Preload();
                foreach (var view in ds.DatabaseMetadata.GetTablesAndViews().Where(t => !t.IsTable))
                {
                    Add(ds.Name, dataSourceType, view.Name);
                }
            }
        }
    }
}
