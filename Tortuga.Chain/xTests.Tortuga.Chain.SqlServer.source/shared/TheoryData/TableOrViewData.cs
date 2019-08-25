using System.Collections.Generic;
using System.Linq;
using Tortuga.Chain.DataSources;
using Xunit;

namespace Tests
{
    public class TablesAndViewColumnsData : TheoryData<string, DataSourceType, string, string>
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
                        Add(ds.Name, DataSourceType.Normal, table.Name, column.SqlName);
                        Add(ds.Name, DataSourceType.Open, table.Name, column.SqlName);
                        Add(ds.Name, DataSourceType.Transactional, table.Name, column.SqlName);
                        Add(ds.Name, DataSourceType.Strict, table.Name, column.SqlName);

                        //if (column.SqlName != column.ClrName)
                        //{
                        //    Add( ds.Name, DataSourceType.Normal, table.Name, column.ClrName);
                        //    Add( ds.Name, DataSourceType.Open, table.Name, column.ClrName);
                        //    Add( ds.Name, DataSourceType.Transactional, table.Name, column.ClrName);
                        //}
                    }
                }
            }
        }
    }

    public class TablesAndViewData : TheoryData<string, DataSourceType, string>
    {
        public TablesAndViewData(IEnumerable<DataSource> dataSources)
        {
            foreach (var ds in dataSources)
            {
                ds.DatabaseMetadata.Preload();
                foreach (var table in ds.DatabaseMetadata.GetTablesAndViews().Where(t => t.IsTable))
                {
                    Add(ds.Name, DataSourceType.Normal, table.Name);
                    Add(ds.Name, DataSourceType.Open, table.Name);
                    Add(ds.Name, DataSourceType.Transactional, table.Name);
                    Add(ds.Name, DataSourceType.Strict, table.Name);
                }
            }
        }
    }
}
