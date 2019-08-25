using System.Collections.Generic;
using Tortuga.Chain.DataSources;
using Xunit;

namespace Tests
{
    public class BasicData : TheoryData<string, DataSourceType>
    {
        public BasicData(params DataSource[] dataSources)
        {
            foreach (var ds in dataSources)
            {
                Add(ds.Name, DataSourceType.Normal);
                Add(ds.Name, DataSourceType.Open);
                Add(ds.Name, DataSourceType.Transactional);
                Add(ds.Name, DataSourceType.Strict);
            }
        }

        public BasicData(IEnumerable<DataSource> dataSources)
        {
            foreach (var ds in dataSources)
            {
                Add(ds.Name, DataSourceType.Normal);
                Add(ds.Name, DataSourceType.Open);
                Add(ds.Name, DataSourceType.Transactional);
                Add(ds.Name, DataSourceType.Strict);
            }
        }
    }
}
