using Tortuga.Chain;
using Tortuga.Chain.DataSources;
using Xunit;

namespace Tests
{
    public class BasicDataWithJoinOptions : TheoryData<string, string, DataSourceType, JoinOptions>
    {
        public BasicDataWithJoinOptions(params DataSource[] dataSources)
        {
            var options = new JoinOptions[]{ JoinOptions.None,
                    JoinOptions.IgnoreUnmatchedChildren,
                    JoinOptions.MultipleParents,
                    JoinOptions.Parallel,
                    JoinOptions.MultipleParents | JoinOptions.Parallel };

            foreach (var option in options)
                foreach (var ds in dataSources)
                {
                    Add(TestBase.AssemblyName, ds.Name, DataSourceType.Normal, option);
                    Add(TestBase.AssemblyName, ds.Name, DataSourceType.Open, option);
                    Add(TestBase.AssemblyName, ds.Name, DataSourceType.Transactional, option);
                }
        }
    }
}
