using System.Collections.Generic;
using Tortuga.Chain.DataSources;
using Xunit;

namespace Tests
{
    public class BasicData : TheoryData<string, string, DataSourceType>
    {
        public BasicData(IEnumerable<DataSource> dataSource)
        {
            foreach (var ds in dataSource)
            {
                Add(TestBase.AssemblyName, ds.Name, DataSourceType.Normal);
                Add(TestBase.AssemblyName, ds.Name, DataSourceType.Open);
                Add(TestBase.AssemblyName, ds.Name, DataSourceType.Transactional);
            }
        }
    }

}
