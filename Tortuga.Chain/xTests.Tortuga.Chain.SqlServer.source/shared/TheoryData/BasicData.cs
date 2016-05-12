using System.Collections.Generic;
using Tortuga.Chain.DataSources;
using Xunit;

namespace Tests
{
    public class BasicData : TheoryData<string, string, DataSourceType>
    {
        public BasicData(params DataSource[] dataSources)
        {
            foreach (var ds in dataSources)
            {
                Add(TestBase.AssemblyName, ds.Name, DataSourceType.Normal);
                Add(TestBase.AssemblyName, ds.Name, DataSourceType.Open);
                Add(TestBase.AssemblyName, ds.Name, DataSourceType.Transactional);
            }
        }

        public BasicData(IEnumerable<DataSource> dataSources)
        {
            foreach (var ds in dataSources)
            {
                Add(TestBase.AssemblyName, ds.Name, DataSourceType.Normal);
                Add(TestBase.AssemblyName, ds.Name, DataSourceType.Open);
                Add(TestBase.AssemblyName, ds.Name, DataSourceType.Transactional);
            }
        }
    }

}
